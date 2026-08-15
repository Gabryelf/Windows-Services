using Microsoft.Data.SqlClient;

namespace AdoNetTrainee
{
    internal class Program
    {
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TraineeDB;Integrated Security=True;";
        static UserRepository repository = new UserRepository(connectionString);

        static void Main(string[] args)
        {
            Console.WriteLine("🏢 СИСТЕМА УПРАВЛЕНИЯ ПОЛЬЗОВАТЕЛЯМИ (ADO.NET)");
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
                Console.Write("\nВаш выбор: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAllUsers();
                        break;

                    case "2":
                        AddNewUser();
                        break;

                    case "3":
                        FindUserById();
                        break;

                    case "4":
                        FindUserByName();
                        break;

                    case "5":
                        UpdateUser();
                        break;

                    case "6":
                        DeleteUser();
                        break;

                    case "7":
                        exit = true;
                        Console.WriteLine("До свидания!");
                        break;

                    default:
                        Console.WriteLine("❌ Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void ShowAllUsers()
        {
            var users = repository.GetAllUsers();

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

        static void AddNewUser()
        {
            try
            {
                Console.Write("Введите имя: ");
                string name = Console.ReadLine();

                Console.Write("Введите возраст: ");
                int age = int.Parse(Console.ReadLine());

                int newId = repository.AddUser(name, age);
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

        static void FindUserById()
        {
            try
            {
                Console.Write("Введите Id: ");
                int id = int.Parse(Console.ReadLine());

                var user = repository.GetUserById(id);

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

        static void FindUserByName()
        {
            try
            {
                Console.Write("Введите имя (name): ");
                string name = Console.ReadLine();

                var user = repository.GetUserByName(name);

                if (user != null)
                {
                    Console.WriteLine($"\n🔍 НАЙДЕН ПОЛЬЗОВАТЕЛЬ:");
                    Console.WriteLine($"Id: {user.Id}");
                    Console.WriteLine($"Имя: {user.Name}");
                    Console.WriteLine($"Возраст: {user.Age}");
                }
                else
                {
                    Console.WriteLine($"\n⚠️ Пользователь с Name={name} не найден.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("❌ Ошибка: некоектное имя!");
            }
        }
        static void UpdateUser()
        {
            try
            {
                Console.Write("Введите Id пользователя для обновления: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Введите новое имя: ");
                string name = Console.ReadLine();

                Console.Write("Введите новый возраст: ");
                int age = int.Parse(Console.ReadLine());

                bool updated = repository.UpdateUser(id, name, age);

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

        static void DeleteUser()
        {
            try
            {
                Console.Write("Введите Id для удаления: ");
                int id = int.Parse(Console.ReadLine());

                bool deleted = repository.DeleteUser(id);

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
    }
}