using Microsoft.Data.SqlClient;
using System.Data;

namespace AdoNetTrainee
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // АСИНХРОННЫЙ МЕТОД ДЛЯ ПОЛУЧЕНИЯ ВСЕХ ПОЛЬЗОВАТЕЛЕЙ
        public async Task<List<User>> GetAllUsersAsync()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "SELECT Id, Name, Age FROM Users ORDER BY Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Age = reader.GetInt32(2)
                        });
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Получает всех пользователей в виде DataTable.
        /// DataTable удобна для привязки к интерфейсам (WPF, WinForms, ASP.NET GridView)
        /// </summary>
        public async Task<DataTable> GetUsersAsDataTableAsync()
        {
            // Создаем пустую таблицу в памяти
            DataTable dataTable = new DataTable();

            // Строка подключения та же, что и везде
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL-запрос - берем всех пользователей
                string sqlQuery = "SELECT Id, Name, Age FROM Users ORDER BY Id";

                // SqlDataAdapter - это мост между базой и DataTable
                // Он сам открывает/закрывает соединение, если оно закрыто
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection))
                {
                    // Асинхронно заполняем DataTable
                    // adapter.FillAsync() - сам выполняет SELECT и заполняет таблицу
                    await Task.Run(() => adapter.Fill(dataTable));

                    // DataTable теперь содержит все строки из базы
                }
            }

            // Возвращаем заполненную таблицу
            return dataTable;
        }

        // АСИНХРОННЫЙ ПОИСК ПО ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "SELECT Id, Name, Age FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Age = reader.GetInt32(2)
                            };
                        }
                    }
                }
            }

            return null;
        }

        // АСИНХРОННЫЙ ПОИСК ПО NAME
        public async Task<User> GetUserByNameAsync(string name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "SELECT Id, Name, Age FROM Users WHERE Name = @Name";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Age = reader.GetInt32(2)
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Ищет пользователей по части имени (регистронезависимый поиск)
        /// </summary>
        public async Task<List<User>> SearchUsersByNameAsync(string searchTerm)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // LIKE для поиска по шаблону
                // COLLATE SQL_Latin1_General_CP1_CI_AS - делает поиск регистронезависимым (Иванов = иванов)
                string sqlQuery = @"
                    SELECT Id, Name, Age 
                    FROM Users 
                    WHERE Name LIKE @SearchTerm 
                    COLLATE SQL_Latin1_General_CP1_CI_AS
                    ORDER BY Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Добавляем % вокруг поискового слова
                    // Если пользователь ввел "ан", то ищем "%ан%"
                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Age = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            return users;
        }

        // АСИНХРОННОЕ ДОБАВЛЕНИЕ
        public async Task<int> AddUserAsync(string name, int age)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO Users (Name, Age) VALUES (@Name, @Age); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        // АСИНХРОННОЕ ОБНОВЛЕНИЕ
        public async Task<bool> UpdateUserAsync(int id, string name, int age)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "UPDATE Users SET Name = @Name, Age = @Age WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        // АСИНХРОННОЕ УДАЛЕНИЕ
        public async Task<bool> DeleteUserAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "DELETE FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        // СИНХРОННЫЕ МЕТОДЫ (оставляем для совместимости, но помечаем как устаревшие)
        [Obsolete("Используйте асинхронную версию GetAllUsersAsync()")]
        public List<User> GetAllUsers() => GetAllUsersAsync().GetAwaiter().GetResult();

        [Obsolete("Используйте асинхронную версию GetUserByIdAsync()")]
        public User GetUserById(int id) => GetUserByIdAsync(id).GetAwaiter().GetResult();

        [Obsolete("Используйте асинхронную версию AddUserAsync()")]
        public int AddUser(string name, int age) => AddUserAsync(name, age).GetAwaiter().GetResult();

        [Obsolete("Используйте асинхронную версию UpdateUserAsync()")]
        public bool UpdateUser(int id, string name, int age) => UpdateUserAsync(id, name, age).GetAwaiter().GetResult();

        [Obsolete("Используйте асинхронную версию DeleteUserAsync()")]
        public bool DeleteUser(int id) => DeleteUserAsync(id).GetAwaiter().GetResult();
    }
}