using System;
using System.Data;
using System.Data.SqlClient;

namespace ShoeCatalog
{
    public static class Database
    {
        private static string connectionString = @"Data Source=(LocalDB)\6;AttachDbFilename=|DataDirectory|\TableProducts.mdf;Integrated Security=True";

        public static DataTable GetProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Products";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }
    }
}