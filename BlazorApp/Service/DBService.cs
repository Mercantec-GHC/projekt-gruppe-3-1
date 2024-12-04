namespace BlazorApp.Service;

using Npgsql;

public class DBService
{
    public static DBService Instance;

    // Essentically our DefaultConnection in our appsettings.json.
    private readonly string _connectionString;

    public DBService(string connectionString)
    {
        _connectionString = connectionString;
        Instance = this;
    }

    // We then made a NpgsqlConnection, open it and then returns it.
    public NpgsqlConnection GetConnection()
    {
        NpgsqlConnection connection = new(_connectionString);
        connection.Open();
        return connection;
    }

    public async Task FunctionName()
    {
        string query = "";
        using (var connection = GetConnection())
        using (var command = new NpgsqlCommand(query, connection))
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                reader.GetInt32(0);
            }
        }
    }

    public async Task AddEvToDbAsync(MiniCooper.EvMiniCooper miniCooper, int userId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string query =
            "INSERT INTO cars (a_car, account_id)" +
            $"VALUES (ROW (ROW (ROW ('{miniCooper.ModelName}', '{miniCooper.Generation}', '{miniCooper.ModelType}', '{miniCooper.Color}', {miniCooper.Price}, {miniCooper.Mileage}, {miniCooper.MaxRange}, {miniCooper.Weight}, '{miniCooper.FuelType}', '{miniCooper.GearType}', {miniCooper.YearlyTax}, ARRAY ['base64string1', 'base64string2'])::mini_cooper, 40, 7.2)::ev_mini_cooper, NULL, NULL)::car,1);";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);
    }
    
    public async Task AddFossilToDbAsync(MiniCooper.FossilMiniCooper miniCooper, int userId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        
        /*var tempCooper = 

            string query = "INSERT INTO cars (a_car, account_id) " +
                           "VALUES (ROW(" +
                           "NULL," +
                           "ROW (" +
                           $"ROW({miniCooper.}))," +
                           "NULL)::car, {userId})";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);*/
    }
    
    public async Task AddHybridToDbAsync(MiniCooper.HybridMiniCooper miniCooper, int userId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        
        /*var tempCooper = 

            string query = "INSERT INTO cars (a_car, account_id) " +
                           "VALUES (ROW(" +
                           "NULL," +
                           "ROW (" +
                           $"ROW({miniCooper.}))," +
                           "NULL)::car, {userId})";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);*/
    }

    /// <summary>
    /// Deletes a car entry from the 'cars' table based on the provided car ID and resets the car IDs to maintain order.
    /// </summary>
    /// <param name="carId">
    /// An <see cref="int"/> representing the unique identifier of the car to be deleted from the 'cars' table.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// After deleting the car record with the specified ID, this method calls <see cref="ResetTableIdsAsync"/> to
    /// ensure that the IDs in the 'cars' table are reset to a continuous series starting from 1. 
    /// 
    /// Note: The SQL query concatenates the car ID directly into the command string, which could lead to SQL Injection 
    /// if the input is not correctly validated in a real-world scenario. It is advisable to use parameterized queries 
    /// instead.
    /// </remarks>
    /// <example>
    /// <code>
    /// await DeleteCarByIdAsync(123);
    /// </code>
    /// </example>
    public async Task DeleteCarByIdAsync(int carId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string query = $"DELETE FROM cars WHERE car_id = {carId}";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);

        await ResetTableIdsAsync("cars");
    }

    /// <summary>
    /// Resets the IDs of a specified table in a PostgreSQL database to start from 1, while preserving the order and 
    /// updating the sequence associated with the table's ID.
    /// </summary>
    /// <param name="tableName">
    /// A <see cref="string"/> representing the name of the table whose IDs should be reset. This parameter should not be null or empty.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method performs the following steps:
    /// 1. Creates a temporary table with an additional column, `new_id`, which renumbers the rows starting from 1.
    /// 2. Updates the original table's `id` column to match the new IDs from the temporary table.
    /// 3. Adjusts the sequence associated with the table IDs to ensure it starts after the maximum ID currently in use.
    /// 4. Drops the temporary table after use.
    /// 
    /// It's important to have appropriate permissions and handle exceptions that may arise from SQL execution in production code.
    /// </remarks>
    /// <example>
    /// <code>
    /// await ResetTableIdsAsync("my_table");
    /// </code>
    /// </example>
    private async Task ResetTableIdsAsync(string tableName)
    {
        // Since we are using "using", we dont have to add a close statement at the end, because "using" does that.
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string query = $"CREATE TEMP TABLE temp_{tableName} AS" +
                       "SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id" +
                       $"FROM {tableName};" +
                       $"UPDATE {tableName}" +
                       $"SET id = temp_{tableName}.new_id" +
                       $"FROM temp_{tableName}" +
                       $"WHERE {tableName}.id = temp_{tableName}.id;" +
                       $"SELECT setval('{tableName}_id_seq', (SELECT MAX(id) FROM {tableName}));" +
                       $"DROP TABLE temp_{tableName};";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);
    }

    /// <summary>
    /// Udfører en given async-SQL kommando og retunerer antal af rækker påvirket.
    /// </summary>
    /// <param name="query">
    /// A <see cref="NpgsqlCommand"/> object that represents the SQL command to execute. This command should be 
    /// set up with the necessary connection and parameters before calling this method.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="int"/> indicating 
    /// the number of records affected by the execution of the non-query command.
    /// </returns>
    /// <remarks>
    /// If the result of the command execution is less than or equal to zero, the method logs a message indicating
    /// that no records were affected. Otherwise, it logs the number of records affected.
    /// </remarks>
    /// <example>
    /// <code>
    /// var command = new NpgsqlCommand("UPDATE my_table SET my_column = value WHERE condition");
    /// int affectedRecords = await RunAsyncQuery(command);
    /// Console.WriteLine($"Total records affected: {affectedRecords}");
    /// </code>
    /// </example>
    private async Task<int> RunAsyncQuery(NpgsqlCommand query)
    {
        int result = await query.ExecuteNonQueryAsync();
        if (result <= 0)
            Console.WriteLine("No records affected.");
        else
            Console.WriteLine("Records affected: " + result);

        return result;
    }
}