﻿using System;
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
        private FileController fc;
        public DisplayImage(List<string> in_basePaths)
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None; // no border
            WindowState = WindowState.Maximized; // fullscreen

            fc = new FileController(in_basePaths);
            ChooseNew(true); // init image
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

        /// <summary>
        /// fill images to screen size
        /// </summary>
        protected void FillImage()
        {
            image.Width = Width;
            image.Height = Height;
            imageBack.Width = Width;
            imageBack.Height = Height;
        }

        /// <summary>
        /// set new image based on Uri
        /// </summary>
        /// <param name="uri">Uri for image</param>
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


