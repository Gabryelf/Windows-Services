
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdoNetTrainee
{
    internal class Program
    {
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TraineeDB;Integrated Security=True;";
        static UserRepository repository = new UserRepository(connectionString);
        static Tests tests = new Tests(connectionString);

        static async Task Main(string[] args)
        {
            Console.WriteLine("🏢 СИСТЕМА УПРАВЛЕНИЯ ПОЛЬЗОВАТЕЛЯМИ (ADO.NET + ASYNC)");
            Console.WriteLine("===================================================\n");

            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Показать всех пользователей");
                Console.WriteLine("2 - Добавить нового пользователя");
                Console.WriteLine("3 - Найти пользователя по Id");
                Console.WriteLine("4 - Найти пользователя по Name");
                Console.WriteLine("5 - Обновить пользователя");
                Console.WriteLine("6 - Удалить пользователя по Id");
                Console.WriteLine("7 - Выход");
                Console.WriteLine("8 - Тестирование");
                Console.Write("\nВаш выбор: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        //await ShowAllUsersAsync();
                        // вызываем альтернативный метод
                        await ShowUsersAsDataTableAsync();
                        break;

                    case "2":
                        await AddNewUserAsync();
                        break;

                    case "3":
                        await FindUserByIdAsync();
                        break;

                    case "4":
                        //await FindUserByNameAsync();
                        // вызываем альтернативный метод
                        await SearchUsersByNameAsync();
                        break;

                    case "5":
                        await UpdateUserAsync();
                        break;

                    case "6":
                        await DeleteUserAsync();
                        break;

                    case "7":
                        exit = true;
                        Console.WriteLine("До свидания!");
                        break;

                    case "8":
                        await TestingSystem();
                        break;

                    default:
                        Console.WriteLine("❌ Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static async Task ShowAllUsersAsync()
        {
            var users = await repository.GetAllUsersAsync();

            if (users.Count == 0)
            {
                Console.WriteLine("📭 В базе нет пользователей.");
                return;
            }

            Console.WriteLine("\n📋 СПИСОК ПОЛЬЗОВАТЕЛЕЙ:");
            Console.WriteLine("┌─────┬──────────────┬─────┐");
            Console.WriteLine("│ Id  │ Name         │ Age │");
            Console.WriteLine("├─────┼──────────────┼─────┤");

            foreach (var user in users)
            {
                Console.WriteLine($"│ {user.Id,-3} │ {user.Name,-12} │ {user.Age,3} │");
            }

            Console.WriteLine("└─────┴──────────────┴─────┘");
        }

        // Дополнительный метод с использованием альтернативного DataTable
        static async Task ShowUsersAsDataTableAsync()
        {
            // Получаем DataTable
            DataTable dt = await repository.GetUsersAsDataTableAsync();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("📭 В базе нет пользователей.");
                return;
            }

            Console.WriteLine("\n📋 СПИСОК ПОЛЬЗОВАТЕЛЕЙ (DataTable):");
            Console.WriteLine("┌─────┬──────────────┬─────┐");
            Console.WriteLine("│ Id  │ Name         │ Age │");
            Console.WriteLine("├─────┼──────────────┼─────┤");

            // Проходим по каждой строке в DataTable
            foreach (DataRow row in dt.Rows)
            {
                // row["Id"] - обращение по имени колонки
                // Convert.ToInt32 - преобразуем object в int
                int id = Convert.ToInt32(row["Id"]);
                string name = row["Name"].ToString();
                int age = Convert.ToInt32(row["Age"]);

                Console.WriteLine($"│ {id,-3} │ {name,-12} │ {age,3} │");
            }

            Console.WriteLine("└─────┴──────────────┴─────┘");
        }

        static async Task AddNewUserAsync()
        {
            try
            {
                Console.Write("Введите имя: ");
                string name = Console.ReadLine();

                Console.Write("Введите возраст: ");
                int age = int.Parse(Console.ReadLine());

                int newId = await repository.AddUserAsync(name, age);
                Console.WriteLine($"✅ Пользователь добавлен с Id: {newId}");
            }
            catch (FormatException)
            {
                Console.WriteLine("❌ Ошибка: возраст должен быть числом!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }
        }

        static async Task FindUserByIdAsync()
        {
            try
            {
                Console.Write("Введите Id: ");
                int id = int.Parse(Console.ReadLine());

                var user = await repository.GetUserByIdAsync(id);

                if (user != null)
                {
                    Console.WriteLine($"\n🔍 НАЙДЕН ПОЛЬЗОВАТЕЛЬ:");
                    Console.WriteLine($"Id: {user.Id}");
                    Console.WriteLine($"Имя: {user.Name}");
                    Console.WriteLine($"Возраст: {user.Age}");
                }
                else
                {
                    Console.WriteLine($"\n⚠️ Пользователь с Id={id} не найден.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("❌ Ошибка: Id должен быть числом!");
            }
        }

        static async Task FindUserByNameAsync()
        {
            try
            {
                Console.Write("Введите имя: ");
                string name = Console.ReadLine();

                var user = await repository.GetUserByNameAsync(name);

                if (user != null)
                {
                    Console.WriteLine($"\n🔍 НАЙДЕН ПОЛЬЗОВАТЕЛЬ:");
                    Console.WriteLine($"Id: {user.Id}");
                    Console.WriteLine($"Имя: {user.Name}");
                    Console.WriteLine($"Возраст: {user.Age}");
                }
                else
                {
                    Console.WriteLine($"\n⚠️ Пользователь с Name='{name}' не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }
        }

        // альтернативный вызов регистронезависимого поиска по имени 
        static async Task SearchUsersByNameAsync()
        {
            try
            {
                Console.Write("Введите часть имени для поиска: ");
                string searchTerm = Console.ReadLine();

                // Если пользователь ничего не ввел - ищем всех
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    Console.WriteLine("Вы не ввели имя для поиска. Показываю всех пользователей...");
                    await ShowAllUsersAsync();
                    return;
                }

                var users = await repository.SearchUsersByNameAsync(searchTerm);

                if (users.Count == 0)
                {
                    Console.WriteLine($"🔍 Пользователей с именем, содержащим '{searchTerm}', не найдено.");
                    return;
                }

                Console.WriteLine($"\n🔍 НАЙДЕНО ПОЛЬЗОВАТЕЛЕЙ: {users.Count}");
                Console.WriteLine("┌─────┬──────────────┬─────┐");
                Console.WriteLine("│ Id  │ Name         │ Age │");
                Console.WriteLine("├─────┼──────────────┼─────┤");

                foreach (var user in users)
                {
                    Console.WriteLine($"│ {user.Id,-3} │ {user.Name,-12} │ {user.Age,3} │");
                }

                Console.WriteLine("└─────┴──────────────┴─────┘");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }
        }

        static async Task UpdateUserAsync()
        {
            try
            {
                Console.Write("Введите Id пользователя для обновления: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Введите новое имя: ");
                string name = Console.ReadLine();

                Console.Write("Введите новый возраст: ");
                int age = int.Parse(Console.ReadLine());

                bool updated = await repository.UpdateUserAsync(id, name, age);

                if (updated)
                    Console.WriteLine($"✅ Пользователь с Id={id} обновлен!");
                else
                    Console.WriteLine($"⚠️ Пользователь с Id={id} не найден.");
            }
            catch (FormatException)
            {
                Console.WriteLine("❌ Ошибка: введите корректные данные!");
            }
        }

        static async Task DeleteUserAsync()
        {
            try
            {
                Console.Write("Введите Id для удаления: ");
                int id = int.Parse(Console.ReadLine());

                bool deleted = await repository.DeleteUserAsync(id);

                if (deleted)
                    Console.WriteLine($"✅ Пользователь с Id={id} удален!");
                else
                    Console.WriteLine($"⚠️ Пользователь с Id={id} не найден.");
            }
            catch (FormatException)
            {
                Console.WriteLine("❌ Ошибка: Id должен быть числом!");
            }
        }

        static async Task TestingSystem()
        {
            await tests.RunTests();
        }
    }
}