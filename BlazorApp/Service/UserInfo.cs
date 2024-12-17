using Npgsql;

namespace BlazorApp.Service
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<UserInfo> GetUserInfoByIdAsync(int userId)
        {
            UserInfo userInfo = null;

            await using var conn = GetConnection();
            await conn.OpenAsync();

            string query = "SELECT id, name, email, mobile FROM users WHERE id = @userId";
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("userId", userId);

            try
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    userInfo = new UserInfo
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        PhoneNumber = reader.GetString(3)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving user info: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
            }

            return userInfo;
        }
    }
}