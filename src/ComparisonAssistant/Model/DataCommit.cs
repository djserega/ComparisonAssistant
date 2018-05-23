using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonAssistant.Model
{
    internal class DataCommit
    {
        internal User User { get; set; }
        internal Task Task { get; set; }
        internal ChangedFiles[] Files { get; set; }
    }
}
