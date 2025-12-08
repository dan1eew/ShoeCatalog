using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace ShoeCatalog
{
    /// <summary>Главное окно приложения - Shoe Сatalog</summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme;
        private readonly List<Product> _products = new();
        private DispatcherTimer _timer;

        /// <summary>Инициализирует главное окно</summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            GenerateID();
        }

        /// <summary>Загружает данные при запуске окна</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadProducts();
                ApplyTheme(_isDarkTheme);
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при загрузке", ex.Message);
            }
        }

        /// <summary>Загружает товары из базы данных</summary>
        private void LoadProducts()
        {
            try
            {
                _products.Clear();
                var dataTable = Database.GetProducts();

                foreach (DataRow row in dataTable.Rows)
                    _products.Add(Product.FromDataRow(row));

                var viewModels = new List<ProductViewModel>();
                foreach (var product in _products)
                    viewModels.Add(new ProductViewModel(product));

                ListViewProducts.ItemsSource = viewModels;
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки данных", ex.Message);
            }
        }

        /// <summary>Обновляет данные в таблице</summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadProducts();
                ApplyTheme(_isDarkTheme);  
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при обновлении", ex.Message);
            }
            //MessageBox.Show($"Данные успешно обновлены!\nВремя обновления: {DateTime.Now}",
            //        "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>Применяет выбранную тему оформления</summary>
        private void ApplyTheme(bool dark)
        {
            _isDarkTheme = dark;
            if (ListViewProducts == null) return;

            if (dark) ApplyDarkTheme();
            else ApplyLightTheme();

            UpdateViewModelsTheme(dark);
            ListViewProducts.Items.Refresh();
        }

        /// <summary>Применяет темную тему</summary>
        private void ApplyDarkTheme()
        {
            var darkBrush = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            var grayBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));

            Background = darkBrush;
            TopPanel.Background = darkBrush;
            BottomPanelText.Background = darkBrush;

            ListViewProducts.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            ListViewProducts.Foreground = Brushes.White;
            ListViewProducts.BorderBrush = grayBrush;

            ChangeTextColors(true);
            ApplyDarkListViewStyle();
        }

        /// <summary>Применяет светлую тему</summary>
        private void ApplyLightTheme()
        {
            Background = Brushes.White;
            TopPanel.Background = Brushes.White;
            BottomPanelText.Background = Brushes.White;
            ListViewProducts.Background = Brushes.White;
            ListViewProducts.Foreground = Brushes.Black;
            ListViewProducts.BorderBrush = Brushes.LightGray;

            ChangeTextColors(false);
            ApplyLightListViewStyle();
        }

        /// <summary>Обновляет тему во всех ViewModel</summary>
        private void UpdateViewModelsTheme(bool dark)
        {
            if (ListViewProducts.ItemsSource is IEnumerable<ProductViewModel> viewModels)
                foreach (var vm in viewModels)
                    vm.UpdateTheme(dark);
        }

        /// <summary>Меняет цвета текста в зависимости от темы</summary>
        private void ChangeTextColors(bool dark)
        {
            var textColor = dark ? Brushes.White : Brushes.Black;

            TitleTextBlock.Foreground = textColor;
            TimeTextBlock.Foreground = textColor;
            IDTextBlock.Foreground = textColor;
            VerTextLabel.Foreground = textColor;
        }

        /// <summary>Создает стиль ListView для темной темы</summary>
        private void ApplyDarkListViewStyle()
        {
            var style = new Style(typeof(ListViewItem));
            AddBaseStyleSetters(style);
            style.Setters.Add(new Setter(ListViewItem.BorderBrushProperty,
                new SolidColorBrush(Color.FromRgb(70, 70, 70))));

            style.Triggers.Add(CreateMouseOverTrigger(new SolidColorBrush(Color.FromRgb(60, 60, 60))));
            style.Triggers.Add(CreateSelectedTrigger(
                new SolidColorBrush(Color.FromRgb(80, 80, 120)),
                new SolidColorBrush(Color.FromRgb(100, 150, 255))));

            ListViewProducts.ItemContainerStyle = style;
        }

        /// <summary>Создает стиль ListView для светлой темы</summary>
        private void ApplyLightListViewStyle()
        {
            var style = new Style(typeof(ListViewItem));
            AddBaseStyleSetters(style);
            style.Setters.Add(new Setter(ListViewItem.BorderBrushProperty, Brushes.LightGray));

            style.Triggers.Add(CreateMouseOverTrigger(
                new SolidColorBrush(Color.FromArgb(255, 232, 244, 255))));
            style.Triggers.Add(CreateSelectedTrigger(
                new SolidColorBrush(Color.FromArgb(255, 209, 239, 255)),
                new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

            ListViewProducts.ItemContainerStyle = style;
        }

        /// <summary>Добавляет базовые настройки стиля ListView</summary>
        private static void AddBaseStyleSetters(Style style)
        {
            style.Setters.Add(new Setter(ListViewItem.BackgroundProperty, new Binding("RowColor")));
            style.Setters.Add(new Setter(ListViewItem.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
            style.Setters.Add(new Setter(ListViewItem.PaddingProperty, new Thickness(5)));
            style.Setters.Add(new Setter(ListViewItem.MinHeightProperty, 70.0));
        }

        /// <summary>Создает триггер при наведении мыши</summary>
        private static Trigger CreateMouseOverTrigger(Brush background)
        {
            return new Trigger
            {
                Property = ListViewItem.IsMouseOverProperty,
                Value = true,
                Setters = { new Setter(ListViewItem.BackgroundProperty, background) }
            };
        }

        /// <summary>Создает триггер при выделении элемента</summary>
        private static Trigger CreateSelectedTrigger(Brush background, Brush border)
        {
            return new Trigger
            {
                Property = ListViewItem.IsSelectedProperty,
                Value = true,
                Setters =
                {
                    new Setter(ListViewItem.BackgroundProperty, background),
                    new Setter(ListViewItem.BorderBrushProperty, border)
                }
            };
        }

        /// <summary>Обрабатывает переключение темы через RadioButton</summary>
        private void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                _isDarkTheme = radioButton.Content.ToString() == "🌙";
                ApplyTheme(_isDarkTheme);
            }
        }

        /// <summary>Показывает окно с ошибкой</summary>
        private static void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>Инициализирует таймер для обновления времени</summary>
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => TimeTextBlock.Text = DateTime.Now.ToString("F");
            _timer.Start();
        }

        /// <summary>Генерирует случайный ID пользователя</summary>
        private void GenerateID()
        {
            var random = new Random();
            IDTextBlock.Text = $"Ваш ID: {random.Next(10000, 99999)}";
        }
    }
}