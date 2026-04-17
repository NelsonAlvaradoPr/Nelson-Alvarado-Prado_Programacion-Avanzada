using VehicleMvcApp.Models;
using MySql.Data.MySqlClient;
using VehicleMvcApp.Services.Interfaces;

namespace VehicleMvcApp.Services.Repositories
{
    /// <summary>
    /// MySQL database-based vehicle repository
    /// Reads and writes vehicle data from/to MySQL database
    /// </summary>
    public class DatabaseVehicleRepository : IVehicleRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseVehicleRepository> _logger;

        public DatabaseVehicleRepository(IConfiguration configuration, ILogger<DatabaseVehicleRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration["DataSource:ConnectionString"];
            
            _logger.LogInformation($"🗄️ Database Repository initialized. Connection: {_connectionString}");
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            var vehicles = new List<Vehicle>();
            try
            {
                _logger.LogInformation("📖 Reading all vehicles from database...");
                
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    const string query = "SELECT ID, color, year, make, type, created_at FROM PRQ_Cars ORDER BY year DESC";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vehicles.Add(new Vehicle
                                {
                                    Id = reader.GetInt32(0),
                                    Color = reader.GetString(1),
                                    Year = reader.GetInt32(2),
                                    Make = reader.GetString(3),
                                    Type = reader.GetString(4),
                                    CreatedAt = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
                
                _logger.LogInformation($"✓ Successfully read {vehicles.Count} vehicles from database");
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error reading from database: {ex.Message}");
                return vehicles;
            }
        }

        public async Task<List<Vehicle>> GetVehiclesByYearRangeAsync(int minYear, int maxYear)
        {
            var vehicles = new List<Vehicle>();
            try
            {
                _logger.LogInformation($"🔍 Filtering vehicles by year range: {minYear}-{maxYear} (Database)");
                
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    const string query = "SELECT ID, color, year, make, type, created_at FROM PRQ_Cars " +
                                       "WHERE year >= @minYear AND year <= @maxYear ORDER BY year DESC";
                    
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@minYear", minYear);
                        command.Parameters.AddWithValue("@maxYear", maxYear);
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vehicles.Add(new Vehicle
                                {
                                    Id = reader.GetInt32(0),
                                    Color = reader.GetString(1),
                                    Year = reader.GetInt32(2),
                                    Make = reader.GetString(3),
                                    Type = reader.GetString(4),
                                    CreatedAt = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
                
                _logger.LogInformation($"✓ Found {vehicles.Count} vehicles in year range {minYear}-{maxYear}");
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error filtering vehicles: {ex.Message}");
                return vehicles;
            }
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving vehicle with ID: {id} (Database)");
                
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    const string query = "SELECT ID, color, year, make, type, created_at FROM PRQ_Cars WHERE ID = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var vehicle = new Vehicle
                                {
                                    Id = reader.GetInt32(0),
                                    Color = reader.GetString(1),
                                    Year = reader.GetInt32(2),
                                    Make = reader.GetString(3),
                                    Type = reader.GetString(4),
                                    CreatedAt = reader.GetDateTime(5)
                                };
                                
                                _logger.LogInformation($"✓ Found vehicle ID {id}: {vehicle.Make}");
                                return vehicle;
                            }
                        }
                    }
                }
                
                _logger.LogWarning($"⚠️ Vehicle ID {id} not found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error retrieving vehicle: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateVehicleAsync(Vehicle vehicle)
        {
            try
            {
                _logger.LogInformation($"➕ Creating new vehicle (Database): {vehicle.Make}");
                
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    const string query = "INSERT INTO PRQ_Cars (color, year, make, type) VALUES (@color, @year, @make, @type)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@color", vehicle.Color);
                        command.Parameters.AddWithValue("@year", vehicle.Year);
                        command.Parameters.AddWithValue("@make", vehicle.Make);
                        command.Parameters.AddWithValue("@type", vehicle.Type);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                _logger.LogInformation($"✓ Vehicle created successfully: {vehicle.Make}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error creating vehicle: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateVehicleAsync(int id, Vehicle vehicle)
        {
            try
            {
                _logger.LogInformation($"✏️ Updating vehicle ID {id} (Database)");
                
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    const string query = "UPDATE PRQ_Cars SET color = @color, year = @year, make = @make, type = @type WHERE ID = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@color", vehicle.Color);
                        command.Parameters.AddWithValue("@year", vehicle.Year);
                        command.Parameters.AddWithValue("@make", vehicle.Make);
                        command.Parameters.AddWithValue("@type", vehicle.Type);
                        
                        var result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            _logger.LogInformation($"✓ Vehicle ID {id} updated successfully");
                            return true;
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ Vehicle ID {id} not found for update");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error updating vehicle: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            try
            {
                _logger.LogInformation($"🗑️ Deleting vehicle ID {id} (Database)");
                
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    const string query = "DELETE FROM PRQ_Cars WHERE ID = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        
                        var result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            _logger.LogInformation($"✓ Vehicle ID {id} deleted successfully");
                            return true;
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ Vehicle ID {id} not found for deletion");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error deleting vehicle: {ex.Message}");
                return false;
            }
        }
    }
}
