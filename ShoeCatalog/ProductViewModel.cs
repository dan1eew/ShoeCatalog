using System.Windows;
using System.Windows.Media;

namespace ShoeCatalog
{
    public class ProductViewModel
    {
        private readonly Product _product;

        /// <summary>Инициализирует ViewModel с товаром</summary>
        public ProductViewModel(Product product)
        {
            _product = product;
        }

        public string Article => _product.Article;
        public string NameProduct => _product.NameProduct;
        public string CategoryProduct => _product.CategoryProduct;
        public decimal Price => _product.Price;
        public decimal CurrentDiscount => _product.CurrentDiscount;
        public int QuantityInStock => _product.QuantityInStock;
        public decimal FinalPrice => _product.FinalPrice;
        public string DescriptionProduct => _product.DescriptionProduct;
        public string Supplier => _product.Supplier;
        public string Producer => _product.Producer;
        public string Unit => _product.Unit;
        public System.Windows.Media.Imaging.BitmapImage ProductImage => _product.GetProductImage();

        /// <summary>Декорации для цены (перечеркивание при скидке)</summary>
        public TextDecorationCollection PriceDecoration =>
            CurrentDiscount > 0 ? TextDecorations.Strikethrough : null;

        /// <summary>Цвет строки в зависимости от кол-ва товара и скидки</summary>
        public Brush RowColor
        {
            get
            {
                if (QuantityInStock == 0)
                    return new SolidColorBrush(Color.FromArgb(255, 173, 216, 230)); // Голубой
                if (CurrentDiscount > 15)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57")); // Зеленый
                return Brushes.Transparent;
            }
        }

        /// <summary>Цвет цены в зависимости от скидки</summary>
        public Brush PriceColor => CurrentDiscount > 0
            ? new SolidColorBrush(Color.FromRgb(255, 100, 100)) // Красный при скидке
            : Brushes.Black; // Черный без скидки

        /// <summary>Цвет текста скидки</summary>
        public Brush DiscountTextColor => CurrentDiscount > 15
            ? Brushes.White // Белый для больших скидок
            : Brushes.Black; // Черный для обычных

        /// <summary>Цвет итоговой цены</summary>
        public Brush FinalPriceColor => Brushes.Black;

        /// <summary>Видимость блока со скидкой</summary>
        public Visibility DiscountVisibility => CurrentDiscount > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>Отображение цены со скидкой</summary>
        public string PriceDisplay => CurrentDiscount > 0
            ? $"{Price:0}₽"
            : $"{Price:0}₽";
    }
}