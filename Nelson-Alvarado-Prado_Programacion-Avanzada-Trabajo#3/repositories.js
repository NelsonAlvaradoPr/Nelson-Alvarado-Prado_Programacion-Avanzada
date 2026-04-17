/**
 * Repository Interfaces and Implementations - Node.js
 * Provides data access layer for car park tables with support for both JSON and Database sources
 */

// ================================================================
// INTERFACES
// ================================================================

/**
 * Base Repository Interface
 * Defines common operations for all repositories
 */
class IRepository {
  async getById(id) {
    throw new Error('getById() must be implemented');
  }

  async getAll() {
    throw new Error('getAll() must be implemented');
  }
}

/**
 * Car Repository Interface
 * Defines queries for PRQ_Cars table
 */
class ICarRepository extends IRepository {
  async getById(id) {
    throw new Error('getById() must be implemented');
  }

  async getAll() {
    throw new Error('getAll() must be implemented');
  }

  async getByColor(color) {
    throw new Error('getByColor() must be implemented');
  }

  async getByYearRange(minYear, maxYear) {
    throw new Error('getByYearRange() must be implemented');
  }

  async getByMake(make) {
    throw new Error('getByMake() must be implemented');
  }

  async getByType(type) {
    throw new Error('getByType() must be implemented');
  }

  async getByColorAndYearRangeAndMakeAndType(color, minYear, maxYear, make, type) {
    throw new Error('getByColorAndYearRangeAndMakeAndType() must be implemented');
  }
}

/**
 * Parking Repository Interface
 * Defines queries for PRQ_Parking table
 */
class IParkingRepository extends IRepository {
  async getById(id) {
    throw new Error('getById() must be implemented');
  }

  async getAll() {
    throw new Error('getAll() must be implemented');
  }

  async getByProvinceName(provinceName) {
    throw new Error('getByProvinceName() must be implemented');
  }

  async getByName(name) {
    throw new Error('getByName() must be implemented');
  }

  async getByHourlyRateRange(minRate, maxRate) {
    throw new Error('getByHourlyRateRange() must be implemented');
  }

  async getByProvinceAndNameAndHourlyRateRange(provinceName, name, minRate, maxRate) {
    throw new Error('getByProvinceAndNameAndHourlyRateRange() must be implemented');
  }
}

/**
 * Car Entry Repository Interface
 * Defines queries for PRQ_Car_Entry table
 */
class ICarEntryRepository extends IRepository {
  async getById(sequentialNumber) {
    throw new Error('getById() must be implemented');
  }

  async getAll() {
    throw new Error('getAll() must be implemented');
  }

  async getHourlyPriceForParking(parkingId) {
    throw new Error('getHourlyPriceForParking() must be implemented');
  }

  async getCarsByTypeInDateRange(carType, startDate, endDate) {
    throw new Error('getCarsByTypeInDateRange() must be implemented');
  }

  async getCarsByProvinceInDateRange(provinceName, startDate, endDate) {
    throw new Error('getCarsByProvinceInDateRange() must be implemented');
  }
}

// ================================================================
// JSON IMPLEMENTATIONS
// ================================================================

const fs = require('fs');
const path = require('path');
const CarParkingSession = require('./CarParkingSession');

/**
 * JSON-based Car Repository
 */
class JsonCarRepository extends ICarRepository {
  constructor(jsonFilePath) {
    super();
    this.data = this.loadJson(jsonFilePath);
  }

  loadJson(filePath) {
    try {
      const content = fs.readFileSync(filePath, 'utf-8');
      return JSON.parse(content);
    } catch (error) {
      console.error(`Error loading JSON file ${filePath}:`, error.message);
      return [];
    }
  }

  async getById(id) {
    return this.data.find(car => car.id === id) || null;
  }

  async getAll() {
    return this.data;
  }

  async getByColor(color) {
    return this.data.filter(car => car.color.toLowerCase() === color.toLowerCase());
  }

