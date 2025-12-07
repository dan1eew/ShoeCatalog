using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace ShoeCatalog
{
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;
        private List<Product> products = new List<Product>();
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            GenerateID();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadProducts();
                ApplyTheme(isDarkTheme);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка из таблицы
        private void LoadProducts()
        {
            try
            {
                products.Clear();
                var dataTable = Database.GetProducts();

                foreach (DataRow row in dataTable.Rows)
                {
                    var product = Product.FromDataRow(row);
                    products.Add(product);
                }

                var viewModels = new List<ProductViewModel>();
                foreach (var product in products)
                {
                    viewModels.Add(new ProductViewModel(product));
                }

                ListViewProducts.ItemsSource = viewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ApplyTheme(bool dark)
        {
            isDarkTheme = dark;

            // Проверка на null
            if (ListViewProducts == null)
            {
                return; 
            }
      
            isDarkTheme = dark;

            if (dark)
            {
                // Темная тема
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                ListViewProducts.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                ListViewProducts.Foreground = Brushes.White;
                ListViewProducts.BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));

                // Обновляем цвета строк
                var style = new Style(typeof(ListViewItem));
                style.Setters.Add(new Setter(ListViewItem.BackgroundProperty, new Binding("RowColor")));
                style.Setters.Add(new Setter(ListViewItem.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(70, 70, 70))));
                style.Setters.Add(new Setter(ListViewItem.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
                style.Setters.Add(new Setter(ListViewItem.PaddingProperty, new Thickness(5)));
                style.Setters.Add(new Setter(ListViewItem.MinHeightProperty, 70.0));

                // Триггеры для темной темы
                var mouseOverTrigger = new Trigger
                {
                    Property = ListViewItem.IsMouseOverProperty,
                    Value = true
                };
                mouseOverTrigger.Setters.Add(new Setter(ListViewItem.BackgroundProperty,
                    new SolidColorBrush(Color.FromRgb(60, 60, 60))));
                style.Triggers.Add(mouseOverTrigger);

                var selectedTrigger = new Trigger
                {
                    Property = ListViewItem.IsSelectedProperty,
                    Value = true
                };
                selectedTrigger.Setters.Add(new Setter(ListViewItem.BackgroundProperty,
                    new SolidColorBrush(Color.FromRgb(80, 80, 120))));
                selectedTrigger.Setters.Add(new Setter(ListViewItem.BorderBrushProperty,
                    new SolidColorBrush(Color.FromRgb(100, 150, 255))));
                style.Triggers.Add(selectedTrigger);

                ListViewProducts.ItemContainerStyle = style;
            }
            else
            {
                // Светлая тема (по умолчанию)
                Background = Brushes.White;
                ListViewProducts.Background = Brushes.White;
                ListViewProducts.Foreground = Brushes.Black;
                ListViewProducts.BorderBrush = Brushes.LightGray;

                // Стиль по умолчанию
                var style = new Style(typeof(ListViewItem));
                style.Setters.Add(new Setter(ListViewItem.BackgroundProperty, new Binding("RowColor")));
                style.Setters.Add(new Setter(ListViewItem.BorderBrushProperty, Brushes.LightGray));
                style.Setters.Add(new Setter(ListViewItem.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
                style.Setters.Add(new Setter(ListViewItem.PaddingProperty, new Thickness(5)));
                style.Setters.Add(new Setter(ListViewItem.MinHeightProperty, 70.0));

                // Триггеры для светлой темы
                var mouseOverTrigger = new Trigger
                {
                    Property = ListViewItem.IsMouseOverProperty,
                    Value = true
                };
                mouseOverTrigger.Setters.Add(new Setter(ListViewItem.BackgroundProperty,
                    new SolidColorBrush(Color.FromArgb(255, 232, 244, 255))));
                style.Triggers.Add(mouseOverTrigger);

                var selectedTrigger = new Trigger
                {
                    Property = ListViewItem.IsSelectedProperty,
                    Value = true
                };
                selectedTrigger.Setters.Add(new Setter(ListViewItem.BackgroundProperty,
                    new SolidColorBrush(Color.FromArgb(255, 209, 239, 255))));
                selectedTrigger.Setters.Add(new Setter(ListViewItem.BorderBrushProperty,
                    new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));
                style.Triggers.Add(selectedTrigger);

                ListViewProducts.ItemContainerStyle = style;
            }

            // Обновляем отображение
            ListViewProducts.Items.Refresh();
        }

        // Смена темы, RadioButton
        private void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (radioButton.Content.ToString() == "☀")
                {
                    isDarkTheme = false;
                }
                else if (radioButton.Content.ToString() == "🌙")
                {
                    isDarkTheme = true;
                }

                ApplyTheme(isDarkTheme);
            }
        }

        // Обновление таблицы
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadProducts();
                ApplyTheme(isDarkTheme);
                MessageBox.Show($"Данные успешно обновлены! \nВремя обновления: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Дата и время
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeTextBlock.Text = DateTime.Now.ToString("F");
        }

        // Генерация ID
        private void GenerateID()
        {
            Random random = new Random();
            int ID = random.Next(10000, 99999);
            IDTextBlock.Text = ($"Ваш ID: {ID}");
        }

    }
}