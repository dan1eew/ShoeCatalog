using System.Windows;
using System.Windows.Media;

namespace ShoeCatalog
{
    public class ProductViewModel
    {
        private readonly Product _product;
        private bool _isDarkTheme;

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

        /// <summary>Цвет строки в зависимости от состояния товара</summary>
        public Brush RowColor
        {
            get
            {
                if (QuantityInStock == 0)
                    return _isDarkTheme
                        ? new SolidColorBrush(Color.FromRgb(30, 90, 120))
                        : new SolidColorBrush(Color.FromArgb(255, 173, 216, 230));

                if (CurrentDiscount > 15)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57"));

                return Brushes.Transparent;
            }
        }

        /// <summary>Декорации для цены (перечеркивание при скидке)</summary>
        public System.Windows.TextDecorationCollection PriceDecoration =>
            CurrentDiscount > 0 ? TextDecorations.Strikethrough : null;

        /// <summary>Цвет цены в зависимости от скидки и темы</summary>
        public Brush PriceColor => CurrentDiscount > 0
            ? _isDarkTheme
                ? new SolidColorBrush(Color.FromRgb(255, 100, 100))
                : new SolidColorBrush(Color.FromRgb(255, 0, 0))
            : _isDarkTheme ? Brushes.White : Brushes.Black;

        /// <summary>Цвет финальной цены в зависимости от темы</summary>
        public Brush FinalPriceColor => _isDarkTheme ? Brushes.White : Brushes.Green;

        /// <summary>Видимость блока со скидкой</summary>
        public Visibility DiscountVisibility => CurrentDiscount > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>Отображение цены со скидкой</summary>
        public string PriceDisplay => CurrentDiscount > 0
            ? $"{Price}₽ → {FinalPrice:0}₽"
            : $"{Price:0}₽";

        ///<summary>Обновляет тему ViewModel</summary>
        /// <param name="darkTheme">true - темная тема, false - светлая</param>
        public void UpdateTheme(bool darkTheme) => _isDarkTheme = darkTheme;
    }
}