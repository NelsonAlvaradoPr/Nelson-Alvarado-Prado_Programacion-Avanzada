using Microsoft.AspNetCore.Mvc;
using VehicleMvcApp.Models;
using VehicleMvcApp.Services.Interfaces;

namespace VehicleMvcApp.Controllers
{
    /// <summary>
    /// Main controller for handling vehicle search and management operations
    /// Demonstrates working with both JSON and Database data sources
    /// </summary>
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(
            IVehicleRepository repository,
            IConfiguration configuration,
            ILogger<VehiclesController> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// GET: Display vehicle search form and results
        /// This action demonstrates the controller working with the repository
        /// The repository implementation (JSON or Database) is determined by appsettings.json
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int? minYear = null, int? maxYear = null)
        {
            try
            {
                var currentYear = int.Parse(_configuration["ApplicationSettings:CurrentYear"] ?? "2026");
                var defaultMinYear = int.Parse(_configuration["ApplicationSettings:DefaultMinYear"] ?? "2018");
                var defaultMaxYear = int.Parse(_configuration["ApplicationSettings:DefaultMaxYear"] ?? "2026");

                // Use provided values or defaults
                minYear = minYear ?? defaultMinYear;
                maxYear = maxYear ?? defaultMaxYear;

                _logger.LogInformation($"🔎 VehiclesController.Index called - Min Year: {minYear}, Max Year: {maxYear}");

                // Get the data source information
                var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
                var dataSourceName = useDatabase ? "Database" : "JSON File";

                _logger.LogWarning($"📊 Data Source: {dataSourceName}");
                _logger.LogWarning($"🔌 Repository Type: {_repository.GetType().Name}");

                // Fetch vehicles from repository (JSON or Database)
                var vehicles = await _repository.GetVehiclesByYearRangeAsync(minYear.Value, maxYear.Value);

                // Create the view model
                var viewModel = new VehicleSearchViewModel
                {
                    Vehicles = vehicles,
                    MinYear = minYear.Value,
                    MaxYear = maxYear.Value,
                    CurrentYear = currentYear,
                    DataSource = dataSourceName,
                    ResultCount = vehicles.Count,
                    HasError = false
                };

                _logger.LogInformation($"✅ Returning {vehicles.Count} vehicles from {dataSourceName}");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.Index: {ex.Message}");

                var viewModel = new VehicleSearchViewModel
                {
                    Vehicles = new List<Vehicle>(),
                    MinYear = minYear ?? 2018,
                    MaxYear = maxYear ?? 2026,
                    CurrentYear = 2026,
                    DataSource = _configuration.GetValue<bool>("DataSource:UseDatabase") ? "Database" : "JSON File",
                    HasError = true,
                    Message = $"Error: {ex.Message}"
                };

                return View(viewModel);
            }
        }

        /// <summary>
        /// GET: Display detailed information for a specific vehicle
        /// Shows how to retrieve a single vehicle from the repository
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation($"🔎 VehiclesController.Details called - Vehicle ID: {id}");

                var vehicle = await _repository.GetVehicleByIdAsync(id);

                if (vehicle == null)
                {
                    _logger.LogWarning($"⚠️ Vehicle with ID {id} not found");
                    return NotFound();
                }

                var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
                var dataSourceName = useDatabase ? "Database" : "JSON File";

                ViewBag.DataSource = dataSourceName;

                _logger.LogInformation($"✅ Vehicle details retrieved from {dataSourceName}");

                return View(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.Details: {ex.Message}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// GET: Display create vehicle form
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
            var dataSourceName = useDatabase ? "Database" : "JSON File";

            ViewBag.DataSource = dataSourceName;
            ViewBag.CurrentYear = int.Parse(_configuration["ApplicationSettings:CurrentYear"] ?? "2026");

            return View();
        }

        /// <summary>
        /// POST: Create a new vehicle
        /// Demonstrates writing data to the repository (JSON or Database)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Color,Year,Make,Type")] Vehicle vehicle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"➕ Creating vehicle: {vehicle.Make}");

                    var success = await _repository.CreateVehicleAsync(vehicle);

