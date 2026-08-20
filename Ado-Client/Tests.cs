using Microsoft.Data.SqlClient;
using System.Data;

namespace AdoNetTrainee
{
    public class Tests
    {
        private readonly string _connectionString;

        public Tests(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task RunTests() {

            using SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync();
                Console.WriteLine("SQL соединение открыто.");

                // Получение результата агрегатной функции
                using var sqlCommandForCount = connection.CreateCommand();
                sqlCommandForCount.CommandText = "SELECT COUNT(*) FROM Users";
                var count = await sqlCommandForCount.ExecuteScalarAsync();

                Console.WriteLine($"Полное число пользователей: {count}");

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
            finally 
            {
                connection.Close();
                Console.WriteLine("SQL соединение закрыто.");
            }
            
        }
    }
}
