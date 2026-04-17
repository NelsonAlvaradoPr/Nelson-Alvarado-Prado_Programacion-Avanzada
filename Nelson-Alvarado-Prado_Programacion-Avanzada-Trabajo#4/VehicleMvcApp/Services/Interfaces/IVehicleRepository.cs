using VehicleMvcApp.Models;

namespace VehicleMvcApp.Services.Interfaces
{
    /// <summary>
    /// Interface for vehicle data repository
    /// </summary>
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<List<Vehicle>> GetVehiclesByYearRangeAsync(int minYear, int maxYear);
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<bool> CreateVehicleAsync(Vehicle vehicle);
        Task<bool> UpdateVehicleAsync(int id, Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(int id);
    }
}
