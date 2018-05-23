using ComparisonAssistant.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public partial class MainWindow : Window
    {
        public List<User> Users { get; set; }
        public Dictionary<User, List<Task>> UserTasks { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonUpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = null;
            try
            {
                parser = new Parser();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (parser == null)
                return;

            parser.ReadFileLog();

            Users = parser.Users;

            ComboboxUsers.ItemsSource = Users;

        }
    }
}
