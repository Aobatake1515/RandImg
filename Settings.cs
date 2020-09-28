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
        public string searchPatternAnd { get; set; }
        public string searchPatternOr { get; set; }
        public string excludePattern { get; set; }
        public bool fullScrn { get; set; }

        public Settings(List<string> in_basePaths, string in_searchPatternAnd = "", string in_searchPatternOr = "", string in_excludePattern = "", bool in_fullScrn = true)
        {
            basePaths = in_basePaths;
            searchPatternAnd = in_searchPatternAnd;
            searchPatternOr = in_searchPatternOr;
            excludePattern = in_excludePattern;
            fullScrn = in_fullScrn;
        }
        public Settings() : this(new List<string>()) { }
    }
}
