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
using TestExam.Model;
using TestExam.Services;

namespace TestExam.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void GoToRegistration(object sender, MouseButtonEventArgs e)
        {
            Session.MainFrame.Navigate(new RegistrationPage());
        }

        private void GuestView(object sender, MouseButtonEventArgs e)
        {
            Session.MainFrame?.Navigate(new CatalogPage());
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == null || PasswordBox.Password == null)
            {
                MessageBox.Show("Заполните все поля", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            User user = Trades.GetContext().User
                .AsNoTracking()
                .Where(u => u.UserLogin == LoginBox.Text).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string hashedPassword = Hashing.ComputeHashSha128(PasswordBox.Password);
            if (user.UserPassword == hashedPassword)
            {
                Session.User = user;
                Session.MainFrame.Navigate(new CatalogPage());
            }
            else
            {
                LoginBox.Text = hashedPassword;
                MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
