# CarParkingSession - Custom Fields for Car Park Database

## Overview

The `CarParkingSession` class provides calculated fields for parking session data from the `PRQ_Car_Entry` table. It automatically computes:

1. **Stay Duration (Minutes)** - Total parking time in minutes
2. **Stay Duration (Hours)** - Total parking time in hours (decimal)
3. **Total Amount Due** - Parking fee calculated from duration and hourly rate

All calculated fields return `null` (None in Python) if the vehicle hasn't exited yet (when `exit_date_time` is null).

---

## Key Features

✅ **Automatic Calculation** - Fields are computed on-the-fly as properties/getters
✅ **Null Safety** - Returns null when exit time is not set
✅ **Status Tracking** - Tracks whether vehicle has exited
✅ **Type-Safe** - Proper type hints and handling
✅ **Serializable** - Easy conversion to JSON/dict for API responses
✅ **Available in Multiple Languages** - Node.js and Python implementations

---

## Implementation Details

### Node.js Class: `CarParkingSession.js`

**Getters (Read-Only Properties):**

```javascript
session.stayDurationMinutes   // Returns: int | null
session.stayDurationHours     // Returns: float | null (2 decimals)
session.totalAmountDue        // Returns: float | null (2 decimals)
session.hasExited             // Returns: bool
session.status                // Returns: 'ACTIVE' | 'EXITED'
```

**Methods:**

```javascript
session.toObject()            // Returns object with all fields including calculated
session.toJSON()              // Alias for toObject()
session.toString()            // Returns formatted string representation
```

**Example:**
```javascript
const CarParkingSession = require('./CarParkingSession');

const session = new CarParkingSession({
  sequential_number: 1,
  parking_id: 1,
  car_id: 1,
  entry_date_time: '2026-04-16T08:00:00',
  exit_date_time: '2026-04-16T12:30:00',
  hourly_rate: 3.50
});

console.log(session.stayDurationMinutes);  // 270
console.log(session.stayDurationHours);    // 4.50
console.log(session.totalAmountDue);       // 15.75
```

### Python Class: `car_parking_session.py`

**Properties (Read-Only):**

```python
session.stay_duration_minutes   # Returns: int | None
session.stay_duration_hours     # Returns: float | None (2 decimals)
session.total_amount_due        # Returns: float | None (2 decimals)
session.has_exited              # Returns: bool
session.status                  # Returns: 'ACTIVE' | 'EXITED'
```

**Methods:**

```python
session.to_dict()               # Returns dict with all fields including calculated
session.to_json()               # Alias for to_dict()
str(session)                    # Returns formatted string representation
repr(session)                   # Returns object representation
```

**Example:**
```python
from car_parking_session import CarParkingSession

session = CarParkingSession({
    'sequential_number': 1,
    'parking_id': 1,
    'car_id': 1,
    'entry_date_time': '2026-04-16 08:00:00',
    'exit_date_time': '2026-04-16 12:30:00',
    'hourly_rate': 3.50
})

print(session.stay_duration_minutes)  # 270
print(session.stay_duration_hours)    # 4.5
print(session.total_amount_due)       # 15.75
```

---

## Calculation Formulas

### 1. Stay Duration in Minutes
```
stay_duration_minutes = (exit_date_time - entry_date_time) in minutes
```
- **Null Condition:** When `exit_date_time` is NULL
- **Example:** Entry 08:00, Exit 12:30 → 270 minutes

### 2. Stay Duration in Hours
```
stay_duration_hours = stay_duration_minutes / 60 (rounded to 2 decimals)
```
- **Null Condition:** When `exit_date_time` is NULL
- **Example:** 270 minutes → 4.50 hours

### 3. Total Amount Due
```
total_amount_due = stay_duration_hours × hourly_rate (rounded to 2 decimals)
```
- **Null Condition:** When `exit_date_time` is NULL
- **Example:** 4.50 hours × €3.50/hour → €15.75

---

## Usage Examples

### Example 1: Completed Session
```javascript
// JavaScript
const session = new CarParkingSession({
  sequential_number: 1,
  parking_id: 1,
  car_id: 1,
  entry_date_time: '2026-04-16T08:00:00',
  exit_date_time: '2026-04-16T12:30:00',
  hourly_rate: 3.50
});

console.log(session.totalAmountDue);  // 15.75
```

```python
# Python
session = CarParkingSession({
    'sequential_number': 1,
    'parking_id': 1,
    'car_id': 1,
    'entry_date_time': '2026-04-16 08:00:00',
    'exit_date_time': '2026-04-16 12:30:00',
    'hourly_rate': 3.50
})

print(session.total_amount_due)  # 15.75
```

