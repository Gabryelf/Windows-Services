using Microsoft.Data.SqlClient;

namespace AdoNetTrainee
{
    public class UserRepository
    {
        private readonly string _connectionString;

        // Конструктор получает строку подключения
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // 1. ПОЛУЧИТЬ ВСЕХ ПОЛЬЗОВАТЕЛЕЙ
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT Id, Name, Age FROM Users ORDER BY Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
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

        public User GetUserByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT Id, Name, Age FROM Users WHERE Name = @Name";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

            return null; // Пользователь не найден
        }

        // 2. НАЙТИ ПОЛЬЗОВАТЕЛЯ ПО ID
        public User GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT Id, Name, Age FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

            return null; // Пользователь не найден
        }

        // 3. ДОБАВИТЬ ПОЛЬЗОВАТЕЛЯ
        public int AddUser(string name, int age)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "INSERT INTO Users (Name, Age) VALUES (@Name, @Age); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);

                    // Возвращает Id добавленной записи
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // 4. ОБНОВИТЬ ПОЛЬЗОВАТЕЛЯ
        public bool UpdateUser(int id, string name, int age)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "UPDATE Users SET Name = @Name, Age = @Age WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // true если обновилось
                }
            }
        }

        // 5. УДАЛИТЬ ПОЛЬЗОВАТЕЛЯ
        public bool DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "DELETE FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // true если удалилось
                }
            }
        }
    }
}
