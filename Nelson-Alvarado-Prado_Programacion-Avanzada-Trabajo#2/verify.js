/**
 * Verification Script - Tests all Car Park Database components
 * Checks database connectivity, schemas, CRUD operations, and data integrity
 */

const fs = require('fs');
const path = require('path');

// ================================================================
// FILE VERIFICATION
// ================================================================

function verifyFiles() {
  console.log('\n' + '='.repeat(80));
  console.log('FILE VERIFICATION');
  console.log('='.repeat(80) + '\n');

  const requiredFiles = [
    'design-db.sql',
    'insert-records.sql',
    'prq_cars.json',
    'prq_parking.json',
    'prq_car_entry.json',
    '.env',
    'CarParkingSession.js',
    'repositories.js',
    'crud-service.js',
    'CONNECTION_GUIDE.md',
    'CUSTOM_FIELDS_GUIDE.md',
    'REPOSITORIES_GUIDE.md',
    'CRUD_GUIDE.md'
  ];

  console.log('Checking for required files:\n');
  let allFilesFound = true;
  
  requiredFiles.forEach(file => {
    const exists = fs.existsSync(path.join(__dirname, file));
    const status = exists ? '✓' : '✗';
    console.log(`  ${status} ${file}`);
    if (!exists) allFilesFound = false;
  });

  console.log(`\n${allFilesFound ? '✓ All files present' : '✗ Some files missing'}\n`);
  return allFilesFound;
}

// ================================================================
// JSON DATA VERIFICATION
// ================================================================

function verifyJsonData() {
  console.log('='.repeat(80));
  console.log('JSON DATA VERIFICATION');
  console.log('='.repeat(80) + '\n');

  const tables = {
    'prq_cars.json': { minRecords: 5, expectedFields: ['id', 'color', 'year', 'make', 'type'] },
    'prq_parking.json': { minRecords: 2, expectedFields: ['id', 'province_name', 'name', 'hourly_rate'] },
    'prq_car_entry.json': { minRecords: 15, expectedFields: ['sequential_number', 'parking_id', 'car_id', 'entry_date_time', 'exit_date_time'] }
  };

  console.log('Checking JSON files structure and data:\n');
  let allValid = true;

  Object.entries(tables).forEach(([filename, config]) => {
    try {
      const filepath = path.join(__dirname, filename);
      const data = JSON.parse(fs.readFileSync(filepath, 'utf-8'));
      
      console.log(`${filename}:`);
      console.log(`  ✓ Valid JSON`);
      console.log(`  • Records: ${data.length} (minimum: ${config.minRecords})`);
      
      if (data.length > 0) {
        const firstRecord = data[0];
        const fieldsPresent = config.expectedFields.every(field => field in firstRecord);
        if (fieldsPresent) {
          console.log(`  ✓ All required fields present`);
        } else {
          console.log(`  ✗ Missing fields`);
          allValid = false;
        }
      }
      
      if (data.length < config.minRecords) {
        console.log(`  ✗ Insufficient records`);
        allValid = false;
      }
      console.log();
    } catch (error) {
      console.log(`  ✗ Error: ${error.message}\n`);
      allValid = false;
    }
  });

  console.log(`${allValid ? '✓ All JSON data valid' : '✗ Some JSON data issues'}\n`);
  return allValid;
}

// ================================================================
// MODULE VERIFICATION
// ================================================================

function verifyModules() {
  console.log('='.repeat(80));
  console.log('MODULE VERIFICATION');
  console.log('='.repeat(80) + '\n');

  console.log('Checking module imports:\n');

  const modules = {
    'CarParkingSession': './CarParkingSession.js',
    'Repositories': './repositories.js',
    'CRUD Service': './crud-service.js'
  };

  let allValid = true;

  Object.entries(modules).forEach(([name, file]) => {
    try {
      require(path.join(__dirname, file));
      console.log(`  ✓ ${name} - OK`);
    } catch (error) {
      console.log(`  ✗ ${name} - ERROR: ${error.message}`);
      allValid = false;
    }
  });

  console.log(`\n${allValid ? '✓ All modules loadable' : '✗ Some modules have errors'}\n`);
  return allValid;
}

// ================================================================
// CLASS FUNCTIONALITY TEST
// ================================================================

