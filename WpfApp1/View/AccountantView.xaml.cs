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
    /// Логика взаимодействия для AccountantView.xaml
    /// </summary>
    public partial class AccountantView : UserControl
    {
        public AccountantView(string fullName)
        {
            InitializeComponent();
            fullNameLabel.Content = fullName;
        }

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            Content = new LoginView();
        }
    }
}
