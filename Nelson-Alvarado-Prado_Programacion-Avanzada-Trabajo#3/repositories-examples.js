/**
 * Repository Usage Examples - Node.js
 * Demonstrates how to use the repository pattern with JSON and Database sources
 */

const {
  JsonCarRepository,
  JsonParkingRepository,
  JsonCarEntryRepository,
  DatabaseCarRepository,
  DatabaseParkingRepository,
  DatabaseCarEntryRepository
} = require('./repositories');

const path = require('path');

// ================================================================
// JSON EXAMPLES
// ================================================================

async function demonstrateJsonRepositories() {
  console.log('\n' + '='.repeat(80));
  console.log('JSON REPOSITORY EXAMPLES');
  console.log('='.repeat(80) + '\n');

  // Initialize JSON repositories
  const carRepo = new JsonCarRepository(path.join(__dirname, 'prq_cars.json'));
  const parkingRepo = new JsonParkingRepository(path.join(__dirname, 'prq_parking.json'));
  const carEntryRepo = new JsonCarEntryRepository(
    path.join(__dirname, 'prq_car_entry.json'),
    path.join(__dirname, 'prq_parking.json')
  );

  // ================================================================
  // CAR QUERIES
  // ================================================================
  console.log('--- CAR QUERIES ---\n');

  // Get all cars
  const allCars = await carRepo.getAll();
  console.log(`1. All cars (${allCars.length} total):`);
  allCars.forEach(car => {
    console.log(`   • ${car.color} ${car.make} (${car.year}) - ${car.type}`);
  });

  // Get cars by color
  const redCars = await carRepo.getByColor('Red');
  console.log(`\n2. Red cars (${redCars.length} found):`);
  redCars.forEach(car => {
    console.log(`   • ${car.make} (${car.year})`);
  });

  // Get cars by year range
  const recentCars = await carRepo.getByYearRange(2020, 2022);
  console.log(`\n3. Cars from 2020-2022 (${recentCars.length} found):`);
  recentCars.forEach(car => {
    console.log(`   • ${car.make} (${car.year})`);
  });

  // Get cars by type
  const sedans = await carRepo.getByType('sedan');
  console.log(`\n4. Sedans (${sedans.length} found):`);
  sedans.forEach(car => {
    console.log(`   • ${car.color} ${car.make}`);
  });

  // Combined query
  const filtered = await carRepo.getByColorAndYearRangeAndMakeAndType(
    null,      // No color filter
    2018, 2022, // Year range
    null,      // No make filter
    '4x4'      // Type filter
  );
  console.log(`\n5. 4x4 vehicles from 2018-2022 (${filtered.length} found):`);
  filtered.forEach(car => {
    console.log(`   • ${car.color} ${car.make} (${car.year})`);
  });

  // ================================================================
  // PARKING QUERIES
  // ================================================================
  console.log('\n--- PARKING QUERIES ---\n');

  // Get all parkings
  const allParkings = await parkingRepo.getAll();
  console.log(`1. All parking facilities (${allParkings.length} total):`);
  allParkings.forEach(p => {
    console.log(`   • ${p.name} (${p.province_name}) - €${p.hourly_rate}/hour`);
  });

  // Get parkings by province
  const madridParkings = await parkingRepo.getByProvinceName('Madrid');
  console.log(`\n2. Madrid parkings (${madridParkings.length} found):`);
  madridParkings.forEach(p => {
    console.log(`   • ${p.name} - €${p.hourly_rate}/hour`);
  });

  // Get parkings by rate range
  const cheapParkings = await parkingRepo.getByHourlyRateRange(2.5, 3.0);
  console.log(`\n3. Parkings €2.50-€3.00/hour (${cheapParkings.length} found):`);
  cheapParkings.forEach(p => {
    console.log(`   • ${p.name} - €${p.hourly_rate}/hour`);
  });

  // Combined query
  const filteredParkings = await parkingRepo.getByProvinceAndNameAndHourlyRateRange(
    'Barcelona',  // Province
    null,         // No name filter
    2.0, 3.5      // Rate range
  );
  console.log(`\n4. Barcelona parkings €2.00-€3.50/hour (${filteredParkings.length} found):`);
  filteredParkings.forEach(p => {
    console.log(`   • ${p.name} - €${p.hourly_rate}/hour`);
  });

  // ================================================================
  // CAR ENTRY QUERIES
  // ================================================================
  console.log('\n--- CAR ENTRY QUERIES ---\n');

  // Get hourly price for parking
  const madridRate = await carEntryRepo.getHourlyPriceForParking(1);
  console.log(`1. Hourly rate for Parking ID 1: €${madridRate}`);

  // Get cars by type in date range
  console.log(`\n2. Sedan entries from 2026-04-16 to 2026-04-17:`);
  const sedanEntries = await carEntryRepo.getCarsByTypeInDateRange(
    'sedan',
    '2026-04-16',
    '2026-04-17'
  );
  console.log(`   Found ${sedanEntries.length} sedan sessions:`);
  sedanEntries.forEach(entry => {
    console.log(
      `   • Entry #${entry.sequential_number}: ${entry.entry_date_time} → ${entry.exit_date_time} ` +
      `(${entry.stay_duration_hours}h) - €${entry.amount_paid}`
    );
  });

  // Get cars by province in date range
  console.log(`\n3. Cars in Madrid from 2026-04-16 to 2026-04-17:`);
  const madridEntries = await carEntryRepo.getCarsByProvinceInDateRange(
    'Madrid',
    '2026-04-16',
    '2026-04-17'
  );
  console.log(`   Found ${madridEntries.length} sessions:`);
  madridEntries.forEach(entry => {
    console.log(
      `   • Entry #${entry.sequential_number} at ${entry.parking_name}: ` +
      `${entry.entry_date_time} → ${entry.exit_date_time} - €${entry.amount_due}`
    );
  });
}

