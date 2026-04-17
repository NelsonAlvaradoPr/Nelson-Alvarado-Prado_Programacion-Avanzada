/**
 * Car Park REST API Server - Node.js with Express
 * Provides complete REST endpoints for all CRUD and query operations
 * 
 * Usage:
 *   npm install express mysql2 dotenv cors body-parser
 *   node api-server.js
 * 
 * Base URL: http://localhost:3000/api
 */

const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
require('dotenv').config();

const { JsonCrudService, DatabaseCrudService } = require('./crud-service');
const { 
  JsonCarRepository, JsonParkingRepository, JsonCarEntryRepository,
  DatabaseCarRepository, DatabaseParkingRepository, DatabaseCarEntryRepository
} = require('./repositories');

const app = express();
const PORT = process.env.PORT || 3000;
const USE_DATABASE = process.env.USE_DATABASE === 'true';

// Middleware
app.use(cors());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Initialize services and repositories
let crudService;
let carRepository, parkingRepository, carEntryRepository;

if (USE_DATABASE) {
  console.log('🗄️  Initializing database connections...');
  // TODO: Initialize with database pool
  // For now, default to JSON
  crudService = new JsonCrudService('.');
  carRepository = new JsonCarRepository('./prq_cars.json');
  parkingRepository = new JsonParkingRepository('./prq_parking.json');
  carEntryRepository = new JsonCarEntryRepository('./prq_car_entry.json');
} else {
  console.log('📄 Initializing JSON file storage...');
  crudService = new JsonCrudService('.');
  carRepository = new JsonCarRepository('./prq_cars.json');
  parkingRepository = new JsonParkingRepository('./prq_parking.json');
  carEntryRepository = new JsonCarEntryRepository('./prq_car_entry.json');
}

// ================================================================
// UTILITY MIDDLEWARE
// ================================================================

/**
 * Error handler middleware
 */
const errorHandler = (err, req, res, next) => {
  console.error('❌ Error:', err);
  res.status(err.status || 500).json({
    success: false,
    error: err.message || 'Internal server error',
    timestamp: new Date().toISOString()
  });
};

/**
 * Validate request body
 */
const validateRequestBody = (req, res, next) => {
  if (!req.body || Object.keys(req.body).length === 0) {
    return res.status(400).json({
      success: false,
      error: 'Request body cannot be empty'
    });
  }
  next();
};

// ================================================================
// API ENDPOINTS - CARS
// ================================================================

// ---- CAR CREATE ----
/**
 * POST /api/cars
 * Insert a new car
 * Body: { color, year, make, type }
 */
