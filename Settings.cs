using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandImg
{
    public class Settings
    {
        public List<string> basePaths { get; set; }
        public string searchPattern { get; set; }
        public string excludePattern { get; set; }
        public bool fullScrn { get; set; }

        public Settings(List<string> in_basePaths, string in_searchPattern = "", string in_excludePattern = "", bool in_fullScrn = true)
        {
            basePaths = in_basePaths;
            searchPattern = in_searchPattern;
            excludePattern = in_excludePattern;
            fullScrn = in_fullScrn;
        }
        public Settings() : this(new List<string>()) { }
    }
}
