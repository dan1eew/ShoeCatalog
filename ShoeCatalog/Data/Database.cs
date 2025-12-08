using System;
using System.Data;
using System.Data.SqlClient;

namespace ShoeCatalog
{
    ///<summary>Статический класс для работы с базой данных</summary>
    public static class Database
    {
        private static readonly string connectionString =
            @"Data Source=(LocalDB)\6;AttachDbFilename=|DataDirectory|\TableProducts.mdf;Integrated Security=True";


        ///<summary>Получает список товаров из базы данных</summary>
        /// <returns>DataTable с данными о товарах</returns>
        public static DataTable GetProducts()
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string query = "SELECT * FROM Products";
                using var command = new SqlCommand(query, connection);
                using var adapter = new SqlDataAdapter(command);

                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка подключения к базе данных: {ex.Message}", ex);
            }
        }
    }
}