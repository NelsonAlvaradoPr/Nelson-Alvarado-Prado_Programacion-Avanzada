using VehicleMvcApp.Models;
using VehicleMvcApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace VehicleMvcApp.Controllers
{
    /// <summary>
    /// Controller for parking lot entry viewing and car search by type
    /// Displays cars grouped by type and their parking history
    /// </summary>
    public class ParkingController : Controller
    {
        private readonly IParkingRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ParkingController> _logger;

        public ParkingController(
            IParkingRepository repository,
            IConfiguration configuration,
            ILogger<ParkingController> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Main parking search page with vehicle type filter
        /// Displays vehicles by type and parking entries when selected
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string? vehicleType = null, int? carId = null)
        {
            try
            {
                var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
                var dataSource = useDatabase ? "Database" : "JSON File";

                _logger.LogInformation($"📖 Loading parking search page. Data source: {dataSource}");

                var viewModel = new ParkingSearchViewModel
                {
                    DataSource = dataSource,
                    SelectedType = vehicleType
                };

                // If vehicle type is selected, get matching vehicles
                if (!string.IsNullOrEmpty(vehicleType))
                {
                    viewModel.MatchingVehicles = await _repository.GetVehiclesByTypeAsync(vehicleType);
                    _logger.LogInformation($"🔍 Found {viewModel.MatchingVehicles.Count} vehicles of type: {vehicleType}");
                }

                // If a specific car is selected, get its parking entries
                if (carId.HasValue && carId > 0)
                {
                    viewModel.SelectedVehicleId = carId;
                    viewModel.SelectedVehicle = await _repository.GetVehicleByIdAsync(carId.Value);

                    if (viewModel.SelectedVehicle != null)
                    {
                        viewModel.ParkingEntries = await _repository.GetParkingEntriesByCarIdAsync(carId.Value);
                        _logger.LogInformation($"🅿️ Found {viewModel.ParkingEntries.Count} parking entries for car {carId}");

                        if (viewModel.ParkingEntries.Count == 0)
                        {
                            _logger.LogWarning($"⚠️ No parking entries found for car {carId}");
                            viewModel.Message = $"No parking entries found for {viewModel.SelectedVehicle.Make} ({viewModel.SelectedVehicle.Year})";
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ Vehicle {carId} not found");
                        viewModel.HasError = true;
                        viewModel.Message = $"Vehicle ID {carId} not found";
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error loading parking search: {ex.Message}");
                var viewModel = new ParkingSearchViewModel
                {
                    HasError = true,
                    Message = $"Error loading data: {ex.Message}"
                };
                return View(viewModel);
            }
        }

        /// <summary>
        /// AJAX endpoint to get parking entries for a selected car
        /// Returns partial view with parking entry details
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCarEntries(int carId)
        {
            try
            {
                _logger.LogInformation($"📋 Retrieving parking entries for car {carId}");

                var vehicle = await _repository.GetVehicleByIdAsync(carId);
                if (vehicle == null)
                {
                    return NotFound($"Vehicle {carId} not found");
                }

                var entries = await _repository.GetParkingEntriesByCarIdAsync(carId);
                _logger.LogInformation($"✓ Retrieved {entries.Count} parking entries for car {carId}");

                var viewModel = new ParkingSearchViewModel
                {
                    SelectedVehicle = vehicle,
                    SelectedVehicleId = carId,
                    ParkingEntries = entries,
                    DataSource = _configuration.GetValue<bool>("DataSource:UseDatabase") ? "Database" : "JSON File"
                };

                return PartialView("_ParkingEntriesPanel", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error retrieving parking entries: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all available vehicle types
        /// Used for populating the type dropdown
        /// </summary>
        [HttpGet]
        public IActionResult GetVehicleTypes()
        {
            try
            {
                var types = new List<string> { "sedan", "4x4", "motorcycle" };
                return Json(types);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error getting vehicle types: {ex.Message}");
                return BadRequest();
            }
        }
    }
}
