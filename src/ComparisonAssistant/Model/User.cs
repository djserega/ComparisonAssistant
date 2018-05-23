using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonAssistant.Model
{
    public class User
    {
        public string Name { get; set; }

        public User(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
