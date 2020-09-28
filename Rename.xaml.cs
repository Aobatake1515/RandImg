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
    /// Interaction logic for Rename.xaml
    /// </summary>
    public partial class Rename : Window
    {
        Action refreshTree;
        string fileName;
        public Rename(string in_fileName, Action in_refreshTree)
        {
            refreshTree = in_refreshTree;

            InitializeComponent();
            Topmost = true;
            fileName = in_fileName;
            string localName = System.IO.Path.GetFileName(fileName);
            oldNameTextLbl.Content = localName;
            newNameTB.Text = localName;
            
            {
                Uri uri = new Uri(fileName);
                BitmapImage bitMapImg = new BitmapImage();
                bitMapImg.BeginInit();
                bitMapImg.UriSource = uri;
                bitMapImg.CacheOption = BitmapCacheOption.OnLoad; // allows file to be released
                bitMapImg.EndInit();
                bitMapImg.Freeze();
                image.Source = bitMapImg;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((string)oldNameTextLbl.Content == newNameTB.Text)
            {
                Close();
                return;
            }

            string directory = System.IO.Path.GetDirectoryName(fileName);
            string newFileName = System.IO.Path.Combine(directory, newNameTB.Text);
            try
            {
                System.IO.File.Move(fileName, newFileName);
            }
            catch 
            {
                MessageBox.Show("File could not be renamed at this time.");
                return;
            }
            try { refreshTree(); } catch { }
            Close();
        }

        private void moveBtn_Click(object sender, RoutedEventArgs e)
        {

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = string.Format("Image file (*.{0})|*.{0}", System.IO.Path.GetExtension(fileName));
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(fileName);
            saveFileDialog.FileName = System.IO.Path.GetFileName(fileName);

            var result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.Move(fileName, saveFileDialog.FileName);
                try { refreshTree(); } catch { }
                Close();
            }
        }

        public void CloseFull()
        {
            image.Source = null;
            UpdateLayout();
            GC.Collect();
            Close();
        }
    }
}
