using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
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

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PwdVisibleCheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            if (PwdVisibleCheckBox.IsChecked == true)
            {
                PwdPasswordBox.Visibility = Visibility.Hidden;
                PwdTextBox.Visibility = Visibility.Visible;
                PwdTextBox.Text = PwdPasswordBox.Password;
            }
            else
            {
                PwdTextBox.Visibility = Visibility.Hidden;
                PwdPasswordBox.Visibility = Visibility.Visible;
                PwdPasswordBox.Password = PwdTextBox.Text;
            }
        }

        private void LoginButton_Clicked(object sender, RoutedEventArgs e)
        {
            string password = "";
            string login = LoginTextBox.Text;
            if (PwdVisibleCheckBox.IsChecked == false)
            {
                password = PwdPasswordBox.Password;
            }
            else
            {
                password = PwdTextBox.Text;
            }

            using (SqliteConnection connection = DbConnector.OpenConnection())
            {
                string query = @"SELECT 'Laborant' as UserType FROM Laborants WHERE Login=@login AND Password=@password
                                UNION
                                SELECT 'Accountant' FROM Accountants WHERE Login=@login AND Password=@password
                                UNION
                                SELECT 'Admin' FROM Administrators WHERE Login=@login AND Password=@password";

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("login", login);
                command.Parameters.AddWithValue("password", password);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string userType = reader.GetString(0);
                        switch (userType)
                        {
                            case "Laborant":
                                SuccessfulLogin("Laborants", login);
                                break;
                            case "Accountant":
                                SuccessfulLogin("Accountant", login);
                                break;
                            case "Admin":
                                SuccessfulLogin("Administrators", login);
                                break;
                        }
                    }
                    else
                    {
                        // неверный логин или пароль
                        Content = new SecondLoginView();
                    }

                }
            }
        }
        public void SuccessfulLogin(string tableName, string login)
        {
            string fullName = "";
            using (SqliteConnection connection = DbConnector.OpenConnection())
            {
                SqliteCommand cmd = new SqliteCommand($"SELECT FullName FROM {tableName} WHERE Login=@login", connection);
                cmd.Parameters.AddWithValue("login", login);
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fullName = reader.GetString(0);
                    }
                }
                switch (tableName)
                {
                    case "Laborants":
                        SqliteCommand lastEnteredCmd = new SqliteCommand("SELECT LastEntered FROM Laborants WHERE Login=@login", connection);
                        lastEnteredCmd.Parameters.AddWithValue("login", login);
                        using(SqliteDataReader reader = lastEnteredCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime lastEntered = DateTime.MinValue;
                                if (reader.IsDBNull(0))
                                {
                                    lastEntered = DateTime.MinValue;
                                }
                                else
                                {
                                    lastEntered = Convert.ToDateTime(reader.GetString(0));
                                }                                
                                DateTime currentTime = DateTime.Now;

                                if(currentTime >= lastEntered.AddMinutes(1))
                                {
                                    // Прошла 1 минута с последнего входа.
                                    Content = new LaborantView(fullName);
                                }
                                else
                                {
                                    MessageBox.Show("Кварцевание не закончено. Ожидайте");
                                }
                            }
                        }
                        
                        break;
                    case "Accountant":
                        Content = new AccountantView(fullName);
                        break;
                    case "Administrators":
                        Content = new AdminView();
                        break;
                }
            }
        }
    }
}