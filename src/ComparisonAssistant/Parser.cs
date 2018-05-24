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
        private readonly string _logFileName = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "log.txt");
        private readonly string _separatorCommit = " --- ";

        internal DateTime DateCreationFile { get; private set; }
        internal DateTime DateEditedFile { get; private set; }

        //internal DataCommit DataCommit { get; private set; }
        internal List<User> Users { get; private set; }
        internal Dictionary<User, List<Task>> UserTasks { get; private set; }

        internal Parser()
        {
            FileInfo fileInfo = new FileInfo(_logFileName);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Файл логов не найден.");
            }

            DateCreationFile = fileInfo.CreationTime;
            DateEditedFile = fileInfo.LastWriteTime;

            Users = new List<User>();
            UserTasks = new Dictionary<User, List<Task>>();
        }

        internal void ReadFileLog()
        {
            using (StreamReader reader = new StreamReader(_logFileName))
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
                            file = rowFile.Split(new string[] { "\tsrc" }, StringSplitOptions.None);

                            if (file.Count() == 2)
                            {
                                fileName = file[1];

                                foreach (Task addedTask in addedTasks)
                                {
                                    if (UserTasks[findedUser].Find(f => f.Name == addedTask.Name).Files.FirstOrDefault(f => f.FileName == fileName) == null)
                                        UserTasks[findedUser].Find(f => f.Name == addedTask.Name).Files.Add(new ChangedFiles(file[0], fileName));
                                }
                            }
                        }
                    }
                    else
                        addedTasks.Clear();
                }

                foreach (var item in Users)
                {
                    UserTasks[item].Sort((a, b) => a.CompareName(b));
                }
            }
        }
    }
}
