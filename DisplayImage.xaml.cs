using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DisplayImage : Window
    {
        //private string[] pics;
        //private int currentPic = 0;
        //private string basePath;
        //private Uri baseUri;
        private FileController fc;
        public DisplayImage(List<string> in_basePaths)
        {
            InitializeComponent();
            FillImage();
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;

            //basePath = in_basePath;
            //baseUri = new Uri(basePath);

            //pics = System.IO.Directory.GetFiles(basePath);

            fc = new FileController(in_basePaths);
            ChooseNew(true);
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Right || e.Key == Key.Space)
            {

                ChooseNew(true);
            }
            if (e.Key == Key.Left)
            {
                ChooseNew(false);
            }
        }


        protected void FillImage()
        {
            image.Width = Width;
            image.Height = Height;
            imageBack.Width = Width;
            imageBack.Height = Height;
        }

        protected void NewImage(Uri uri)
        {
            FillImage();
            BitmapImage bitMapImg = new BitmapImage();
            bitMapImg.BeginInit();
            bitMapImg.UriSource = uri;
            bitMapImg.EndInit();
            image.Source = bitMapImg;
            imageBack.Source = bitMapImg;
        }

        protected void ChooseNew(bool direction)
        {
            NewImage(new Uri(fc.GetNewPath(direction)));
        }

    }
}


