using VehicleMvcApp.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using VehicleMvcApp.Services.Interfaces;

namespace VehicleMvcApp.Services.Repositories
{
    /// <summary>
    /// JSON file-based vehicle repository
    /// Reads and writes vehicle data from/to JSON files
    /// </summary>
    public class JsonVehicleRepository : IVehicleRepository
    {
        private readonly string _filePath;
        private readonly ILogger<JsonVehicleRepository> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonVehicleRepository(IConfiguration configuration, ILogger<JsonVehicleRepository> logger)
        {
            _logger = logger;
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = configuration["DataSource:JsonFilePath"] ?? "../prq_cars.json";
            _filePath = Path.Combine(basePath, configPath);
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _logger.LogInformation($"📄 JSON Repository initialized. File path: {_filePath}");
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            try
            {
                _logger.LogInformation("📖 Reading all vehicles from JSON file...");
                
                if (!File.Exists(_filePath))
                {
                    _logger.LogWarning($"⚠️ JSON file not found: {_filePath}");
                    return new List<Vehicle>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(json, _jsonOptions) ?? new List<Vehicle>();
                
                _logger.LogInformation($"✓ Successfully read {vehicles.Count} vehicles from JSON");
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error reading JSON file: {ex.Message}");
                return new List<Vehicle>();
            }
        }

        public async Task<List<Vehicle>> GetVehiclesByYearRangeAsync(int minYear, int maxYear)
        {
            try
            {
                _logger.LogInformation($"🔍 Filtering vehicles by year range: {minYear}-{maxYear} (JSON)");
                
                var allVehicles = await GetAllVehiclesAsync();
                var filtered = allVehicles
                    .Where(v => v.Year >= minYear && v.Year <= maxYear)
                    .OrderBy(v => v.Year)
                    .ToList();

                _logger.LogInformation($"✓ Found {filtered.Count} vehicles in year range {minYear}-{maxYear}");
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error filtering vehicles: {ex.Message}");
                return new List<Vehicle>();
            }
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving vehicle with ID: {id} (JSON)");
                
                var allVehicles = await GetAllVehiclesAsync();
                var vehicle = allVehicles.FirstOrDefault(v => v.Id == id);

                if (vehicle != null)
                {
                    _logger.LogInformation($"✓ Found vehicle ID {id}: {vehicle.Make}");
                }
                else
                {
                    _logger.LogWarning($"⚠️ Vehicle ID {id} not found");
                }

                return vehicle;
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
                _logger.LogInformation($"➕ Creating new vehicle (JSON): {vehicle.Make}");
                
                var vehicles = await GetAllVehiclesAsync();
                vehicle.Id = vehicles.Any() ? vehicles.Max(v => v.Id) + 1 : 1;
                vehicle.CreatedAt = DateTime.Now;
                
                vehicles.Add(vehicle);
                
                var json = JsonSerializer.Serialize(vehicles, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
                
                _logger.LogInformation($"✓ Vehicle created successfully with ID: {vehicle.Id}");
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
                _logger.LogInformation($"✏️ Updating vehicle ID {id} (JSON)");
                
                var vehicles = await GetAllVehiclesAsync();
                var existingVehicle = vehicles.FirstOrDefault(v => v.Id == id);

                if (existingVehicle == null)
                {
                    _logger.LogWarning($"⚠️ Vehicle ID {id} not found for update");
                    return false;
                }

                existingVehicle.Color = vehicle.Color;
                existingVehicle.Year = vehicle.Year;
                existingVehicle.Make = vehicle.Make;
                existingVehicle.Type = vehicle.Type;

                var json = JsonSerializer.Serialize(vehicles, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);

                _logger.LogInformation($"✓ Vehicle ID {id} updated successfully");
                return true;
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
                _logger.LogInformation($"🗑️ Deleting vehicle ID {id} (JSON)");
                
                var vehicles = await GetAllVehiclesAsync();
                var vehicleToRemove = vehicles.FirstOrDefault(v => v.Id == id);

                if (vehicleToRemove == null)
                {
                    _logger.LogWarning($"⚠️ Vehicle ID {id} not found for deletion");
                    return false;
                }

                vehicles.Remove(vehicleToRemove);
                var json = JsonSerializer.Serialize(vehicles, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);

                _logger.LogInformation($"✓ Vehicle ID {id} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error deleting vehicle: {ex.Message}");
                return false;
            }
        }
    }
}
