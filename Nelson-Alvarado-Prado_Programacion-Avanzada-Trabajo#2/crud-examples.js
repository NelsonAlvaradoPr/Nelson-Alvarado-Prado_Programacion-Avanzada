/**
 * CRUD Operations Examples - Node.js
 * Demonstrates Insert, Update, Delete operations on all tables
 */

const { JsonCrudService, DatabaseCrudService } = require('./crud-service');

// ================================================================
// JSON CRUD EXAMPLES
// ================================================================

function demonstrateJsonCrud() {
  console.log('\n' + '='.repeat(80));
  console.log('JSON CRUD OPERATIONS - CAR PARK DATABASE');
  console.log('='.repeat(80) + '\n');

  const crud = new JsonCrudService('.');

  // ================================================================
  // CAR CRUD OPERATIONS
  // ================================================================
  console.log('--- CAR OPERATIONS ---\n');

  // INSERT
  console.log('1. INSERT - Adding new car');
  const insertCarResult = crud.insertCar({
    color: 'Green',
    year: 2023,
    make: 'Audi A4',
    type: 'sedan'
  });
  console.log(`   Result: ${insertCarResult.success ? '✓ Success' : '✗ Failed'}`);
  if (insertCarResult.success) {
    console.log(`   New Car ID: ${insertCarResult.data.id}`);
    console.log(`   Data: ${JSON.stringify(insertCarResult.data)}\n`);
  }

  // UPDATE
  console.log('2. UPDATE - Modifying car #1');
  const updateCarResult = crud.updateCar(1, {
    color: 'Dark Red',
    year: 2020,
    make: 'Toyota Corolla',
    type: 'sedan'
  });
  console.log(`   Result: ${updateCarResult.success ? '✓ Success' : '✗ Failed'}`);
  if (updateCarResult.success) {
    console.log(`   Updated Data: ${JSON.stringify(updateCarResult.data)}\n`);
  }

  // DELETE
  console.log('3. DELETE - Removing car #6 (if it exists)');
  const deleteCarResult = crud.deleteCar(6);
  console.log(`   Result: ${deleteCarResult.success ? '✓ Success' : '✗ Failed'}`);
  if (deleteCarResult.success) {
    console.log(`   Deleted: ${JSON.stringify(deleteCarResult.data)}\n`);
  } else {
    console.log(`   Info: ${deleteCarResult.error}\n`);
  }

  // ================================================================
  // PARKING CRUD OPERATIONS
  // ================================================================
  console.log('--- PARKING OPERATIONS ---\n');

  // INSERT
  console.log('1. INSERT - Adding new parking facility');
  const insertParkingResult = crud.insertParking({
    province_name: 'Valencia',
    name: 'Parking Centro Histórico',
    hourly_rate: 2.25
  });
  console.log(`   Result: ${insertParkingResult.success ? '✓ Success' : '✗ Failed'}`);
  if (insertParkingResult.success) {
    console.log(`   New Parking ID: ${insertParkingResult.data.id}`);
    console.log(`   Data: ${JSON.stringify(insertParkingResult.data)}\n`);
  }

  // UPDATE
  console.log('2. UPDATE - Modifying parking #1 rate');
  const updateParkingResult = crud.updateParking(1, {
    province_name: 'Madrid',
    name: 'Parking Centro Plaza Mayor - Premium',
    hourly_rate: 4.00
  });
  console.log(`   Result: ${updateParkingResult.success ? '✓ Success' : '✗ Failed'}`);
  if (updateParkingResult.success) {
    console.log(`   Updated Data: ${JSON.stringify(updateParkingResult.data)}\n`);
  }

  // DELETE (will fail due to foreign keys in typical scenario)
  console.log('3. DELETE - Attempting to delete parking #3 (if it exists)');
  const deleteParkingResult = crud.deleteParking(3);
  console.log(`   Result: ${deleteParkingResult.success ? '✓ Success' : '✗ Failed'}`);
  if (deleteParkingResult.success) {
    console.log(`   Deleted: ${JSON.stringify(deleteParkingResult.data)}\n`);
  } else {
    console.log(`   Info: ${deleteParkingResult.error}\n`);
  }

  // ================================================================
  // CAR ENTRY CRUD OPERATIONS
  // ================================================================
  console.log('--- CAR ENTRY OPERATIONS ---\n');

  // INSERT
  console.log('1. INSERT - Adding new car entry');
  const insertEntryResult = crud.insertCarEntry({
    parking_id: 1,
    car_id: 3,
    entry_date_time: '2026-04-17T14:00:00',
    exit_date_time: '2026-04-17T17:30:00'
  });
  console.log(`   Result: ${insertEntryResult.success ? '✓ Success' : '✗ Failed'}`);
  if (insertEntryResult.success) {
    console.log(`   New Entry #: ${insertEntryResult.data.sequential_number}`);
    console.log(`   Data: ${JSON.stringify(insertEntryResult.data)}\n`);
  }

  // UPDATE
  console.log('2. UPDATE - Recording exit time for entry #3');
  const updateEntryResult = crud.updateCarEntry(3, {
    parking_id: 1,
    car_id: 3,
    entry_date_time: '2026-04-16T10:00:00',
    exit_date_time: '2026-04-16T15:30:00'
  });
  console.log(`   Result: ${updateEntryResult.success ? '✓ Success' : '✗ Failed'}`);
  if (updateEntryResult.success) {
    console.log(`   Updated Data: ${JSON.stringify(updateEntryResult.data)}\n`);
  }

  // DELETE
  console.log('3. DELETE - Removing entry #16 (if it exists)');
  const deleteEntryResult = crud.deleteCarEntry(16);
  console.log(`   Result: ${deleteEntryResult.success ? '✓ Success' : '✗ Failed'}`);
  if (deleteEntryResult.success) {
    console.log(`   Deleted: ${JSON.stringify(deleteEntryResult.data)}\n`);
  } else {
    console.log(`   Info: ${deleteEntryResult.error}\n`);
  }

  // ================================================================
  // VERIFICATION
  // ================================================================
  console.log('--- VERIFICATION ---\n');

  const carsRepo = require('./repositories').JsonCarRepository;
  const parkingRepo = require('./repositories').JsonParkingRepository;
  const entryRepo = require('./repositories').JsonCarEntryRepository;

  console.log('Current state of tables (post-CRUD operations):');
  
  const cars = new carsRepo('./prq_cars.json');
  const allCars = cars.data;
  console.log(`  • Total Cars: ${allCars.length}`);
  
  const parkings = new parkingRepo('./prq_parking.json');
  const allParkings = parkings.data;
  console.log(`  • Total Parkings: ${allParkings.length}`);
  
  const entries = new entryRepo('./prq_car_entry.json', './prq_parking.json');
  const allEntries = entries.carEntryData;
  console.log(`  • Total Entries: ${allEntries.length}\n`);
}

