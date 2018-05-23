using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonAssistant.Model
{
    public class ChangedFiles
    {
        public string Status { get; set; }
        public string FileName { get; set; }

        public ChangedFiles(string status, string fileName)
        {
            Status = status;
            FileName = fileName;
        }
    }
}
