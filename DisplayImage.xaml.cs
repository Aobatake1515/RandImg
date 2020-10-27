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
        private FileController fc;
        private System.Windows.Forms.Timer timer;
        private bool fullScreen = true;
        private Action unminimizeMain;
        public DisplayImage(Settings settings, Action in_unminimizeMain)
        {
            InitializeComponent();
            unminimizeMain = in_unminimizeMain;
            WindowStyle = WindowStyle.None; // no border
            Topmost = true; // always on top
            fullScreen = settings.fullScrn;
            if (settings.fullScrn)
            {
                WindowState = WindowState.Maximized; // fullscreen
            }

            try
            {
                fc = new FileController(settings.basePaths, settings.searchPatternAnd, settings.searchPatternOr, settings.excludePattern);
            }
            catch (Exception e) { throw e; }

            timer = new System.Windows.Forms.Timer();
            timer.Interval = (int)Math.Round(settings.autoDuration * 1000, 0); // convert float seconds to int ms
            timer.Tick += new EventHandler(TimerStart);
            timer.Enabled = settings.autoPlay; // enable from settings

            ChooseNew(true); // init image
        }

        public void SetSize(double left, double top, double width, double height)
        {
            WindowState = WindowState.Normal; // normal
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Topmost = true;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseFull();
            }
            if (e.Key == Key.Right)
            {
                ChooseNew(true);
                ResetTimer();
            }
            if (e.Key == Key.Left)
            {
                ChooseNew(false);
                ResetTimer();
            }
            if (e.Key == Key.Space)
            {
                timer.Enabled = !timer.Enabled;
            }
            if (e.Key == Key.R)
            {
                try
                {
                    Rename rename = new Rename(fc.GetCurrentPath(), ResetFiles);
                    rename.Show();
                }
                catch { }
            }
            // toggle border
            if (e.Key == Key.B)
            {
                if (WindowStyle == WindowStyle.None)
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    WindowStyle = WindowStyle.None;
                }
            }
            // toggle minimize
            if (e.Key == Key.M)
            {
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = fullScreen ? WindowState.Maximized : WindowState.Normal;
                    FillImage();
                }
                else
                {
                    WindowState = WindowState.Minimized;
                }
            }
            // toggle fullscreen
            if (e.Key == Key.F)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    fullScreen = false;
                    FillImage();
                }
                else
                {
                    WindowState = WindowState.Maximized;
                    fullScreen = true;
                    FillImage();
                }
            }
        }

        protected void TimerStart(object Sender, EventArgs e)
        {
            ChooseNew(true);
        }

        private void ResetTimer()
        {
            timer.Enabled = !timer.Enabled;
            timer.Enabled = !timer.Enabled;
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
            bitMapImg.CacheOption = BitmapCacheOption.OnLoad; // allows file to be released
            bitMapImg.EndInit();
            bitMapImg.Freeze();
            image.Source = bitMapImg;
            imageBack.Source = bitMapImg;
        }

        protected void ChooseNew(bool direction)
        {
            try
            {
                NewImage(new Uri(fc.GetNewPath(direction)));
            }
            catch { }
        }

        private void ResetFiles()
        {
            try
            {
                fc.ResetDirectories();
            }
            catch
            {
                System.Windows.MessageBox.Show("No matching files were found :(\n please enter another search");
            }
        }

        public void CloseFull()
        {
            image.Source = null;
            imageBack.Source = null;
            UpdateLayout();
            GC.Collect();

            try
            {
                unminimizeMain();
            }
            catch { }

            Close();
        }
    }
}