// ================================================================
// DATABASE CRUD EXAMPLES
// ================================================================

async function demonstrateDatabaseCrud() {
  console.log('\n' + '='.repeat(80));
  console.log('DATABASE CRUD OPERATIONS - CAR PARK DATABASE');
  console.log('='.repeat(80) + '\n');

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
    // Test connection
    const testConnection = await pool.getConnection();
    testConnection.release();
    console.log('✓ Database connection established\n');

    const crud = new DatabaseCrudService(pool);

    // ================================================================
    // CAR CRUD OPERATIONS
    // ================================================================
    console.log('--- CAR OPERATIONS ---\n');

    // INSERT
    console.log('1. INSERT - Adding new car');
    const insertCarResult = await crud.insertCar({
      color: 'Purple',
      year: 2023,
      make: 'Volkswagen Golf',
      type: 'sedan'
    });
    console.log(`   Result: ${insertCarResult.success ? '✓ Success' : '✗ Failed'}`);
    console.log(`   ID: ${insertCarResult.data?.id}\n`);

    // UPDATE
    console.log('2. UPDATE - Modifying car #1');
    const updateCarResult = await crud.updateCar(1, {
      color: 'Burgundy',
      year: 2021,
      make: 'Toyota Corolla SE',
      type: 'sedan'
    });
    console.log(`   Result: ${updateCarResult.success ? '✓ Success' : '✗ Failed'}\n`);

    // ================================================================
    // PARKING CRUD OPERATIONS
    // ================================================================
    console.log('--- PARKING OPERATIONS ---\n');

    // INSERT
    console.log('1. INSERT - Adding new parking');
    const insertParkingResult = await crud.insertParking({
      province_name: 'Seville',
      name: 'Parking Torre del Oro',
      hourly_rate: 2.50
    });
    console.log(`   Result: ${insertParkingResult.success ? '✓ Success' : '✗ Failed'}`);
    console.log(`   ID: ${insertParkingResult.data?.id}\n`);

    // UPDATE
    console.log('2. UPDATE - Modifying parking #1');
    const updateParkingResult = await crud.updateParking(1, {
      province_name: 'Madrid',
      name: 'Parking Centro Renovado',
      hourly_rate: 3.75
    });
    console.log(`   Result: ${updateParkingResult.success ? '✓ Success' : '✗ Failed'}\n`);

    // ================================================================
    // CAR ENTRY CRUD OPERATIONS
    // ================================================================
    console.log('--- CAR ENTRY OPERATIONS ---\n');

    // INSERT
    console.log('1. INSERT - Adding new entry');
    const insertEntryResult = await crud.insertCarEntry({
      parking_id: 1,
      car_id: 2,
      entry_date_time: '2026-04-17 15:00:00',
      exit_date_time: '2026-04-17 18:00:00'
    });
    console.log(`   Result: ${insertEntryResult.success ? '✓ Success' : '✗ Failed'}`);
    console.log(`   Sequential #: ${insertEntryResult.data?.sequential_number}\n`);

    // UPDATE
    console.log('2. UPDATE - Recording exit time for entry #1');
    const updateEntryResult = await crud.updateCarEntry(1, {
      parking_id: 1,
      car_id: 1,
      entry_date_time: '2026-04-16 08:00:00',
      exit_date_time: '2026-04-16 13:00:00'
    });
    console.log(`   Result: ${updateEntryResult.success ? '✓ Success' : '✗ Failed'}\n`);

  } catch (error) {
    console.error('Error:', error.message);
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
    demonstrateJsonCrud();

    // Run database examples (optional)
    // await demonstrateDatabaseCrud();

  } catch (error) {
    console.error('Error:', error);
  }
}

main();
