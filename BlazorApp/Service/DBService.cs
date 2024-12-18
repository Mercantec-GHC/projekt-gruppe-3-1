namespace BlazorApp.Service;

//"Host=ep-restless-snow-a2okt6hh.eu-central-1.aws.neon.tech;Username=bilbasen_owner;Password=RIA6nJt1Xwgo;Database=bilbasen;sslmode=require;"
using Npgsql;

public class DBService
{
    // public static DBService Instance;

    // Essentially our DefaultConnection in our appsettings.json.
    private readonly string _connectionString;

    public DBService(string connectionString)
    {
        _connectionString = connectionString;
        // Instance = this;
    }

    // We then made a NpgsqlConnection, open it and then returns it.
    private NpgsqlConnection GetConnection()
    {
        NpgsqlConnection connection = new(_connectionString);
        connection.Open();
        return connection;
    }

    /*public async Task<List<UsersService.User>> GetAllUsersAsync()
    {
        Console.WriteLine("Getting all users...");
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
    }*/

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
        Console.WriteLine("Getting all mini coopers...");
        List<MiniCooper.FullMiniCooper> fullMiniCoopers = new();

        await using var conn = GetConnection();

        string query =
            "SELECT id, (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car, account_id FROM cars;";
        await using var cmd = new NpgsqlCommand(query, conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        try
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
        catch (Exception ex)
        {
            Console.WriteLine("Error getting all mini coopers: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            throw;
        }

        return fullMiniCoopers;
    }

    /// <summary>
    /// Retrieves a full Mini Cooper object based on the user's ID and the specified car name.
    /// </summary>
    /// <param name="userId">The unique identifier of the user who owns the car.</param>
    /// <param name="carName">The specific name of the car to search for within the user's collection of Mini Coopers.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="MiniCooper.FullMiniCooper"/> instance.
    /// The instance will be populated based on whether the car type is electric, fossil, or hybrid.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database and executes a query to retrieve records for cars matching the given user ID.
    /// Depending on the type of car (electric, fossil, or hybrid), respective helper methods such as <see cref="GetEvByCarIdAndCarName"/>,
    /// <see cref="GetFossilByCarIdAndCarName"/>, or <see cref="GetHybridByIdAndCarName"/> are invoked to fetch detailed car data.
    /// Console messages are logged during the process to indicate the status of the retrieval operation or if no car type is found.
    /// </remarks>
    public async Task<MiniCooper.FullMiniCooper> GetFullMiniCooperByUserIdAndName(int userId, string carName)
    {
        Console.WriteLine("Getting full mini cooper by user id and name...");
        MiniCooper.FullMiniCooper fullMiniCooper = new();

        await using var conn = GetConnection();

        string query =
            $"SELECT id, (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car FROM cars WHERE account_id = {userId};";
        await using var cmd = new NpgsqlCommand(query, conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        try
        {
            while (await reader.ReadAsync())
            {
                var currentCarId = reader.GetInt32(0);

                if (!reader.IsDBNull(1))
                {
                    var tempFullCooper = await GetEvByCarIdAndCarName(currentCarId, carName);
                    if (tempFullCooper.GetEvCooper() == null)
                        Console.WriteLine("Ev not found...");
                    else
                    {
                        fullMiniCooper.SetMiniCooper(tempFullCooper.GetEvCooper());
                        fullMiniCooper.SetIds(currentCarId, userId);
                        return fullMiniCooper;
                    }
                }
                else if (!reader.IsDBNull(2))
                {
                    var tempFullCooper = await GetFossilByCarIdAndCarName(currentCarId, carName);
                    if (tempFullCooper.GetFossilCooper() == null)
                        Console.WriteLine("Fossil not found...");
                    else
                    {
                        fullMiniCooper.SetMiniCooper(tempFullCooper.GetFossilCooper());
                        fullMiniCooper.SetIds(currentCarId, userId);
                        return fullMiniCooper;
                    }
                }
                else if (!reader.IsDBNull(3))
                {
                    var tempFullCooper = await GetHybridByIdAndCarName(currentCarId, carName);
                    if (tempFullCooper.GetHybridCooper() == null)
                        Console.WriteLine("Hybrid not found...");
                    else
                    {
                        fullMiniCooper.SetMiniCooper(tempFullCooper.GetHybridCooper());
                        fullMiniCooper.SetIds(currentCarId, userId);
                        return fullMiniCooper;
                    }
                }
                else
                    Console.WriteLine("No car has been assigned to this object.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting full mini coopers by user id and name: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            throw;
        }

        fullMiniCooper = null;
        return fullMiniCooper;
    }

    /// <summary>
    /// Retrieves data for an electric Mini Cooper based on its unique identifier and model name from the database.
    /// </summary>
    /// <param name="carId">The unique identifier of the car.</param>
    /// <param name="carName">The model name of the car.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="MiniCooper.FullMiniCooper"/> instance.
    /// The instance encompasses detailed information about the electric Mini Cooper.
    /// </returns>
    /// <remarks>
    /// This method constructs a database query using the specified car ID and model name to fetch the electric Mini Cooper's details.
    /// It reads the query results, populates the instance of <see cref="MiniCooper.FullMiniCooper"/>, and handles errors by logging messages to the console.
    /// </remarks>
    private async Task<MiniCooper.FullMiniCooper> GetEvByCarIdAndCarName(int carId, string carName)
    {
        Console.WriteLine("Getting ev by name...");
        MiniCooper.EvMiniCooper evCooper = new();
        MiniCooper.FullMiniCooper fullCooper = new();

        var conn = GetConnection();

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
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {chargeCapacity}, {kmPrKwh} FROM cars WHERE id = {carId} AND {modelName} = '{carName}';";
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
                evCooper.Base64Images = await GetImagesByIdAndTypeAsync(carId, "electric_car");

                fullCooper.SetMiniCooper(evCooper);
            }
            else
                Console.WriteLine("No rows returned.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting ev by id and name: " + e.Message);
            Console.WriteLine("StackTrace: " + e.StackTrace);
        }

        return fullCooper;
    }

    /// <summary>
    /// Retrieves a full Mini Cooper fossil car entry from the database based on the provided car ID and name.
    /// </summary>
    /// <param name="carId">The unique identifier of the fossil car in the database.</param>
    /// <param name="carName">The name or model name of the fossil car to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="MiniCooper.FullMiniCooper"/> object.
    /// The object contains detailed information about the retrieved fossil car.
    /// </returns>
    /// <remarks>
    /// The method establishes a database connection and executes a query to retrieve information about a specific fossil Mini Cooper car.
    /// If the car is found, it extracts details such as model name, generation, type, fuel type, gear type, and various specifications.
    /// If no car matches the criteria or an exception occurs, appropriate console messages are logged.
    /// </remarks>
    private async Task<MiniCooper.FullMiniCooper> GetFossilByCarIdAndCarName(int carId, string carName)
    {
        Console.WriteLine("Getting fossil by name...");
        MiniCooper.FossilMiniCooper fossilCooper = new();
        MiniCooper.FullMiniCooper fullCooper = new();

        var conn = GetConnection();

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
        var chargeCapacity = "(a_car).fossile_car.tank_capacity";
        var kmPrLiter = "(a_car).fossile_car.km_pr_liter";

        string query =
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {chargeCapacity}, {kmPrLiter} FROM cars WHERE id = {carId} AND {modelName} = '{carName}';";
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
                fossilCooper.Base64Images = await GetImagesByIdAndTypeAsync(carId, "fossile_car");

                fullCooper.SetMiniCooper(fossilCooper);
            }
            else
                Console.WriteLine("No rows returned.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting fossil by id and name: " + e.Message);
            Console.WriteLine("StackTrace: " + e.StackTrace);
        }

        return fullCooper;
    }

