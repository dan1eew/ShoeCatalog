using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShoeCatalog
{
    public class Product
    {
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

        public decimal FinalPrice => Price * (1 - CurrentDiscount / 100);
        public bool HasDiscount => CurrentDiscount > 0;
        public bool HasBigDiscount => CurrentDiscount > 15;
        public bool OutOfStock => QuantityInStock == 0;

        public BitmapImage GetProductImage()
        {
            try
            {
                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                    return LoadDefaultImage();
                }

                if (!string.IsNullOrEmpty(Photo) && Photo.ToLower() != "null")
                {
                    string photoFile = Photo.Trim();
                    string imagePath = Path.Combine(imagesDirectory, photoFile);

                    if (File.Exists(imagePath))
                    {
                        return LoadImageFromFile(imagePath);
                    }
                }

                string defaultImagePath = Path.Combine(imagesDirectory, "picture.png");
                if (File.Exists(defaultImagePath))
                {
                    return LoadImageFromFile(defaultImagePath);
                }

                return LoadDefaultImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения для {Article}: {ex.Message}");
                return LoadDefaultImage();
            }
        }

        private BitmapImage LoadImageFromFile(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
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
                System.Diagnostics.Debug.WriteLine($"Ошибка создания BitmapImage из {path}: {ex.Message}");
                return LoadDefaultImage();
            }
        }

        private BitmapImage LoadDefaultImage()
        {
            try
            {
                string defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "picture.png");
                if (File.Exists(defaultImagePath))
                {
                    return LoadImageFromFile(defaultImagePath);
                }

                // Создаем простую программную заглушку
                return CreateDefaultBitmapImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения по умолчанию: {ex.Message}");
                return CreateDefaultBitmapImage();
            }
        }

        private BitmapImage CreateDefaultBitmapImage()
        {
            try
            {
                // Создаем простое изображение программно
                int width = 100;
                int height = 100;

                RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                DrawingVisual drawingVisual = new DrawingVisual();

                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, width, height));

                    FormattedText text = new FormattedText(
                        "No Image",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        12,
                        Brushes.Gray,
                        1.0);

                    drawingContext.DrawText(text, new Point(10, 40));
                }

                rtb.Render(drawingVisual);

                // Конвертируем RenderTargetBitmap в BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                using (MemoryStream stream = new MemoryStream())
                {
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

        public static Product FromDataRow(DataRow row)
        {
            try
            {
                return new Product
                {
                    ProductID = row["ProductID"] != DBNull.Value ? Convert.ToInt32(row["ProductID"]) : 0,
                    Article = row["Article"] != DBNull.Value ? row["Article"].ToString() : "",
                    NameProduct = row["NameProduct"] != DBNull.Value ? row["NameProduct"].ToString() : "",
                    Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : "шт.",
                    Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0,
                    Supplier = row["Supplier"] != DBNull.Value ? row["Supplier"].ToString() : "",
                    Producer = row["Producer"] != DBNull.Value ? row["Producer"].ToString() : "",
                    CategoryProduct = row["CategoryProduct"] != DBNull.Value ? row["CategoryProduct"].ToString() : "",
                    CurrentDiscount = row["CurrentDiscount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentDiscount"]) : 0,
                    QuantityInStock = row["QuantityInStock"] != DBNull.Value ? Convert.ToInt32(row["QuantityInStock"]) : 0,
                    DescriptionProduct = row["DescriptionProduct"] != DBNull.Value ? row["DescriptionProduct"].ToString() : "",
                    Photo = row["Photo"] != DBNull.Value ? row["Photo"].ToString() : null
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при создании Product: {ex.Message}");
                return new Product();
            }
        }
    }
}