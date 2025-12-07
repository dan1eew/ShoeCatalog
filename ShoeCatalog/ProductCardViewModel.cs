using ShoeCatalog;
using System.Windows;
using System.Windows.Media;

namespace TableWPF
{
    public class ProductViewModel
    {
        private Product product;
        private bool isDarkTheme;

        public ProductViewModel(Product product, bool isDarkTheme = false)
        {
            this.product = product;
            this.isDarkTheme = isDarkTheme;
        }

        public string Article => product.Article;
        public string NameProduct => product.NameProduct;
        public string CategoryProduct => product.CategoryProduct;
        public decimal Price => product.Price;
        public decimal CurrentDiscount => product.CurrentDiscount;
        public int QuantityInStock => product.QuantityInStock;
        public decimal FinalPrice => product.FinalPrice;
        public string DescriptionProduct => product.DescriptionProduct;
        public string Supplier => product.Supplier;
        public string Producer => product.Producer;
        public string Unit => product.Unit;

        public System.Windows.Media.Imaging.BitmapImage ProductImage => product.GetProductImage();

        public Brush RowColor
        {
            get
            {
                if (QuantityInStock == 0)
                {
                    return isDarkTheme ?
                        new SolidColorBrush(Color.FromRgb(30, 90, 120)) : // Темный голубой
                        new SolidColorBrush(Color.FromRgb(173, 216, 230)); // Светлый голубой
                }
                if (CurrentDiscount > 15)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57")); // Зеленый
                }
                return Brushes.Transparent;
            }
        }

        public System.Windows.TextDecorationCollection PriceDecoration =>
            CurrentDiscount > 0 ? TextDecorations.Strikethrough : null;

        public Brush PriceColor
        {
            get
            {
                if (CurrentDiscount > 0)
                    return isDarkTheme ?
                        new SolidColorBrush(Color.FromRgb(255, 100, 100)) : // Светло-красный для темной темы
                        new SolidColorBrush(Color.FromRgb(255, 0, 0)); // Красный для светлой темы
                return isDarkTheme ? Brushes.White : Brushes.Black;
            }
        }

        public Brush FinalPriceColor =>
            isDarkTheme ? Brushes.White : Brushes.Green;

        public Visibility DiscountVisibility =>
            CurrentDiscount > 0 ? Visibility.Visible : Visibility.Collapsed;

        public string PriceDisplay =>
            CurrentDiscount > 0 ? $"{Price}₽ → {FinalPrice:0}₽" : $"{Price:0}₽";

        public void UpdateTheme(bool darkTheme)
        {
            isDarkTheme = darkTheme;
        }
    }
}