### Example 2: Active Session (Still Parked)
```javascript
// JavaScript
const session = new CarParkingSession({
  sequential_number: 3,
  parking_id: 1,
  car_id: 3,
  entry_date_time: '2026-04-16T10:00:00',
  exit_date_time: null,  // Not exited yet
  hourly_rate: 3.50
});

console.log(session.stayDurationMinutes);  // null
console.log(session.stayDurationHours);    // null
console.log(session.totalAmountDue);       // null
console.log(session.status);               // 'ACTIVE'
```

```python
# Python
session = CarParkingSession({
    'sequential_number': 3,
    'parking_id': 1,
    'car_id': 3,
    'entry_date_time': '2026-04-16 10:00:00',
    'exit_date_time': None,  # Not exited yet
    'hourly_rate': 3.50
})

print(session.stay_duration_minutes)  # None
print(session.stay_duration_hours)    # None
print(session.total_amount_due)       # None
print(session.status)                 # 'ACTIVE'
```

### Example 3: Process Database Results
```javascript
// Node.js - with database connection
const db = require('./db-connection-nodejs.js');

// Get all completed sessions
const entries = await db.getAllCarEntries();
const sessions = entries.map(entry => new CarParkingSession(entry));

// Filter for completed sessions with revenue > €20
const highRevenue = sessions
  .filter(s => s.hasExited && s.totalAmountDue > 20);

highRevenue.forEach(s => {
  console.log(`Session #${s.sequentialNumber}: €${s.totalAmountDue}`);
});
```

```python
# Python - with database connection
from db_connection_python import DatabaseConnection
from car_parking_session import CarParkingSession

db = DatabaseConnection()
if db.connect():
    entries = db.get_all_car_entries()
    sessions = [CarParkingSession(e) for e in entries if e]
    
    # Calculate total revenue
    total_revenue = sum(s.total_amount_due for s in sessions if s.total_amount_due)
    print(f"Total Revenue: €{total_revenue}")
    
    db.disconnect()
```

---

## Integration with Database

### SQL Query to Retrieve Session Data
```sql
SELECT 
  ce.sequential_number,
  ce.parking_id,
  ce.car_id,
  ce.entry_date_time,
  ce.exit_date_time,
  p.hourly_rate
FROM PRQ_Car_Entry ce
JOIN PRQ_Parking p ON ce.parking_id = p.ID
ORDER BY ce.sequential_number DESC;
```

### Creating Sessions from Database Rows
```javascript
// Node.js
const rows = await db.getAllCarEntries();
const sessions = rows.map(row => new CarParkingSession(row));
```

```python
# Python
rows = db.get_all_car_entries()
sessions = [CarParkingSession(row) for row in rows]
```

---

## API Response Examples

### Single Completed Session
```json
{
  "sequential_number": 1,
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2026-04-16T08:00:00",
  "exit_date_time": "2026-04-16T12:30:00",
  "stay_duration_minutes": 270,
  "stay_duration_hours": 4.5,
  "total_amount_due": 15.75,
  "status": "EXITED",
  "hourly_rate": 3.5
}
```

### Active Session (Still Parked)
```json
{
  "sequential_number": 3,
  "parking_id": 1,
  "car_id": 3,
  "entry_date_time": "2026-04-16T10:00:00",
  "exit_date_time": null,
  "stay_duration_minutes": null,
  "stay_duration_hours": null,
  "total_amount_due": null,
  "status": "ACTIVE",
  "hourly_rate": 3.5
}
```

---

## Testing and Validation

### Node.js
Run the examples file to test all functionality:
```bash
node CarParkingSession-examples.js
```

### Python
Run the module directly:
```bash
python car_parking_session.py
```

---

## Common Operations

### Calculate Revenue Report
```javascript
const completedSessions = sessions.filter(s => s.hasExited);
const totalRevenue = completedSessions.reduce((sum, s) => sum + s.totalAmountDue, 0);
const avgRevenue = totalRevenue / completedSessions.length;

console.log(`Total Revenue: €${totalRevenue.toFixed(2)}`);
console.log(`Average per Session: €${avgRevenue.toFixed(2)}`);
```

### Find Long-Stay Vehicles
```javascript
const longStays = sessions.filter(s => s.stayDurationHours > 8);
```

### Export for Billing
```javascript
const billableData = sessions
  .filter(s => s.hasExited)
  .map(s => ({
    id: s.sequentialNumber,
    carId: s.carId,
    duration: s.stayDurationHours,
    amount: s.totalAmountDue
  }));
```

---

## Summary

| Feature | Node.js | Python |
|---------|---------|--------|
| **File** | `CarParkingSession.js` | `car_parking_session.py` |
| **Class Name** | `CarParkingSession` | `CarParkingSession` |
| **Null Value** | `null` | `None` |
| **DateTime Type** | `Date` | `datetime` |
| **Examples** | `CarParkingSession-examples.js` | Built-in docstring examples |
| **Serialization** | `.toObject()`, `.toJSON()` | `.to_dict()`, `.to_json()` |

Both implementations provide identical functionality and can be used interchangeably depending on your technology stack.
