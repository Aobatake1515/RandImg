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
        private const int searchRetryMax = 10;

        private readonly string[] acceptableExts = { ".jpg", ".png" }; // acceptable file extensions
        // general pattern: prefix~pattern~restofname.extension
        const char PATTERN_DIVIDER = '~'; // seperates pattern from rest of name
        private string searchPatternAnd; // chars that are needed to be accepted, all must be present, null for any available
        private string searchPatternOr; // chars that are needed to be accepted, at lease one must be present, null for any available
        private string excludePattern; // chars that can't be accepted
        private Directory[] directories; // directories to search
        public int numMatch { get; private set;} // total number of files that match the pattern
        private int currentFile; // file index before being randomized
        private List<int> randomMap; // routes currentFile to random value in range; maps all input to all output 1:1

        public FileController(List<string> baseDirs, string in_searchPatternAnd = "", string in_searchPatternOr = "", string in_excludePattern = "")
        {
            directories = new Directory[baseDirs.Count];
            currentFile = 0;
            searchPatternAnd = in_searchPatternAnd;
            searchPatternOr = in_searchPatternOr;
            excludePattern = in_excludePattern;

            numMatch = 0;
            for (int i = 0; i < baseDirs.Count; i++)
            {
                directories[i] = new Directory(baseDirs[i], IsValid);
                numMatch += directories[i].numMatch;
            }
            if (numMatch == 0)
            {
                throw new Exception("FileController: No matching files were found in any directory");
            }

            GenerateRandomMap();
        }

        public void ResetDirectories()
        {
            numMatch = 0;
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = new Directory(directories[i].basePath, IsValid);
                numMatch += directories[i].numMatch;
            }
            if (numMatch == 0)
            {
                throw new Exception("FileController: No matching files were found in any directory");
            }

            GenerateRandomMap();
        }

        /// <summary>
        /// generates a random map between integers between 0 and numMatch - 1, all numbers included once
        /// </summary>
        private void GenerateRandomMap()
        {
            randomMap = new List<int>(numMatch);
            for (int i = 0; i < numMatch; i++)
            {
                randomMap.Add(i);
            }

            Random rng = new Random();
            for (int i = numMatch - 1; i > 0; i--) // Fisher-Yates shuffle
            {
                int j = rng.Next(i);
                int temp = randomMap[i];
                randomMap[i] = randomMap[j];
                randomMap[j] = temp;
            }
        }

        /// <summary>
        /// gets path for new randomized image
        /// </summary>
        /// <param name="direction">true = foreward, false = backwards</param>
        /// <returns>string path of image</returns>
        public string GetNewPath(bool direction)
        {
            int targetVal = direction ? NextRandom() : LastRandom();
            return GetPath(targetVal);
        }

        public string GetCurrentPath()
        {
            return GetPath(CurrentIndex());
        }

        public string GetPath(int targetVal)
        {
            for (int i = 0, numMatchTot = 0; i < directories.Length; i++)
            {
                numMatchTot += directories[i].numMatch;
                if (targetVal < numMatchTot)
                {
                    // offset by numMatch - numMatchTot to normalize later dir indicies to start from 0 in dir
                    return directories[i].GetImgPath(targetVal + directories[i].numMatch - numMatchTot);
                }
            }
            // not found somehow
            string errorMsg = "GetPath: failed to find path";
            Debug.Assert(false, errorMsg);
            throw new Exception(errorMsg);
        }


        public bool IsValid(string fileName)
        {
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

            // add dimensions testing here if desired later

            string localName = System.IO.Path.GetFileName(fileName);
            string filePattern = ExtractPattern(localName);

            // return false if any exclusions match
            if (excludePattern != "")
            {
                foreach (char c in excludePattern)
                {
                    if (c == 0) break; // check null terminator
                    if (filePattern.Contains(c))
                    {
                        return false;
                    }
                }
            }


            // sieve for each filePattern char in searchPatternAnd
            if (searchPatternAnd != "")
            {
                foreach (char c in searchPatternAnd)
                {
                    if (c == 0) break; // check null terminator
                    if (!filePattern.Contains(c))
                    {
                        return false;
                    }
                }
            }

            // sieve for at least one filePattern char in searchPatternOr
            if (searchPatternAnd != "")
            {
                bool found = false;
                foreach (char c in searchPatternOr)
                {
                    if (c == 0) break; // check null terminator
                    if (filePattern.Contains(c))
                    {
                        found = true;
                    }
                }
                if (!found) return false;
            }


            // valid if not shown to be invalid
            return true;
        }

        private static string ExtractPattern(string localName)
        {
            string filePattern = "";
            bool inFilter = false;
            foreach (char c in localName)
            {
                if (c == 0) break; // check null terminator
                if (c.Equals('~'))
                {
                    if (!inFilter) inFilter = true;
                    else break;
                }
                else if (inFilter)
                {
                    filePattern += c;
                }
            }
            return filePattern;
        }

        private int NextRandom()
        {
            currentFile++;
            if (currentFile >= numMatch) currentFile -= numMatch;
            return CurrentIndex();
        }

        private int LastRandom()
        {
            currentFile--;
            if (currentFile < 0) currentFile += numMatch;
            return CurrentIndex();
        }

        private int CurrentIndex()
        {
            return randomMap[currentFile];
        }


        private class Directory
        {
            public string basePath { get; } // absolute path of the directory
            public readonly int numMatch; // number of matches total
            private Func<string, bool> isValid; // delegate for whether given file is valid given selection pattern
            private List<int> fileMatchTree; // holds the number of elements at each folder or directory in specialized postfix notation

            public Directory(string in_basePath, Func<string, bool> in_isValid)
            {
                basePath = in_basePath;
                isValid = in_isValid;
                fileMatchTree = new List<int>();
                SetFileMatchTree();
                numMatch = fileMatchTree.Max<int>();
            }

            public string GetImgPath(int fileNum)
            {
                return GetImgPath(fileNum, basePath, TreeSearch(fileMatchTree, fileNum), 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileNum">index of the target file</param>
            /// <param name="currentPath">path of current directory</param>
            /// <param name="pathToFile">indicates where to look for file, 0 = current dir, 1+ = internal dirs</param>
            /// <param name="index">index in pathToFile, depth in tree</param>
            /// <returns></returns>
            public string GetImgPath(int fileNum, string currentPath, List<int> pathToFile, int index)
            {
                // check for file number too large
                if (fileNum >= numMatch)
                {
                    Debug.Assert(false);
                    return null;
                }

                // if file is at this directory level
                if (pathToFile[index] == 0) 
                {
                    for (int i = 0; i < searchRetryMax; i++) // retry #times
                    {
                        string[] filePaths = System.IO.Directory.GetFiles(currentPath);
                        List<string> validPaths = new List<string>();
                        foreach (string s in filePaths) // filter for valid paths
                        {
                            if (isValid(s)) validPaths.Add(s);
                        }
                        // check that valid files exist
                        if (validPaths.Count != 0)
                        {
                            return validPaths[fileNum % validPaths.Count];
                        }
                    }
                    string errorMsg = "GetImgPath: could not find specified file";
                    Debug.Assert(false, errorMsg);
                    throw new Exception(errorMsg);
                }
                // if in directories
                else if (pathToFile[index] > 0)
                {
                    string[] dirPaths = System.IO.Directory.GetDirectories(currentPath);
                    return GetImgPath(fileNum, dirPaths[pathToFile[index] - 1], pathToFile, index + 1);
                }

                // path shouldn't be negative
                Debug.Assert(false);
                return null;
            }

            /// <summary>
            /// Initializes the file match tree for the directory
            /// </summary>
            private void SetFileMatchTree()
            {
                //try
                {
                    SetFileMatchTreeRec(basePath, 0, 0, 0);
                }
                //catch
                //{
                //    MessageBox.Show("Error in SetFileMatchTree");
                //    // error handling later?
                //}
            }

            /// <summary>
            /// Recursive helper to set file match tree
            /// </summary>
            /// <param name="currentPath">path for this dir</param>
            /// <param name="treeIndex">index in file match tree of the files of this dir</param>
            /// <param name="totFiles">total files up to this directory</param>
            /// <param name="depth">level of the directory in tree, starts at 0</param>
            private void SetFileMatchTreeRec(string currentPath, int treeIndex, int totFiles, int depth) // returns the number of nodes in path
            {

                int fileMatch = 0; // files matched in this dir
                foreach (string s in System.IO.Directory.GetFiles(currentPath))
                {
                    if (isValid(s)) fileMatch++;
                }
                Debug.Assert(treeIndex == fileMatchTree.Count); // check that next element is where expected
                fileMatchTree.Add(fileMatch + totFiles); // found files + files up to this point
                totFiles = fileMatchTree[treeIndex]; // copy into total files, same as totFiles += fileMatch

                int dirTreeIndex = treeIndex + 1; // index of the next directory's root node
                int lastDirAmount = -1; // total matched files in last directory scanned
                foreach (string s in System.IO.Directory.GetDirectories(currentPath)) // loop all directories
                {
                    Debug.Assert(dirTreeIndex == fileMatchTree.Count); // check that next element is where expected
                    fileMatchTree.Add(0); // add dummy value to be set later, added at index dirTreeIndex
                    SetFileMatchTreeRec(s, dirTreeIndex + 1, totFiles, depth + 1); // recursively set downstream tree
                    int lastTreeIndex = fileMatchTree.Count - 1; // index of current last element
                    fileMatchTree[dirTreeIndex] = fileMatchTree[lastTreeIndex]; // set parent node to last child value, which is equal to the total num files
                    totFiles = fileMatchTree[dirTreeIndex]; // update the total files
                    // directory is empty if the total size with it = total size before it
                    if (lastDirAmount == fileMatchTree[dirTreeIndex]) // indicates given directory is completely empty
                    {
                        fileMatchTree[dirTreeIndex] = -1 * depth; // set to sentinal negative val with info for depth
                    }
                    else lastDirAmount = fileMatchTree[dirTreeIndex]; // update last total size
                    dirTreeIndex = lastTreeIndex + 1; // set next file index to one more than last current index
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tree">tree of file match tree</param>
            /// <param name="target">target value to search for</param>
            /// <param name="index">current index in the tree list</param>
            /// <param name="depth">level of the directory in tree, starts at 0</param>
            /// <returns>list describing how to get to target value</returns>
            private static List<int> TreeSearch(List<int> tree, int target, int index = 0, int depth = 0)
            {
                // check if file is in files of this dir
                if (target < tree[index] && tree[index] > 0)
                {
                    return new List<int> { 0 }; // 0 indicates files in this dir
                }
                for (int i = index + 1, dir = 1, threshold = 0; i < tree.Count; i++) // loop through dirs in this dir
                {
                    // check if the tree target is in this dir
                    if (target < tree[i])
                    {
                        var retVal = TreeSearch(tree, target, i + 1, depth + 1); // save directions for inside dir
                        retVal.Insert(0, dir); // front add
                        return retVal;
                    }
                    // check if this dir exceeds the highest value scanned
                    if (tree[i] > threshold)
                    {
                        threshold = tree[i];
                        dir++;
                    }
                    // check if an empty dir at this depth is found
                    else if (tree[i] != 0 && tree[i] == -1 * depth) // cant be 0, check if inverted depth val
                    {
                        dir++;
                    }
                    // else just increment i
                }

                // target not found for some reason
                Debug.Assert(false);
                return new List<int> { };
            }

        }

    }
}
