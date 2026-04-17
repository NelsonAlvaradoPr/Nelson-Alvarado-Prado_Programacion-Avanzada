# CRUD Operations Guide - Car Park Database

## Overview

The CRUD services provide methods to **Create (Insert), Read (already covered in repositories), Update, and Delete** records in all three tables:
- **PRQ_Cars**
- **PRQ_Parking**
- **PRQ_Car_Entry**

Both **JSON** and **Database** implementations are available.

---

## Architecture

### JsonCrudService
- Operates on JSON files directly
- Perfect for local development and testing
- File-based persistence
- Synchronous operations

### DatabaseCrudService
- Operates on Azure MySQL database
- Production-ready
- Connection pooling support
- Async/await operations

---

## Car Operations

### Insert Car

**Node.js (JSON):**
```javascript
const { JsonCrudService } = require('./crud-service');
const crud = new JsonCrudService('.');

const result = crud.insertCar({
  color: 'Blue',
  year: 2023,
  make: 'Honda Civic',
  type: 'sedan'
});

console.log(result.data);  // { id: 6, color: 'Blue', year: 2023, ... }
```

**Python (JSON):**
```python
from crud_service import JsonCrudService

crud = JsonCrudService('.')

result = crud.insert_car({
    'color': 'Blue',
    'year': 2023,
    'make': 'Honda Civic',
    'type': 'sedan'
})

print(result['data'])  # {'id': 6, 'color': 'Blue', ...}
```

**Node.js (Database):**
```javascript
const { DatabaseCrudService } = require('./crud-service');
const mysql = require('mysql2/promise');

const pool = mysql.createPool({...});
const crud = new DatabaseCrudService(pool);

const result = await crud.insertCar({
  color: 'Blue',
  year: 2023,
  make: 'Honda Civic',
  type: 'sedan'
});
```

### Update Car

**Node.js:**
```javascript
const result = crud.updateCar(1, {
  color: 'Navy Blue',
  year: 2022,
  make: 'Toyota Camry',
  type: 'sedan'
});
```

**Python:**
```python
result = crud.update_car(1, {
    'color': 'Navy Blue',
    'year': 2022,
    'make': 'Toyota Camry',
    'type': 'sedan'
})
```

### Delete Car

**Node.js:**
```javascript
const result = crud.deleteCar(5);
// Returns deleted car data or error
```

**Python:**
```python
result = crud.delete_car(5)
# Returns deleted car data or error
```

---

## Parking Operations

### Insert Parking

**Node.js (JSON):**
```javascript
const result = crud.insertParking({
  province_name: 'Valencia',
  name: 'Parking Centro',
  hourly_rate: 2.50
});

console.log(result.data);  // { id: 3, province_name: 'Valencia', ... }
```

**Python (JSON):**
```python
result = crud.insert_parking({
    'province_name': 'Valencia',
    'name': 'Parking Centro',
    'hourly_rate': 2.50
})

print(result['data'])
```

### Update Parking

**Node.js:**
```javascript
const result = crud.updateParking(1, {
  province_name: 'Madrid',
  name: 'Parking Centro Premium',
  hourly_rate: 3.75
});
```

**Python:**
```python
result = crud.update_parking(1, {
    'province_name': 'Madrid',
    'name': 'Parking Centro Premium',
    'hourly_rate': 3.75
})
```

### Delete Parking

**Node.js:**
```javascript
const result = crud.deleteParking(3);
```

**Python:**
```python
result = crud.delete_parking(3)
```

---

## Car Entry Operations

### Insert Car Entry

**Node.js (JSON):**
```javascript
const result = crud.insertCarEntry({
  parking_id: 1,
  car_id: 2,
  entry_date_time: '2026-04-17T14:00:00',
  exit_date_time: '2026-04-17T17:30:00'
});

console.log(result.data);  // { sequential_number: 16, parking_id: 1, ... }
```

**Python (JSON):**
```python
result = crud.insert_car_entry({
    'parking_id': 1,
    'car_id': 2,
    'entry_date_time': '2026-04-17T14:00:00',
    'exit_date_time': '2026-04-17T17:30:00'
})

print(result['data'])
```

### Update Car Entry

**Node.js:**
```javascript
// Record exit time for active entry
const result = crud.updateCarEntry(3, {
  parking_id: 1,
  car_id: 3,
  entry_date_time: '2026-04-16 10:00:00',
  exit_date_time: '2026-04-16 15:30:00'
});
```

