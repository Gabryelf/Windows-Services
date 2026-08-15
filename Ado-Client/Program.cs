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
            Console.WriteLine("🏢 ДОБРО ПОЖАЛОВАТЬ В СИСТЕМУ УПРАВЛЕНИЯ ПОЛЬЗОВАТЕЛЯМИ");
            Console.WriteLine("===================================================\n");

            bool exit = false;

            while (!exit) // Бесконечный цикл, пока пользователь не выберет "Выход"
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Показать всех пользователей");
                Console.WriteLine("2 - Добавить нового пользователя");
                Console.WriteLine("3 - Найти пользователя по Id");
                Console.WriteLine("4 - Удалить пользователя по Id");
                Console.WriteLine("5 - Выход");
                Console.Write("\nВаш выбор: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAllUsers();
                        break;

                    case "2":
                        Console.Write("Введите имя: ");
                        string name = Console.ReadLine();
                        Console.Write("Введите возраст: ");
                        int age = int.Parse(Console.ReadLine());
                        AddUser(name, age);
                        break;

                    case "3":
                        Console.Write("Введите Id: ");
                        int idFind = int.Parse(Console.ReadLine());
                        FindUserById(idFind);
                        break;

                    case "4":
                        Console.Write("Введите Id: ");
                        int idDelete = int.Parse(Console.ReadLine());
                        DeleteUser(idDelete);
                        break;

                    case "5":
                        exit = true;
                        Console.WriteLine("До свидания!");
                        break;

                    default:
                        Console.WriteLine("❌ Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
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

        // МЕТОД УДАЛЕНИЯ ПОЛЬЗОВАТЕЛЯ
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

        // МЕТОД ЗАПРОСА НА ПОКАЗ ПОЛЬЗОВАТЕЛЕЙ ИЗ ТАБЛИЦЫ
        static void ShowAllUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT Id, Name, Age FROM Users ORDER BY Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // SqlDataReader - это "курсор", который идет по строкам результата
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("\n📋 СПИСОК ПОЛЬЗОВАТЕЛЕЙ:");
                        Console.WriteLine("┌─────┬──────────────┬─────┐");
                        Console.WriteLine("│ Id  │ Name         │ Age │");
                        Console.WriteLine("├─────┼──────────────┼─────┤");

                        while (reader.Read()) // Читаем построчно, пока есть данные
                        {
                            int id = reader.GetInt32(0);        // Первая колонка (Id)
                            string name = reader.GetString(1);  // Вторая колонка (Name)
                            int age = reader.GetInt32(2);       // Третья колонка (Age)

                            Console.WriteLine($"│ {id,-3} │ {name,-12} │ {age,3} │");
                        }

                        Console.WriteLine("└─────┴──────────────┴─────┘");
                    }
                }
            }
        }

        // МЕТОД ПОИСКА ПОЛЬЗОВАТЕЛЯ ПО "ID"
        static void FindUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT Id, Name, Age FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Если есть хотя бы одна строка
                        {
                            string name = reader.GetString(1);
                            int age = reader.GetInt32(2);

                            Console.WriteLine($"\n🔍 НАЙДЕН ПОЛЬЗОВАТЕЛЬ:");
                            Console.WriteLine($"Id: {id}");
                            Console.WriteLine($"Имя: {name}");
                            Console.WriteLine($"Возраст: {age}");
                        }
                        else
                        {
                            Console.WriteLine($"\n⚠️ Пользователь с Id={id} не найден.");
                        }
                    }
                }
            }
        }
    }
}
