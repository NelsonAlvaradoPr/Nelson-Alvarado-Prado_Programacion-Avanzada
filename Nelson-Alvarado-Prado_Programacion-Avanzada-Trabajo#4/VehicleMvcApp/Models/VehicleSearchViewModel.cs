namespace VehicleMvcApp.Models
{
    /// <summary>
    /// View model for vehicle search results and filtering
    /// </summary>
    public class VehicleSearchViewModel
    {
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        public int CurrentYear { get; set; }
        public string DataSource { get; set; } // "Database" or "JSON"
        public string Message { get; set; }
        public bool HasError { get; set; }
        public int ResultCount { get; set; }
    }
}
