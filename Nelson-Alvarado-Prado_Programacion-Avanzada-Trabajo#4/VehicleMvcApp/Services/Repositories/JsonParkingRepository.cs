using VehicleMvcApp.Models;
using VehicleMvcApp.Services.Interfaces;
using System.Text.Json;

namespace VehicleMvcApp.Services.Repositories
{
    /// <summary>
    /// JSON file-based parking lot repository
    /// Loads car data from JSON files (parking entry data typically from database only)
    /// </summary>
    public class JsonParkingRepository : IParkingRepository
    {
        private readonly string _jsonFilePath;
        private readonly ILogger<JsonParkingRepository> _logger;
        private List<Vehicle>? _cachedVehicles;

        public JsonParkingRepository(IConfiguration configuration, ILogger<JsonParkingRepository> logger)
        {
            _logger = logger;
            var relativePath = configuration["DataSource:JsonFilePath"] ?? "../prq_cars.json";
            _jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            
            _logger.LogInformation($"📄 JSON Parking Repository initialized. File path: {_jsonFilePath}");
        }

        private async Task<List<Vehicle>> LoadVehiclesAsync()
        {
            if (_cachedVehicles != null)
                return _cachedVehicles;

            try
            {
                if (!File.Exists(_jsonFilePath))
                {
                    _logger.LogWarning($"⚠️ JSON file not found: {_jsonFilePath}");
                    return new List<Vehicle>();
                }

                var json = await File.ReadAllTextAsync(_jsonFilePath);
                _cachedVehicles = JsonSerializer.Deserialize<List<Vehicle>>(json) ?? new List<Vehicle>();
                _logger.LogInformation($"✓ Loaded {_cachedVehicles.Count} vehicles from JSON");
                return _cachedVehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error loading JSON: {ex.Message}");
                return new List<Vehicle>();
            }
        }

        public async Task<List<Vehicle>> GetVehiclesByTypeAsync(string vehicleType)
        {
            try
            {
                _logger.LogInformation($"🔍 Filtering vehicles by type: {vehicleType} (JSON)");
                var vehicles = await LoadVehiclesAsync();
                var filtered = vehicles
                    .Where(v => v.Type?.ToLower() == vehicleType.ToLower())
                    .OrderBy(v => v.Make)
                    .ToList();

                _logger.LogInformation($"✓ Found {filtered.Count} vehicles of type {vehicleType}");
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error filtering by type: {ex.Message}");
                return new List<Vehicle>();
            }
        }

        public async Task<List<CarParkingEntry>> GetParkingEntriesByCarIdAsync(int carId)
        {
            try
            {
                _logger.LogInformation($"⚠️ JSON parking entries not supported. Car ID {carId} (use Database mode)");
                // JSON files don't typically contain parking entry data
                // This should be queried from the database
                return new List<CarParkingEntry>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error querying parking entries: {ex.Message}");
                return new List<CarParkingEntry>();
            }
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int carId)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving vehicle {carId} (JSON)");
                var vehicles = await LoadVehiclesAsync();
                var vehicle = vehicles.FirstOrDefault(v => v.Id == carId);

                if (vehicle != null)
                {
                    _logger.LogInformation($"✓ Found vehicle {carId}");
                }
                else
                {
                    _logger.LogWarning($"⚠️ Vehicle {carId} not found");
                }

                return vehicle;
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
                _logger.LogInformation($"⚠️ JSON parking lot details not supported. ID {parkingLotId} (use Database mode)");
                // Parking lot data should come from database
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