app.post('/api/cars', validateRequestBody, (req, res, next) => {
  try {
    const result = crudService.insertCar(req.body);
    if (result.success) {
      res.status(201).json(result);
    } else {
      res.status(400).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (ALL) ----
/**
 * GET /api/cars
 * Retrieve all cars
 */
app.get('/api/cars', async (req, res, next) => {
  try {
    const cars = await carRepository.getAll();
    res.json({
      success: true,
      data: cars,
      count: cars.length
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (BY ID) ----
/**
 * GET /api/cars/:id
 * Retrieve a specific car by ID
 */
app.get('/api/cars/:id', async (req, res, next) => {
  try {
    const car = await carRepository.getById(parseInt(req.params.id));
    if (!car) {
      return res.status(404).json({
        success: false,
        error: `Car with ID ${req.params.id} not found`
      });
    }
    res.json({
      success: true,
      data: car
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (BY COLOR) ----
/**
 * GET /api/cars/filter/color/:color
 * Retrieve cars by color
 */
app.get('/api/cars/filter/color/:color', async (req, res, next) => {
  try {
    const cars = await carRepository.getByColor(req.params.color);
    res.json({
      success: true,
      data: cars,
      count: cars.length,
      filter: { color: req.params.color }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (BY MAKE) ----
/**
 * GET /api/cars/filter/make/:make
 * Retrieve cars by make
 */
app.get('/api/cars/filter/make/:make', async (req, res, next) => {
  try {
    const cars = await carRepository.getByMake(req.params.make);
    res.json({
      success: true,
      data: cars,
      count: cars.length,
      filter: { make: req.params.make }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (BY TYPE) ----
/**
 * GET /api/cars/filter/type/:type
 * Retrieve cars by type (sedan, 4x4, motorcycle)
 */
app.get('/api/cars/filter/type/:type', async (req, res, next) => {
  try {
    const cars = await carRepository.getByType(req.params.type);
    res.json({
      success: true,
      data: cars,
      count: cars.length,
      filter: { type: req.params.type }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (BY YEAR RANGE) ----
/**
 * GET /api/cars/filter/year-range?min=YYYY&max=YYYY
 * Retrieve cars by year range
 */
app.get('/api/cars/filter/year-range', async (req, res, next) => {
  try {
    const minYear = parseInt(req.query.min) || 1900;
    const maxYear = parseInt(req.query.max) || new Date().getFullYear();
    const cars = await carRepository.getByYearRange(minYear, maxYear);
    res.json({
      success: true,
      data: cars,
      count: cars.length,
      filter: { minYear, maxYear }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR READ (ADVANCED FILTER) ----
/**
 * GET /api/cars/filter/advanced?color=&minYear=&maxYear=&make=&type=
 * Retrieve cars with multiple filters
 */
app.get('/api/cars/filter/advanced', async (req, res, next) => {
  try {
    const { color, minYear, maxYear, make, type } = req.query;
    const cars = await carRepository.getByColorAndYearRangeAndMakeAndType(
      color || null,
      minYear ? parseInt(minYear) : 1900,
      maxYear ? parseInt(maxYear) : new Date().getFullYear(),
      make || null,
      type || null
    );
    res.json({
      success: true,
      data: cars,
      count: cars.length,
      filters: { color, minYear, maxYear, make, type }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR UPDATE ----
/**
 * PUT /api/cars/:id
 * Update a car
 * Body: { color, year, make, type }
 */
app.put('/api/cars/:id', validateRequestBody, (req, res, next) => {
  try {
    const result = crudService.updateCar(parseInt(req.params.id), req.body);
    if (result.success) {
      res.json(result);
    } else {
      res.status(404).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ---- CAR DELETE ----
/**
 * DELETE /api/cars/:id
 * Delete a car
 */
app.delete('/api/cars/:id', (req, res, next) => {
  try {
    const result = crudService.deleteCar(parseInt(req.params.id));
    if (result.success) {
      res.json(result);
    } else {
      res.status(404).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ================================================================
// API ENDPOINTS - PARKING
// ================================================================

// ---- PARKING CREATE ----
/**
 * POST /api/parking
 * Insert a new parking facility
 * Body: { province_name, name, hourly_rate }
 */
app.post('/api/parking', validateRequestBody, (req, res, next) => {
  try {
    const result = crudService.insertParking(req.body);
    if (result.success) {
      res.status(201).json(result);
    } else {
      res.status(400).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ---- PARKING READ (ALL) ----
/**
 * GET /api/parking
 * Retrieve all parking facilities
 */
app.get('/api/parking', async (req, res, next) => {
  try {
    const parking = await parkingRepository.getAll();
    res.json({
      success: true,
      data: parking,
      count: parking.length
    });
  } catch (err) {
    next(err);
  }
});

// ---- PARKING READ (BY ID) ----
/**
 * GET /api/parking/:id
 * Retrieve a specific parking facility by ID
 */
app.get('/api/parking/:id', async (req, res, next) => {
  try {
    const parking = await parkingRepository.getById(parseInt(req.params.id));
    if (!parking) {
      return res.status(404).json({
        success: false,
        error: `Parking with ID ${req.params.id} not found`
      });
    }
    res.json({
      success: true,
      data: parking
    });
  } catch (err) {
    next(err);
  }
});

// ---- PARKING READ (BY PROVINCE) ----
/**
 * GET /api/parking/filter/province/:province
 * Retrieve parking facilities by province
 */
app.get('/api/parking/filter/province/:province', async (req, res, next) => {
  try {
    const parking = await parkingRepository.getByProvinceName(req.params.province);
    res.json({
      success: true,
      data: parking,
      count: parking.length,
      filter: { province: req.params.province }
    });
  } catch (err) {
    next(err);
  }
});

// ---- PARKING READ (BY NAME) ----
/**
 * GET /api/parking/filter/name/:name
 * Retrieve parking facilities by name
 */
app.get('/api/parking/filter/name/:name', async (req, res, next) => {
  try {
    const parking = await parkingRepository.getByName(req.params.name);
    res.json({
      success: true,
      data: parking,
      count: parking.length,
      filter: { name: req.params.name }
    });
  } catch (err) {
    next(err);
  }
});

// ---- PARKING READ (BY HOURLY RATE RANGE) ----
/**
 * GET /api/parking/filter/rate-range?min=&max=
 * Retrieve parking facilities by hourly rate range
 */
app.get('/api/parking/filter/rate-range', async (req, res, next) => {
  try {
    const minRate = parseFloat(req.query.min) || 0;
    const maxRate = parseFloat(req.query.max) || 9999;
    const parking = await parkingRepository.getByHourlyRateRange(minRate, maxRate);
    res.json({
      success: true,
      data: parking,
      count: parking.length,
      filter: { minRate, maxRate }
    });
  } catch (err) {
    next(err);
  }
});

// ---- PARKING READ (ADVANCED FILTER) ----
/**
 * GET /api/parking/filter/advanced?province=&name=&minRate=&maxRate=
 * Retrieve parking facilities with multiple filters
 */
app.get('/api/parking/filter/advanced', async (req, res, next) => {
  try {
    const { province, name, minRate, maxRate } = req.query;
    const parking = await parkingRepository.getByProvinceAndNameAndHourlyRateRange(
      province || null,
      name || null,
      minRate ? parseFloat(minRate) : 0,
      maxRate ? parseFloat(maxRate) : 9999
    );
    res.json({
      success: true,
      data: parking,
      count: parking.length,
      filters: { province, name, minRate, maxRate }
    });
  } catch (err) {
    next(err);
  }
});

// ---- PARKING UPDATE ----
/**
 * PUT /api/parking/:id
 * Update a parking facility
 * Body: { province_name, name, hourly_rate }
 */
app.put('/api/parking/:id', validateRequestBody, (req, res, next) => {
  try {
    const result = crudService.updateParking(parseInt(req.params.id), req.body);
    if (result.success) {
      res.json(result);
    } else {
      res.status(404).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ---- PARKING DELETE ----
/**
 * DELETE /api/parking/:id
 * Delete a parking facility
 */
app.delete('/api/parking/:id', (req, res, next) => {
  try {
    const result = crudService.deleteParking(parseInt(req.params.id));
    if (result.success) {
      res.json(result);
    } else {
      res.status(404).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ================================================================
// API ENDPOINTS - CAR ENTRY
// ================================================================

// ---- CAR ENTRY CREATE ----
/**
 * POST /api/car-entry
 * Insert a new car entry
 * Body: { parking_id, car_id, entry_date_time, exit_date_time (optional) }
 */
app.post('/api/car-entry', validateRequestBody, (req, res, next) => {
  try {
    const result = crudService.insertCarEntry(req.body);
    if (result.success) {
      res.status(201).json(result);
    } else {
      res.status(400).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY READ (ALL) ----
/**
 * GET /api/car-entry
 * Retrieve all car entries
 */
app.get('/api/car-entry', async (req, res, next) => {
  try {
    const entries = await carEntryRepository.getAll();
    res.json({
      success: true,
      data: entries,
      count: entries.length
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY READ (BY ID) ----
/**
 * GET /api/car-entry/:id
 * Retrieve a specific car entry by sequential number
 */
app.get('/api/car-entry/:id', async (req, res, next) => {
  try {
    const entry = await carEntryRepository.getById(parseInt(req.params.id));
    if (!entry) {
      return res.status(404).json({
        success: false,
        error: `Car entry with ID ${req.params.id} not found`
      });
    }
    res.json({
      success: true,
      data: entry
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY READ (BY PARKING ID) ----
/**
 * GET /api/car-entry/filter/parking/:parkingId
 * Retrieve car entries for a specific parking facility
 */
app.get('/api/car-entry/filter/parking/:parkingId', async (req, res, next) => {
  try {
    const entries = await carEntryRepository.getCarsByProvinceInDateRange(
      null,
      req.params.parkingId
    );
    res.json({
      success: true,
      data: entries,
      count: entries.length,
      filter: { parkingId: req.params.parkingId }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY READ (BY CAR TYPE IN DATE RANGE) ----
/**
 * GET /api/car-entry/filter/car-type/:type?start=&end=
 * Retrieve car entries by car type in a date range
 */
app.get('/api/car-entry/filter/car-type/:type', async (req, res, next) => {
  try {
    const { start, end } = req.query;
    const entries = await carEntryRepository.getCarsByTypeInDateRange(
      req.params.type,
      start || '2020-01-01',
      end || new Date().toISOString().split('T')[0]
    );
    res.json({
      success: true,
      data: entries,
      count: entries.length,
      filters: { carType: req.params.type, startDate: start, endDate: end }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY READ (BY PROVINCE IN DATE RANGE) ----
/**
 * GET /api/car-entry/filter/province/:province?start=&end=
 * Retrieve car entries by province in a date range
 */
app.get('/api/car-entry/filter/province/:province', async (req, res, next) => {
  try {
    const { start, end } = req.query;
    const entries = await carEntryRepository.getCarsByProvinceInDateRange(
      req.params.province,
      start || '2020-01-01',
      end || new Date().toISOString().split('T')[0]
    );
    res.json({
      success: true,
      data: entries,
      count: entries.length,
      filters: { province: req.params.province, startDate: start, endDate: end }
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY READ (HOURLY PRICE FOR PARKING) ----
/**
 * GET /api/car-entry/price/:parkingId
 * Get hourly price for a specific parking facility
 */
app.get('/api/car-entry/price/:parkingId', async (req, res, next) => {
  try {
    const price = await carEntryRepository.getHourlyPriceForParking(
      parseInt(req.params.parkingId)
    );
    res.json({
      success: true,
      data: { hourlyPrice: price },
      parkingId: req.params.parkingId
    });
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY UPDATE ----
/**
 * PUT /api/car-entry/:id
 * Update a car entry
 * Body: { parking_id, car_id, entry_date_time, exit_date_time }
 */
app.put('/api/car-entry/:id', validateRequestBody, (req, res, next) => {
  try {
    const result = crudService.updateCarEntry(parseInt(req.params.id), req.body);
    if (result.success) {
      res.json(result);
    } else {
      res.status(404).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ---- CAR ENTRY DELETE ----
/**
 * DELETE /api/car-entry/:id
 * Delete a car entry
 */
app.delete('/api/car-entry/:id', (req, res, next) => {
  try {
    const result = crudService.deleteCarEntry(parseInt(req.params.id));
    if (result.success) {
      res.json(result);
    } else {
      res.status(404).json(result);
    }
  } catch (err) {
    next(err);
  }
});

// ================================================================
// HEALTH & INFO ENDPOINTS
// ================================================================

/**
 * GET /api/health
 * Server health check
 */
app.get('/api/health', (req, res) => {
  res.json({
    status: 'healthy',
    timestamp: new Date().toISOString(),
    storage: USE_DATABASE ? 'database' : 'json',
    version: '1.0.0'
  });
});

/**
 * GET /api/endpoints
 * List all available endpoints
 */
app.get('/api/endpoints', (req, res) => {
  const endpoints = {
    cars: {
      create: 'POST /api/cars',
      getAll: 'GET /api/cars',
      getById: 'GET /api/cars/:id',
      getByColor: 'GET /api/cars/filter/color/:color',
      getByMake: 'GET /api/cars/filter/make/:make',
      getByType: 'GET /api/cars/filter/type/:type',
      getByYearRange: 'GET /api/cars/filter/year-range?min=YYYY&max=YYYY',
      advancedFilter: 'GET /api/cars/filter/advanced?color=&minYear=&maxYear=&make=&type=',
      update: 'PUT /api/cars/:id',
      delete: 'DELETE /api/cars/:id'
    },
    parking: {
      create: 'POST /api/parking',
      getAll: 'GET /api/parking',
      getById: 'GET /api/parking/:id',
      getByProvince: 'GET /api/parking/filter/province/:province',
      getByName: 'GET /api/parking/filter/name/:name',
      getByRateRange: 'GET /api/parking/filter/rate-range?min=&max=',
      advancedFilter: 'GET /api/parking/filter/advanced?province=&name=&minRate=&maxRate=',
      update: 'PUT /api/parking/:id',
      delete: 'DELETE /api/parking/:id'
    },
    carEntry: {
      create: 'POST /api/car-entry',
      getAll: 'GET /api/car-entry',
      getById: 'GET /api/car-entry/:id',
      getByParking: 'GET /api/car-entry/filter/parking/:parkingId',
      getByCarType: 'GET /api/car-entry/filter/car-type/:type?start=&end=',
      getByProvince: 'GET /api/car-entry/filter/province/:province?start=&end=',
      getHourlyPrice: 'GET /api/car-entry/price/:parkingId',
      update: 'PUT /api/car-entry/:id',
      delete: 'DELETE /api/car-entry/:id'
    },
    system: {
      health: 'GET /api/health',
      endpoints: 'GET /api/endpoints'
    }
  };
  res.json(endpoints);
});

// 404 Handler
app.use((req, res) => {
  res.status(404).json({
    success: false,
    error: 'Endpoint not found',
    path: req.path,
    method: req.method
  });
});

// Error handling middleware
app.use(errorHandler);

// ================================================================
// START SERVER
// ================================================================

app.listen(PORT, () => {
  console.log(`
╔════════════════════════════════════════╗
║   Car Park REST API Server Running     ║
╠════════════════════════════════════════╣
║ 🚗 Base URL: http://localhost:${PORT}/api   ║
║ 📊 Storage: ${USE_DATABASE ? 'Database' : 'JSON Files'}            ║
║ 📝 API Docs: http://localhost:${PORT}/api/endpoints ║
║ 💚 Health: http://localhost:${PORT}/api/health      ║
╚════════════════════════════════════════╝
  `);
});

module.exports = app;
