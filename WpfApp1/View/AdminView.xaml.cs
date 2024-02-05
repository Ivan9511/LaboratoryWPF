using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
        }
        
        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            Content = new LoginView();
        }
    }
}