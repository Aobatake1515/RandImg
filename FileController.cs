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
        private readonly string[] acceptableExts = { ".jpg", ".png" }; // acceptable file extensions
        // general pattern: prefix~pattern~restofname.extension
        private const char PATTERN_DIVIDER = '~'; // seperates pattern from rest of name
        private string searchPatternAnd; // chars that are needed to be accepted, all must be present, null for any available
        private string searchPatternOr; // chars that are needed to be accepted, at lease one must be present, null for any available
        private string excludePattern; // chars that can't be accepted
        private List<string> baseDirs; // base directories to search
        private List<string> filenames; // list of filenames for matched files as strings
        public int numMatch { get { return filenames.Count; } } // total number of files that match the pattern
        private int currentFile; // file index before being randomized
        private List<int> randomMap; // routes currentFile to random value in range; maps all input to all output 1:1




        public FileController(List<string> in_baseDirs, string in_searchPatternAnd = "", string in_searchPatternOr = "", string in_excludePattern = "")
        {
            currentFile = 0;
            baseDirs = new List<string>(in_baseDirs);
            searchPatternAnd = in_searchPatternAnd;
            searchPatternOr = in_searchPatternOr;
            excludePattern = in_excludePattern;

            filenames = new List<string>();

            ResetDirectories();
        }

        /// <summary>
        /// Sets the file names of matched files into filenames for each of the base directories
        /// </summary>
        private void SetFilenames()
        {
            Debug.Assert(filenames.Count == 0); // assumed to be empty at this point

            foreach (string s in baseDirs)
            {
                SetFilenames(s);
            }
        }

        /// <summary>
        /// recursive helper to set file names of the current dir into filenames
        /// </summary>
        /// <param name="currentPath"> current directory being searched</param>
        private void SetFilenames(string currentPath)
        {
            foreach (string s in System.IO.Directory.GetFiles(currentPath)) // loop all local files
            {
                if (IsValid(s)) filenames.Add(s);
            }
            foreach (string s in System.IO.Directory.GetDirectories(currentPath)) // loop all directories
            {
                SetFilenames(s); // recurse
            }
        }

        public void ResetDirectories()
        {
            filenames.Clear();
            SetFilenames();
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

        /// <summary>
        /// gets the current path
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPath()
        {
            return GetPath(CurrentIndex());
        }

        /// <summary>
        /// gets the path at the given index, checks for validity, throws if invalid
        /// </summary>
        /// <param name="targetVal">index intended in filenames</param>
        /// <returns>file name of the requested file with full path</returns>
        public string GetPath(int targetVal)
        {
            if (targetVal < filenames.Count && targetVal >= 0)
            {
                return filenames[targetVal];
            }
            else
            {
                // not found somehow
                string errorMsg = "GetPath: failed to find path";
                Debug.Assert(false, errorMsg);
                throw new Exception(errorMsg);
            }
        }

        /// <summary>
        /// checks whether the filename given is valid for the selection constraints
        /// </summary>
        /// <param name="fileName">name of file to check</param>
        /// <returns>whether it maches constraints</returns>
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
            if (searchPatternOr != "")
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

        /// <summary>
        /// gets the pattern from the given local filename
        /// </summary>
        /// <param name="localName">file name without path</param>
        /// <returns>extracted selection patten (text between ~~)</returns>
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

        /// <summary>
        /// gets next deterministic random value, wraps around numMatch
        /// </summary>
        /// <returns></returns>
        private int NextRandom()
        {
            currentFile++;
            if (currentFile >= numMatch) currentFile -= numMatch;
            return CurrentIndex();
        }

        /// <summary>
        /// gets previous deterministic random value, wraps around numMatch
        /// </summary>
        /// <returns></returns>
        private int LastRandom()
        {
            currentFile--;
            if (currentFile < 0) currentFile += numMatch;
            return CurrentIndex();
        }

        /// <summary>
        /// index of the current file selected
        /// </summary>
        /// <returns></returns>
        private int CurrentIndex()
        {
            return randomMap[currentFile];
        }
    }
}