    /// <summary>
    /// Retrieves a hybrid Mini Cooper car entry from the database based on the provided car ID and car name.
    /// </summary>
    /// <param name="carId">
    /// The unique identifier of the car in the database.
    /// </param>
    /// <param name="carName">
    /// The name of the car model to be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="MiniCooper.FullMiniCooper"/> instance.
    /// The result contains detailed information about the hybrid Mini Cooper if found; otherwise, returns an empty instance.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database, executes an SQL query to retrieve detailed information about
    /// the specified hybrid Mini Cooper, including base specifications and hybrid-specific attributes, such as fuel types,
    /// tank capacity, and charge capacity.
    /// If the car is not found in the database, logs are written to the console indicating the absence of results or any
    /// encountered errors.
    /// </remarks>
    private async Task<MiniCooper.FullMiniCooper> GetHybridByIdAndCarName(int carId, string carName)
    {
        Console.WriteLine("Getting hybrid by name...");
        MiniCooper.HybridMiniCooper hybridCooper = new();
        MiniCooper.FullMiniCooper fullCooper = new();

        var conn = GetConnection();

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
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {fuelType1}, {fuelType2}, {tankCapacity}, {chargeCapacity}, {kmPrLiter}, {kmPrKwh}, {gears} FROM cars WHERE id = {carId} AND {modelName} = '{carName}';";
        // Console.WriteLine("Query: " + query);
        try
        {
            await using var cmd = new NpgsqlCommand(query, conn);

            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
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
                hybridCooper.Base64Images = await GetImagesByIdAndTypeAsync(carId, "hybrid_car");

                fullCooper.SetMiniCooper(hybridCooper);
            }
            else
                Console.WriteLine("No rows returned.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting hybrid by id and name: " + e.Message);
            Console.WriteLine("StackTrace: " + e.StackTrace);
        }

