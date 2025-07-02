using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Product> _products; 
        private List<Product> _sortedProducts;
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
        public Category SelectedCategory { get; set; } // Свойство для корректного отображения категорий

        // Свойство для выключения управляющих элементов
        private bool _isAdmin = true;
        public bool IsAdmin
        {
            get 
            { 
                return _isAdmin; 
            } 
            set
            {
                _isAdmin = value;
                OnPropertyChanged("IsAdmin");
            }
        }
        public CatalogPage()
        {
            InitializeComponent();

            // Подписываемся на событие фильтрации и сортировки
            searchBox.TextChanged += (s, e) => filterProducts();
            cbCategory.SelectionChanged += (s, e) => filterProducts();
            cbSort.SelectionChanged += (s, e) => filterProducts();

            this.DataContext = this;
            
            // Если роль пользователя не админ - то скрываем управляющие кнопки
            if (Session.User == null || Session.User.UserRole != 1)
            {
                IsAdmin = false;
                //AddNewBtn.IsEnabled = false;
                //AddNewBtn.Visibility = Visibility.Hidden;
                //RemoveBtn.IsEnabled = false;
                //RemoveBtn.Visibility = Visibility.Hidden;
                //RedactBtn.IsEnabled = false;
                //RedactBtn.Visibility = Visibility.Hidden;
            }

            // Настройки для гостевого просмотра
            if (Session.User == null) 
            {
                FIO.Text = "Гостевой\nпросмотр";
                LogOutBtn.Content = "К авторизации";
                OrdersBtn.IsEnabled = false;
                OrdersBtn.Visibility = Visibility.Hidden;
            }
            else // Выводим данные пользователя
            {
                if (string.IsNullOrEmpty(Session.User.UserPatronymic))
                {
                    FIO.Text = $"{Session.User.UserSurname}\n{Session.User.UserName}";
                }
                else
                {
                    FIO.Text = $"{Session.User.UserSurname}\n{Session.User.UserName}\n{Session.User.UserPatronymic}";
                }
            }

            // Выводим товары
            _products = Trades.GetContext().Product.ToList();
            maxCountEntries = _products.Count;
            lvProducts.ItemsSource = _products;


            // Настройки комбобокса категорий
            List<Category> _categories = Trades.GetContext().Category.ToList();
            var allCategories = new Category { Category1 = "Все категории", CategoryID = 0 };
            _categories.Insert(0, allCategories);
            cbCategory.ItemsSource = _categories;
            SelectedCategory = allCategories;

            cbSort.SelectedIndex = 0;

            // Обновляем свойство
            CountEntries = $"Найдено {maxCountEntries} из {maxCountEntries} записей";
        }

        // Функция для выхода на страницу авторизации
        private void LogOut(object sender, RoutedEventArgs e)
        {
            Session.User = null;
            Session.MainFrame.Navigate(new AuthorizationPage());
        }

        // Функция для перехода к заказам пользователя
        private void GoToOrders(object sender, RoutedEventArgs e)
        {
            Session.MainFrame.Navigate(new OrdersPage());
        }

        // Функция для перехода к форме создания товара
        private void GoToAddProduct(object sender, RoutedEventArgs e)
        {
            Session.MainFrame.Navigate(new AddProductPage(null));
        }

        //
        private void GoToRedactProduct(object sender, RoutedEventArgs e)
        {
            Session.MainFrame.Navigate(new AddProductPage((sender as Button).DataContext as Product));
        }

        private void RemoveProduct(object sender, RoutedEventArgs e)
        {
            Product ProductForDeliting = (Product)lvProducts.SelectedItem;

            if (ProductForDeliting == null)
            {
                MessageBox.Show("Не выбран ни один товар", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить этот элемент?", "Внимание", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Trades.GetContext().Product.Remove(ProductForDeliting);
                    Trades.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");

                    // Сброс параметров фильтрации и сортировки
                    Session.MainFrame.Navigate(new CatalogPage());

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Функция для фильтрации и сортировки товаров
        private void filterProducts()
        {

            // Поиск
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                _sortedProducts = _products;
            }
            else
            {
                _sortedProducts = _products.Where(p => p.ProductName.ToLower().Contains(searchBox.Text.ToLower()) 
                || p.ProductDescription.ToLower().Contains(searchBox.Text.ToLower())).ToList();
            }

            // Фильтрация по категории
            if (cbCategory.SelectedItem != null && cbCategory.SelectedIndex != 0)
            {
                Category category = cbCategory.SelectedItem as Category;
                _sortedProducts = _sortedProducts.Where(p => p.Category == category).ToList();
            }

            //Сортировка
            var selectedTag = (cbSort.SelectedItem as ComboBoxItem)?.Tag.ToString();

            switch (selectedTag)
            {
                case "NameAsc":
                    _sortedProducts = _sortedProducts.OrderBy(a => a.ProductName).ToList();
                    break;
                case "NameDesc":
                    _sortedProducts = _sortedProducts.OrderByDescending(a => a.ProductName).ToList();
                    break;
                case "PriceAsc":
                    _sortedProducts = _sortedProducts.OrderBy(a => a.ProductCost).ToList();
                    break;
                case "PriceDesc":
                    _sortedProducts = _sortedProducts.OrderByDescending(a => a.ProductCost).ToList();
                    break;
                case "None":
                default:
                    _sortedProducts = _sortedProducts.OrderBy(a => a.ProductArticleNumber).ToList();
                    break;
            }

            lvProducts.ItemsSource = _sortedProducts;
            CountEntries = $"Найдено {_sortedProducts.Count} из {maxCountEntries} записей"; // Обновляем свойство
            tbCountEntries.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget(); // Принудительно обновляем UI
        }
    }
}
