/**
 * CarParking Session Class - Node.js
 * Represents a car parking session with calculated fields:
 * - stayDurationMinutes: Total time parked in minutes
 * - stayDurationHours: Total time parked in hours (decimal)
 * - totalAmountDue: Parking fee based on duration and hourly rate
 * 
 * All calculated fields return null if exit_date_time is not set
 */

class CarParkingSession {
  constructor(data) {
    this.sequentialNumber = data.sequential_number;
    this.parkingId = data.parking_id;
    this.carId = data.car_id;
    this.entryDateTime = new Date(data.entry_date_time);
    this.exitDateTime = data.exit_date_time ? new Date(data.exit_date_time) : null;
    this.hourlyRate = data.hourly_rate;
  }

  /**
   * Calculate stay duration in minutes
   * Returns null if vehicle hasn't exited yet (exitDateTime is null)
   */
  get stayDurationMinutes() {
    if (!this.exitDateTime) {
      return null;
    }
    const diffMs = this.exitDateTime - this.entryDateTime;
    const diffMinutes = Math.floor(diffMs / 1000 / 60);
    return diffMinutes;
  }

  /**
   * Calculate stay duration in hours (decimal format)
   * Returns null if vehicle hasn't exited yet (exitDateTime is null)
   */
  get stayDurationHours() {
    if (!this.exitDateTime) {
      return null;
    }
    const minutes = this.stayDurationMinutes;
    return parseFloat((minutes / 60).toFixed(2));
  }

  /**
   * Calculate total amount due
   * Formula: stay duration in hours × hourly rate
   * Returns null if vehicle hasn't exited yet (exitDateTime is null)
   */
  get totalAmountDue() {
    if (!this.exitDateTime) {
      return null;
    }
    const hours = this.stayDurationHours;
    return parseFloat((hours * this.hourlyRate).toFixed(2));
  }

  /**
   * Check if the vehicle has exited
   */
  get hasExited() {
    return this.exitDateTime !== null;
  }

  /**
   * Get vehicle status
   */
  get status() {
    return this.hasExited ? 'EXITED' : 'ACTIVE';
  }

  /**
   * Return object with all session data and calculated fields
   */
  toObject() {
    return {
      sequential_number: this.sequentialNumber,
      parking_id: this.parkingId,
      car_id: this.carId,
      entry_date_time: this.entryDateTime.toISOString(),
      exit_date_time: this.exitDateTime ? this.exitDateTime.toISOString() : null,
      stay_duration_minutes: this.stayDurationMinutes,
      stay_duration_hours: this.stayDurationHours,
      total_amount_due: this.totalAmountDue,
      status: this.status,
      hourly_rate: this.hourlyRate
    };
  }

  /**
   * Return JSON representation
   */
  toJSON() {
    return this.toObject();
  }

  /**
   * Return formatted string representation
   */
  toString() {
    return `CarParkingSession #${this.sequentialNumber} - Car ${this.carId} at Parking ${this.parkingId}
    Status: ${this.status}
    Entry: ${this.entryDateTime.toLocaleString()}
    Exit: ${this.exitDateTime ? this.exitDateTime.toLocaleString() : 'Not exited'}
    Duration: ${this.stayDurationMinutes} minutes (${this.stayDurationHours} hours)
    Amount Due: €${this.totalAmountDue}`;
  }
}

module.exports = CarParkingSession;
