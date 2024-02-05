using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using WpfApp1.Model;
using WpfApp1.View;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для LaborantView.xaml
    /// </summary>
    public partial class LaborantView : UserControl
    {
        private DispatcherTimer _timer;
        private TimeSpan _time;
        ObservableCollection<Patient> patientCollection = new ObservableCollection<Patient>();
        ObservableCollection<Service> serviceCollection = new ObservableCollection<Service>();

        public LaborantView(string fullName)
        {
            InitializeComponent();
            fullNameLabel.Content = fullName;
            SessionTimer();
            UpdateDataGrid();
        }

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            UpdateLastEntered();
            _timer.Stop();
            Content = new LoginView();
        }

        private void UpdateDataGrid()
        {
            using (SqliteConnection connection = DbConnector.OpenConnection())
            {
                SqliteCommand getPatients = new SqliteCommand("SELECT * FROM Patients", connection);
                using(SqliteDataReader reader = getPatients.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Patient patient = new Patient();
                        patient.Id = reader.GetInt32(0);
                        patient.Login = reader.GetString(1);
                        patient.Password = reader.GetString(2);
                        patient.FullName = reader.GetString(3);
                        patient.Phone = reader.GetString(4);
                        patient.Email = reader.GetString(5);
                        patient.InsuranceNumber = reader.GetString(6);
                        patientCollection.Add(patient);
                    }
                }

                SqliteCommand getServices = new SqliteCommand("SELECT * FROM Services", connection);
                using(SqliteDataReader serviceReader = getServices.ExecuteReader())
                {
                    while (serviceReader.Read())
                    {
                        Service service = new Service();
                        service.Code = serviceReader.GetString(0);
                        service.Name = serviceReader.GetString(1);
                        service.Price = serviceReader.GetString(2);
                        serviceCollection.Add(service);
                    }
                }
            }
            PatientDataGrid.ItemsSource = patientCollection;
            ServicesDataGrid.ItemsSource = serviceCollection;
        }

        private void SessionTimer()
        {
            _time = TimeSpan.FromMinutes(10);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _time = _time.Add(TimeSpan.FromSeconds(-1));
            SessionTimeLabel.Content = _time.ToString(@"mm\:ss");
            if(_time == TimeSpan.Zero)
            {
                MessageBox.Show("Осталось 5 минут до окончания сеанса.");
            }
            else if (_time == TimeSpan.Zero)
            {
                _timer.Stop();
                MessageBox.Show("Сеанс автоматически завершен. Начинается кварцевание.");
                Content = new LoginView();
            }
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;

            TextBox searchTextBox = (TextBox)sender;
            string searchText = searchTextBox.Text.ToLower();

            IEnumerable<object> filteredItems = null;
            DataGrid targetDataGrid = null;

            if (selectedTab == PatientTabItem)
            {
                filteredItems = patientCollection.Where(searchObj =>
                searchObj.FullName.ToLower().Contains(searchText)
                );
                targetDataGrid = PatientDataGrid;
            }
            else if (selectedTab == ServicesTabItem)
            {
                filteredItems = serviceCollection.Where(searchObj =>
                searchObj.Code.ToString().Contains(searchText) || // Преобразуем в строку для поиска
                    searchObj.Name?.ToLower().Contains(searchText) == true ||
                    searchObj.Price.ToString().Contains(searchText) == true
                    );
                targetDataGrid = ServicesDataGrid;
            }
            else
            {
                return;
            }
            targetDataGrid.ItemsSource = filteredItems.ToList();
        }

        private void UpdateLastEntered()
        {
            using(SqliteConnection connection = DbConnector.OpenConnection())
            {
                SqliteCommand cmd = new SqliteCommand("UPDATE Laborants SET LastEntered=@lastEntered WHERE FullName=@fullName", connection);
                cmd.Parameters.AddWithValue("fullName", fullNameLabel.Content);
                cmd.Parameters.AddWithValue("lastEntered", DateTime.Now.ToString());
                cmd.ExecuteNonQuery();
            }
        }
    }
}
