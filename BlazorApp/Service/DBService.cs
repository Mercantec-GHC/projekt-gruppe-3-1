using Npgsql;

namespace BlazorApp.Service;

public class DBService
{
    // Essentically our DefaultConnection, in our appsettings.json.
    private readonly string _connectionString;

    public DBService(string connectionString)
    {
        Console.WriteLine("Conneting DBService:"+ connectionString);
        _connectionString = connectionString;
    }

    // We then made a NpgsqlConnection, open it and then returns it.
    public NpgsqlConnection GetConnection()
    {
        NpgsqlConnection connection = new(_connectionString);
        connection.Open();
        return connection;
    }
}