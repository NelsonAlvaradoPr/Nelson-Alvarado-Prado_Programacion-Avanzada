namespace VehicleMvcApp.Models
{
    /// <summary>
    /// View model for the parking lot search and results display
    /// </summary>
    public class ParkingSearchViewModel
    {
        /// <summary>List of available vehicle types (sedan, 4x4, motorcycle)</summary>
        public List<string> AvailableTypes { get; set; } = new()
        {
            "sedan",
            "4x4",
            "motorcycle"
        };

        /// <summary>Currently selected vehicle type filter</summary>
        public string? SelectedType { get; set; }

        /// <summary>List of vehicles matching the selected type</summary>
        public List<Vehicle> MatchingVehicles { get; set; } = new();

        /// <summary>Currently selected vehicle ID for detail view</summary>
        public int? SelectedVehicleId { get; set; }

        /// <summary>Currently selected vehicle for display</summary>
        public Vehicle? SelectedVehicle { get; set; }

        /// <summary>Parking lot entries for the selected vehicle</summary>
        public List<CarParkingEntry> ParkingEntries { get; set; } = new();

        /// <summary>Total number of parking entries for selected vehicle</summary>
        public int TotalEntries => ParkingEntries.Count;

        /// <summary>Number of currently parked vehicles</summary>
        public int CurrentlyParkedCount => ParkingEntries.Count(e => e.IsCurrentlyParked);

        /// <summary>Total revenue from completed parking sessions</summary>
        public decimal TotalRevenue => ParkingEntries.Where(e => !e.IsCurrentlyParked).Sum(e => e.AmountDue);

        /// <summary>Current data source (JSON or Database)</summary>
        public string DataSource { get; set; } = "JSON File";

        /// <summary>Error or status message</summary>
        public string? Message { get; set; }

        /// <summary>Indicates if there's an error</summary>
        public bool HasError { get; set; }
    }
}