  async getByYearRange(minYear, maxYear) {
    return this.data.filter(car => car.year >= minYear && car.year <= maxYear);
  }

  async getByMake(make) {
    return this.data.filter(car => car.make.toLowerCase().includes(make.toLowerCase()));
  }

  async getByType(type) {
    return this.data.filter(car => car.type === type);
  }

  async getByColorAndYearRangeAndMakeAndType(color, minYear, maxYear, make, type) {
    return this.data.filter(car => {
      const colorMatch = !color || car.color.toLowerCase() === color.toLowerCase();
      const yearMatch = car.year >= minYear && car.year <= maxYear;
      const makeMatch = !make || car.make.toLowerCase().includes(make.toLowerCase());
      const typeMatch = !type || car.type === type;
      return colorMatch && yearMatch && makeMatch && typeMatch;
    });
  }
}

/**
 * JSON-based Parking Repository
 */
class JsonParkingRepository extends IParkingRepository {
  constructor(jsonFilePath) {
    super();
    this.data = this.loadJson(jsonFilePath);
  }

  loadJson(filePath) {
    try {
      const content = fs.readFileSync(filePath, 'utf-8');
      return JSON.parse(content);
    } catch (error) {
      console.error(`Error loading JSON file ${filePath}:`, error.message);
      return [];
    }
  }

  async getById(id) {
    return this.data.find(parking => parking.id === id) || null;
  }

  async getAll() {
    return this.data;
  }

  async getByProvinceName(provinceName) {
    return this.data.filter(p => p.province_name.toLowerCase().includes(provinceName.toLowerCase()));
  }

  async getByName(name) {
    return this.data.filter(p => p.name.toLowerCase().includes(name.toLowerCase()));
  }

  async getByHourlyRateRange(minRate, maxRate) {
    return this.data.filter(p => p.hourly_rate >= minRate && p.hourly_rate <= maxRate);
  }

  async getByProvinceAndNameAndHourlyRateRange(provinceName, name, minRate, maxRate) {
    return this.data.filter(p => {
      const provinceMatch = !provinceName || p.province_name.toLowerCase().includes(provinceName.toLowerCase());
      const nameMatch = !name || p.name.toLowerCase().includes(name.toLowerCase());
      const rateMatch = p.hourly_rate >= minRate && p.hourly_rate <= maxRate;
      return provinceMatch && nameMatch && rateMatch;
    });
  }
}

/**
 * JSON-based Car Entry Repository
 */
class JsonCarEntryRepository extends ICarEntryRepository {
  constructor(carEntryJsonPath, parkingJsonPath) {
    super();
    this.carEntryData = this.loadJson(carEntryJsonPath);
    this.parkingRepository = new JsonParkingRepository(parkingJsonPath);
  }

  loadJson(filePath) {
    try {
      const content = fs.readFileSync(filePath, 'utf-8');
      return JSON.parse(content);
    } catch (error) {
      console.error(`Error loading JSON file ${filePath}:`, error.message);
      return [];
    }
  }

  async getById(sequentialNumber) {
    return this.carEntryData.find(entry => entry.sequential_number === sequentialNumber) || null;
  }

  async getAll() {
    return this.carEntryData;
  }

  async getHourlyPriceForParking(parkingId) {
    const parking = await this.parkingRepository.getById(parkingId);
    return parking ? parking.hourly_rate : null;
  }

  async getCarsByTypeInDateRange(carType, startDate, endDate) {
    const startDt = new Date(startDate);
    const endDt = new Date(endDate);

    const entries = this.carEntryData.filter(entry => {
      const entryTime = new Date(entry.entry_date_time);
      return entryTime >= startDt && entryTime <= endDt && entry.exit_date_time !== null;
    });

    const result = [];
    for (const entry of entries) {
      const session = new CarParkingSession(entry);
      const hourlyRate = await this.getHourlyPriceForParking(entry.parking_id);
      
      result.push({
        sequential_number: entry.sequential_number,
        car_id: entry.car_id,
        car_type: carType,
        parking_id: entry.parking_id,
        entry_date_time: entry.entry_date_time,
        exit_date_time: entry.exit_date_time,
        stay_duration_minutes: session.stayDurationMinutes,
        stay_duration_hours: session.stayDurationHours,
        hourly_rate: hourlyRate,
        amount_paid: session.totalAmountDue
      });
    }

    return result;
  }

