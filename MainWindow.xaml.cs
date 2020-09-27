﻿using System;
using System.Collections.Generic;
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
        private List<string> basePathStrings = new List<string>
        { 
            "C:/Users/alexobatake/source/repos/RandImg/Images",
            "C:/Users/alexobatake/source/repos/RandImg/ImagesCopy",
            "C:/Users/alexobatake/source/repos/RandImg/Images_testPatterns",
            "F:\\bup\\Win Backups\\12-28-18\\New folder\\小鸟酱30G"
        }; // default paths to search

        public MainWindow()
        {
            InitializeComponent();
            basePathsLB.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            RefreshListbox();

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
                        basePathStrings.Add(fullPath);
                    }
                    // files
                    else if ((fileAttributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        basePathStrings.Add(System.IO.Path.GetDirectoryName(fullPath));
                    }
                    RefreshListbox();
                }
            }
        }
        private void LoadPresets_Drop(object sender, System.Windows.DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayImage dispImg = new DisplayImage(basePathStrings, searchPatternTB.Text, excludePatternTB.Text);

                if ((bool)fullScreenRB.IsChecked)
                {
                    dispImg.Show();
                }
                else if ((bool)resizeRB.IsChecked)
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
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                basePathStrings.Add(folderBrowserDialog.SelectedPath);
                RefreshListbox();
            }
        }

        private void DirRemove_Click(object sender, RoutedEventArgs e)
        {
            basePathStrings.RemoveAt(basePathsLB.SelectedIndex);
            RefreshListbox();
        }

        private string GetPathsString()
        {
            string retVal = "";
            for (int i = 0; i < basePathStrings.Count; i++)
            {
                retVal += basePathStrings[i];
                if (i + 1 < basePathStrings.Count)
                {
                    retVal += "\n";
                }

            }
            return retVal;
        }

        private void RefreshListbox()
        {
            basePathsLB.BeginInit();
            basePathsLB.Items.Clear();
            foreach (string s in basePathStrings)
            {
                basePathsLB.Items.Add(s);
            }
            basePathsLB.EndInit();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void fullScreenRB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void resizeRB_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
