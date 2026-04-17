/**
 * API Usage Examples - JavaScript/Node.js
 * Demonstrates how to use all REST API endpoints
 */

const fetch = require('node-fetch');

const BASE_URL = 'http://localhost:3000/api';

// ================================================================
// UTILITY FUNCTIONS
// ================================================================

/**
 * Pretty print JSON responses
 */
function printResponse(title, response) {
  console.log(`\n${'='.repeat(50)}`);
  console.log(`📋 ${title}`);
  console.log('='.repeat(50));
  console.log(JSON.stringify(response, null, 2));
}

/**
 * Make GET request
 */
async function get(endpoint) {
  try {
    const response = await fetch(`${BASE_URL}${endpoint}`);
    return await response.json();
  } catch (error) {
    console.error('GET Error:', error);
    return null;
  }
}

/**
 * Make POST request
 */
async function post(endpoint, data) {
  try {
    const response = await fetch(`${BASE_URL}${endpoint}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    return await response.json();
  } catch (error) {
    console.error('POST Error:', error);
    return null;
  }
}

/**
 * Make PUT request
 */
async function put(endpoint, data) {
  try {
    const response = await fetch(`${BASE_URL}${endpoint}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    return await response.json();
  } catch (error) {
    console.error('PUT Error:', error);
    return null;
  }
}

/**
 * Make DELETE request
 */
async function del(endpoint) {
  try {
    const response = await fetch(`${BASE_URL}${endpoint}`, {
      method: 'DELETE'
    });
    return await response.json();
  } catch (error) {
    console.error('DELETE Error:', error);
    return null;
  }
}

// ================================================================
// CAR OPERATIONS EXAMPLES
// ================================================================

/**
 * Example: Create a new car
 */
async function exampleCreateCar() {
  console.log('\n🚗 CREATING A NEW CAR');
  const newCar = {
    color: 'Silver',
    year: 2024,
    make: 'Audi A4',
    type: 'sedan'
  };
  const result = await post('/cars', newCar);
  printResponse('Create Car Response', result);
  return result.data?.id;
}

/**
 * Example: Get all cars
 */
async function exampleGetAllCars() {
  console.log('\n🚗 GETTING ALL CARS');
  const result = await get('/cars');
  printResponse('Get All Cars Response', result);
  return result.data;
}

/**
 * Example: Get car by ID
 */
async function exampleGetCarById(carId) {
  console.log(`\n🚗 GETTING CAR BY ID: ${carId}`);
  const result = await get(`/cars/${carId}`);
  printResponse('Get Car by ID Response', result);
}

/**
 * Example: Get cars by color
 */
async function exampleGetCarsByColor(color) {
  console.log(`\n🚗 GETTING CARS BY COLOR: ${color}`);
  const result = await get(`/cars/filter/color/${color}`);
  printResponse('Get Cars by Color Response', result);
}

/**
 * Example: Get cars by make
 */
async function exampleGetCarsByMake(make) {
  console.log(`\n🚗 GETTING CARS BY MAKE: ${make}`);
  const result = await get(`/cars/filter/make/${encodeURIComponent(make)}`);
  printResponse('Get Cars by Make Response', result);
}

/**
 * Example: Get cars by type
 */
async function exampleGetCarsByType(type) {
  console.log(`\n🚗 GETTING CARS BY TYPE: ${type}`);
  const result = await get(`/cars/filter/type/${type}`);
  printResponse('Get Cars by Type Response', result);
}

/**
 * Example: Get cars by year range
 */
async function exampleGetCarsByYearRange(minYear, maxYear) {
  console.log(`\n🚗 GETTING CARS BY YEAR RANGE: ${minYear}-${maxYear}`);
  const result = await get(`/cars/filter/year-range?min=${minYear}&max=${maxYear}`);
  printResponse('Get Cars by Year Range Response', result);
}

/**
 * Example: Get cars with advanced filter
 */
async function exampleGetCarsAdvancedFilter() {
  console.log('\n🚗 GETTING CARS WITH ADVANCED FILTER');
  const params = new URLSearchParams({
    color: 'Red',
    minYear: 2020,
    maxYear: 2025,
    type: 'sedan'
  });
  const result = await get(`/cars/filter/advanced?${params}`);
  printResponse('Get Cars Advanced Filter Response', result);
}

/**
 * Example: Update a car
 */
async function exampleUpdateCar(carId) {
  console.log(`\n🚗 UPDATING CAR: ${carId}`);
  const updateData = {
    color: 'Gold',
    year: 2024,
    make: 'Audi A6',
    type: 'sedan'
  };
  const result = await put(`/cars/${carId}`, updateData);
  printResponse('Update Car Response', result);
}

/**
 * Example: Delete a car
 */
async function exampleDeleteCar(carId) {
  console.log(`\n🚗 DELETING CAR: ${carId}`);
  const result = await del(`/cars/${carId}`);
  printResponse('Delete Car Response', result);
}

// ================================================================
// PARKING OPERATIONS EXAMPLES
// ================================================================

/**
 * Example: Create a new parking facility
 */
async function exampleCreateParking() {
  console.log('\n🅿️  CREATING A NEW PARKING FACILITY');
  const newParking = {
    province_name: 'Murcia',
    name: 'Parking Aeropuerto',
    hourly_rate: 3.75
  };
  const result = await post('/parking', newParking);
  printResponse('Create Parking Response', result);
  return result.data?.id;
}

/**
 * Example: Get all parking facilities
 */
async function exampleGetAllParking() {
  console.log('\n🅿️  GETTING ALL PARKING FACILITIES');
  const result = await get('/parking');
  printResponse('Get All Parking Response', result);
  return result.data;
}

/**
 * Example: Get parking by ID
 */
async function exampleGetParkingById(parkingId) {
  console.log(`\n🅿️  GETTING PARKING BY ID: ${parkingId}`);
  const result = await get(`/parking/${parkingId}`);
  printResponse('Get Parking by ID Response', result);
}

/**
 * Example: Get parking by province
 */
async function exampleGetParkingByProvince(province) {
  console.log(`\n🅿️  GETTING PARKING BY PROVINCE: ${province}`);
  const result = await get(`/parking/filter/province/${province}`);
  printResponse('Get Parking by Province Response', result);
}

/**
 * Example: Get parking by name
 */
async function exampleGetParkingByName(name) {
  console.log(`\n🅿️  GETTING PARKING BY NAME: ${name}`);
  const result = await get(`/parking/filter/name/${encodeURIComponent(name)}`);
  printResponse('Get Parking by Name Response', result);
}

/**
 * Example: Get parking by rate range
 */
async function exampleGetParkingByRateRange(minRate, maxRate) {
  console.log(`\n🅿️  GETTING PARKING BY RATE RANGE: €${minRate}-€${maxRate}`);
  const result = await get(`/parking/filter/rate-range?min=${minRate}&max=${maxRate}`);
  printResponse('Get Parking by Rate Range Response', result);
}

/**
 * Example: Get parking with advanced filter
 */
async function exampleGetParkingAdvancedFilter() {
  console.log('\n🅿️  GETTING PARKING WITH ADVANCED FILTER');
  const params = new URLSearchParams({
    province: 'Madrid',
    minRate: 3.0,
    maxRate: 4.5
  });
  const result = await get(`/parking/filter/advanced?${params}`);
  printResponse('Get Parking Advanced Filter Response', result);
}

/**
 * Example: Update a parking facility
 */
async function exampleUpdateParking(parkingId) {
  console.log(`\n🅿️  UPDATING PARKING: ${parkingId}`);
  const updateData = {
    province_name: 'Murcia',
    name: 'Parking Aeropuerto Premium',
    hourly_rate: 4.50
  };
  const result = await put(`/parking/${parkingId}`, updateData);
  printResponse('Update Parking Response', result);
}

/**
 * Example: Delete a parking facility
 */
async function exampleDeleteParking(parkingId) {
  console.log(`\n🅿️  DELETING PARKING: ${parkingId}`);
  const result = await del(`/parking/${parkingId}`);
  printResponse('Delete Parking Response', result);
}

// ================================================================
// CAR ENTRY OPERATIONS EXAMPLES
// ================================================================

/**
 * Example: Create a new car entry
 */
async function exampleCreateCarEntry() {
  console.log('\n🚙 CREATING A NEW CAR ENTRY');
  const newEntry = {
    parking_id: 1,
    car_id: 1,
    entry_date_time: '2026-04-17T10:00:00',
    exit_date_time: null
  };
  const result = await post('/car-entry', newEntry);
  printResponse('Create Car Entry Response', result);
  return result.data?.sequential_number;
}

/**
 * Example: Get all car entries
 */
async function exampleGetAllCarEntries() {
  console.log('\n🚙 GETTING ALL CAR ENTRIES');
  const result = await get('/car-entry');
  printResponse('Get All Car Entries Response', result);
  return result.data;
}

/**
 * Example: Get car entry by ID
 */
async function exampleGetCarEntryById(entryId) {
  console.log(`\n🚙 GETTING CAR ENTRY BY ID: ${entryId}`);
  const result = await get(`/car-entry/${entryId}`);
  printResponse('Get Car Entry by ID Response', result);
}

/**
 * Example: Get car entries by parking
 */
async function exampleGetEntriesByParking(parkingId) {
  console.log(`\n🚙 GETTING ENTRIES FOR PARKING: ${parkingId}`);
  const result = await get(`/car-entry/filter/parking/${parkingId}`);
  printResponse('Get Entries by Parking Response', result);
}

/**
 * Example: Get car entries by car type in date range
 */
async function exampleGetEntriesByCarType(carType) {
  console.log(`\n🚙 GETTING ENTRIES FOR CAR TYPE: ${carType}`);
  const params = new URLSearchParams({
    start: '2026-04-10',
    end: '2026-04-20'
  });
  const result = await get(`/car-entry/filter/car-type/${carType}?${params}`);
  printResponse('Get Entries by Car Type Response', result);
}

/**
 * Example: Get car entries by province in date range
 */
async function exampleGetEntriesByProvince(province) {
  console.log(`\n🚙 GETTING ENTRIES FOR PROVINCE: ${province}`);
  const params = new URLSearchParams({
    start: '2026-04-10',
    end: '2026-04-20'
  });
  const result = await get(`/car-entry/filter/province/${province}?${params}`);
  printResponse('Get Entries by Province Response', result);
}

/**
 * Example: Get hourly price for parking
 */
async function exampleGetHourlyPrice(parkingId) {
  console.log(`\n🚙 GETTING HOURLY PRICE FOR PARKING: ${parkingId}`);
  const result = await get(`/car-entry/price/${parkingId}`);
  printResponse('Get Hourly Price Response', result);
}

/**
 * Example: Update a car entry
 */
async function exampleUpdateCarEntry(entryId) {
  console.log(`\n🚙 UPDATING CAR ENTRY: ${entryId}`);
  const updateData = {
    parking_id: 1,
    car_id: 1,
    entry_date_time: '2026-04-17T10:00:00',
    exit_date_time: '2026-04-17T13:00:00'
  };
  const result = await put(`/car-entry/${entryId}`, updateData);
  printResponse('Update Car Entry Response', result);
}

/**
 * Example: Delete a car entry
 */
async function exampleDeleteCarEntry(entryId) {
  console.log(`\n🚙 DELETING CAR ENTRY: ${entryId}`);
  const result = await del(`/car-entry/${entryId}`);
  printResponse('Delete Car Entry Response', result);
}

// ================================================================
// SYSTEM ENDPOINTS EXAMPLES
// ================================================================

/**
 * Example: Health check
 */
async function exampleHealthCheck() {
  console.log('\n💚 HEALTH CHECK');
  const result = await get('/health');
  printResponse('Health Check Response', result);
}

/**
 * Example: Get all endpoints
 */
async function exampleGetEndpoints() {
  console.log('\n📋 GET ALL ENDPOINTS');
  const result = await get('/endpoints');
  printResponse('Endpoints List', result);
}

// ================================================================
// MAIN EXECUTION
// ================================================================

/**
 * Run all examples
 */
async function runAllExamples() {
  try {
    console.log(`
╔════════════════════════════════════════╗
║   Car Park REST API - JavaScript       ║
║           Usage Examples               ║
╚════════════════════════════════════════╝
    `);

    // System endpoints
    await exampleHealthCheck();
    await exampleGetEndpoints();

    // Car operations
    const newCarId = await exampleCreateCar();
    await exampleGetAllCars();
    if (newCarId) await exampleGetCarById(newCarId);
    await exampleGetCarsByColor('Red');
    await exampleGetCarsByMake('Toyota Corolla');
    await exampleGetCarsByType('sedan');
    await exampleGetCarsByYearRange(2020, 2025);
    await exampleGetCarsAdvancedFilter();
    if (newCarId) await exampleUpdateCar(newCarId);

    // Parking operations
    const newParkingId = await exampleCreateParking();
    await exampleGetAllParking();
    if (newParkingId) await exampleGetParkingById(newParkingId);
    await exampleGetParkingByProvince('Madrid');
    await exampleGetParkingByRateRange(2.0, 4.0);
    await exampleGetParkingAdvancedFilter();
    if (newParkingId) await exampleUpdateParking(newParkingId);

    // Car entry operations
    const newEntryId = await exampleCreateCarEntry();
    await exampleGetAllCarEntries();
    if (newEntryId) await exampleGetCarEntryById(newEntryId);
    await exampleGetEntriesByParking(1);
    await exampleGetEntriesByCarType('sedan');
    await exampleGetEntriesByProvince('Madrid');
    await exampleGetHourlyPrice(1);
    if (newEntryId) await exampleUpdateCarEntry(newEntryId);

    console.log(`
╔════════════════════════════════════════╗
║        Examples Completed! ✅           ║
╚════════════════════════════════════════╝
    `);
  } catch (error) {
    console.error('❌ Error running examples:', error);
  }
}

// Run examples if this file is executed directly
if (require.main === module) {
  runAllExamples();
}

module.exports = {
  get, post, put, del,
  exampleCreateCar, exampleGetAllCars, exampleGetCarById,
  exampleCreateParking, exampleGetAllParking,
  exampleCreateCarEntry, exampleGetAllCarEntries,
  runAllExamples
};