// ================================================================
// DATABASE EXAMPLES
// ================================================================

async function demonstrateDatabaseRepositories() {
  console.log('\n' + '='.repeat(80));
  console.log('DATABASE REPOSITORY EXAMPLES');
  console.log('='.repeat(80) + '\n');

  // Create database connection pool
  const mysql = require('mysql2/promise');
  require('dotenv').config();

  const pool = mysql.createPool({
    host: process.env.DB_HOST,
    port: process.env.DB_PORT,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    ssl: 'REQUIRED',
    waitForConnections: true,
    connectionLimit: 5,
    queueLimit: 0
  });

  try {
    // Initialize database repositories
    const carRepo = new DatabaseCarRepository(pool);
    const parkingRepo = new DatabaseParkingRepository(pool);
    const carEntryRepo = new DatabaseCarEntryRepository(pool);

    // Test connection
    const testConnection = await pool.getConnection();
    testConnection.release();
    console.log('✓ Database connection established\n');

    // ================================================================
    // CAR QUERIES
    // ================================================================
    console.log('--- CAR QUERIES ---\n');

    // Get all cars
    const allCars = await carRepo.getAll();
    console.log(`1. All cars (${allCars.length} total)`);

    // Get cars by make
    const toyotas = await carRepo.getByMake('Toyota');
    console.log(`2. Toyota cars (${toyotas.length} found)`);

    // Get cars by year range
    const recentCars = await carRepo.getByYearRange(2020, 2022);
    console.log(`3. Cars from 2020-2022 (${recentCars.length} found)`);

    // Combined query
    const filtered = await carRepo.getByColorAndYearRangeAndMakeAndType(
      null, 2018, 2022, null, 'sedan'
    );
    console.log(`4. Sedans from 2018-2022 (${filtered.length} found)`);

    // ================================================================
    // PARKING QUERIES
    // ================================================================
    console.log('\n--- PARKING QUERIES ---\n');

    // Get all parkings
    const allParkings = await parkingRepo.getAll();
    console.log(`1. All parking facilities (${allParkings.length} total)`);

    // Get parkings by province
    const madridParkings = await parkingRepo.getByProvinceName('Madrid');
    console.log(`2. Madrid parkings (${madridParkings.length} found)`);

    // Get parkings by rate range
    const ratedParkings = await parkingRepo.getByHourlyRateRange(2.0, 3.5);
    console.log(`3. Parkings €2.00-€3.50/hour (${ratedParkings.length} found)`);

    // ================================================================
    // CAR ENTRY QUERIES
    // ================================================================
    console.log('\n--- CAR ENTRY QUERIES ---\n');

    // Get hourly price
    const rate = await carEntryRepo.getHourlyPriceForParking(1);
    console.log(`1. Hourly rate for Parking ID 1: €${rate}`);

    // Get cars by type in date range
    const typeEntries = await carEntryRepo.getCarsByTypeInDateRange(
      'sedan',
      '2026-04-16',
      '2026-04-18'
    );
    console.log(`2. Sedan entries in date range (${typeEntries.length} found)`);

    // Get cars by province in date range
    const provinceEntries = await carEntryRepo.getCarsByProvinceInDateRange(
      'Madrid',
      '2026-04-16',
      '2026-04-18'
    );
    console.log(`3. Madrid entries in date range (${provinceEntries.length} found)`);

  } catch (error) {
    console.error('Database error:', error.message);
  } finally {
    await pool.end();
  }
}

// ================================================================
// MAIN EXECUTION
// ================================================================

async function main() {
  try {
    // Run JSON examples
    await demonstrateJsonRepositories();

    // Run database examples (optional - comment out if no database connection)
    // await demonstrateDatabaseRepositories();

  } catch (error) {
    console.error('Error:', error);
  }
}

main();