        return fullCooper;
    }

    /// <summary>
    /// Retrieves detailed information of an electric Mini Cooper by the given ID from the database.
    /// </summary>
    /// <param name="carId">
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
    public async Task<MiniCooper.FullMiniCooper> GetEvByIdAsync(int carId)
    {
        Console.WriteLine("Getting ev by id...");
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
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {chargeCapacity}, {kmPrKwh} FROM cars WHERE id = {carId};";
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
                evCooper.Base64Images = await GetImagesByIdAndTypeAsync(carId, "electric_car");
            }
            else
            {
                Console.WriteLine("No rows returned.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting ev by id: " + e.Message);
            Console.WriteLine("StackTrace: " + e.StackTrace);
        }

        MiniCooper.FullMiniCooper fullCooper = new();
        fullCooper.SetMiniCooper(evCooper);
        return fullCooper;
    }

    /// <summary>
    /// Asynchronously retrieves detailed information for a fossil Mini Cooper car entry from the database based on the provided ID.
    /// </summary>
    /// <param name="carId">
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
    private async Task<MiniCooper.FullMiniCooper> GetFossilByIdAsync(int carId)
    {
        Console.WriteLine("Getting fossil by id...");
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
            $"SELECT {modelName}, {generation}, {modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {tankCapacity}, {kmPrLiter}, {gears} FROM cars WHERE id = {carId};";
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
                fossilCooper.Base64Images = await GetImagesByIdAndTypeAsync(carId, "fossile_car");
            }
            else
            {
                Console.WriteLine("No rows returned.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting fossil by id: " + e.Message);
            Console.WriteLine("StackTrace: " + e.StackTrace);
        }

        MiniCooper.FullMiniCooper fullCooper = new();
        fullCooper.SetMiniCooper(fossilCooper);
        return fullCooper;
    }

    /// <summary>
    /// Retrieves a hybrid Mini Cooper from the database based on the provided ID.
    /// </summary>
    /// <param name="carId">
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
    private async Task<MiniCooper.FullMiniCooper> GetHybridByIdAsync(int carId)
    {
        Console.WriteLine("Getting hybrid by id...");
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
            $"SELECT {modelName}, {generation},{modelType}, {color}, {price}, {kmDriven}, {maxRange}, {weight}, {fuelType}, {gearType}, {yearlyTax}, {fuelType1}, {fuelType2}, {tankCapacity}, {chargeCapacity}, {kmPrLiter}, {kmPrKwh}, {gears} FROM cars WHERE id = {carId};";
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
                hybridCooper.Base64Images = await GetImagesByIdAndTypeAsync(carId, "hybrid_car");
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

    /// <summary>
    /// Retrieves a list of full Mini Cooper entries associated with a specific user from the database.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user for whom the Mini Cooper entries will be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a list of <see cref="MiniCooper.FullMiniCooper"/> instances.
    /// Each instance represents a Mini Cooper entry belonging to the specified user.
    /// </returns>
    /// <remarks>
    /// This method connects to the database and retrieves car data linked to the given user ID. It processes each record to determine the type of car
    /// (electric, fossil, or hybrid). For each car type, it invokes corresponding methods such as <see cref="GetEvByIdAsync"/>,
    /// <see cref="GetFossilByIdAsync"/>, or <see cref="GetHybridByIdAsync"/> to get detailed information about the car.
    /// The method logs messages indicating the types of cars being added and includes functionality to set user and car IDs for the resulting entries.
    /// </remarks>
    public async Task<List<MiniCooper.FullMiniCooper>> GetFullMiniCoopersByUserId(int userId)
    {
        Console.WriteLine("Getting full mini coopers by user id...");
        List<MiniCooper.FullMiniCooper> fullMiniCoopers = new();

        await using var conn = GetConnection();

        string query =
            $"SELECT id, (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car FROM cars WHERE account_id = {userId}";
        await using var cmd = new NpgsqlCommand(query, conn);

        try
        {
            await using var reader = await cmd.ExecuteReaderAsync();
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
        catch (Exception ex)
        {
            Console.WriteLine("Error getting full mini coopers by user id: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            throw;
        }


        return fullMiniCoopers;
    }

    /// <summary>
    /// Retrieves a full Mini Cooper instance identified by its unique car ID from the database.
    /// </summary>
    /// <param name="carId">
    /// The unique identifier of the Mini Cooper car to be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="MiniCooper.FullMiniCooper"/> instance.
    /// The instance provides detailed information about a full Mini Cooper.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database to fetch the specified car's details.
    /// Based on the type of the car (electric, fossil, or hybrid), respective methods such as <see cref="GetEvByIdAsync"/>,
    /// <see cref="GetFossilByIdAsync"/>, or <see cref="GetHybridByIdAsync"/> are invoked to retrieve additional details.
    /// The retrieved details include type-specific properties and relationships such as the associated user account.
    /// If successful, the method returns a fully populated Mini Cooper object for further use.
    /// Appropriate console messages are logged during the retrieval process.
    /// </remarks>
    public async Task<MiniCooper.FullMiniCooper> GetFullMiniCooperById(int carId)
    {
        Console.WriteLine("CardID:" + carId);
        Console.WriteLine("Getting full mini cooper by id...");
        MiniCooper.FullMiniCooper fullMiniCooper = new();

        await using var conn = GetConnection();

        string query =
            $"SELECT (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car, account_id FROM cars WHERE id = {carId}";
        await using var cmd = new NpgsqlCommand(query, conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        try
        {
            if (await reader.ReadAsync())
            {
                var userId = reader.GetInt32(3);
                Console.WriteLine("UserId in get full mini coopers by id: " + userId);

                if (!reader.IsDBNull(0))
                {
                    Console.WriteLine("Ev added!");
                    var tempFullCooper = await GetEvByIdAsync(carId);
                    fullMiniCooper.SetMiniCooper(tempFullCooper.GetEvCooper());
                    fullMiniCooper.SetIds(carId, userId);
                }
                else if (!reader.IsDBNull(1))
                {
                    Console.WriteLine("Fossil added!");
                    var tempFullCooper = await GetFossilByIdAsync(carId);
                    fullMiniCooper.SetMiniCooper(tempFullCooper.GetFossilCooper());
                    fullMiniCooper.SetIds(carId, userId);
                }
                else if (!reader.IsDBNull(2))
                {
                    Console.WriteLine("Hybrid added!");
                    var tempFullCooper = await GetHybridByIdAsync(carId);
                    fullMiniCooper.SetMiniCooper(tempFullCooper.GetHybridCooper());
                    fullMiniCooper.SetIds(carId, userId);
                }
                else
                {
                    Console.WriteLine("No car has been assigned to this object.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting Full Mini Cooper by Id: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.Message);
            throw;
        }

        return fullMiniCooper;
    }

    /// <summary>
    /// Retrieves a list of base64-encoded image strings for a specified Mini Cooper identified by its ID and type.
    /// </summary>
    /// <param name="carId">
    /// The unique identifier of the car in the database whose images are to be retrieved.
    /// </param>
    /// <param name="cooperType">
    /// The type of the Mini Cooper, such as "electric_car" or "fossile_car", to specify which set of images to retrieve.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of base64-encoded strings,
    /// each representing an image associated with the specified Mini Cooper.
    /// </returns>
    /// <remarks>
    /// The method queries the database using the provided car ID and type to retrieve images stored in a nested array.
    /// In case of any errors during the database operation, an error message will be logged to the console.
    /// </remarks>
    private async Task<List<string>> GetImagesByIdAndTypeAsync(int carId, string cooperType)
    {
        Console.WriteLine("Getting images by id...");
        List<string> base64Images = new();

        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            string query =
                $"SELECT images FROM cars, unnest((a_car).{cooperType}.base_cooper.base64_images) AS images WHERE id = {carId}";

            await using var cmd = new NpgsqlCommand(query, conn);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                base64Images.Add(reader.GetString(0));
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
    /// <param name="userId">
    /// An <see cref="int"/> representing the user's unique identifier in the database.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing an instance of <see cref="UsersService.User"/> with the user's details if found.
    /// </returns>
    /// <remarks>
    /// This method constructs and executes a SQL query to select user details from the 'users' table by the specified ID.
    /// The query uses direct string interpolation, which may be vulnerable to SQL Injection attacks. It is advisable to use parameterized queries to enhance security.
    /// </remarks>
    public async Task<UsersService.User> GetUserByIdAsync(int userId)
    {
        Console.WriteLine("Getting user by id...");
        UsersService.User user = new();

        var conn = GetConnection();
        string query = "SELECT " +
                       "id, " +
                       "(a_user).name, " +
                       "(a_user).password, " +
                       "(a_user).mobile, " +
                       "(a_user).email," +
                       "(a_user).city," +
                       "(a_user).address " +
                       $"FROM users WHERE id = {userId}";

        // string query = $"SELECT * FROM users WHERE id = {id}"; This won't work because we access a UDT.

        var cmd = new NpgsqlCommand(query, conn);

        try
        {
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
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting user by id: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            throw;
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
        Console.WriteLine("Adding ev to db...");
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
        Console.WriteLine("Adding fossil to db...");
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
        Console.WriteLine("Adding hybrid to db...");
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
        Console.WriteLine("Deleting car by id...");
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string query = $"DELETE FROM cars WHERE id = {carId}";
        await using var cmd = new NpgsqlCommand(query, conn);

        await RunAsyncQuery(cmd);

        await ResetTableIdsAsync("cars");
    }

    /// <summary>
    /// Resets the IDs of a specified table in a PostgresSQL database to start from 1, while preserving the order and 
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
        Console.WriteLine("Resetting table IDs...");
        // Since we are using "using", we don't have to add a close statement at the end, because "using" does that.
        await using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        string query = $"CREATE TEMP TABLE temp_{tableName} AS " +
                       "SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id " +
                       $"FROM {tableName};" + // Creates temperary table.
                       $"UPDATE {tableName} " +
                       $"SET id = temp_{tableName}.new_id " +
                       $"FROM temp_{tableName} " +
                       $"WHERE {tableName}.id = temp_{tableName}.id;" + // Updates the original table.
                       $"SELECT setval('{tableName}_id_seq', (SELECT MAX(id) FROM {tableName}));" + // Resets the table ID to be the next ID in order.
                       $"DROP TABLE temp_{tableName};"; // Finally, drops the temporary table.
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
        Console.WriteLine("Running async query...");
        int result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
            Console.WriteLine("No records affected.");
        else
            Console.WriteLine("Records affected: " + result);

        return result;
    }

    private async Task<string> ResolveImagesAsync(List<string> base64Images)
    {
        Console.WriteLine("Resolving images...");
        string resolvedImages = "";

        foreach (var base64Image in base64Images)
        {
            resolvedImages += $"'{base64Image}',";
        }

        resolvedImages = resolvedImages.Substring(0, resolvedImages.Length - 1);

        return resolvedImages;
    }

    /// <summary>
    /// Adds a new user to the database after validating the uniqueness of their email and mobile number.
    /// </summary>
    /// <param name="user">
    /// The user instance containing details such as name, password, mobile number, email, city, and address.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of adding a user to the database.
    /// </returns>
    /// <remarks>
    /// This method validates the user by checking whether the provided email and mobile number are already in use.
    /// If either is taken, the operation stops and appropriate messages are logged to the console.
    /// If validation is successful, the method constructs an SQL query to insert the user details into the database
    /// and executes the query asynchronously. In case of an error during database operation, it logs the error
    /// message and stack trace, and rethrows the exception.
    /// </remarks>
    public async Task AddUserAsync(UsersService.User user)
    {
        Console.WriteLine("Adding user...");
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

    /// <summary>
    /// Checks whether the given email address is already associated with an existing user in the database.
    /// </summary>
    /// <param name="email">
    /// The email address to be verified for existence in the database.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value:
    /// true if the email is already taken, or false if the email is available.
    /// </returns>
    /// <remarks>
    /// This method connects to the database and executes a query to check if the provided email exists in the users table.
    /// If an email match is found, it returns true; otherwise, it returns false. Any exceptions that occur during the operation
    /// are logged and rethrown to the caller for further handling.
    /// </remarks>
    private async Task<bool> IsEmailTakenAsync(string email)
    {
        Console.WriteLine("Checking if email is taken...");
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

    /// <summary>
    /// Checks if the specified mobile number is already assigned to an existing user in the database.
    /// </summary>
    /// <param name="mobile">
    /// The mobile number to check for existence in the database.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a boolean value indicating whether the mobile number is already taken.
    /// Returns <c>true</c> if the mobile number exists in the database; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method establishes a connection to the database and runs a SQL query to search for the specified mobile number in the users table.
    /// If the mobile number is found, the method returns <c>true</c>, indicating that the number is already taken.
    /// If any issue occurs while executing the query, an exception is logged and rethrown.
    /// </remarks>
    private async Task<bool> IsMobileTakenAsync(int mobile)
    {
        Console.WriteLine("Checking if mobile is taken...");
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

    /// <summary>
    /// Verifies a user's credentials and retrieves user information from the database if the credentials are valid.
    /// </summary>
    /// <param name="email">The user's email address provided for authentication.</param>
    /// <param name="password">The user's password provided for authentication.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing an instance of <see cref="UsersService.User"/>.
    /// If the user's credentials are valid, the returned instance contains the user's details; otherwise, it returns a default instance.
    /// </returns>
    /// <remarks>
    /// This method connects to the PostgresSQL database and executes a query to find a user matching the provided email and password.
    /// If a matching user is found, their details are populated into a new <see cref="UsersService.User"/> instance.
    /// If no matching user is found or an error occurs during the database operation, the reason is logged to the console.
    /// </remarks>
    public async Task<UsersService.User> LogUserOn(string email, string password)
    {
        Console.WriteLine("Logging user on...");
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

    /// <summary>
    /// Updates an existing Mini Cooper record in the database identified by its unique car ID.
    /// </summary>
    /// <param name="fullCooper">
    /// An instance of <see cref="MiniCooper.FullMiniCooper"/> containing detailed information about the Mini Cooper to be updated.
    /// </param>
    /// <param name="carId">
    /// The unique identifier of the Mini Cooper record in the database that needs to be updated.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of updating the Mini Cooper record in the database.
    /// </returns>
    /// <remarks>
    /// This method constructs and executes a database update query based on the type of Mini Cooper.
    /// It processes the `fullCooper` object to extract relevant data and utilizes the car's type
    /// (e.g., electric, fossil, or hybrid) to form a specialized query. Specialized attributes and
    /// configurations for the given car type are handled within respective conditional blocks.
    /// Additionally, the method connects to the database, logs progress messages to the console,
    /// and processes image references asynchronously via <see cref="ResolveImagesAsync"/> where necessary. Note that this
    /// method updates EVERY element in a <see cref="MiniCooper.FullMiniCooper"/>, on the database.
    /// </remarks>
    public async Task UpdateCooper(MiniCooper.FullMiniCooper fullCooper, int carId)
    {
        Console.WriteLine("Updating cooper...");
        var conn = GetConnection();

        string query;
        string baseCooperQuery = "ROW(" +
                                 "ROW(" +
                                 $"'{fullCooper.GetModelName()}'," +
                                 $"{fullCooper.GetGeneration()}," +
                                 $"'{fullCooper.GetModelType()}'," +
                                 $"'{fullCooper.GetColor()}'," +
                                 $"{fullCooper.GetPrice()}," +
                                 $"{fullCooper.GetMileage()}," +
                                 $"{fullCooper.GetMaxRange()}," +
                                 $"{fullCooper.GetWeight()}," +
                                 $"'{fullCooper.GetFuelType()}'," +
                                 $"'{fullCooper.GetGearType()}'," +
                                 $"{fullCooper.GetYearlyTax()}," +
                                 $"ARRAY [{await ResolveImagesAsync(fullCooper.GetImages())}])::mini_cooper,";

        string carType = fullCooper.GetCooperTypeInUse();
        if (carType == "ev")
        {
            Console.WriteLine("Updating ev...");
            query = "UPDATE cars " +
                    "SET a_car.electric_car = " +
                    baseCooperQuery +
                    $"{fullCooper.GetEvCooper().GetChargeCapacity()}," +
                    $"{fullCooper.GetEvCooper().GetKmPrKwh()})::ev_mini_cooper " +
                    $"WHERE id = {carId};";
            Console.WriteLine("Query: " + query);
        }
        else if (carType == "fossil")
        {
            Console.WriteLine("Updating fossil...");
            query = "UPDATE cars " +
                    "SET a_car.fossile_car = " +
                    baseCooperQuery +
                    $"{fullCooper.GetFossilCooper().GetTankCapacity()}," +
                    $"{fullCooper.GetFossilCooper().GetKmPrLiter()}," +
                    $"{fullCooper.GetFossilCooper().GetGears()})::fossil_mini_cooper " +
                    $"WHERE id = {carId};";
            Console.WriteLine("Query: " + query);
        }
        else if (carType == "hybrid")
        {
            Console.WriteLine("Updating hybrid...");
            query = "UPDATE cars " +
                    "SET a_car.hybrid_car = " +
                    baseCooperQuery +
                    $"'{fullCooper.GetHybridCooper().GetFuelType1()}'," +
                    $"'{fullCooper.GetHybridCooper().GetFuelType2()}'," +
                    $"{fullCooper.GetHybridCooper().GetTankCapacity()}," +
                    $"{fullCooper.GetHybridCooper().GetChargeCapacity()}," +
                    $"{fullCooper.GetHybridCooper().GetKmPrLiter()}," +
                    $"{fullCooper.GetHybridCooper().GetKmPrKwh()}," +
                    $"{fullCooper.GetHybridCooper().GetGears()})::hybrid_mini_cooper " +
                    $"WHERE id = {carId};";
            Console.WriteLine("Query: " + query);
        }
        else
        {
            Console.WriteLine("Unknown car type.");
            return;
        }

        var cmd = new NpgsqlCommand(query, conn);
        try
        {
            await RunAsyncQuery(cmd);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating cooper: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing user record in the database with the provided user details.
    /// </summary>
    /// <param name="user">
    /// An instance of <see cref="UsersService.User"/> containing updated user information such as name, password, email, mobile, city, and address.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of updating the user record in the database.
    /// </returns>
    /// <remarks>
    /// This method establishes a database connection and executes an update query using the provided user's information.
    /// The method modifies the corresponding user record identified by the user's unique ID. Note that this method technically
    /// updates every single element of a <see cref="UsersService.User"/>, so make sure that the other elements are not null/0.
    /// </remarks>
    public async Task UpdateUser(UsersService.User user)
    {
        Console.WriteLine("Updating user...");
        var conn = GetConnection();
        string query =
            $"UPDATE users SET a_user.name = '{user.Name}', a_user.password = '{user.Password}', a_user.email = '{user.Email}', a_user.mobile = {user.Mobile}, a_user.city = '{user.City}', a_user.address = '{user.Address}' WHERE id = {user.Id}";
        var cmd = new NpgsqlCommand(query, conn);
        await RunAsyncQuery(cmd);
    }
}