**Python:**
```python
# Record exit time for active entry
result = crud.update_car_entry(3, {
    'parking_id': 1,
    'car_id': 3,
    'entry_date_time': '2026-04-16 10:00:00',
    'exit_date_time': '2026-04-16 15:30:00'
})
```

### Delete Car Entry

**Node.js:**
```javascript
const result = crud.deleteCarEntry(15);
```

**Python:**
```python
result = crud.delete_car_entry(15)
```

---

## Response Format

### Success Response

```json
{
  "success": true,
  "data": {
    "id": 6,
    "color": "Blue",
    "year": 2023,
    "make": "Honda Civic",
    "type": "sedan"
  }
}
```

### Error Response

```json
{
  "success": false,
  "error": "Car with ID 999 not found"
}
```

---

## Complete Workflow Example

**Car Check-In and Check-Out:**

```javascript
// 1. Check-In: Insert car entry
const checkIn = crud.insertCarEntry({
  parking_id: 1,
  car_id: 3,
  entry_date_time: '2026-04-17T10:00:00',
  exit_date_time: null  // No exit yet
});

// 2. Car is parked...
// Time passes...

// 3. Check-Out: Update entry with exit time
const checkOut = crud.updateCarEntry(checkIn.data.sequential_number, {
  parking_id: 1,
  car_id: 3,
  entry_date_time: '2026-04-17T10:00:00',
  exit_date_time: '2026-04-17T13:45:00'  // Record exit time
});

// 4. Calculate fees using CarParkingSession
const CarParkingSession = require('./CarParkingSession');
const session = new CarParkingSession(checkOut.data);
console.log(`Duration: ${session.stayDurationHours}h`);
console.log(`Amount Due: €${session.totalAmountDue}`);
```

---

## Batch Operations Example

**Processing Multiple Records:**

```javascript
// Update rates for all Madrid parkings
const parkings = await parkingRepo.getByProvinceName('Madrid');

parkings.forEach(parking => {
  const newRate = parking.hourly_rate * 1.1;  // 10% increase
  const result = crud.updateParking(parking.id, {
    province_name: parking.province_name,
    name: parking.name,
    hourly_rate: newRate
  });
  
  console.log(`Updated ${parking.name}: €${newRate}/hour`);
});
```

---

## Transaction Considerations

### Important Notes:

⚠️ **Foreign Key Constraints:**
- Deleting a Car or Parking that has entries in Car_Entry may fail (cascade policy varies)
- Always check for related records before deletion

⚠️ **Data Consistency:**
- For database operations, ensure transactions are properly handled
- JSON operations are file-based and atomic per file

✅ **Best Practices:**
- Always validate data before insert/update
- Check operation results before proceeding
- Log all modifications for audit trail
- Use transactions for related updates

---

## Files Provided

| File | Description |
|------|-------------|
| `crud-service.js` | Node.js CRUD implementations |
| `crud_service.py` | Python CRUD implementations |
| `crud-examples.js` | Node.js usage examples |
| `crud_examples.py` | Python usage examples |
| `CRUD_GUIDE.md` | This comprehensive guide |

---

## Summary

### Methods Available

**JsonCrudService & DatabaseCrudService:**

**Cars:**
- `insertCar(data)` - Add new car
- `updateCar(id, data)` - Modify existing car
- `deleteCar(id)` - Remove car

**Parking:**
- `insertParking(data)` - Add new parking
- `updateParking(id, data)` - Modify parking
- `deleteParking(id)` - Remove parking

**Car Entry:**
- `insertCarEntry(data)` - Record entry
- `updateCarEntry(id, data)` - Update entry (e.g., add exit time)
- `deleteCarEntry(id)` - Remove entry

### Data Validation

Ensure data matches table schema:

**Car Data:**
```json
{
  "color": "string",
  "year": "integer",
  "make": "string",
  "type": "enum: sedan|4x4|motorcycle"
}
```

**Parking Data:**
```json
{
  "province_name": "string",
  "name": "string",
  "hourly_rate": "decimal(10,2)"
}
```

**Car Entry Data:**
```json
{
  "parking_id": "integer (foreign key)",
  "car_id": "integer (foreign key)",
  "entry_date_time": "datetime",
  "exit_date_time": "datetime or null"
}
```

---

Both implementations are production-ready and fully integrated with your repository pattern!
