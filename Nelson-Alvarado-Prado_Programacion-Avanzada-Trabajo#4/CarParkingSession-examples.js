/**
 * CarParkingSession Usage Examples - Node.js
 * Demonstrates how to use the CarParkingSession class with the database
 */

const CarParkingSession = require('./CarParkingSession');

// ================================================================
// EXAMPLE 1: Create session from database query result
// ================================================================
const dbRow = {
  sequential_number: 1,
  parking_id: 1,
  car_id: 1,
  entry_date_time: '2026-04-16T08:00:00',
  exit_date_time: '2026-04-16T12:30:00',
  hourly_rate: 3.50
};

const session1 = new CarParkingSession(dbRow);

console.log('=== EXAMPLE 1: Completed Parking Session ===');
console.log(session1.toString());
console.log(`\nCalculated Fields:`);
console.log(`  Stay Duration (minutes): ${session1.stayDurationMinutes}`);
console.log(`  Stay Duration (hours): ${session1.stayDurationHours}`);
console.log(`  Total Amount Due: €${session1.totalAmountDue}`);
console.log(`  Status: ${session1.status}`);
console.log(`  Full Object: ${JSON.stringify(session1.toObject(), null, 2)}\n`);

// ================================================================
// EXAMPLE 2: Active session (vehicle still parked)
// ================================================================
const dbRow2 = {
  sequential_number: 3,
  parking_id: 1,
  car_id: 3,
  entry_date_time: '2026-04-16T10:00:00',
  exit_date_time: null,  // NOT EXITED
  hourly_rate: 3.50
};

const session2 = new CarParkingSession(dbRow2);

console.log('=== EXAMPLE 2: Active Parking Session (Vehicle Still Parked) ===');
console.log(session2.toString());
console.log(`\nCalculated Fields (should be null since vehicle hasn't exited):`);
console.log(`  Stay Duration (minutes): ${session2.stayDurationMinutes}`);
console.log(`  Stay Duration (hours): ${session2.stayDurationHours}`);
console.log(`  Total Amount Due: ${session2.totalAmountDue}`);
console.log(`  Status: ${session2.status}\n`);

// ================================================================
// EXAMPLE 3: Different parking facility (lower rate)
// ================================================================
const dbRow3 = {
  sequential_number: 12,
  parking_id: 2,
  car_id: 2,
  entry_date_time: '2026-04-17T08:30:00',
  exit_date_time: '2026-04-17T13:00:00',
  hourly_rate: 2.75
};

const session3 = new CarParkingSession(dbRow3);

console.log('=== EXAMPLE 3: Barcelona Parking (Lower Rate) ===');
console.log(session3.toString());
console.log(`\nCalculated Fields:`);
console.log(`  Stay Duration (minutes): ${session3.stayDurationMinutes}`);
console.log(`  Stay Duration (hours): ${session3.stayDurationHours}`);
console.log(`  Total Amount Due: €${session3.totalAmountDue}\n`);

// ================================================================
// EXAMPLE 4: Process multiple sessions from query results
// ================================================================
const allSessions = [
  {
    sequential_number: 1,
    parking_id: 1,
    car_id: 1,
    entry_date_time: '2026-04-16T08:00:00',
    exit_date_time: '2026-04-16T12:30:00',
    hourly_rate: 3.50
  },
  {
    sequential_number: 2,
    parking_id: 1,
    car_id: 2,
    entry_date_time: '2026-04-16T09:15:00',
    exit_date_time: '2026-04-16T18:45:00',
    hourly_rate: 3.50
  },
  {
    sequential_number: 3,
    parking_id: 1,
    car_id: 3,
    entry_date_time: '2026-04-16T10:00:00',
    exit_date_time: null,
    hourly_rate: 3.50
  }
];

console.log('=== EXAMPLE 4: Process Multiple Sessions ===');
const sessions = allSessions.map(row => new CarParkingSession(row));

console.log('Sessions Summary:');
sessions.forEach(session => {
  console.log(`  #${session.sequentialNumber.toString().padEnd(3)} - Car ${session.carId} | ${session.status.padEnd(7)} | Duration: ${
    session.stayDurationHours ? `${session.stayDurationHours}h` : 'N/A'
  } | Due: €${session.totalAmountDue || 'N/A'}`);
});

// ================================================================
// EXAMPLE 5: Calculate total revenue from multiple sessions
// ================================================================
console.log('\n=== EXAMPLE 5: Calculate Revenue ===');

const totalRevenue = sessions
  .filter(s => s.hasExited)
  .reduce((sum, s) => sum + (s.totalAmountDue || 0), 0);

const completedSessions = sessions.filter(s => s.hasExited).length;
const activeSessions = sessions.filter(s => !s.hasExited).length;

console.log(`Total Sessions: ${sessions.length}`);
console.log(`  - Completed: ${completedSessions}`);
console.log(`  - Active: ${activeSessions}`);
console.log(`Total Revenue Generated: €${totalRevenue.toFixed(2)}`);
console.log(`Average Revenue per Completed Session: €${(totalRevenue / completedSessions).toFixed(2)}\n`);

// ================================================================
// EXAMPLE 6: Filter sessions by criteria
// ================================================================
console.log('=== EXAMPLE 6: Filter Sessions ===');

const longStays = sessions.filter(s => s.stayDurationHours && s.stayDurationHours > 5);
console.log(`Sessions with duration > 5 hours: ${longStays.length}`);
longStays.forEach(s => {
  console.log(`  - Session #${s.sequentialNumber}: ${s.stayDurationHours} hours - €${s.totalAmountDue}`);
});

const highValue = sessions.filter(s => s.totalAmountDue && s.totalAmountDue > 20);
console.log(`\nSessions with total due > €20: ${highValue.length}`);
highValue.forEach(s => {
  console.log(`  - Session #${s.sequentialNumber}: €${s.totalAmountDue}`);
});

// ================================================================
// EXAMPLE 7: Export data for reporting
// ================================================================
console.log('\n=== EXAMPLE 7: Export for Reporting ===');
const reportData = sessions.map(s => s.toObject());
console.log('Exported data (JSON):');
console.log(JSON.stringify(reportData, null, 2).substring(0, 500) + '...');
