using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace TestExam.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Order> _orders;
        private List<Order> _sortedOrders;

        private readonly int maxCountEntries; // Максимальное количество записей в таблице
        private string _countEntries;
        public string CountEntries // Свойство для отображения количества найденых записей
        {
            get
            {
                return _countEntries;
            }
            set
            {
                _countEntries = value;
                OnPropertyChanged("CountEntries");
            }
        }

        public OrdersPage()
        {
            InitializeComponent();

            // Подписываемся на событие фильтрации и сортировки
            searchBox.TextChanged += (s, e) => filterOrders();
            cbSort.SelectionChanged += (s, e) => filterOrders();

            this.DataContext = this;

            // Выводим данные пользователя

            if (string.IsNullOrEmpty(Session.User.UserPatronymic))
            {
                FIO.Text = $"{Session.User.UserSurname}\n{Session.User.UserName}";
            }
            else
            {
                FIO.Text = $"{Session.User.UserSurname}\n{Session.User.UserName}\n{Session.User.UserPatronymic}";
            }

            // Выводим товары
            _orders = Trades.GetContext().Order.Where(o => o.UserID == Session.User.UserID).ToList();
            maxCountEntries = _orders.Count;
            lvOrders.ItemsSource = _orders;

            cbSort.SelectedIndex = 0;
        }

        // Функция для выхода на страницу авторизации
        private void LogOut(object sender, RoutedEventArgs e)
        {
            Session.User = null;
            Session.MainFrame.Navigate(new AuthorizationPage());
        }

        // Функция для перехода к каталогу
        private void GoToCatalog(object sender, RoutedEventArgs e)
        {
            Session.MainFrame.Navigate(new CatalogPage());
        }

        // Функция для фильтрации и сортировки заказов
        private void filterOrders()
        {

            // Поиск
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                _sortedOrders = _orders;
            }
            else
            {
                _sortedOrders = _orders.Where(o => o.OrderProducts.ToLower().Contains(searchBox.Text.ToLower())).ToList();
            }

            //Сортировка
            var selectedTag = (cbSort.SelectedItem as ComboBoxItem)?.Tag.ToString();

            switch (selectedTag)
            {
                case "DateAsc":
                    _sortedOrders = _sortedOrders.OrderBy(a => a.OrderDate).ToList();
                    break;
                case "DateDesc":
                    _sortedOrders = _sortedOrders.OrderByDescending(a => a.OrderDate).ToList();
                    break;
                case "PickUpPointAsc":
                    _sortedOrders = _sortedOrders.OrderBy(a => a.OrderPickupPoint).ToList();
                    break;
                case "None":
                default:
                    _sortedOrders = _sortedOrders.OrderBy(a => a.OrderID).ToList();
                    break;
            }

            lvOrders.ItemsSource = _sortedOrders;
            CountEntries = $"Найдено {_sortedOrders.Count} из {maxCountEntries} записей"; // Обновляем свойство
            tbCountEntries.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget(); // Принудительно обновляем UI
        }
    }
}
