namespace VehicleMvcApp.Models
{
    /// <summary>
    /// Represents a car's parking lot entry with all related information
    /// </summary>
    public class CarParkingEntry
    {
        /// <summary>Entry sequential number (primary key)</summary>
        public int EntryNumber { get; set; }

        /// <summary>Parking lot ID</summary>
        public int ParkingLotId { get; set; }

        /// <summary>Parking lot name</summary>
        public string? ParkingLotName { get; set; }

        /// <summary>Parking lot province</summary>
        public string? ParkingLotProvince { get; set; }

        /// <summary>Car ID</summary>
        public int CarId { get; set; }

        /// <summary>Entry date and time</summary>
        public DateTime EntryDateTime { get; set; }

        /// <summary>Exit date and time (nullable if still parked)</summary>
        public DateTime? ExitDateTime { get; set; }

        /// <summary>Hourly rate of the parking lot</summary>
        public decimal HourlyRate { get; set; }

        /// <summary>Calculated: Stay duration in minutes</summary>
        public int StayDurationMinutes
        {
            get
            {
                if (ExitDateTime.HasValue)
                {
                    return (int)(ExitDateTime.Value - EntryDateTime).TotalMinutes;
                }
                return 0;
            }
        }

        /// <summary>Calculated: Stay duration in hours (rounded to 2 decimals)</summary>
        public decimal StayDurationHours
        {
            get
            {
                if (ExitDateTime.HasValue)
                {
                    var totalHours = (ExitDateTime.Value - EntryDateTime).TotalHours;
                    return (decimal)Math.Round(totalHours, 2);
                }
                return 0;
            }
        }

        /// <summary>Calculated: Amount due based on stay duration and hourly rate</summary>
        public decimal AmountDue
        {
            get
            {
                if (ExitDateTime.HasValue)
                {
                    // Calculate based on stay duration in hours
                    var hours = StayDurationHours;
                    // Round up to nearest hour for billing
                    var billableHours = hours > 0 ? Math.Ceiling((double)hours) : 1;
                    return HourlyRate * (decimal)billableHours;
                }
                return 0;
            }
        }

        /// <summary>Status: whether the car is currently parked</summary>
        public bool IsCurrentlyParked => !ExitDateTime.HasValue;

        /// <summary>Formatted entry time string</summary>
        public string EntryDateTimeFormatted => EntryDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>Formatted exit time string</summary>
        public string ExitDateTimeFormatted => ExitDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Still Parked";
    }
}
