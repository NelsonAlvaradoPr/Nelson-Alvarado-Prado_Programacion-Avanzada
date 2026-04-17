namespace VehicleMvcApp.Models
{
    /// <summary>
    /// Vehicle model representing a car in the system
    /// </summary>
    public class Vehicle
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
