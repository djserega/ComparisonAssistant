using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ComparisonAssistant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private AvailableNewFileLogEvents _availableNewFileLogEvents = new AvailableNewFileLogEvents();
        private bool _availableNewFileLog = false;
        private Watcher _watcher;

        public List<Model.User> Users { get; set; }
        public Dictionary<Model.User, List<Model.Task>> UserTasks { get; set; }

        public DateTime DateEdited { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Users = new List<Model.User>();
            UserTasks = new Dictionary<Model.User, List<Model.Task>>();


            _availableNewFileLogEvents.AvailableNewFileLog += _availableNewFileLogEvents_AvailableNewFileLog;

            _watcher = new Watcher(_availableNewFileLogEvents);

            SetVisibleAvailableNewFileLog();
        }

        private void _availableNewFileLogEvents_AvailableNewFileLog()
        {
            _availableNewFileLog = true;
            SetVisibleAvailableNewFileLog();
        }

        private void FormMainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFileLogsAsync();
        }

        private void ButtonUpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadFileLogsAsync();
        }

        private void ComboboxUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboboxUsers.SelectedItem is Model.User user)
            {
                ComboboxTasks.ItemsSource = UserTasks[user];
                DataGridChanges.ItemsSource = null;
            }
        }

        private void ComboboxTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboboxTasks.SelectedItem is Model.Task task)
            {
                DataGridChanges.ItemsSource = task.Files;
            }
        }

        private async void LoadFileLogsAsync()
        {
            StackPanelDataFile.Visibility = Visibility.Collapsed;
            LabelUpdating.Visibility = Visibility.Visible;

            Parser parser = await LoadFileLogs();

            if (parser != null)
            {
                Users = parser.Users;
                UserTasks = parser.UserTasks;
            }

            ComboboxUsers.ItemsSource = Users;
            ComboboxTasks.ItemsSource = null;
            DataGridChanges.ItemsSource = null;
            DatePickerEdited.SelectedDate = parser?.DateEditedFile;
            DatePickerUpdate.SelectedDate = DateTime.Now;

            LabelUpdating.Visibility = Visibility.Collapsed;
            StackPanelDataFile.Visibility = Visibility.Visible;

            _availableNewFileLog = false;
            SetVisibleAvailableNewFileLog();
        }

        private async Task<Parser> LoadFileLogs()
        {
            return await Task.Run(() => {
                Parser parser = null;
                try
                {
                    parser = new Parser();
                }
                catch (FileNotFoundException ex)
                {
                    Messages.Show(ex.Message);
                }

                if (parser == null)
                    return null;

                parser.ReadFileLog();

                return parser;
            });
        }

        private void ButtonLockObject_Click(object sender, RoutedEventArgs e)
        {
            List<Model.ChangedFiles> listItem = new List<Model.ChangedFiles>();
            foreach (Model.ChangedFiles item in DataGridChanges.SelectedItems)
            {
                listItem.Add(item);
            }
            
            if (listItem.Count > 0)
                new OneScript().LockObject(listItem.ToList());
        }

        private void SetVisibleAvailableNewFileLog()
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                LabelAvailableNewFileLog.Visibility = _availableNewFileLog ? Visibility.Visible : Visibility.Collapsed;
            }));
        }
    }
}
