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
    }
}
