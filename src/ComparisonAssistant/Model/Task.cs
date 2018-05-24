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

        internal int CompareName(Task b)
        {
            int idA = Convert.ToInt32(Name.Replace("DEV-", ""));
            int idB = Convert.ToInt32(b.Name.Replace("DEV-", ""));

            return -idA.CompareTo(idB);
        }
    }
}
