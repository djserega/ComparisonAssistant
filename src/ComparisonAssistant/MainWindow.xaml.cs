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
using System.Windows.Markup;
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

        private bool _visibleStackPanelFilter = true;
        private bool _visibleStackPanelStorage = false;
        private bool _visibleTypeConnection = false;

        public List<Model.User> Users { get; set; }
        public Dictionary<Model.User, List<Model.Task>> UserTasks { get; set; }

        public DateTime DateEdited { get; set; }
        public DateTime DateUpdated { get; set; }

        public DateTime FilterDateStart { get; set; }
        public DateTime FilterDateEnd { get; set; }
        public List<Model.StandardFilterPeriods> StandardFilterPeriods { get; }

        public MainWindow()
        {
            InitializeComponent();

            StandardFilterPeriods = new Model.StandardFilterPeriods().GetListPeriodsByDefault();
            Users = new List<Model.User>();
            UserTasks = new Dictionary<Model.User, List<Model.Task>>();

            _availableNewFileLogEvents.AvailableNewFileLog += _availableNewFileLogEvents_AvailableNewFileLog;

            _watcher = new Watcher(_availableNewFileLogEvents);

            DataGridChanges.Items.Clear();

            ComboBoxStandartFilterPeriod.SelectedItem = StandardFilterPeriods.Find(f => f.Name == "Сегодня");

            XmlLanguage currentDateLanguage = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

            DatePickerEdited.Language = currentDateLanguage;
            DatePickerUpdate.Language = currentDateLanguage;
            DatePickerFilterStart.Language = currentDateLanguage;
            DatePickerFilterEnd.Language = currentDateLanguage;

            DataContext = this;
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
            SetVisibleAvailableNewFileLog();
            SetVisibleTypeConnection();

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
            DateEdited = parser == null ? DateTime.MinValue : parser.DateEditedFile;
            DateUpdated = DateTime.Now;

            LabelUpdating.Visibility = Visibility.Collapsed;
            StackPanelDataFile.Visibility = Visibility.Visible;

            BindingOperations.GetBindingExpression(DatePickerEdited, DatePicker.SelectedDateProperty).UpdateTarget();
            BindingOperations.GetBindingExpression(DatePickerUpdate, DatePicker.SelectedDateProperty).UpdateTarget();

            _availableNewFileLog = false;
            SetVisibleAvailableNewFileLog();
        }

        private async Task<Parser> LoadFileLogs()
        {
            return await Task.Run(() =>
            {
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

                parser.DateMin = FilterDateStart;
                parser.DateMax = FilterDateEnd;
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
                TypeConnection = GetTextControl(CheckBoxTypeConnection),
                Server = GetTextControl(TextBoxServer),
                Base = GetTextControl(TextBoxBase),
                PathBase = GetTextControl(TextBoxPathBase),
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
        private bool GetTextControl(CheckBox checkBox) => checkBox.IsChecked ?? false;

        private void SetTextControl(TextBox textBox, string text) => textBox.Text = text;
        private void SetTextControl(CheckBox checkBox, bool value) => checkBox.IsChecked = value;

        private void SetVisibleAvailableNewFileLog()
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                LabelAvailableNewFileLog.Visibility = _availableNewFileLog ? Visibility.Visible : Visibility.Collapsed;
            }));
        }

        private void ChangeVisibleStackPanel()
        {
            SetVisibleStackPanelFilter();
            SetVisibleStackPanelStorage();
        }

        private void SetVisibleStackPanelStorage()
        {
            if (_visibleStackPanelStorage)
                SetPositionSeparatorPanel(51, 308);
            SetVisibleStackPanel(_visibleStackPanelStorage, StackPanelStorage);
        }

        private void SetVisibleStackPanelFilter()
        {
            if (_visibleStackPanelFilter)
                SetPositionSeparatorPanel(1, 48);
            SetVisibleStackPanel(_visibleStackPanelFilter, StackPanelFilter);
        }

        private void SetVisibleStackPanel(bool visible, StackPanel stackPanel)
        {
            if (visible)
            {
                stackPanel.Visibility = Visibility.Visible;
                stackPanel.Background = new SolidColorBrush(Colors.LightSteelBlue)
                {
                    Opacity = 0.5
                };
            }
            else
            {
                stackPanel.Visibility = Visibility.Collapsed;
                stackPanel.Background = new SolidColorBrush(Colors.LightSteelBlue)
                {
                    Opacity = 0
                };
            }
        }

        private void SetPositionSeparatorPanel(double left, double width)
        {
            TextBlockSeparator.Margin = new Thickness(left, 0, 0, 0);
            TextBlockSeparator.Width = width;
        }

        private void LoadUserSettings()
        {
            var properties = Properties.Settings.Default;
            SetTextControl(CheckBoxTypeConnection, properties.TypeConnector);
            SetTextControl(TextBoxServer, properties.Server);
            SetTextControl(TextBoxBase, properties.Base);
            SetTextControl(TextBoxPathBase, properties.PathBase);
            SetTextControl(TextBoxStoragePath, properties.StoragePath);
            SetTextControl(TextBoxStorageUser, properties.StorageUser);
        }

        private void SaveUserSettings()
        {
            var properties = Properties.Settings.Default;
            properties.TypeConnector = GetTextControl(CheckBoxTypeConnection);
            properties.Server = GetTextControl(TextBoxServer);
            properties.Base = GetTextControl(TextBoxBase);
            properties.PathBase = GetTextControl(TextBoxPathBase);
            properties.StoragePath = GetTextControl(TextBoxStoragePath);
            properties.StorageUser = GetTextControl(TextBoxStorageUser);
            properties.Save();
        }

        private void CheckBoxTypeConnection_Click(object sender, RoutedEventArgs e)
        {
            SetVisibleTypeConnection();
        }

        private void SetVisibleTypeConnection()
        {
            _visibleTypeConnection = CheckBoxTypeConnection.IsChecked.HasValue && CheckBoxTypeConnection.IsChecked.Value;
            StackPanelTypeServer.Visibility = _visibleTypeConnection ? Visibility.Visible : Visibility.Collapsed;
            StackPanelTypeFile.Visibility = _visibleTypeConnection ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonParameters_Click(object sender, RoutedEventArgs e)
        {
            _visibleStackPanelFilter = false;
            _visibleStackPanelStorage = true;
            ChangeVisibleStackPanel();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            _visibleStackPanelFilter = true;
            _visibleStackPanelStorage = false;
            ChangeVisibleStackPanel();
        }

        private void ComboBoxStandartFilterPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxStandartFilterPeriod.SelectedItem is Model.StandardFilterPeriods periods)
            {
                FilterDateStart = periods.DateStart ?? DateTime.MinValue;
                FilterDateEnd = periods.DateEnd ?? DateTime.MaxValue;

                BindingOperations.GetBindingExpression(DatePickerFilterStart, DatePicker.SelectedDateProperty).UpdateTarget();
                BindingOperations.GetBindingExpression(DatePickerFilterEnd, DatePicker.SelectedDateProperty).UpdateTarget();
            }
        }
    }
}
