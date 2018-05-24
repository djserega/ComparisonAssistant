using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace ComparisonAssistant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public List<Model.User> Users { get; set; }
        public Dictionary<Model.User, List<Model.Task>> UserTasks { get; set; }

        public DateTime DateEdited { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Users = new List<Model.User>();
            UserTasks = new Dictionary<Model.User, List<Model.Task>>();
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
                    Task.Run(() => MessageBox.Show(ex.Message));
                }

                if (parser == null)
                    return null;

                parser.ReadFileLog();
                

                return parser;
            });
        }
    }
}
