using ShoeCatalog.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace ShoeCatalog
{
    /// <summary>Главное окно приложения - Shoe Catalog</summary>
    public partial class MainWindow : Window
    {
        private readonly List<Product> _products = new();
        private readonly Dictionary<string, bool> _columnVisibility = new()
        {
            { "Фото", true },
            { "Артикул", false },
            { "Наименование", true },
            { "Категория", true },
            { "Производитель", false },
            { "Поставщик", false },
            { "Цена", true },
            { "Скидка", true },
            { "Кол-во", true },
            { "Описание", false }
        };

        /// <summary>Инициализирует главное окно</summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Загружает данные при запуске окна</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadProducts();
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
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при обновлении", ex.Message);
            }
        }

        /// <summary>Показывает окно настроек столбцов</summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();

            // Устанавливаем текущие настройки
            foreach (var kvp in _columnVisibility)
            {
                if (settingsWindow.CurrentVisibility.ContainsKey(kvp.Key))
                {
                    settingsWindow.CurrentVisibility[kvp.Key] = kvp.Value;
                }
            }

            if (settingsWindow.ShowDialog() == true)
            {
                // Применяем новые настройки
                foreach (var kvp in settingsWindow.CurrentVisibility)
                {
                    if (_columnVisibility.ContainsKey(kvp.Key))
                    {
                        _columnVisibility[kvp.Key] = kvp.Value;
                    }
                }

                ApplyColumnVisibility();
            }
        }

        /// <summary>Применяет настройки видимости столбцов</summary>
        private void ApplyColumnVisibility()
        {
            if (ListViewProducts.View is System.Windows.Controls.GridView gridView)
            {
                foreach (System.Windows.Controls.GridViewColumn column in gridView.Columns)
                {
                    string columnName = GetColumnName(column);
                    if (columnName != null && _columnVisibility.ContainsKey(columnName))
                    {
                        column.Width = _columnVisibility[columnName] ? GetDefaultWidth(columnName) : 0;
                    }
                }
            }
        }

        /// <summary>Получает название столбца по объекту колонки</summary>
        private string GetColumnName(System.Windows.Controls.GridViewColumn column)
        {
            return column.Header?.ToString() switch
            {
                "Фото" => "Фото",
                "Артикул" => "Артикул",
                "Наименование" => "Наименование",
                "Категория" => "Категория",
                "Производитель" => "Производитель",
                "Поставщик" => "Поставщик",
                "Цена" => "Цена",
                "Скидка" => "Скидка",
                "Кол-во" => "Кол-во",
                "Описание" => "Описание",
                _ => null
            };
        }

        /// <summary>Получает ширину по умолчанию для столбца</summary>
        private double GetDefaultWidth(string columnName)
        {
            return columnName switch
            {
                "Фото" => 100,
                "Артикул" => 90,
                "Наименование" => 200,
                "Категория" => 130,
                "Производитель" => 130,
                "Поставщик" => 130,
                "Цена" => 140,
                "Скидка" => 70,
                "Кол-во" => 80,
                "Описание" => 250,
                _ => 100
            };
        }

        /// <summary>Показывает окно с ошибкой</summary>
        private static void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}