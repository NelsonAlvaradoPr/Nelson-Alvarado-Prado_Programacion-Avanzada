using VehicleMvcApp.Models;
using MySql.Data.MySqlClient;
using VehicleMvcApp.Services.Interfaces;

namespace VehicleMvcApp.Services.Repositories
{
    /// <summary>
    /// MySQL database-based parking lot repository
    /// Queries parking lot, car, and entry data from the database
    /// </summary>
    public class DatabaseParkingRepository : IParkingRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseParkingRepository> _logger;

        public DatabaseParkingRepository(IConfiguration configuration, ILogger<DatabaseParkingRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration["DataSource:ConnectionString"] ?? "";
            _logger.LogInformation("🗄️ Database Parking Repository initialized");
        }

        public async Task<List<Vehicle>> GetVehiclesByTypeAsync(string vehicleType)
        {
            var vehicles = new List<Vehicle>();
            try
            {
                _logger.LogInformation($"🔍 Querying vehicles of type: {vehicleType} (Database)");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string query = "SELECT ID, color, year, make, type FROM PRQ_Cars WHERE type = @type ORDER BY make";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@type", vehicleType);

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
                                    CreatedAt = DateTime.Now
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation($"✓ Found {vehicles.Count} vehicles of type {vehicleType}");
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error querying vehicles by type: {ex.Message}");
                return vehicles;
            }
        }

        public async Task<List<CarParkingEntry>> GetParkingEntriesByCarIdAsync(int carId)
        {
            var entries = new List<CarParkingEntry>();
            try
            {
                _logger.LogInformation($"🔍 Querying parking entries for car ID {carId} (Database)");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
                        SELECT 
                            pce.sequential_number,
                            pce.parking_lot_id,
                            pce.car_id,
                            pce.entry_date_time,
                            pce.exit_date_time,
                            pl.name,
                            pl.province_name,
                            pl.hourly_rate
                        FROM PRQ_Car_Entry pce
                        INNER JOIN PRQ_Parking_Lot pl ON pce.parking_lot_id = pl.ID
                        WHERE pce.car_id = @carId
                        ORDER BY pce.entry_date_time DESC";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@carId", carId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var exitDateTime = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

                                entries.Add(new CarParkingEntry
                                {
                                    EntryNumber = reader.GetInt32(0),
                                    ParkingLotId = reader.GetInt32(1),
                                    CarId = reader.GetInt32(2),
                                    EntryDateTime = reader.GetDateTime(3),
                                    ExitDateTime = exitDateTime,
                                    ParkingLotName = reader.GetString(5),
                                    ParkingLotProvince = reader.GetString(6),
                                    HourlyRate = reader.GetDecimal(7)
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation($"✓ Found {entries.Count} parking entries for car {carId}");
                return entries;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error querying parking entries: {ex.Message}");
                return entries;
            }
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int carId)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving vehicle ID {carId} (Database)");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string query = "SELECT ID, color, year, make, type FROM PRQ_Cars WHERE ID = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", carId);

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
                                    CreatedAt = DateTime.Now
                                };

                                _logger.LogInformation($"✓ Found vehicle {carId}");
                                return vehicle;
                            }
                        }
                    }
                }

                _logger.LogWarning($"⚠️ Vehicle {carId} not found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error retrieving vehicle: {ex.Message}");
                return null;
            }
        }

        public async Task<(string Name, string Province, decimal HourlyRate)?> GetParkingLotDetailsAsync(int parkingLotId)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving parking lot ID {parkingLotId} (Database)");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string query = "SELECT name, province_name, hourly_rate FROM PRQ_Parking_Lot WHERE ID = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", parkingLotId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var result = (
                                    reader.GetString(0),
                                    reader.GetString(1),
                                    reader.GetDecimal(2)
                                );

                                _logger.LogInformation($"✓ Found parking lot {parkingLotId}");
                                return result;
                            }
                        }
                    }
                }

                _logger.LogWarning($"⚠️ Parking lot {parkingLotId} not found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error retrieving parking lot: {ex.Message}");
                return null;
            }
        }
    }
}
