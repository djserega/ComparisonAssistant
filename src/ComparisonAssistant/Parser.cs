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

        internal DateTime? DateMin { get; set; }
        internal DateTime? DateMax { get; set; }

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
            DateTime filterDateMin = DateMin == null ? DateTime.MinValue : (DateTime)DateMin;
            DateTime filterDateMax = DateMax == null || DateMax == DateTime.MinValue ? DateTime.MaxValue : (DateTime)DateMax;


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

                string emptyTask = "<без задачи>";

                List<Task> addedTasks = new List<Task>();

                #region Read file log
                while (!reader.EndOfStream)
                {
                    rowFile = reader.ReadLine();

                    thisCommit = new Regex(Regex.Escape(_separatorCommit)).Matches(rowFile).Count == 2;
                    if (thisCommit)
                    {
                        #region Read commit
                        string[] commitParts = rowFile.Split(new string[] { _separatorCommit }, StringSplitOptions.RemoveEmptyEntries);

                        if (commitParts.Count() == 3)
                        {
                            DateTime dateCommit;
                            try
                            {
                                dateCommit = DateTime.Parse(commitParts[2]);
                            }
                            catch (FormatException)
                            {
                                dateCommit = DateTime.MinValue;
                            }

                            if (!(filterDateMin <= dateCommit && dateCommit <= filterDateMax))
                                continue;

                            userName = commitParts[0];

                            findedUser = Users.FirstOrDefault(f => f.Name == userName);
                            if (findedUser == null)
                            {
                                findedUser = new User(userName);
                                Users.Add(findedUser);
                            }

                            MatchCollection matches = new Regex("DEV-[0-9]*").Matches(commitParts[1]);
                            if (matches.Count > 0)
                            {
                                foreach (Match item in matches)
                                {
                                    AddUserTask(findedUser, item.Value, addedTasks);
                                }
                            }
                            else
                            {
                                AddUserTask(findedUser, emptyTask, addedTasks);
                            }
                        }
                        #endregion
                    }
                    else if (!string.IsNullOrWhiteSpace(rowFile))
                    {
                        #region Read log
                        if (findedUser != null)
                        {
                            file = rowFile.Split(new string[] { "\t" }, StringSplitOptions.None);

                            if (file.Count() == 2 || file.Count() == 3)
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
                        #endregion
                    }
                    else
                    {
                        findedUser = null;
                        addedTasks.Clear();
                    }
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

                ChangedFiles elementObject;
                foreach (User item in Users)
                {
                    for (int i = 0; i < UserTasks[item].Count; i++)
                    {
                        List<ChangedFiles> updateChangedFiles = new List<ChangedFiles>();
                        foreach (ChangedFiles elementFile in UserTasks[item][i].Files)
                        {
                            if (elementFile.EndFileBSL || elementFile.EndFileXML)
                            {
                                elementObject = updateChangedFiles.FirstOrDefault(f => f.FileNameWithoutExtension == elementFile.FileNameWithoutLastPart);
                                if (elementObject == null)
                                    updateChangedFiles.Add(elementFile);
                                else
                                {
                                    elementObject.CompareObject(elementFile);
                                }
                            }
                            else
                                updateChangedFiles.Add(elementFile);
                        }

                        UserTasks[item][i].Files = updateChangedFiles;
                    }
                }
            }
        }

        private Task AddUserTask(User findedUser, string taskName, List<Task> addedTasks)
        {
            Task userTask = new Task(taskName);
            if (UserTasks.ContainsKey(findedUser))
            {
                if (UserTasks[findedUser].FirstOrDefault(f => f.Name == taskName) == null)
                    UserTasks[findedUser].Add(userTask);
            }
            else
                UserTasks.Add(findedUser, new List<Task>() { userTask });

            addedTasks.Add(userTask);
            return userTask;
        }

        private string GetNameObject(string fileName)
        {
            foreach (string prefix in _removePrefixFileName)
            {
                fileName = fileName.RemoveStartText(prefix);
            }

            foreach (KeyValuePair<string, string> item in _translateObject)
            {
                fileName = fileName.ReplaseStartText(item, true);
                fileName = fileName.ReplaseStartText(item);
            }

            return fileName;
        }

    }
}
