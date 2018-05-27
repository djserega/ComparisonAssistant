using ComparisonAssistant.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace ComparisonAssistant
{
    internal class Parser
    {
        private readonly string _separatorCommit = " --- ";

        internal string LogFileName { get; } = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "log.txt");
        internal DateTime DateCreationFile { get; private set; }
        internal DateTime DateEditedFile { get; private set; }

        internal List<User> Users { get; private set; }
        internal Dictionary<User, List<Task>> UserTasks { get; private set; }

        private List<string> _removePrefixFileName;
        private Dictionary<string, string> _translateObject;
        private List<string> _listSkipFiles;

        internal Parser()
        {
            FileInfo fileInfo = new FileInfo(LogFileName);

            if (!fileInfo.Exists)
                throw new FileNotFoundException("Файл логов не найден.");

            DateCreationFile = fileInfo.CreationTime;
            DateEditedFile = fileInfo.LastWriteTime;

            InitialParser();
        }

        private void InitialParser()
        {
            Users = new List<User>();
            UserTasks = new Dictionary<User, List<Task>>();

            _removePrefixFileName = new List<string>()
            {
                "src/cf/"
            };

            _translateObject = new TranslateObject().DictionaryObjct;

            _listSkipFiles = new List<string>()
            {
                "VERSION",
                "renames.txt",
                "AUTHORS",
                ".gitignore"
            };
        }

        internal void ReadFileLog()
        {
            using (StreamReader reader = new StreamReader(LogFileName))
            {
                string rowFile;
                bool thisCommit;

                User findedUser = null;
                Task userTask = null;
                string userName;
                string taskName;
                string[] file;
                string fileName;

                List<Task> addedTasks = new List<Task>();

                #region Read file log
                while (!reader.EndOfStream)
                {
                    rowFile = reader.ReadLine();

                    thisCommit = new Regex(Regex.Escape(_separatorCommit)).Matches(rowFile).Count == 2;
                    if (thisCommit)
                    {
                        string[] commits = rowFile.Split(new string[] { _separatorCommit }, StringSplitOptions.RemoveEmptyEntries);

                        if (commits.Count() == 3)
                        {
                            userName = commits[0];

                            foreach (Match item in new Regex("DEV-[0-9]*").Matches(commits[1]))
                            {
                                taskName = item.Value;

                                findedUser = Users.FirstOrDefault(f => f.Name == userName);
                                if (findedUser == null)
                                {
                                    findedUser = new User(userName);
                                    Users.Add(findedUser);
                                }

                                userTask = new Task(taskName);
                                if (UserTasks.ContainsKey(findedUser))
                                {
                                    if (UserTasks[findedUser].FirstOrDefault(f => f.Name == taskName) == null)
                                        UserTasks[findedUser].Add(userTask);
                                }
                                else
                                    UserTasks.Add(findedUser, new List<Task>() { userTask });

                                addedTasks.Add(userTask);
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rowFile))
                    {
                        if (findedUser != null)
                        {
                            file = rowFile.Split(new string[] { "\t" }, StringSplitOptions.None);

                            if (file.Count() == 2)
                            {
                                fileName = GetNameObject(file[1]);

                                if (_listSkipFiles.FirstOrDefault(f => f == fileName) == null)
                                {
                                    foreach (Task addedTask in addedTasks)
                                    {
                                        if (UserTasks[findedUser].Find(f => f.Name == addedTask.Name).Files.FirstOrDefault(f => f.FileName == fileName) == null)
                                        {
                                            UserTasks[findedUser].Find(f => f.Name == addedTask.Name).Files.Add(new ChangedFiles(file[0], fileName));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                        addedTasks.Clear();
                }
                #endregion

                foreach (User item in Users)
                {
                    UserTasks[item].Sort((a, b) => a.CompareName(b));
                    for (int i = 0; i < UserTasks[item].Count; i++)
                    {
                        UserTasks[item][i].Files.Sort((a, b) => a.FileName.CompareTo(b.FileName));
                    }
                }

                foreach (User item in Users)
                {
                    for (int i = 0; i < UserTasks[item].Count; i++)
                    {
                        List<ChangedFiles> changedFiles = new List<ChangedFiles>();
                        foreach (ChangedFiles elementFile in UserTasks[item][i].Files)
                        {
                            changedFiles.Add(elementFile);
                        }
                    }
                }
            }
        }

        private string GetNameObject(string fileName)
        {
            foreach (string prefix in _removePrefixFileName)
            {
                fileName = RemoveStartText(fileName, prefix);
            }

            foreach (KeyValuePair<string, string> item in _translateObject)
            {
                fileName = ReplaseStartText(fileName, item, true);
                fileName = ReplaseStartText(fileName, item);
            }

            return fileName;
        }

        private string ReplaseStartText(string text, KeyValuePair<string, string> keyValue, bool addPostfix = false)
        {
            string find = keyValue.Key;
            if (addPostfix)
                find = find + "s";

            if (text.StartsWith(find))
                return keyValue.Value + text.Substring(find.Length);
            else
                return text;
        }

        private string RemoveStartText(string text, string find)
        {
            if (text.StartsWith(find))
                return text.Substring(find.Length);
            else
                return text;
        }

        private string RemoveEndText(string text, string find)
        {
            if (text.EndsWith(find))
                return text.Remove(text.Length - find.Length);
            else
                return text;
        }

        private string RemoveSpace(string text)
        {
            return text.Replace(" ", "");
        }
    }
}
