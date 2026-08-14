using Microsoft.Data.SqlClient; // Подключаем библиотеку для работы с SQL

namespace AdoNetTrainee
{
    internal class Program
    {
        // ЭТО СТРОКА ПОДКЛЮЧЕНИЯ (Connection String)
        // Она говорит программе, где лежит база данных
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TraineeDB;Integrated Security=True;";

        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в ADO.NET тренажер!");
            Console.WriteLine("Нажимай любую клавишу, чтобы добавить первого пользователя...");
            Console.ReadKey();

            // Вызываем наш метод, который добавит запись
            AddUser("Михаил", 32);
            DeleteUser(1);


            Console.WriteLine("Готово! Проверь таблицу в обозревателе SQL Server (нажми правой кнопкой по таблице Users -> Показать данные)");
            Console.ReadKey();
        }

        // МЕТОД ДЛЯ ДОБАВЛЕНИЯ ПОЛЬЗОВАТЕЛЯ
        static void AddUser(string name, int age)
        {
            // using - это волшебная конструкция. Она гарантирует, что соединение с БД закроется АВТОМАТИЧЕСКИ, даже если будет ошибка.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Открываем дверь в базу данных
                connection.Open();

                // SQL-запрос. @Name и @Age - это "параметры". Никогда НЕ склеивай строки с именами через +, это опасно (SQL-инъекции)!
                string sqlQuery = "INSERT INTO Users (Name, Age) VALUES (@Name, @Age)";

                // SqlCommand - это наша инструкция для базы
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Подставляем реальные значения вместо @Name и @Age
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);

                    // Выполняем команду. ExecuteNonQuery - для команд, которые ничего не возвращают (INSERT, UPDATE, DELETE).
                    int rowsAffected = command.ExecuteNonQuery();

                    Console.WriteLine($"Добавлено строк: {rowsAffected}");
                }
            } // Здесь connection автоматически закроется (даже если была ошибка)
        }

        static void DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "DELETE FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();

                    Console.WriteLine($"Строка удалена: {rowsAffected}");
                }
            }
        }
    }
}
