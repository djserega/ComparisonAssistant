using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    internal class Watcher
    {
        private AvailableNewFileLogEvents _callAvailableNewFileLog;

        internal Watcher(AvailableNewFileLogEvents callAvailableNewFileLog)
        {
            _callAvailableNewFileLog = callAvailableNewFileLog;

            FileInfo fileInfo = new FileInfo(new Parser().LogFileName);

            FileSystemWatcher watcher = new FileSystemWatcher(fileInfo.Directory.FullName)
            {
                EnableRaisingEvents = true,
                Filter = fileInfo.Name
            };
            watcher.Changed += Watcher_Changed;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _callAvailableNewFileLog.EvokeAvailableNewFileLog();
        }
    }
}
