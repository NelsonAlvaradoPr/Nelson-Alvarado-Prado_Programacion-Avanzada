/**
 * CRUD Service for Car Park Database - Node.js
 * Handles Insert, Update, Delete operations on all tables
 * Supports both JSON and Database sources
 */

const fs = require('fs');
const path = require('path');

// ================================================================
// JSON CRUD SERVICE
// ================================================================

class JsonCrudService {
  constructor(dataDir = '.') {
    this.dataDir = dataDir;
    this.carsFile = path.join(dataDir, 'prq_cars.json');
    this.parkingFile = path.join(dataDir, 'prq_parking.json');
    this.carEntryFile = path.join(dataDir, 'prq_car_entry.json');
  }

  /**
   * Load JSON file safely
   */
  loadJson(filePath) {
    try {
      const content = fs.readFileSync(filePath, 'utf-8');
      return JSON.parse(content);
    } catch (error) {
      console.error(`Error loading ${filePath}:`, error.message);
      return [];
    }
  }

  /**
   * Save JSON file safely
   */
  saveJson(filePath, data) {
    try {
      fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf-8');
      return true;
    } catch (error) {
      console.error(`Error saving ${filePath}:`, error.message);
      return false;
    }
  }

  /**
   * Get next ID for a table
   */
  getNextId(data) {
    if (data.length === 0) return 1;
    const maxId = Math.max(...data.map(item => item.id || item.sequential_number || 0));
    return maxId + 1;
  }

  // ================================================================
  // CAR OPERATIONS
  // ================================================================

  insertCar(carData) {
    const cars = this.loadJson(this.carsFile);
    const newCar = {
      id: this.getNextId(cars),
      color: carData.color,
      year: carData.year,
      make: carData.make,
      type: carData.type
    };
    cars.push(newCar);
    if (this.saveJson(this.carsFile, cars)) {
      return { success: true, data: newCar };
    }
    return { success: false, error: 'Failed to save car' };
  }

  updateCar(id, carData) {
    const cars = this.loadJson(this.carsFile);
    const carIndex = cars.findIndex(c => c.id === id);
    if (carIndex === -1) {
      return { success: false, error: `Car with ID ${id} not found` };
    }
    cars[carIndex] = { id, ...carData };
    if (this.saveJson(this.carsFile, cars)) {
      return { success: true, data: cars[carIndex] };
    }
    return { success: false, error: 'Failed to update car' };
  }

  deleteCar(id) {
    const cars = this.loadJson(this.carsFile);
    const carIndex = cars.findIndex(c => c.id === id);
    if (carIndex === -1) {
      return { success: false, error: `Car with ID ${id} not found` };
    }
    const deletedCar = cars[carIndex];
    cars.splice(carIndex, 1);
    if (this.saveJson(this.carsFile, cars)) {
      return { success: true, data: deletedCar };
    }
    return { success: false, error: 'Failed to delete car' };
  }

  // ================================================================
  // PARKING OPERATIONS
  // ================================================================

  insertParking(parkingData) {
    const parkings = this.loadJson(this.parkingFile);
    const newParking = {
      id: this.getNextId(parkings),
      province_name: parkingData.province_name,
      name: parkingData.name,
      hourly_rate: parkingData.hourly_rate
    };
    parkings.push(newParking);
    if (this.saveJson(this.parkingFile, parkings)) {
      return { success: true, data: newParking };
    }
    return { success: false, error: 'Failed to save parking' };
  }

  updateParking(id, parkingData) {
    const parkings = this.loadJson(this.parkingFile);
    const parkingIndex = parkings.findIndex(p => p.id === id);
    if (parkingIndex === -1) {
      return { success: false, error: `Parking with ID ${id} not found` };
    }
    parkings[parkingIndex] = { id, ...parkingData };
    if (this.saveJson(this.parkingFile, parkings)) {
      return { success: true, data: parkings[parkingIndex] };
    }
    return { success: false, error: 'Failed to update parking' };
  }

  deleteParking(id) {
    const parkings = this.loadJson(this.parkingFile);
    const parkingIndex = parkings.findIndex(p => p.id === id);
    if (parkingIndex === -1) {
      return { success: false, error: `Parking with ID ${id} not found` };
    }
    const deletedParking = parkings[parkingIndex];
    parkings.splice(parkingIndex, 1);
    if (this.saveJson(this.parkingFile, parkings)) {
      return { success: true, data: deletedParking };
    }
    return { success: false, error: 'Failed to delete parking' };
  }

  // ================================================================
  // CAR ENTRY OPERATIONS
  // ================================================================

  insertCarEntry(entryData) {
    const entries = this.loadJson(this.carEntryFile);
    const newEntry = {
      sequential_number: this.getNextId(entries),
      parking_id: entryData.parking_id,
      car_id: entryData.car_id,
      entry_date_time: entryData.entry_date_time,
      exit_date_time: entryData.exit_date_time || null
    };
    entries.push(newEntry);
    if (this.saveJson(this.carEntryFile, entries)) {
      return { success: true, data: newEntry };
    }
    return { success: false, error: 'Failed to save car entry' };
  }

