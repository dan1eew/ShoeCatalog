using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ShoeCatalog
{
    public partial class SettingsWindow : Window
    {
        // Словарь для хранения видимости столбцов
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
            { "Описание", false },
            { "Ед. изм.", false }
        };

        // Словарь для связи CheckBox с названиями столбцов
        private readonly Dictionary<string, CheckBox> _checkBoxes = new();

        // Обязательные столбцы, которые нельзя скрыть
        private readonly List<string> _requiredColumns = new() { "Наименование", "Цена" };

        // Свойство для получения текущих настроек
        public Dictionary<string, bool> CurrentVisibility => _columnVisibility;

        public SettingsWindow()
        {
            InitializeComponent();
            InitializeCheckBoxes();
        }

        /// <summary>
        /// Инициализация CheckBox для каждого столбца
        /// </summary>
        private void InitializeCheckBoxes()
        {
            ColumnsPanel.Children.Clear();
            _checkBoxes.Clear();

            // Порядок отображения столбцов
            var columnsOrder = new List<string>
            {
                "Фото", "Артикул", "Наименование", "Категория",
                "Производитель", "Поставщик", "Цена", "Скидка",
                "Кол-во", "Описание", "Ед. изм."
            };

            foreach (var columnName in columnsOrder)
            {
                if (!_columnVisibility.ContainsKey(columnName))
                    continue;

                var checkBox = new CheckBox
                {
                    Content = columnName,
                    IsChecked = _columnVisibility[columnName],
                    FontSize = 14,
                    Margin = new Thickness(5, 3, 5, 3)
                };

                // Если столбец обязательный, делаем его недоступным для изменения
                if (_requiredColumns.Contains(columnName))
                {
                    checkBox.IsEnabled = false;
                    checkBox.FontWeight = FontWeights.Bold;
                    checkBox.ToolTip = "Этот столбец обязателен для отображения";
                }

                _checkBoxes[columnName] = checkBox;
                ColumnsPanel.Children.Add(checkBox);
            }
        }

        /// <summary>
        /// Выбрать все столбцы
        /// </summary>
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkBox in _checkBoxes.Values)
            {
                if (checkBox.IsEnabled)
                    checkBox.IsChecked = true;
            }
        }

        /// <summary>
        /// Установить настройки по умолчанию
        /// </summary>
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            // Настройки по умолчанию из ТЗ
            var defaultSettings = new Dictionary<string, bool>
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
                { "Описание", false },
                { "Ед. изм.", false }
            };

            foreach (var kvp in defaultSettings)
            {
                if (_checkBoxes.TryGetValue(kvp.Key, out var checkBox) && checkBox.IsEnabled)
                {
                    checkBox.IsChecked = kvp.Value;
                }
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем словарь видимости
            foreach (var kvp in _checkBoxes)
            {
                _columnVisibility[kvp.Key] = kvp.Value.IsChecked == true;
            }

            // Проверяем, что выбрана хотя бы одна колонка
            int visibleCount = _columnVisibility.Count(kvp => kvp.Value);
            if (visibleCount == 0)
            {
                MessageBox.Show("Должна быть видна хотя бы одна колонка!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем, что обязательные колонки выбраны
            foreach (var requiredColumn in _requiredColumns)
            {
                if (!_columnVisibility[requiredColumn])
                {
                    MessageBox.Show($"Столбец '{requiredColumn}' должен быть видимым!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DialogResult = true;
        }

        /// <summary>
        /// Отменить изменения
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}