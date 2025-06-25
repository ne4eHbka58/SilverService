using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TestExam.Model;
using TestExam.Services;

namespace TestExam.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }
        private void GoToSignIn(object sender, MouseButtonEventArgs e)
        {
            Session.MainFrame.Navigate(new AuthorizationPage());
        }
        private void Registrate(object sender, EventArgs e)
        {
            if ((SurnameBox.Text == null) || (NameBox.Text == null) || (LoginBox.Text == null) || (PasswordBox.Password == null))
            {
                MessageBox.Show("Все поля кроме отчества должны быть обязательно заполнены", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PasswordBox.Password != SecPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {

                // Проверка на уже существующий логин
                var existingLogin = Trades.GetContext().User
                .AsNoTracking()
                .Where(u => u.UserLogin == LoginBox.Text).FirstOrDefault();

                if (existingLogin != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                User newUser = new User
                {
                    UserLogin = LoginBox.Text,
                    UserName = NameBox.Text,
                    UserSurname = SurnameBox.Text,
                    UserPatronymic = PatrynomicBox.Text,
                    UserPassword = Hashing.ComputeHashSha128(PasswordBox.Password),
                    UserRole = 3,
                };
                Trades.GetContext().User.Add(newUser);
                Trades.GetContext().SaveChanges();
                MessageBox.Show("Вы зарегистрированы!");
                Session.User = newUser;
                Session.MainFrame.Navigate(new CatalogPage());
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
    }
}
