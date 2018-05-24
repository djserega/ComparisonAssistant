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
        private string _fullNameOneScript;
        private string _fullNameLockObject;

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
        }

        internal void LockObject(List<Model.ChangedFiles> changedFiles)
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

            Messages.Show("Начало процесса захвата объектов.");

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                //WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.StartInfo = startInfo;
            process.Start();

            using (StreamWriter pWriter = process.StandardInput)
            {
                if (pWriter.BaseStream.CanWrite)
                    pWriter.WriteLine($"{_fullNameOneScript} {_fullNameLockObject}");
            }

            Messages.Show("Процесс захвата объектов завершен.");
        }
    }
}