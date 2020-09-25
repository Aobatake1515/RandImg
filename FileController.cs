using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandImg
{
    public class FileController
    {
        private readonly string[] acceptableExts = { ".jpg", ".png" };
        private string pattern;
        private Directory[] directories;
        readonly int numMatch; // total number of files that match the pattern
        private int currentFile;
        //private int seed; // seed for random number generation
        private List<int> randomMap;

        public FileController(List<string> baseDirs)
        {
            directories = new Directory[baseDirs.Count];
            numMatch = 0;
            for (int i = 0; i < baseDirs.Count; i++)
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

            // check that file ends in acceptable extension
            bool matchExt = false;
            foreach (string s in acceptableExts)
            {
                if (fileName.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                {
                    matchExt = true;
                }
            }
            if (!matchExt) return false;



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
                int lastDirAmount = -1; // tree amount in last directory scanned
                foreach (string s in System.IO.Directory.GetDirectories(currentPath))
                {
                    Debug.Assert(dirTreeIndex == fileMatchTree.Count); // check that next element is where expected
                    fileMatchTree.Add(0); // add dummy value to be set later
                    int childNodes = SetFileMatchTreeRec(s, dirTreeIndex + 1, maxFileInd); // number of nodes downstream
                    directoryNodes += childNodes + 1; // add one for directory itself
                    fileMatchTree[dirTreeIndex] = (fileMatchTree[dirTreeIndex + childNodes]); // set parent node to last child value
                    maxFileInd = fileMatchTree[dirTreeIndex]; // copy into max file index
                    if (lastDirAmount == fileMatchTree[dirTreeIndex]) // indicates given directory is completely empty
                    {
                        fileMatchTree[dirTreeIndex] = EMPTY_DIR; // set to flag for empty directory
                    }
                    else lastDirAmount = fileMatchTree[dirTreeIndex];
                    dirTreeIndex += childNodes + 1; // add downstream + 1 to get index of next dir if exists
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

                Debug.Assert(true);
                return new List<int> { };
            }

        }

    }
}
