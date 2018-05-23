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

        internal DataCommit DataCommit { get; private set; }
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

                User findedUser;
                Task newTask;
                string userName;
                string taskName;

                while (!reader.EndOfStream)
                {
                    rowFile = reader.ReadLine();

                    thisCommit = new Regex(Regex.Escape(_separatorCommit)).Matches(rowFile).Count == 2;
                    if (thisCommit)
                    {
                        string[] commits = rowFile.Split(new string[] { _separatorCommit}, StringSplitOptions.RemoveEmptyEntries);

                        if (commits.Count() == 3)
                        {
                            userName = commits[0];
                            taskName = commits[1];

                            findedUser = Users.FirstOrDefault(f => f.Name == userName);
                            if (findedUser == null)
                            {
                                findedUser = new User(userName);
                                Users.Add(findedUser);
                            }

                            newTask = new Task(commits[1]);
                            if (UserTasks.ContainsKey(findedUser))
                                UserTasks[findedUser].Add(newTask);
                            else
                                UserTasks.Add(findedUser, new List<Task>() { newTask });
                        }
                    }
                    else
                    {

                    }
                }

            }
        }

    }
}