  async getCarsByProvinceInDateRange(provinceName, startDate, endDate) {
    const startDt = new Date(startDate);
    const endDt = new Date(endDate);

    // Get all parkings in province
    const parkings = await this.parkingRepository.getByProvinceName(provinceName);
    const parkingIds = parkings.map(p => p.id);

    const entries = this.carEntryData.filter(entry => {
      const entryTime = new Date(entry.entry_date_time);
      return parkingIds.includes(entry.parking_id) &&
             entryTime >= startDt &&
             entryTime <= endDt &&
             entry.exit_date_time !== null;
    });

    const result = [];
    for (const entry of entries) {
      const session = new CarParkingSession(entry);
      const parking = await this.parkingRepository.getById(entry.parking_id);

      result.push({
        sequential_number: entry.sequential_number,
        car_id: entry.car_id,
        parking_id: entry.parking_id,
        parking_name: parking.name,
        province_name: parking.province_name,
        entry_date_time: entry.entry_date_time,
        exit_date_time: entry.exit_date_time,
        stay_duration_minutes: session.stayDurationMinutes,
        stay_duration_hours: session.stayDurationHours,
        hourly_rate: parking.hourly_rate,
        amount_due: session.totalAmountDue
      });
    }

    return result;
  }
}

// ================================================================
// DATABASE IMPLEMENTATIONS
// ================================================================

/**
 * Database-based Car Repository
 */
class DatabaseCarRepository extends ICarRepository {
  constructor(pool) {
    super();
    this.pool = pool;
  }

  async getById(id) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Cars WHERE ID = ?', [id]);
      return rows.length > 0 ? rows[0] : null;
    } finally {
      connection.release();
    }
  }

  async getAll() {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Cars');
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByColor(color) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Cars WHERE color LIKE ?',
        [`%${color}%`]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByYearRange(minYear, maxYear) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Cars WHERE year >= ? AND year <= ?',
        [minYear, maxYear]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByMake(make) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Cars WHERE make LIKE ?',
        [`%${make}%`]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByType(type) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Cars WHERE type = ?',
        [type]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByColorAndYearRangeAndMakeAndType(color, minYear, maxYear, make, type) {
    let query = 'SELECT * FROM PRQ_Cars WHERE 1=1';
    const params = [];

    if (color) {
      query += ' AND color LIKE ?';
      params.push(`%${color}%`);
    }
    query += ' AND year >= ? AND year <= ?';
    params.push(minYear, maxYear);

    if (make) {
      query += ' AND make LIKE ?';
      params.push(`%${make}%`);
    }
    if (type) {
      query += ' AND type = ?';
      params.push(type);
    }

    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(query, params);
      return rows;
    } finally {
      connection.release();
    }
  }
}

/**
 * Database-based Parking Repository
 */
class DatabaseParkingRepository extends IParkingRepository {
  constructor(pool) {
    super();
    this.pool = pool;
  }

  async getById(id) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Parking WHERE ID = ?', [id]);
      return rows.length > 0 ? rows[0] : null;
    } finally {
      connection.release();
    }
  }

  async getAll() {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Parking');
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByProvinceName(provinceName) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Parking WHERE province_name LIKE ?',
        [`%${provinceName}%`]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByName(name) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Parking WHERE name LIKE ?',
        [`%${name}%`]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByHourlyRateRange(minRate, maxRate) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Parking WHERE hourly_rate >= ? AND hourly_rate <= ?',
        [minRate, maxRate]
      );
      return rows;
    } finally {
      connection.release();
    }
  }

  async getByProvinceAndNameAndHourlyRateRange(provinceName, name, minRate, maxRate) {
    let query = 'SELECT * FROM PRQ_Parking WHERE 1=1';
    const params = [];

    if (provinceName) {
      query += ' AND province_name LIKE ?';
      params.push(`%${provinceName}%`);
    }
    if (name) {
      query += ' AND name LIKE ?';
      params.push(`%${name}%`);
    }
    query += ' AND hourly_rate >= ? AND hourly_rate <= ?';
    params.push(minRate, maxRate);

    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(query, params);
      return rows;
    } finally {
      connection.release();
    }
  }
}