  updateCarEntry(sequentialNumber, entryData) {
    const entries = this.loadJson(this.carEntryFile);
    const entryIndex = entries.findIndex(e => e.sequential_number === sequentialNumber);
    if (entryIndex === -1) {
      return { success: false, error: `Car entry #${sequentialNumber} not found` };
    }
    entries[entryIndex] = { sequential_number: sequentialNumber, ...entryData };
    if (this.saveJson(this.carEntryFile, entries)) {
      return { success: true, data: entries[entryIndex] };
    }
    return { success: false, error: 'Failed to update car entry' };
  }

  deleteCarEntry(sequentialNumber) {
    const entries = this.loadJson(this.carEntryFile);
    const entryIndex = entries.findIndex(e => e.sequential_number === sequentialNumber);
    if (entryIndex === -1) {
      return { success: false, error: `Car entry #${sequentialNumber} not found` };
    }
    const deletedEntry = entries[entryIndex];
    entries.splice(entryIndex, 1);
    if (this.saveJson(this.carEntryFile, entries)) {
      return { success: true, data: deletedEntry };
    }
    return { success: false, error: 'Failed to delete car entry' };
  }
}

// ================================================================
// DATABASE CRUD SERVICE
// ================================================================

class DatabaseCrudService {
  constructor(pool) {
    this.pool = pool;
  }

  // ================================================================
  // CAR OPERATIONS
  // ================================================================

  async insertCar(carData) {
    const connection = await this.pool.getConnection();
    try {
      const [result] = await connection.execute(
        'INSERT INTO PRQ_Cars (color, year, make, type) VALUES (?, ?, ?, ?)',
        [carData.color, carData.year, carData.make, carData.type]
      );
      return { success: true, data: { id: result.insertId, ...carData } };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  async updateCar(id, carData) {
    const connection = await this.pool.getConnection();
    try {
      await connection.execute(
        'UPDATE PRQ_Cars SET color = ?, year = ?, make = ?, type = ? WHERE ID = ?',
        [carData.color, carData.year, carData.make, carData.type, id]
      );
      return { success: true, data: { id, ...carData } };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  async deleteCar(id) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Cars WHERE ID = ?', [id]);
      if (rows.length === 0) {
        return { success: false, error: `Car with ID ${id} not found` };
      }
      const car = rows[0];
      await connection.execute('DELETE FROM PRQ_Cars WHERE ID = ?', [id]);
      return { success: true, data: car };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  // ================================================================
  // PARKING OPERATIONS
  // ================================================================

  async insertParking(parkingData) {
    const connection = await this.pool.getConnection();
    try {
      const [result] = await connection.execute(
        'INSERT INTO PRQ_Parking (province_name, name, hourly_rate) VALUES (?, ?, ?)',
        [parkingData.province_name, parkingData.name, parkingData.hourly_rate]
      );
      return { success: true, data: { id: result.insertId, ...parkingData } };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  async updateParking(id, parkingData) {
    const connection = await this.pool.getConnection();
    try {
      await connection.execute(
        'UPDATE PRQ_Parking SET province_name = ?, name = ?, hourly_rate = ? WHERE ID = ?',
        [parkingData.province_name, parkingData.name, parkingData.hourly_rate, id]
      );
      return { success: true, data: { id, ...parkingData } };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  async deleteParking(id) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Parking WHERE ID = ?', [id]);
      if (rows.length === 0) {
        return { success: false, error: `Parking with ID ${id} not found` };
      }
      const parking = rows[0];
      await connection.execute('DELETE FROM PRQ_Parking WHERE ID = ?', [id]);
      return { success: true, data: parking };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  // ================================================================
  // CAR ENTRY OPERATIONS
  // ================================================================

  async insertCarEntry(entryData) {
    const connection = await this.pool.getConnection();
    try {
      const [result] = await connection.execute(
        'INSERT INTO PRQ_Car_Entry (parking_id, car_id, entry_date_time, exit_date_time) VALUES (?, ?, ?, ?)',
        [entryData.parking_id, entryData.car_id, entryData.entry_date_time, entryData.exit_date_time || null]
      );
      return { success: true, data: { sequential_number: result.insertId, ...entryData } };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  async updateCarEntry(sequentialNumber, entryData) {
    const connection = await this.pool.getConnection();
    try {
      await connection.execute(
        'UPDATE PRQ_Car_Entry SET parking_id = ?, car_id = ?, entry_date_time = ?, exit_date_time = ? WHERE sequential_number = ?',
        [entryData.parking_id, entryData.car_id, entryData.entry_date_time, entryData.exit_date_time || null, sequentialNumber]
      );
      return { success: true, data: { sequential_number: sequentialNumber, ...entryData } };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }

  async deleteCarEntry(sequentialNumber) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Car_Entry WHERE sequential_number = ?',
        [sequentialNumber]
      );
      if (rows.length === 0) {
        return { success: false, error: `Car entry #${sequentialNumber} not found` };
      }
      const entry = rows[0];
      await connection.execute('DELETE FROM PRQ_Car_Entry WHERE sequential_number = ?', [sequentialNumber]);
      return { success: true, data: entry };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      connection.release();
    }
  }
}

module.exports = {
  JsonCrudService,
  DatabaseCrudService
};
