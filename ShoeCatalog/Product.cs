using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShoeCatalog
{
    ///<summary>Класс, представляющий товар</summary>
    public class Product
    {
        #region Свойства
        public int ProductID { get; set; }
        public string Article { get; set; }
        public string NameProduct { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Supplier { get; set; }
        public string Producer { get; set; }
        public string CategoryProduct { get; set; }
        public decimal CurrentDiscount { get; set; }
        public int QuantityInStock { get; set; }
        public string DescriptionProduct { get; set; }
        public string Photo { get; set; }

        /// <summary>Финальная цена со скидкой</summary>
        public decimal FinalPrice => Price * (1 - CurrentDiscount / 100);

        /// <summary>Есть ли скидка</summary>
        public bool HasDiscount => CurrentDiscount > 0;

        /// <summary>Большая ли скидка (>15%)</summary>
        public bool HasBigDiscount => CurrentDiscount > 15;

        /// <summary>Товар отсутствует на складе</summary>
        public bool OutOfStock => QuantityInStock == 0;
  
        private static BitmapImage _defaultImage;

        ///<summary>Загружает изображение товара</summary>
        public BitmapImage GetProductImage()
        {
            try
            {
                string imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                if (!Directory.Exists(imagesDir))
                {
                    Directory.CreateDirectory(imagesDir);
                    return LoadDefaultImage();
                }

                if (!string.IsNullOrWhiteSpace(Photo) && Photo.ToLower() != "null")
                {
                    string imagePath = Path.Combine(imagesDir, Photo.Trim());
                    if (File.Exists(imagePath))
                        return LoadImageFromFile(imagePath);
                }

                string defaultPath = Path.Combine(imagesDir, "picture.png");
                return File.Exists(defaultPath)
                    ? LoadImageFromFile(defaultPath)
                    : LoadDefaultImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения {Article}: {ex.Message}");
                return LoadDefaultImage();
            }
        }

        ///<summary>Загружает изображение из файла</summary>
        private BitmapImage LoadImageFromFile(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания BitmapImage: {ex.Message}");
                return LoadDefaultImage();
            }
        }

        ///<summary>Загружает изображение по умолчанию</summary>
        private BitmapImage LoadDefaultImage()
        {
            if (_defaultImage != null) return _defaultImage;

            try
            {
                string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "picture.png");
                _defaultImage = File.Exists(defaultPath)
                    ? LoadImageFromFile(defaultPath)
                    : CreateDefaultBitmapImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения по умолчанию: {ex.Message}");
                _defaultImage = CreateDefaultBitmapImage();
            }

            return _defaultImage;
        }

        ///<summary>Создает дефолтное изображение программно</summary>
        private BitmapImage CreateDefaultBitmapImage()
        {
            try
            {
                const int size = 100;
                var rtb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, size, size));

                    var text = new FormattedText("No Image",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"), 12, Brushes.Blue, 1.0);

                    drawingContext.DrawText(text, new Point(10, 40));
                }

                rtb.Render(drawingVisual);
                var bitmapImage = new BitmapImage();

                using (var stream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(stream);

                    stream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }

                return bitmapImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания программного изображения: {ex.Message}");
                return null;
            }
        }

        ///<summary>Создает объект Product из строки DataRow</summary>
        public static Product FromDataRow(DataRow row)
        {
            try
            {
                return new Product
                {
                    ProductID = GetValue<int>(row, "ProductID"),
                    Article = GetValue<string>(row, "Article"),
                    NameProduct = GetValue<string>(row, "NameProduct"),
                    Unit = GetValue(row, "Unit", "шт."),
                    Price = GetValue<decimal>(row, "Price"),
                    Supplier = GetValue<string>(row, "Supplier"),
                    Producer = GetValue<string>(row, "Producer"),
                    CategoryProduct = GetValue<string>(row, "CategoryProduct"),
                    CurrentDiscount = GetValue<decimal>(row, "CurrentDiscount"),
                    QuantityInStock = GetValue<int>(row, "QuantityInStock"),
                    DescriptionProduct = GetValue<string>(row, "DescriptionProduct"),
                    Photo = GetValue<string>(row, "Photo")
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания Product: {ex.Message}");
                return new Product();
            }
        }
        private static T GetValue<T>(DataRow row, string column)
        {
            return row[column] != DBNull.Value ? (T)Convert.ChangeType(row[column], typeof(T)) : default;
        }
        private static string GetValue(DataRow row, string column, string defaultValue)
        {
            return row[column] != DBNull.Value ? row[column].ToString() : defaultValue;
        }
    }
} 
#endregion