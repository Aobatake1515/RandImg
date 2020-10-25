using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandImg
{
    public class Settings
    {
        public const float minAutoDuration = 0.1f; // shortest allowed duration, seconds

        public List<string> basePaths { get; set; }
        public string searchPatternAnd { get; set; }
        public string searchPatternOr { get; set; }
        public string excludePattern { get; set; }
        public bool fullScrn { get; set; }
        public bool autoPlay { get; set; }
        public float autoDuration { get; set; } // in seconds

        public Settings
            (
            List<string> in_basePaths,
            string in_searchPatternAnd = "",
            string in_searchPatternOr = "",
            string in_excludePattern = "",
            bool in_fullScrn = true,
            bool in_autoPlay = false,
            float in_autoDuration = 5.000f
            )
        {
            basePaths = in_basePaths;
            searchPatternAnd = in_searchPatternAnd;
            searchPatternOr = in_searchPatternOr;
            excludePattern = in_excludePattern;
            fullScrn = in_fullScrn;
            autoPlay = in_autoPlay;
            autoDuration = in_autoDuration;
        }
        public Settings() : this(new List<string>()) { }
    }
}