                    if (success)
                    {
                        TempData["SuccessMessage"] = $"Vehicle '{vehicle.Make}' created successfully!";
                        _logger.LogInformation($"✅ Vehicle created successfully");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error creating vehicle");
                        _logger.LogError("❌ Failed to create vehicle");
                    }
                }

                return View(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.Create: {ex.Message}");
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(vehicle);
            }
        }

        /// <summary>
        /// GET: Display edit vehicle form
        /// Retrieves the vehicle from repository for editing
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation($"📝 VehiclesController.Edit called - Vehicle ID: {id}");

                var vehicle = await _repository.GetVehicleByIdAsync(id);

                if (vehicle == null)
                {
                    _logger.LogWarning($"⚠️ Vehicle with ID {id} not found");
                    return NotFound();
                }

                var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
                var dataSourceName = useDatabase ? "Database" : "JSON File";

                ViewBag.DataSource = dataSourceName;
                ViewBag.CurrentYear = int.Parse(_configuration["ApplicationSettings:CurrentYear"] ?? "2026");

                return View(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.Edit: {ex.Message}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// POST: Update an existing vehicle
        /// Demonstrates updating data in the repository (JSON or Database)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Color,Year,Make,Type")] Vehicle vehicle)
        {
            try
            {
                if (id != vehicle.Id)
                {
                    _logger.LogWarning($"⚠️ Vehicle ID mismatch: {id} vs {vehicle.Id}");
                    return BadRequest();
                }

                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"✏️ Updating vehicle ID {id}: {vehicle.Make}");

                    var success = await _repository.UpdateVehicleAsync(id, vehicle);

                    if (success)
                    {
                        TempData["SuccessMessage"] = $"Vehicle '{vehicle.Make}' updated successfully!";
                        _logger.LogInformation($"✅ Vehicle updated successfully");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error updating vehicle");
                        _logger.LogError("❌ Failed to update vehicle");
                    }
                }

                return View(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.Edit: {ex.Message}");
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(vehicle);
            }
        }

        /// <summary>
        /// GET: Display delete confirmation page
        /// Retrieves the vehicle from repository for confirmation
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"🗑️ VehiclesController.Delete called - Vehicle ID: {id}");

                var vehicle = await _repository.GetVehicleByIdAsync(id);

                if (vehicle == null)
                {
                    _logger.LogWarning($"⚠️ Vehicle with ID {id} not found");
                    return NotFound();
                }

                var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
                var dataSourceName = useDatabase ? "Database" : "JSON File";

                ViewBag.DataSource = dataSourceName;

                return View(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.Delete: {ex.Message}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// POST: Confirm delete of a vehicle
        /// Demonstrates deleting data from the repository (JSON or Database)
        /// </summary>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation($"🗑️ Confirming deletion of vehicle ID {id}");

                var success = await _repository.DeleteVehicleAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Vehicle deleted successfully!";
                    _logger.LogInformation($"✅ Vehicle deleted successfully");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("❌ Failed to delete vehicle");
                    TempData["ErrorMessage"] = "Error deleting vehicle";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error in VehiclesController.DeleteConfirmed: {ex.Message}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: Display system information about data source
        /// Shows which data source is currently being used
        /// </summary>
        [HttpGet]
        public IActionResult SystemInfo()
        {
            var useDatabase = _configuration.GetValue<bool>("DataSource:UseDatabase");
            var dataSourceName = useDatabase ? "Database (MySQL)" : "JSON File";
            var jsonPath = _configuration["DataSource:JsonFilePath"];
            var connectionString = _configuration["DataSource:ConnectionString"];
            var repositoryType = _repository.GetType().Name;

            var info = new
            {
                DataSource = dataSourceName,
                UseDatabase = useDatabase,
                RepositoryType = repositoryType,
                JsonFilePath = jsonPath,
                ConnectionString = useDatabase ? connectionString : "N/A",
                CurrentYear = _configuration["ApplicationSettings:CurrentYear"],
                DefaultMinYear = _configuration["ApplicationSettings:DefaultMinYear"],
                DefaultMaxYear = _configuration["ApplicationSettings:DefaultMaxYear"]
            };

            ViewBag.SystemInfo = info;
            _logger.LogWarning($"📊 System Info displayed - Data Source: {dataSourceName}, Repository: {repositoryType}");

            return View(info);
        }
    }
}
