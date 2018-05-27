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

        private bool _visibleStackPanelStorage = true;

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

            DataGridChanges.Items.Clear();

            SetVisibleAvailableNewFileLog();
        }

        private void _availableNewFileLogEvents_AvailableNewFileLog()
        {
            _availableNewFileLog = true;
            SetVisibleAvailableNewFileLog();
        }

        private void FormMainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserSettings();
            SetVisibleStackPanelStorage();
            LoadFileLogsAsync();
        }

        private void FormMainMenu_Closed(object sender, EventArgs e)
        {
            SaveUserSettings();
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
            Model.ConnectorStorage connector = GetConnectorStorage();
            if (connector == null)
                return;

            List<Model.ChangedFiles> listItem = new List<Model.ChangedFiles>();
            foreach (Model.ChangedFiles item in DataGridChanges.SelectedItems)
            {
                listItem.Add(item);
            }
            
            if (listItem.Count > 0)
                new OneScript().LockObject(connector, listItem.ToList());
        }

        private Model.ConnectorStorage GetConnectorStorage()
        {
            var connector = new Model.ConnectorStorage()
            {
                Server = GetTextControl(TextBoxServer),
                Base = GetTextControl(TextBoxBase),
                StoragePath = GetTextControl(TextBoxStoragePath),
                StorageUser = GetTextControl(TextBoxStorageUser),
                StoragePass = GetTextControl(TextBoxStoragePass)
            };

            if (connector.CheckFilledParameters())
                return connector;
            else
            {
                Messages.Show("Один или несколько параметров подключения не заполнены.");
                return null;
            }
        }

        private string GetTextControl(TextBox textBox) => textBox.Text;
        private string GetTextControl(PasswordBox passwordBox) => passwordBox.Password;

        private string SetTextControl(TextBox textBox, string text) => textBox.Text = text;

        private void SetVisibleAvailableNewFileLog()
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                LabelAvailableNewFileLog.Visibility = _availableNewFileLog ? Visibility.Visible : Visibility.Collapsed;
            }));
        }

        private void HyperLinkParameters_Click(object sender, RoutedEventArgs e)
        {
            //_visibleStackPanelStorage = !_visibleStackPanelStorage;
            //
            //SetVisibleStackPanelStorage();
        }

        private void SetVisibleStackPanelStorage()
        {
            if (_visibleStackPanelStorage)
            {
                StackPanelStorage.Visibility = Visibility.Visible;
                GridParameters.Background = new SolidColorBrush(Colors.LightSteelBlue)
                {
                    Opacity = 0.5
                };
            }
            else
            {
                StackPanelStorage.Visibility = Visibility.Collapsed;
                GridParameters.Background = new SolidColorBrush(Colors.LightSteelBlue)
                {
                    Opacity = 0
                };
            }
        }

        private void LoadUserSettings()
        {
            var properties = Properties.Settings.Default;
            SetTextControl(TextBoxServer, properties.Server);
            SetTextControl(TextBoxBase, properties.Base);
            SetTextControl(TextBoxStoragePath, properties.StoragePath);
            SetTextControl(TextBoxStorageUser, properties.StorageUser);
        }
        
        private void SaveUserSettings()
        {
            var properties = Properties.Settings.Default;
            properties.Server = GetTextControl(TextBoxServer);
            properties.Base = GetTextControl(TextBoxBase);
            properties.StoragePath = GetTextControl(TextBoxStoragePath);
            properties.StorageUser = GetTextControl(TextBoxStorageUser);
            properties.Save();
        }
    }
}