/**
 * Database-based Car Entry Repository
 */
class DatabaseCarEntryRepository extends ICarEntryRepository {
  constructor(pool) {
    super();
    this.pool = pool;
    this.parkingRepository = new DatabaseParkingRepository(pool);
  }

  async getById(sequentialNumber) {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute(
        'SELECT * FROM PRQ_Car_Entry WHERE sequential_number = ?',
        [sequentialNumber]
      );
      return rows.length > 0 ? rows[0] : null;
    } finally {
      connection.release();
    }
  }

  async getAll() {
    const connection = await this.pool.getConnection();
    try {
      const [rows] = await connection.execute('SELECT * FROM PRQ_Car_Entry');
      return rows;
    } finally {
      connection.release();
    }
  }

  async getHourlyPriceForParking(parkingId) {
    const parking = await this.parkingRepository.getById(parkingId);
    return parking ? parking.hourly_rate : null;
  }

  async getCarsByTypeInDateRange(carType, startDate, endDate) {
    const connection = await this.pool.getConnection();
    try {
      const query = `
        SELECT 
          ce.sequential_number,
          ce.car_id,
          c.type as car_type,
          ce.parking_id,
          ce.entry_date_time,
          ce.exit_date_time,
          TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) as stay_duration_minutes,
          ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60, 2) as stay_duration_hours,
          p.hourly_rate,
          ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60 * p.hourly_rate, 2) as amount_paid
        FROM PRQ_Car_Entry ce
        JOIN PRQ_Cars c ON ce.car_id = c.ID
        JOIN PRQ_Parking p ON ce.parking_id = p.ID
        WHERE c.type = ? 
          AND ce.entry_date_time >= ? 
          AND ce.entry_date_time <= ?
          AND ce.exit_date_time IS NOT NULL
        ORDER BY ce.entry_date_time DESC
      `;
      const [rows] = await connection.execute(query, [carType, startDate, endDate]);
      return rows;
    } finally {
      connection.release();
    }
  }

  async getCarsByProvinceInDateRange(provinceName, startDate, endDate) {
    const connection = await this.pool.getConnection();
    try {
      const query = `
        SELECT 
          ce.sequential_number,
          ce.car_id,
          ce.parking_id,
          p.name as parking_name,
          p.province_name,
          ce.entry_date_time,
          ce.exit_date_time,
          TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) as stay_duration_minutes,
          ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60, 2) as stay_duration_hours,
          p.hourly_rate,
          ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60 * p.hourly_rate, 2) as amount_due
        FROM PRQ_Car_Entry ce
        JOIN PRQ_Parking p ON ce.parking_id = p.ID
        WHERE p.province_name LIKE ? 
          AND ce.entry_date_time >= ? 
          AND ce.entry_date_time <= ?
          AND ce.exit_date_time IS NOT NULL
        ORDER BY ce.entry_date_time DESC
      `;
      const [rows] = await connection.execute(query, [`%${provinceName}%`, startDate, endDate]);
      return rows;
    } finally {
      connection.release();
    }
  }
}

// ================================================================
// EXPORTS
// ================================================================

module.exports = {
  // Interfaces
  IRepository,
  ICarRepository,
  IParkingRepository,
  ICarEntryRepository,
  
  // JSON Implementations
  JsonCarRepository,
  JsonParkingRepository,
  JsonCarEntryRepository,
  
  // Database Implementations
  DatabaseCarRepository,
  DatabaseParkingRepository,
  DatabaseCarEntryRepository
};
