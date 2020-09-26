using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RandImg
{
    /// <summary>
    /// Interaction logic for Resize.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        private List<string> basePathStrings;
        private string searchPattern;
        private string excludePattern;
        public ResizeWindow(List<string> in_basePaths, string in_searchPattern = "", string in_excludePattern = "")
        {
            InitializeComponent();
            //WindowStyle = WindowStyle.None; // no border

            basePathStrings = in_basePaths;
            searchPattern = in_searchPattern;
            excludePattern = in_excludePattern;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            DisplayImage dispImg = new DisplayImage(basePathStrings, Left, Top, ActualWidth, ActualHeight, searchPattern, excludePattern);
            dispImg.Show();
            Close();
        }
    }
}
