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
    /// Логика взаимодействия для AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : Page
    {
        private Product _current = new Product();
        private bool isRedacting = false;
        public AddProductPage(Product selectedProduct)
        {
            InitializeComponent();
            if (selectedProduct != null)
            {
                _current = selectedProduct;
                isRedacting = true;
            }

            DataContext = _current;
            cbCategory.ItemsSource = Trades.GetContext().Category.ToList();
        }
        private void number_input(object sender, TextCompositionEventArgs e)
        {
            if ((!Char.IsDigit(e.Text, 0)) && (e.Text != ".") && (e.Text != ",")) {
                e.Handled = true;
            } 
        }

        private void intNumber_input(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void CreateProduct(object sender, EventArgs e)
        {
            StringBuilder error = new StringBuilder();

            // Заполнение всех полей
            if ((string.IsNullOrEmpty(ArticleBox.Text)) || (string.IsNullOrEmpty(ProductNameBox.Text)) || (string.IsNullOrEmpty(DescriptionBox.Text)) || 
                (cbCategory.SelectedItem == null) || (string.IsNullOrEmpty(CostBox.Text)) || (string.IsNullOrEmpty(CountBox.Text))
                || (string.IsNullOrEmpty(StatusBox.Text)))
            {
                error.AppendLine("Вы заполнили не все поля");
            }

            if (ArticleBox.Text.Length != 6)
            {
                error.AppendLine("Артикул должен состоять из 6 символов");
            }

            // Обработка десятичных цен
            string costCopy = CostBox.Text.Replace('.', ',');

            decimal cost;

            // Проверка поля цены на числовой ввод
            if (!decimal.TryParse(costCopy, out cost))
            {
                error.AppendLine("Поле цена принимает только числовые значения");
            }

            // Проверка цены на неотрицательность
            if (cost <= 0)
            {
                error.AppendLine("Цена должна быть больше 0");
            }

            int count;

            // Проверка поля количества на складе на числовой ввод
            if (!Int32.TryParse(CountBox.Text, out count))
            {
                error.AppendLine("Поле количество товара принимает только числовые значения");
            }

            // Проверка количества на неотрицательность
            if (count < 0)
            {
                error.AppendLine("Количество не может быть отрицательным");
            }

            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString());
                return;
            }

            try
            {

                // Проверка на уже существующий артикул

                if (!isRedacting)
                {
                    var existingArticle = Trades.GetContext().Product
                                    .AsNoTracking()
                                    .Where(u => u.ProductArticleNumber == ArticleBox.Text).FirstOrDefault();

                    if (existingArticle != null)
                    {
                        error.AppendLine("Товар с таким артикулом уже существует");
                    }
                }

                // Проверка на изменение артикула
                else
                {
                    if (_current.ProductArticleNumber != ArticleBox.Text)
                    {
                        error.AppendLine("Нельзя менять артикул товара");
                    }
                }

                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                    return;
                }

                _current.ProductPhoto = $"{ArticleBox.Text}.jpg";
                _current.ProductManufacturer = "1";
                _current.ProductDiscountAmount = 0;

                //Product newProduct = new Product
                //{
                //    ProductArticleNumber = ArticleBox.Text,
                //    ProductName = ProductNameBox.Text,
                //    ProductDescription = DescriptionBox.Text,

                //    ProductPhoto = $"{ArticleBox.Text}.jpg",
                //    ProductManufacturer = "1",
                //    ProductCost = cost,
                //    ProductDiscountAmount = 0,
                //    ProductQuantityInStock = count,
                //    ProductStatus = StatusBox.Text,
                //};

                if (!isRedacting)
                {
                    Trades.GetContext().Product.Add(_current);
                }

                Trades.GetContext().SaveChanges();
                MessageBox.Show("Информация успешна сохранена!");
                Session.MainFrame.Navigate(new CatalogPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        private void GoToCatalog(object sender, MouseButtonEventArgs e)
        {
            Session.MainFrame.Navigate(new CatalogPage());
        }
    }
}
