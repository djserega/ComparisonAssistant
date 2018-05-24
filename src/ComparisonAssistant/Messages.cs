using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComparisonAssistant
{
    internal static class Messages
    {
        internal static void Show(string text)
        {
            Task.Run(() => MessageBox.Show(text));
        }
    }
}
