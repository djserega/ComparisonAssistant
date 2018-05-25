using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    internal delegate void AvailableNewFileLog();

    internal class AvailableNewFileLogEvents : EventArgs
    {
        internal event AvailableNewFileLog AvailableNewFileLog;

        internal void EvokeAvailableNewFileLog()
        {
            if (AvailableNewFileLog == null)
                return;

            AvailableNewFileLog();
        }
    }
}
