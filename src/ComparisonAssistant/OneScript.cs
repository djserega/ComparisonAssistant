﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComparisonAssistant
{
    internal class OneScript
    {
        private readonly string _prefix = "onescript";
        private readonly string _prefixBin = "bin";
        private readonly string _fileNameOneScript = "oscript.exe";
        private readonly string _fileNameScriptLockObject = "lockobject.os";
        private readonly string _fileNameFileNameListLockObject = "listobject.xml";
        private string _fullNameOneScript;
        private string _fullNameLockObject;
        private string _fullNameListLockObject;

        internal OneScript()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            _fullNameOneScript = Path.Combine(
                basePath,
                _prefix,
                _prefixBin,
                _fileNameOneScript);

            _fullNameLockObject = Path.Combine(
                basePath,
                _prefix,
                _prefixBin,
                _fileNameScriptLockObject);

            _fullNameListLockObject = Path.Combine(
                basePath,
                _prefix,
                _prefixBin,
                _fileNameFileNameListLockObject);
        }

        internal void LockObject(Model.ConnectorStorage connector, List<Model.ChangedFiles> changedFiles)
        {
            FileInfo fileInfo;

            bool error = false;

            fileInfo = new FileInfo(_fullNameOneScript);
            if (!fileInfo.Exists)
            {
                error = true;
                Messages.Show($"Не найден файл: {_fileNameOneScript}");
            }

            fileInfo = new FileInfo(_fullNameLockObject);
            if (!fileInfo.Exists)
            {
                error = true;
                Messages.Show($"Не найден файл: {_fileNameScriptLockObject}");
            }

            if (error)
                return;

            if (!CreateFileLockObject(changedFiles))
                return;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.GetEncoding(866),
                UseShellExecute = false
            };
            process.StartInfo = startInfo;
            process.Start();

            string parameters = $"" +
                $"\"{connector.ConnectionString}\" " +
                $"\"{connector.StoragePath}\" " +
                $"\"{_fullNameListLockObject}\" " +
                $"\"{connector.StorageUser}\" " +
                $"\"{connector.StoragePass}\"";

            using (StreamWriter writer = process.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                    writer.WriteLine($"{ _fullNameOneScript} {_fullNameLockObject} {parameters}");
            }

            string startDebug = "---------lock start---------";
            string endDebug = "---------lock end---------";

            string result = string.Empty;
            bool addToResult = false;
            string line;
            using (StreamReader reader = process.StandardOutput)
            {
                if (reader.BaseStream.CanRead)
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (line.EndsWith(startDebug))
                            addToResult = true;
                        else if (line.EndsWith(endDebug))
                            addToResult = false;
                        else if (addToResult)
                            result += (line + "\n");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(result))
                Messages.Show(result);

        }

        private bool CreateFileLockObject(List<Model.ChangedFiles> changedFiles)
        {

            return true;
        }

    }
}
