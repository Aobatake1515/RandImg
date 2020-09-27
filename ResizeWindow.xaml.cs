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
        private DisplayImage dispImg;
        public ResizeWindow(DisplayImage in_dispImg)
        {
            InitializeComponent();
            //WindowStyle = WindowStyle.None; // no border

            dispImg = in_dispImg;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            dispImg.Show();
            Close();
        }
    }
}
