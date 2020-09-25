using System;
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
        private DisplayImage testImg = null;
        private List<string> basePathStrings = new List<string>{ "C:/Users/alexobatake/source/repos/RandImg/Images/", "C:/Users/alexobatake/source/repos/RandImg/ImagesCopy/" };

        public MainWindow()
        {
            InitializeComponent();
            basePath.Content = GetPathsString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (testImg == null || !testImg.IsLoaded)
            {
                testImg = new DisplayImage(basePathStrings);
            }
            testImg.Show();
        }

        private void DirSelect_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                basePathStrings.Add(folderBrowserDialog.SelectedPath);
                basePath.Content = GetPathsString();
            }
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
    }
}
