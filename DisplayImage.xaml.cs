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
        public DisplayImage(string[] in_basePaths)
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
            if (e.Key == Key.Space)
            {
                Size size = this.RenderSize;
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens.First<System.Windows.Forms.Screen>();
                string s1 = string.Format("Width: {0:f4} Height: {1:f4}", image.Width, image.Height);
                string s2 = string.Format("Width: {0:f4} Height: {1:f4}", image.ActualWidth, image.ActualHeight);
                //string s2 = string.Format("Width: {0:d} Height: {1:d}", screen.WorkingArea.Width, screen.WorkingArea.Height);
                string s3 = string.Format("Width: {0:f4} Height: {1:f4}", size.Width, size.Height);
                MessageBox.Show(string.Format("{0}\n{1}\n{2}\n", s1, s2, s3));
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

    public class FileController
    {
        private string pattern;
        private Directory[] directories;
        readonly int numMatch; // total number of files that match the pattern
        private int currentFile;
        //private int seed; // seed for random number generation
        private List<int> randomMap;
        
        public FileController (string[] baseDirs)
        {
            directories = new Directory[baseDirs.Length];
            numMatch = 0;
            for (int i = 0; i < baseDirs.Length; i++)
            {
                directories[i] = new Directory(baseDirs[i], IsValid);
                numMatch += directories[i].numMatch;
            }
            randomMap = new List<int>(numMatch);
            for (int i = 0; i < numMatch; i++)
            {
                randomMap.Add(i);
            }
            Random rng = new Random();
            for (int i = numMatch - 1; i > 0; i--)
            {
                int j = rng.Next(i);
                int temp = randomMap[i];
                randomMap[i] = randomMap[j];
                randomMap[j] = temp;
            }
            currentFile = 0;
            //seed = new Random().Next();
        }

        public string GetNewPath(bool direction)
        {
            int targetVal = direction ? NextRandom() : LastRandom();
            for (int i = 0, numMatchTot = 0; i < directories.Length; i++)
            {
                numMatchTot += directories[i].numMatch;
                if (targetVal < numMatchTot)
                {
                    return directories[i].GetImgPath(targetVal + directories[i].numMatch - numMatchTot);
                }
            }
            // error
            return null;
        }

        public bool IsValid(string fileName)
        {
            // DUMMY CODE
            return true;
        }

        private int NextRandom()
        {
            currentFile++;
            if (currentFile >= numMatch) currentFile -= numMatch;
            return randomMap[currentFile];
        }

        private int LastRandom()
        {
            currentFile--;
            if (currentFile < 0) currentFile += numMatch;
            return randomMap[currentFile];
        }


        private class Directory
        {
            const int EMPTY_DIR = -1; // sentinal marker value for empty directories in fileMatchTree
            public string basePath { get; }
            public int numMatch { get; }
            private Func<string, bool> isValid;
            private List<int> fileMatchTree; // holds highest index at tree node

            public Directory(string in_basePath, Func<string, bool> in_isValid)
            {
                basePath = in_basePath;
                isValid = in_isValid;
                fileMatchTree = new List<int>();
                SetFileMatchTree();
                numMatch = fileMatchTree.Max<int>() + 1; // add 1 since size = index + 1
            }

            public string GetImgPath(int fileNum) 
            {
                //try
                {
                    return GetImgPath(fileNum, basePath, 0, TreeSearch(fileMatchTree, fileNum));
                } 
                //catch (Exception e)
                //{
                //    MessageBox.Show("Error in GetImgPath\n" + e.ToString());
                //    return null;
                //}
            }
            public string GetImgPath(int fileNum, string currentPath, int index, List<int> pathToFile)
            {
                if (fileNum >= numMatch) return null;

                if (pathToFile[index] == 0)
                {
                    string[] filePaths = System.IO.Directory.GetFiles(currentPath);
                    List<string> validPaths = new List<string>();
                    foreach (string s in filePaths)
                    {
                        if (isValid(s)) validPaths.Add(s);
                    }
                    if (filePaths.Length == 0) return null;
                    return validPaths[fileNum % validPaths.Count];
                }
                else if (pathToFile[index] > 0)
                {
                    string[] dirPaths = System.IO.Directory.GetDirectories(currentPath);
                    return GetImgPath(fileNum, dirPaths[pathToFile[index] - 1], index + 1, pathToFile);
                }
                return null;
            }

            private int GetNumMatch(string currentPath)
            {
                int retVal = 0;
                foreach (string s in System.IO.Directory.GetFiles(currentPath))
                {
                    if (isValid(s))
                    {
                        retVal++;
                    }
                }
                foreach (string s in System.IO.Directory.GetDirectories(currentPath))
                {
                    retVal += GetNumMatch(s);
                }
                return retVal;
            }

            private void SetFileMatchTree()
            {
                //try
                {
                    SetFileMatchTreeRec(basePath, 0, -1); // start index at -1 since no items present
                }
                //catch
                //{
                //    MessageBox.Show("Error in SetFileMatchTree");
                //    // error handling later?
                //}
            }

            private int SetFileMatchTreeRec(string currentPath, int treeIndex, int maxFileInd) // returns the number of nodes in path
            {
                int fileMatch = 0; // files matched
                foreach (string s in System.IO.Directory.GetFiles(currentPath))
                {
                    if (isValid(s)) fileMatch++;
                }
                Debug.Assert(treeIndex == fileMatchTree.Count); // check that next element is where expected
                fileMatchTree.Add(fileMatch + maxFileInd); // -1 because it holds max index, not size
                maxFileInd = fileMatchTree[treeIndex]; // copy into max file index

                int directoryNodes = 0; // number of total nodes associated with directories
                int dirTreeIndex = treeIndex + 1; // index of the directory's root node
                int lastChildNodes = -1; // number of nodes downstream from directory last iteration
                foreach (string s in System.IO.Directory.GetDirectories(currentPath))
                {
                    Debug.Assert(dirTreeIndex == fileMatchTree.Count); // check that next element is where expected
                    fileMatchTree.Add(0); // add dummy value to be set later
                    int childNodes = SetFileMatchTreeRec(s, dirTreeIndex + 1, maxFileInd); // number of nodes downstream
                    directoryNodes += childNodes + 1; // add one for directory itself
                    fileMatchTree[dirTreeIndex] = (fileMatchTree[dirTreeIndex + childNodes]); // set parent node to last child value
                    maxFileInd = fileMatchTree[dirTreeIndex]; // copy into max file index
                    dirTreeIndex += childNodes + 1; // add downstream + 1 to get index of next dir if exists
                    if (lastChildNodes == childNodes) // indicates given directory is completely empty
                    {
                        fileMatchTree[dirTreeIndex] = EMPTY_DIR; // set to flag for empty directory
                    }
                    else lastChildNodes = childNodes;
                }
                return 1 + directoryNodes; // one set of files matched + number in directories

            }

            private static List<int> TreeSearch(List<int> tree, int target, int index = 0)
            {
                if (target <= tree[index])
                {
                    return new List<int> { 0 };
                }
                for (int i = index + 1, dir = 1, threshold = 0; i < tree.Count; i++)
                {
                    if (target <= tree[i])
                    {
                        var retVal = TreeSearch(tree, target, i + 1);
                        retVal.Insert(0, dir);
                        return retVal;
                    }
                    if (tree[i] > threshold)
                    {
                        threshold = tree[i];
                        dir++;
                    }
                    else if (tree[i] == EMPTY_DIR) // if value = sentinal value for empty directory
                    {
                        dir++;
                    }
                    // uncaught empty directories would not increment the directory number
                }

                return new List<int> { };
            }

        }

    }
}