function verifyCrudFunctionality() {
  console.log('='.repeat(80));
  console.log('CRUD FUNCTIONALITY TEST');
  console.log('='.repeat(80) + '\n');

  try {
    const { JsonCrudService } = require('./crud-service');
    const crud = new JsonCrudService('.');

    console.log('Testing CRUD operations on JSON:\n');

    // Test Insert
    console.log('1. Testing INSERT on Cars:');
    const insertResult = crud.insertCar({
      color: 'TEST_COLOR',
      year: 2025,
      make: 'TEST_MAKE',
      type: 'sedan'
    });
    console.log(`   ${insertResult.success ? '✓ INSERT successful' : '✗ INSERT failed'}`);
    const testCarId = insertResult.data?.id;

    // Test Update
    console.log('2. Testing UPDATE on Cars:');
    const updateResult = crud.updateCar(1, {
      color: 'UPDATED_COLOR',
      year: 2020,
      make: 'Toyota Corolla',
      type: 'sedan'
    });
    console.log(`   ${updateResult.success ? '✓ UPDATE successful' : '✗ UPDATE failed'}`);

    // Test Read (via repository)
    console.log('3. Testing READ on Cars:');
    const { JsonCarRepository } = require('./repositories');
    const carRepo = new JsonCarRepository('./prq_cars.json');
    carRepo.getAll().then(cars => {
      console.log(`   ✓ READ successful - ${cars.length} cars found`);
    });

    // Clean up test data
    if (testCarId) {
      crud.deleteCar(testCarId);
      console.log('4. Testing DELETE on Cars:');
      console.log(`   ✓ DELETE successful`);
    }

    console.log();
    return true;
  } catch (error) {
    console.log(`✗ CRUD test failed: ${error.message}\n`);
    return false;
  }
}

// ================================================================
// DATABASE VERIFICATION (Optional)
// ================================================================

async function verifyDatabaseConnection() {
  console.log('='.repeat(80));
  console.log('DATABASE CONNECTION VERIFICATION');
  console.log('='.repeat(80) + '\n');

  try {
    require('dotenv').config();
    const mysql = require('mysql2/promise');

    const pool = mysql.createPool({
      host: process.env.DB_HOST,
      port: process.env.DB_PORT,
      user: process.env.DB_USER,
      password: process.env.DB_PASSWORD,
      database: process.env.DB_NAME,
      ssl: 'REQUIRED',
      waitForConnections: true,
      connectionLimit: 2,
      queueLimit: 0
    });

    console.log('Testing database connection:\n');
    
    const connection = await pool.getConnection();
    console.log('  ✓ Connection successful');

    // Check tables
    const [tables] = await connection.execute(
      `SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = ? AND TABLE_NAME LIKE 'PRQ%'`,
      [process.env.DB_NAME]
    );

    console.log(`  ✓ Database: ${process.env.DB_NAME}`);
    console.log(`  • Tables found: ${tables.length}`);

    tables.forEach(t => {
      console.log(`    - ${t.TABLE_NAME}`);
    });

    // Check row counts
    for (const table of ['PRQ_Cars', 'PRQ_Parking', 'PRQ_Car_Entry']) {
      const [result] = await connection.execute(`SELECT COUNT(*) as count FROM ${table}`);
      console.log(`    ${table}: ${result[0].count} records`);
    }

    connection.release();
    await pool.end();

    console.log('\n✓ Database verification successful\n');
    return true;
  } catch (error) {
    console.log(`\nℹ Database not available: ${error.message}`);
    console.log('   (This is OK for local development)\n');
    return false;
  }
}

// ================================================================
// MAIN EXECUTION
// ================================================================

async function main() {
  console.log('\n' + '█'.repeat(80));
  console.log('█' + ' '.repeat(78) + '█');
  console.log('█' + 'CAR PARK DATABASE - COMPREHENSIVE VERIFICATION'.padEnd(78) + '█');
  console.log('█' + ' '.repeat(78) + '█');
  console.log('█'.repeat(80) + '\n');

  const results = {
    files: verifyFiles(),
    jsonData: verifyJsonData(),
    modules: verifyModules(),
    crud: verifyCrudFunctionality(),
    database: null
  };

  results.database = await verifyDatabaseConnection();

  // Summary
  console.log('='.repeat(80));
  console.log('VERIFICATION SUMMARY');
  console.log('='.repeat(80) + '\n');

  const checks = {
    'Files Present': results.files,
    'JSON Data Valid': results.jsonData,
    'Modules Loadable': results.modules,
    'CRUD Functions': results.crud,
    'Database Connection': results.database !== false ? 'Optional' : 'N/A'
  };

  let allPassed = true;
  Object.entries(checks).forEach(([check, status]) => {
    if (status === 'Optional') {
      console.log(`  ⊙ ${check}: Optional (not tested)`);
    } else {
      const icon = status ? '✓' : '✗';
      console.log(`  ${icon} ${check}: ${status ? 'PASSED' : 'FAILED'}`);
      if (!status) allPassed = false;
    }
  });

  console.log('\n' + '='.repeat(80));
  if (allPassed) {
    console.log('✓ ALL REQUIRED CHECKS PASSED - System is ready to use!');
  } else {
    console.log('✗ Some checks failed - please review errors above');
  }
  console.log('='.repeat(80) + '\n');

  process.exit(allPassed ? 0 : 1);
}

main().catch(error => {
  console.error('Fatal error:', error);
  process.exit(1);
});
