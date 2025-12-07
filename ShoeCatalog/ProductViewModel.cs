using System;
using System.Windows;
using System.Windows.Media;

namespace ShoeCatalog
{
    public class ProductViewModel
    {
        private Product product;

        public ProductViewModel(Product product)
        {
            this.product = product;
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
                    return new SolidColorBrush(Color.FromArgb(255, 173, 216, 230)); // Голубой
                if (CurrentDiscount > 15)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57")); // Зеленый
                return Brushes.Transparent;
            }
        }

        public System.Windows.TextDecorationCollection PriceDecoration =>
            CurrentDiscount > 0 ? TextDecorations.Strikethrough : null;

        public Brush PriceColor =>
            CurrentDiscount > 0 ? Brushes.Red : Brushes.Black;

        public Visibility DiscountVisibility =>
            CurrentDiscount > 0 ? Visibility.Visible : Visibility.Collapsed;

        public string PriceDisplay =>
            CurrentDiscount > 0 ? $"{Price}₽ → {FinalPrice:0}₽" : $"{Price:0}₽";
    }
}