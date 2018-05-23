using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonAssistant.Model
{
    public class Task
    {
        public string Name { get; set; }
        public List<ChangedFiles> Files { get; set; }

        public Task(string name)
        {
            Name = name;
            Files = new List<ChangedFiles>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
