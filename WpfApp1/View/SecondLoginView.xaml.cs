using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для SecondLoginView.xaml
    /// </summary>
    public partial class SecondLoginView : UserControl
    {
        public SecondLoginView()
        {
            InitializeComponent();
            CaptchaLabel.Content = GenerateCaptcha();
        }

        public static string GenerateCaptcha()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }

        private void UpdateCaptchaButton_Clicked(object sender, RoutedEventArgs e)
        {
            CaptchaLabel.Content = GenerateCaptcha();
        }

        private void LoginButton_Clicked(object sender, RoutedEventArgs e)
        {
            string userCaptcha = CaptchaTextBox.Text;
            string systemCaptcha = CaptchaLabel.Content.ToString();
            if (userCaptcha == systemCaptcha)
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
                            LoginView loginView = new LoginView();
                            string userType = reader.GetString(0);
                            switch (userType)
                            {
                                case "Laborant":
                                    loginView.SuccessfulLogin("Laborants", login);
                                    break;
                                case "Accountant":
                                    loginView.SuccessfulLogin("Accountant", login);
                                    break;
                                case "Admin":
                                    loginView.SuccessfulLogin("Administrators", login);
                                    break;
                            }
                        }
                        else
                        {
                            // неверный логин или пароль
                            MessageBox.Show("Не верный логин или пароль.");
                            DisableButtonTemporarily();
                        }

                    }
                }
            }
            else
            {
                // не верно введена Captcha
                MessageBox.Show("Не верно введена CAPTCHA.");
                DisableButtonTemporarily();


            }

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

        private async void DisableButtonTemporarily()
        {
            LoginButton.IsEnabled = false;
            await Task.Delay(10000);
            LoginButton.IsEnabled = true;
        }
    }
}
