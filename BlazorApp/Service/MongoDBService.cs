using MongoDB.Driver;
using MongoDB.Bson;

namespace BlazorApp.Service;

public class MongoDBService
{
    private readonly string _connectionString;

    public MongoDBService(string connectionString)
    {
        // Console.WriteLine("Connection string MongoDBServcie:" + connectionString);
        _connectionString = connectionString;
    }

    public MongoClient GetConnection()
    {
        // Console.WriteLine("Connection string GetConnection:" + _connectionString);
        var settings = MongoClientSettings.FromConnectionString(_connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        MongoClient client = new(settings);

        return client;
    }

    public async Task<bool> CheckConnectionAsync()
    {
        try
        {
            var connection = GetConnection();
            var result = await connection.GetDatabase("admin")
                .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            return result.Contains("ok") && result["ok"] == 1.0; // Checks if "ok" exists and that ok has the value 1.
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error connection to Mongo server: " + ex.Message);
            return false;
        }
    }
}