﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RandImg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settings;
        private const string SETTINGS_EXTENSION = "randimg";
        public MainWindow()
        {
            settings = new Settings();

            InitializeComponent();
            basePathsLB.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            RefreshWindow();

            loadPresets.Drop += LoadPresets_Drop;
            basePathsLB.Drop += BasePathsLB_Drop;
        }

        /// <summary>
        /// adds a new path with drag drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasePathsLB_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // check that it is a filename
            if (e.Data.GetDataPresent("FileName"))
            {
                string[] fileNames = (string[])e.Data.GetData("FileName");
                foreach (string s in fileNames)
                {
                    // standardize path name format (has strange '~'s otherwise)
                    string fullPath = System.IO.Path.GetFullPath(s);
                    FileAttributes fileAttributes = System.IO.File.GetAttributes(fullPath); // get paths
                    // directories
                    if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        settings.basePaths.Add(fullPath);
                    }
                    // files
                    else if ((fileAttributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        settings.basePaths.Add(System.IO.Path.GetDirectoryName(fullPath));
                    }
                    RefreshListbox();
                }
            }
        }
        private void LoadPresets_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // check that it is a filename
            if (e.Data.GetDataPresent("FileName"))
            {
                string[] fileNames = (string[])e.Data.GetData("FileName");
                foreach (string s in fileNames)
                {
                    // standardize path name format (has strange '~'s otherwise)
                    string fullPath = System.IO.Path.GetFullPath(s);
                    if (fullPath.EndsWith(SETTINGS_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    {
                        string jsonString = File.ReadAllText(fullPath);
                        settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(jsonString);
                        RefreshWindow();
                    }
                }
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // check that settings matches UI
            Debug.Assert(settings.searchPatternAnd == searchPatternAndTB.Text);
            Debug.Assert(settings.searchPatternOr == searchPatternOrTB.Text);
            Debug.Assert(settings.excludePattern == excludePatternTB.Text);
            Debug.Assert(settings.fullScrn == fullScreenRB.IsChecked && settings.fullScrn != resizeRB.IsChecked);

            try
            {
                DisplayImage dispImg = new DisplayImage(settings, Unminimize);

                if (settings.fullScrn)
                {
                    dispImg.Show();
                }
                else
                {
                    ResizeWindow resize = new ResizeWindow(dispImg);
                    resize.Show();
                }
                WindowState = WindowState.Minimized;
            }
            catch
            {
                System.Windows.MessageBox.Show("No matching files were found :(\n please enter another search");
            }
        }

        private void DirSelect_Click(object sender, RoutedEventArgs e)
        {
            // Folder Browser Dialog is pretty awful, should replace later
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                settings.basePaths.Add(folderBrowserDialog.SelectedPath);
                RefreshListbox();
            }
        }

        private void DirRemove_Click(object sender, RoutedEventArgs e)
        {
            settings.basePaths.RemoveAt(basePathsLB.SelectedIndex);
            RefreshListbox();
        }

        private void RefreshListbox()
        {
            basePathsLB.BeginInit();
            basePathsLB.Items.Clear();
            foreach (string s in settings.basePaths)
            {
                basePathsLB.Items.Add(s);
            }
            basePathsLB.EndInit();
        }

        /// <summary>
        /// sets display to match settings
        /// </summary>
        private void RefreshWindow()
        {
            RefreshListbox();

            fullScreenRB.IsChecked = settings.fullScrn;
            resizeRB.IsChecked = !settings.fullScrn;

            searchPatternAndTB.Text = settings.searchPatternAnd;
            searchPatternOrTB.Text = settings.searchPatternOr;
            excludePatternTB.Text = settings.excludePattern;
        }

        public void Unminimize()
        {
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Closes this and application to fully close
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            System.Windows.Application.Current.Shutdown();
        }


        private void fullScreenRB_Checked(object sender, RoutedEventArgs e)
        {
            settings.fullScrn = true;
        }

        private void resizeRB_Checked(object sender, RoutedEventArgs e)
        {
            settings.fullScrn = false;
        }

        private void savePresets_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = string.Format("RandImg file (*.{0})|*.{0}", SETTINGS_EXTENSION);
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Stream stream;
                    if ((stream = saveFileDialog.OpenFile()) != null)
                    {
                        StreamWriter streamWriter = new StreamWriter(stream);
                        // settings to print formatted
                        var writeSettings = new System.Text.Json.JsonSerializerOptions();
                        writeSettings.WriteIndented = true;
                        // serialize and output
                        streamWriter.Write(System.Text.Json.JsonSerializer.Serialize(settings, writeSettings));
                        // close streams
                        streamWriter.Close();
                        stream.Close();
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("The settings could not be saved properly");
                }
            }
        }

        private void loadPresets_Click(object sender, RoutedEventArgs e)
        {
            var loadFileDialog = new OpenFileDialog();
            loadFileDialog.Filter = string.Format("RandImg file (*.{0})|*.{0}", SETTINGS_EXTENSION);
            loadFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var result = loadFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string jsonString = File.ReadAllText(loadFileDialog.FileName);
                    settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(jsonString);
                    RefreshWindow();
                }
                catch
                {
                    System.Windows.MessageBox.Show("The settings could not be saved properly");
                }
            }
        }

        private void excludePatternTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.excludePattern = excludePatternTB.Text;
        }

        private void searchPatternAndTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.searchPatternAnd = searchPatternAndTB.Text;
        }

        private void searchPatternOrTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.searchPatternOr = searchPatternOrTB.Text;
        }
    }
}
