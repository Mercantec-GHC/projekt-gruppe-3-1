using System.Data;
using Npgsql.PostgresTypes;

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
    public async Task<List<UsersService.User>> GetAllUsersAsync()
    {
        List<UsersService.User> users = new();

        var conn = GetConnection();
        string query = "SELECT " +
                       "id, " +
                       "(a_user).name, " +
                       "(a_user).password, " +
                       "(a_user).mobile, " +
                       "(a_user).email, " +
                       "(a_user).city, " +
                       "(a_user).address " +
                       "FROM users";

        await using var cmd = new NpgsqlCommand(query, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            UsersService.User user = new()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Password = reader.GetString(2),
                Mobile = reader.GetInt32(3),
                Email = reader.GetString(4),
                City = reader.GetString(5),
                Address = reader.GetString(6)
            };

            users.Add(user);
        }

        return users;
    }
    
    /// <summary>
    /// Retrieves a list of Mini Cooper car entries from the database and identifies their type as either electric, fossil, or hybrid.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a list of <see cref="MiniCooper.FullMiniCooper"/> instances.
    /// Each instance represents a Mini Cooper entry from the database.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database, executes a query to retrieve car data, and processes each record to identify the type of car.
    /// Based on the type, respective methods such as <see cref="GetEvByIdAsync"/> or <see cref="GetFossilByIdAsync"/> are called to get detailed information.
    /// The method also logs messages to the console indicating which type of car has been added to the list.
    /// </remarks>
    public async Task<List<MiniCooper.FullMiniCooper>> GetAllMiniCoopersAsync()
    {
        List<MiniCooper.FullMiniCooper> fullMiniCoopers = new();

        await using var conn = GetConnection();

        string query =
            "SELECT id, (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car, account_id FROM cars;";
        await using var cmd = new NpgsqlCommand(query, conn);

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var currentId = reader.GetInt32(0);

                if (!reader.IsDBNull(1))
                {
                    Console.WriteLine("Ev added!");
                    var tempFullCooper = await GetEvByIdAsync(currentId);
                    tempFullCooper.SetIds(currentId, reader.GetInt32(4));
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else if (!reader.IsDBNull(2))
                {
                    Console.WriteLine("Fossil added!");
                    var tempFullCooper = await GetFossilByIdAsync(currentId);
                    tempFullCooper.SetIds(currentId, reader.GetInt32(4));
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else if (!reader.IsDBNull(3))
                {
                    Console.WriteLine("Hybrid added!");
                    var tempFullCooper = await GetHybridByIdAsync(currentId);
                    tempFullCooper.SetIds(currentId, reader.GetInt32(4));
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else
                {
                    Console.WriteLine("No car has been assigned to this object.");
                }
            }
        }

        return fullMiniCoopers;
    }
    
        /// <summary>
    /// Retrieves detailed information of an electric Mini Cooper by the given ID from the database.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the unique identifier of the electric Mini Cooper to be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing an instance of <see cref="MiniCooper.FullMiniCooper"/>
    /// with detailed information about the specified electric Mini Cooper.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database and executes a query to fetch detailed information for
    /// the electric Mini Cooper with the specified ID. It handles exceptions by logging error messages to the console.
    /// The retrieved data is used to populate an instance of <see cref="MiniCooper.FullMiniCooper"/>.
    /// Note: The SQL query concatenates the car ID directly into the command string, which could lead to SQL Injection
    /// if the input is not correctly validated in a real-world scenario. It is advisable to use parameterized queries instead.
    /// </remarks>
    public async Task<MiniCooper.FullMiniCooper> GetEvByIdAsync(int id)
    {
        MiniCooper.EvMiniCooper evCooper = new();

        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var modelName = "(a_car).electric_car.base_cooper.model_name";
        var generation = "(a_car).electric_car.base_cooper.generation";
        var modelType = "(a_car).electric_car.base_cooper.model_type";
        var color = "(a_car).electric_car.base_cooper.color";
        var price = "(a_car).electric_car.base_cooper.price";
        var kmDriven = "(a_car).electric_car.base_cooper.km_driven";
        var maxRange = "(a_car).electric_car.base_cooper.max_range";
        var weight = "(a_car).electric_car.base_cooper.weight";
        var fuelType = "(a_car).electric_car.base_cooper.fuel_type";
        var gearType = "(a_car).electric_car.base_cooper.geartype";
        var yearlyTax = "(a_car).electric_car.base_cooper.yearly_tax";
        var chargeCapacity = "(a_car).electric_car.charge_capacity";
        var kmPrKwh = "(a_car).electric_car.km_pr_kwh";

        string query =
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {chargeCapacity}, {kmPrKwh} FROM cars WHERE id = {id};";
        // Console.WriteLine("Query: " + query);
        try
        {
            await using var cmd = new NpgsqlCommand(query, conn);

            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                evCooper.ModelName = reader.GetString(0);
                evCooper.Generation = reader.GetInt32(1);
                evCooper.ModelType = reader.GetString(2);
                evCooper.Color = reader.GetString(3);
                evCooper.Price = reader.GetInt32(4);
                evCooper.Mileage = reader.GetInt32(5);
                evCooper.MaxRange = reader.GetInt32(6);
                evCooper.Weight = reader.GetInt32(7);
                evCooper.FuelType = reader.GetString(8);
                evCooper.GearType = reader.GetString(9);
                evCooper.YearlyTax = reader.GetDecimal(10);
                evCooper.ChargeCapacity = reader.GetInt32(11);
                evCooper.KmPrKwh = reader.GetFloat(12);
                evCooper.Base64Images = await GetImagesByIdAndTypeAsync(id, "electric_car");
            }
            else
            {
                Console.WriteLine("No rows returned.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting ev by id: " + e.Message);
        }

        MiniCooper.FullMiniCooper fullCooper = new();
        fullCooper.SetMiniCooper(evCooper);
        return fullCooper;
    }

    /// <summary>
    /// Asynchronously retrieves detailed information for a fossil Mini Cooper car entry from the database based on the provided ID.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the unique identifier of the fossil Mini Cooper to be retrieved from the database.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="MiniCooper.FullMiniCooper"/> instance.
    /// This instance includes detailed information about the specified fossil Mini Cooper.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database and executes a query to retrieve detailed information for the fossil Mini Cooper
    /// identified by the given ID. The query retrieves various attributes such as model name, generation, color, and other specifics related
    /// to the fossil car's specifications. In case of an exception during the query execution, an error message is logged to the console.
    /// Note: Ensure that the ID parameter is validated to prevent SQL Injection vulnerabilities as the query is constructed using string concatenation.
    /// </remarks>
    public async Task<MiniCooper.FullMiniCooper> GetFossilByIdAsync(int id)
    {
        MiniCooper.FossilMiniCooper fossilCooper = new();

        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var modelName = "(a_car).fossile_car.base_cooper.model_name";
        var generation = "(a_car).fossile_car.base_cooper.generation";
        var modelType = "(a_car).fossile_car.base_cooper.model_type";
        var color = "(a_car).fossile_car.base_cooper.color";
        var price = "(a_car).fossile_car.base_cooper.price";
        var kmDriven = "(a_car).fossile_car.base_cooper.km_driven";
        var maxRange = "(a_car).fossile_car.base_cooper.max_range";
        var weight = "(a_car).fossile_car.base_cooper.weight";
        var fuelType = "(a_car).fossile_car.base_cooper.fuel_type";
        var gearType = "(a_car).fossile_car.base_cooper.geartype";
        var yearlyTax = "(a_car).fossile_car.base_cooper.yearly_tax";
        var tankCapacity = "(a_car).fossile_car.tank_capacity";
        var kmPrLiter = "(a_car).fossile_car.km_pr_liter";
        var gears = "(a_car).fossile_car.gears";

        string query =
            $"SELECT {modelName}, {generation}, {modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {tankCapacity}, {kmPrLiter}, {gears} FROM cars WHERE id = {id};";
        // Console.WriteLine("Query: " + query);
        try
        {
            await using var cmd = new NpgsqlCommand(query, conn);

            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                fossilCooper.ModelName = reader.GetString(0);
                fossilCooper.Generation = reader.GetInt32(1);
                fossilCooper.ModelType = reader.GetString(2);
                fossilCooper.Color = reader.GetString(3);
                fossilCooper.Price = reader.GetInt32(4);
                fossilCooper.Mileage = reader.GetInt32(5);
                fossilCooper.MaxRange = reader.GetInt32(6);
                fossilCooper.Weight = reader.GetInt32(7);
                fossilCooper.FuelType = reader.GetString(8);
                fossilCooper.GearType = reader.GetString(9);
                fossilCooper.YearlyTax = reader.GetDecimal(10);
                fossilCooper.TankCapacity = reader.GetInt32(11);
                fossilCooper.KmPrLiter = reader.GetFloat(12);
                fossilCooper.Gears = reader.GetInt32(13);
                fossilCooper.Base64Images = await GetImagesByIdAndTypeAsync(id, "fossile_car");
            }
            else
            {
                Console.WriteLine("No rows returned.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting fossil by id: " + e.Message);
        }

        MiniCooper.FullMiniCooper fullCooper = new();
        fullCooper.SetMiniCooper(fossilCooper);
        return fullCooper;
    }

    /// <summary>
    /// Retrieves a hybrid Mini Cooper from the database based on the provided ID.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the unique identifier of the hybrid Mini Cooper to be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="MiniCooper.FullMiniCooper"/> object
    /// that includes the details of the hybrid Mini Cooper.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database, formulates a SQL query to fetch the hybrid car details, and constructs a
    /// <see cref="MiniCooper.FullMiniCooper"/> object from the retrieved data. If an exception occurs during database operations, it is logged.
    /// </remarks>
    public async Task<MiniCooper.FullMiniCooper> GetHybridByIdAsync(int id)
    {
        MiniCooper.HybridMiniCooper hybridCooper = new();

        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var modelName = "(a_car).hybrid_car.base_cooper.model_name";
        var generation = "(a_car).hybrid_car.base_cooper.generation";
        var modelType = "(a_car).hybrid_car.base_cooper.model_type";
        var color = "(a_car).hybrid_car.base_cooper.color";
        var price = "(a_car).hybrid_car.base_cooper.price";
        var kmDriven = "(a_car).hybrid_car.base_cooper.km_driven";
        var maxRange = "(a_car).hybrid_car.base_cooper.max_range";
        var weight = "(a_car).hybrid_car.base_cooper.weight";
        var fuelType = "(a_car).hybrid_car.base_cooper.fuel_type";
        var gearType = "(a_car).hybrid_car.base_cooper.geartype";
        var yearlyTax = "(a_car).hybrid_car.base_cooper.yearly_tax";
        var fuelType1 = "(a_car).hybrid_car.fuel_type1";
        var fuelType2 = "(a_car).hybrid_car.fuel_type2";
        var tankCapacity = "(a_car).hybrid_car.tank_capacity";
        var chargeCapacity = "(a_car).hybrid_car.charge_capacity";
        var kmPrLiter = "(a_car).hybrid_car.km_pr_liter";
        var kmPrKwh = "(a_car).hybrid_car.km_pr_kwh";
        var gears = "(a_car).hybrid_car.gears";

        string query =
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {fuelType1}, {fuelType2}, {tankCapacity}, {chargeCapacity}, {kmPrLiter}, {kmPrKwh}, {gears} FROM cars WHERE id = {id};";
        // Console.WriteLine("Query: " + query);
        try
        {
            await using var cmd = new NpgsqlCommand(query, conn);

            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                Console.WriteLine("Getting stuff:");
                hybridCooper.ModelName = reader.GetString(0);
                hybridCooper.Generation = reader.GetInt32(1);
                hybridCooper.ModelType = reader.GetString(2);
                hybridCooper.Color = reader.GetString(3);
                hybridCooper.Price = reader.GetInt32(4);
                hybridCooper.Mileage = reader.GetInt32(5);
                hybridCooper.MaxRange = reader.GetInt32(6);
                hybridCooper.Weight = reader.GetInt32(7);
                hybridCooper.FuelType = reader.GetString(8);
                hybridCooper.GearType = reader.GetString(9);
                hybridCooper.YearlyTax = reader.GetDecimal(10);
                hybridCooper.FuelType1 = reader.GetString(11);
                hybridCooper.FuelType2 = reader.GetString(12);
                hybridCooper.TankCapacity = reader.GetInt32(13);
                hybridCooper.ChargeCapacity = reader.GetInt32(14);
                hybridCooper.KmPrLiter = reader.GetFloat(15);
                hybridCooper.KmPrKwh = reader.GetFloat(16);
                hybridCooper.Gears = reader.GetInt32(17);
                hybridCooper.Base64Images = await GetImagesByIdAndTypeAsync(id, "hybrid_car");
            }
            else
            {
                Console.WriteLine("No rows returned.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting hybrid by id: " + e.Message);
            Console.WriteLine("Stacktrace: " + e.StackTrace);
        }

        MiniCooper.FullMiniCooper fullCooper = new();
        fullCooper.SetMiniCooper(hybridCooper);
        return fullCooper;
    }
    
    public async Task<List<MiniCooper.FullMiniCooper>> GetFullMiniCoopersByUserId(int userId)
    {
        List<MiniCooper.FullMiniCooper> fullMiniCoopers = new();

        await using var conn = GetConnection();

        string query =
            $"SELECT id, (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car FROM cars WHERE account_id = {userId}";
        await using var cmd = new NpgsqlCommand(query, conn);

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var currentId = reader.GetInt32(0);

                if (!reader.IsDBNull(1))
                {
                    Console.WriteLine("Ev added!");
                    var tempFullCooper = await GetEvByIdAsync(currentId);
                    tempFullCooper.SetIds(currentId, userId);
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else if (!reader.IsDBNull(2))
                {
                    Console.WriteLine("Fossil added!");
                    var tempFullCooper = await GetFossilByIdAsync(currentId);
                    tempFullCooper.SetIds(currentId, userId);
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else if (!reader.IsDBNull(3))
                {
                    Console.WriteLine("Hybrid added!");
                    var tempFullCooper = await GetHybridByIdAsync(currentId);
                    tempFullCooper.SetIds(currentId, userId);
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else
                {
                    Console.WriteLine("No car has been assigned to this object.");
                }
            }
        }

        return fullMiniCoopers;
    }
    
    public async Task<List<MiniCooper.FullMiniCooper>> GetFullMiniCooperById(int carId)
    {
        List<MiniCooper.FullMiniCooper> fullMiniCoopers = new();

        await using var conn = GetConnection();

        string query =
            $"SELECT (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car, account_id FROM cars WHERE id = {carId}";
        await using var cmd = new NpgsqlCommand(query, conn);

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var userId = reader.GetInt32(3);
                
                if (!reader.IsDBNull(0))
                {
                    Console.WriteLine("Ev added!");
                    var tempFullCooper = await GetEvByIdAsync(carId);
                    tempFullCooper.SetIds(carId, userId);
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else if (!reader.IsDBNull(1))
                {
                    Console.WriteLine("Fossil added!");
                    var tempFullCooper = await GetFossilByIdAsync(carId);
                    tempFullCooper.SetIds(carId, userId);
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else if (!reader.IsDBNull(2))
                {
                    Console.WriteLine("Hybrid added!");
                    var tempFullCooper = await GetHybridByIdAsync(carId);
                    tempFullCooper.SetIds(carId, userId);
                    fullMiniCoopers.Add(tempFullCooper);
                }
                else
                {
                    Console.WriteLine("No car has been assigned to this object.");
                }
            }
        }

        return fullMiniCoopers;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cooperType"></param>
    /// <remarks>
    /// The "cooperType" parameter HAS to be either "electric_car", "fossil_car" or "hybrid_car".
    /// </remarks>
    /// <returns></returns>
    private async Task<List<string>> GetImagesByIdAndTypeAsync(int id, string cooperType)
    {
        Console.WriteLine("Getting images by id...");
        List<string> base64Images = new();

        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            string query =
                $"SELECT images FROM cars, unnest((a_car).{cooperType}.base_cooper.base64_images) AS images WHERE id = {id}";

            await using var cmd = new NpgsqlCommand(query, conn);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    base64Images.Add(reader.GetString(0));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting images by id and type: " + e.Message);
        }

        return base64Images;
    }
    
    /// <summary>
    /// Asynchronously retrieves a user from the database using their unique identifier.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the user's unique identifier in the database.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing an instance of <see cref="UsersService.User"/> with the user's details if found.
    /// </returns>
    /// <remarks>
    /// This method constructs and executes a SQL query to select user details from the 'users' table by the specified ID.
    /// The query uses direct string interpolation, which may be vulnerable to SQL Injection attacks. It is advisable to use parameterized queries to enhance security.
    /// </remarks>
    public async Task<UsersService.User> GetUserByIdAsync(int id)
    {
        UsersService.User user = new();

        var conn = GetConnection();
        string query = "SELECT" +
                       "id," +
                       "(a_user).name," +
                       "(a_user).password," +
                       "(a_user).mobile," +
                       "(a_user).email," +
                       "(a_user).city," +
                       "(a_user).address" +
                       $"FROM users WHERE id = {id}";

        var cmd = new NpgsqlCommand(query, conn);

        var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            user.Id = reader.GetInt32(0);
            user.Name = reader.GetString(1);
            user.Password = reader.GetString(2);
            user.Mobile = reader.GetInt32(3);
            user.Email = reader.GetString(4);
            user.City = reader.GetString(5);
            user.Address = reader.GetString(6);
        }

        return user;
    }

    /// <summary>
    /// Asynchronously adds an electric vehicle (EV) Mini Cooper entry to the 'cars' table in the database for a specified user.
    /// </summary>
    /// <param name="evCooper">
    /// An instance of <see cref="MiniCooper.EvMiniCooper"/> containing details of the electric Mini Cooper to be added.
    /// </param>
    /// <param name="userId">
    /// An <see cref="int"/> representing the user's unique identifier to associate with the new car entry.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of adding the EV to the database.
    /// </returns>
    /// <remarks>
    /// This method constructs a SQL query to insert a new car record into the database, associating it with the provided user ID.
    /// The query uses concatenated string representations for Mini Cooper data, which could lead to SQL Injection vulnerabilities.
    /// It is recommended to use parameterized queries in production environments to improve security.
    /// </remarks>
    public async Task AddEvToDbAsync(MiniCooper.EvMiniCooper evCooper, int userId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var resolvedImagesString = await ResolveImagesAsync(evCooper.Base64Images);

        string query =
            "INSERT INTO cars (a_car, account_id)" +
            "VALUES (" +
            "ROW (" +
            "ROW (" +
            $"ROW ('{evCooper.ModelName}', '{evCooper.Generation}', '{evCooper.ModelType}', '{evCooper.Color}', {evCooper.Price}, {evCooper.Mileage}, {evCooper.MaxRange}, {evCooper.Weight}, '{evCooper.FuelType}', '{evCooper.GearType}', {evCooper.YearlyTax}, ARRAY [{resolvedImagesString}])::mini_cooper, " +
            $"{evCooper.ChargeCapacity}, {evCooper.KmPrKwh})::ev_mini_cooper, " +
            "NULL," +
            "NULL" +
            ")::car," +
            $"{userId});";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);
    }

    /// <summary>
    /// Asynchronously adds a fossil fuel Mini Cooper entry to the 'cars' table in the database for a specified user.
    /// </summary>
    /// <param name="fossilCooper">
    /// An instance of <see cref="MiniCooper.FossilMiniCooper"/> containing details of the fossil fuel Mini Cooper to be added.
    /// </param>
    /// <param name="userId">
    /// An <see cref="int"/> representing the user's unique identifier to associate with the new car entry.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of adding the fossil fuel Mini Cooper to the database.
    /// </returns>
    /// <remarks>
    /// This method constructs a SQL query to insert a new car record into the database, associating it with the provided user ID.
    /// The query uses concatenated string representations for Mini Cooper data, which could lead to SQL Injection vulnerabilities.
    /// It is recommended to use parameterized queries in production environments to enhance security.
    /// </remarks>
    public async Task AddFossilToDbAsync(MiniCooper.FossilMiniCooper fossilCooper, int userId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var resolvedImagesString = await ResolveImagesAsync(fossilCooper.Base64Images);

        string query =
            "INSERT INTO cars (a_car, account_id)" +
            "VALUES (" +
            "ROW (" +
            "NULL," +
            "ROW (" +
            $"ROW ('{fossilCooper.ModelName}', '{fossilCooper.Generation}', '{fossilCooper.ModelType}', '{fossilCooper.Color}', {fossilCooper.Price}, {fossilCooper.Mileage}, {fossilCooper.MaxRange}, {fossilCooper.Weight}, '{fossilCooper.FuelType}', '{fossilCooper.GearType}', {fossilCooper.YearlyTax}, ARRAY [{resolvedImagesString}])::mini_cooper, " +
            $"{fossilCooper.TankCapacity}, {fossilCooper.KmPrLiter}, {fossilCooper.Gears})::fossil_mini_cooper, " +
            "NULL" +
            ")::car," +
            $"{userId});";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);
    }

    /// <summary>
    /// Asynchronously adds a hybrid Mini Cooper entry to the 'cars' table in the database for a specified user.
    /// </summary>
    /// <param name="hybridCooper">
    /// An instance of <see cref="MiniCooper.HybridMiniCooper"/> containing details of the hybrid Mini Cooper to be added.
    /// </param>
    /// <param name="userId">
    /// An <see cref="int"/> representing the user's unique identifier to associate with the new car entry.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation of adding the hybrid Mini Cooper to the database.
    /// </returns>
    /// <remarks>
    /// This method constructs a SQL query to insert a new car record into the database, associating it with the provided user ID.
    /// The insertion utilizes a concatenated string representation of the Mini Cooper data, which could pose a risk for SQL Injection.
    /// It is advisable to implement parameterized queries in production environments to enhance security.
    /// </remarks>
    public async Task AddHybridToDbAsync(MiniCooper.HybridMiniCooper hybridCooper, int userId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var resolvedImagesString = await ResolveImagesAsync(hybridCooper.Base64Images);

        string query =
            "INSERT INTO cars (a_car, account_id)" +
            "VALUES (" +
            "ROW (" +
            "NULL," +
            "NULL," +
            "ROW (" +
            "ROW (" +
            $"'{hybridCooper.ModelName}'," +
            $"'{hybridCooper.Generation}'," +
            $"'{hybridCooper.ModelType}'," +
            $"'{hybridCooper.Color}'," +
            $"{hybridCooper.Price}," +
            $"{hybridCooper.Mileage}," +
            $"{hybridCooper.MaxRange}," +
            $"{hybridCooper.Weight}," +
            $"'{hybridCooper.FuelType}'," +
            $"'{hybridCooper.GearType}'," +
            $"{hybridCooper.YearlyTax}," +
            $"ARRAY [{resolvedImagesString}])::mini_cooper, " +
            $"'{hybridCooper.FuelType1}'," +
            $"'{hybridCooper.FuelType2}'," +
            $"{hybridCooper.TankCapacity}," +
            $"{hybridCooper.ChargeCapacity}," +
            $"{hybridCooper.KmPrLiter}," +
            $"{hybridCooper.KmPrKwh}," +
            $"{hybridCooper.Gears})::hybrid_mini_cooper " +
            ")::car," +
            $"{userId});";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);
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

    public async Task DELETEEVERYTHING(string tableName)
    {
        await using var conn = GetConnection();

        string query = "DELETE FROM cars;";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);

        await ResetTableIdsAsync(tableName);
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
    public async Task ResetTableIdsAsync(string tableName)
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
    /// <param name="command">
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
    private async Task<int> RunAsyncQuery(NpgsqlCommand command)
    {
        int result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
            Console.WriteLine("No records affected.");
        else
            Console.WriteLine("Records affected: " + result);

        return result;
    }

    private async Task<string> ResolveImagesAsync(List<string> base64Images)
    {
        string resolvedImages = "";

        foreach (var base64Image in base64Images)
        {
            resolvedImages += $"'{base64Image}',";
        }

        resolvedImages = resolvedImages.Substring(0, resolvedImages.Length - 1);

        return resolvedImages;
    }

    public async Task AddUserAsync(UsersService.User user)
    {
        if (await IsEmailTakenAsync(user.Email))
        {
            Console.WriteLine("Email is already in use.");
            return;
        }

        if (await IsMobileTakenAsync(user.Mobile))
        {
            Console.WriteLine("Mobile is already in use.");
            return;
        }

        var conn = GetConnection();

        string query =
            $"INSERT INTO users (a_user) VALUES (ROW('{user.Name}', '{user.Password}', {user.Mobile}, '{user.Email}', '{user.City}', '{user.Address}')::account)";

        var cmd = new NpgsqlCommand(query, conn);

        try
        {
            await RunAsyncQuery(cmd);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding user to database: " + ex.Message);
            Console.WriteLine("Stacktrace: " + ex.StackTrace);
            throw;
        }
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var conn = GetConnection();

        string query = $"SELECT (a_user).email FROM users WHERE (a_user).email = '{email}'";

        var cmd = new NpgsqlCommand(query, conn);

        try
        {
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting email: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<bool> IsMobileTakenAsync(int mobile)
    {
        var conn = GetConnection();

        string query = $"SELECT (a_user).mobile FROM users WHERE (a_user).mobile = {mobile}";

        var cmd = new NpgsqlCommand(query, conn);

        try
        {
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting email: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<UsersService.User> LogUserOn(string email, string password)
    {
        var user = new UsersService.User();

        var conn = GetConnection();

        string query =
            $"SELECT id, (a_user).name, (a_user).password, (a_user).mobile, (a_user).email, (a_user).city, (a_user).address FROM users WHERE (a_user).email = '{email}' AND (a_user).password = '{password}'";

        var cmd = new NpgsqlCommand(query, conn);

        try
        {
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                user.Id = reader.GetInt32(0);
                user.Name = reader.GetString(1);
                user.Password = reader.GetString(2);
                user.Mobile = reader.GetInt32(3);
                user.Email = reader.GetString(4);
                user.City = reader.GetString(5);
                user.Address = reader.GetString(6);
            }
            else
            {
                Console.WriteLine("No user with this email and password was found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error signing user in: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            throw;
        }

        return user;
    }

    
}