using VehicleMvcApp.Models;

namespace VehicleMvcApp.Services.Interfaces
{
    /// <summary>
    /// Repository interface for parking lot operations
    /// Handles queries related to car parking entries and parking lots
    /// </summary>
    public interface IParkingRepository
    {
        /// <summary>Get all cars of a specific type</summary>
        Task<List<Vehicle>> GetVehiclesByTypeAsync(string vehicleType);

        /// <summary>Get all parking lot entries for a specific car</summary>
        Task<List<CarParkingEntry>> GetParkingEntriesByCarIdAsync(int carId);

        /// <summary>Get a specific car by ID</summary>
        Task<Vehicle?> GetVehicleByIdAsync(int carId);

        /// <summary>Get parking lot details by ID</summary>
        Task<(string Name, string Province, decimal HourlyRate)?> GetParkingLotDetailsAsync(int parkingLotId);
    }
}
