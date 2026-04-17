# Repository Pattern - Car Park Database

## Overview

The repository pattern provides a flexible data access layer that supports querying from multiple sources (JSON files or database) through a consistent interface. This allows your application to work seamlessly with either data source without changing the business logic.

---

## Architecture

### Interface-Based Design

```
┌─────────────────────────────────────┐
│   Business Logic / Controllers      │
└─────────────────────────────────────┘
           ↓ depends on ↓
┌─────────────────────────────────────┐
│      Repository Interfaces          │
│  (ICarRepository, IParkingRepository,│
│   ICarEntryRepository)              │
└─────────────────────────────────────┘
           ↓ implements ↓
┌──────────────────┬──────────────────┐
│  JSON            │  Database        │
│  Repositories    │  Repositories    │
└──────────────────┴──────────────────┘
        ↓                   ↓
   JSON Files         MySQL Database
```

### Benefits

✅ **Switchable Data Sources** - Change from JSON to database without touching business logic
✅ **Testability** - Mock repositories for unit testing
✅ **Consistency** - Same interface regardless of data source
✅ **Maintainability** - Centralized data access logic
✅ **Scalability** - Easy to add new data sources

---

## Interfaces

### IRepository (Base Interface)

```javascript
// Node.js
async getById(id) → Promise<Object|null>
async getAll() → Promise<Array>
```

```python
# Python
async get_by_id(id: Any) → Optional[Dict]
async get_all() → List[Dict]
```

All repositories inherit from this base interface.

---

## Car Repository (`ICarRepository`)

Queries for PRQ_Cars table.

### Methods

| Method | Parameters | Returns | Description |
|--------|-----------|---------|-------------|
| **getById** | `id: int` | `Car \| null` | Get car by primary key |
| **getAll** | — | `Car[]` | Get all cars |
| **getByColor** | `color: string` | `Car[]` | Find cars by color (approximate) |
| **getByYearRange** | `minYear: int, maxYear: int` | `Car[]` | Find cars by year range |
| **getByMake** | `make: string` | `Car[]` | Find cars by manufacturer (approximate) |
| **getByType** | `type: string` | `Car[]` | Find cars by type (sedan, 4x4, motorcycle) |
| **getByColorAndYearRangeAndMakeAndType** | `color?, minYear, maxYear, make?, type?` | `Car[]` | Combined filter query |

### Examples

**Node.js:**
```javascript
const carRepo = new JsonCarRepository('./prq_cars.json');

// Get all cars
const allCars = await carRepo.getAll();

// Get cars by color
const redCars = await carRepo.getByColor('Red');

// Get cars by year range
const recent = await carRepo.getByYearRange(2020, 2022);

// Get cars by type
const sedans = await carRepo.getByType('sedan');

// Combined query
const filtered = await carRepo.getByColorAndYearRangeAndMakeAndType(
  null,          // No color filter
  2018, 2022,    // Year 2018-2022
  null,          // No make filter
  '4x4'          // Only 4x4 vehicles
);
```

**Python:**
```python
car_repo = JsonCarRepository('./prq_cars.json')

# Get all cars
all_cars = await car_repo.get_all()

# Get cars by color
red_cars = await car_repo.get_by_color('Red')

# Get cars by year range
recent = await car_repo.get_by_year_range(2020, 2022)

# Get cars by type
sedans = await car_repo.get_by_type('sedan')

# Combined query
filtered = await car_repo.get_by_color_and_year_range_and_make_and_type(
    None,          # No color filter
    2018, 2022,    # Year 2018-2022
    None,          # No make filter
    '4x4'          # Only 4x4 vehicles
)
```

---

## Parking Repository (`IParkingRepository`)

Queries for PRQ_Parking table.

### Methods

| Method | Parameters | Returns | Description |
|--------|-----------|---------|-------------|
| **getById** | `id: int` | `Parking \| null` | Get parking by primary key |
| **getAll** | — | `Parking[]` | Get all parkings |
| **getByProvinceName** | `province: string` | `Parking[]` | Find parkings by province (approximate) |
| **getByName** | `name: string` | `Parking[]` | Find parkings by name (approximate) |
| **getByHourlyRateRange** | `minRate: float, maxRate: float` | `Parking[]` | Find parkings by hourly rate range |
| **getByProvinceAndNameAndHourlyRateRange** | `province?, name?, minRate, maxRate` | `Parking[]` | Combined filter query |

### Examples

**Node.js:**
```javascript
const parkingRepo = new JsonParkingRepository('./prq_parking.json');

// Get all parkings
const allParkings = await parkingRepo.getAll();

// Get parkings by province
const madridParking = await parkingRepo.getByProvinceName('Madrid');

// Get parkings by rate range
const cheap = await parkingRepo.getByHourlyRateRange(2.0, 3.0);

// Get parking by ID
const parking = await parkingRepo.getById(1);

// Combined query
const filtered = await parkingRepo.getByProvinceAndNameAndHourlyRateRange(
  'Barcelona',    // Province
  null,           // No name filter
  2.5, 3.5        // €2.50-€3.50/hour
);
```

**Python:**
```python
parking_repo = JsonParkingRepository('./prq_parking.json')

# Get all parkings
all_parkings = await parking_repo.get_all()

# Get parkings by province
madrid_parking = await parking_repo.get_by_province_name('Madrid')

# Get parkings by rate range
cheap = await parking_repo.get_by_hourly_rate_range(2.0, 3.0)

# Get parking by ID
parking = await parking_repo.get_by_id(1)

# Combined query
filtered = await parking_repo.get_by_province_and_name_and_hourly_rate_range(
    'Barcelona',    # Province
    None,           # No name filter
    2.5, 3.5        # €2.50-€3.50/hour
)
```

---

## Car Entry Repository (`ICarEntryRepository`)

Queries for PRQ_Car_Entry table with advanced filtering and calculated fields.

### Methods

| Method | Parameters | Returns | Description |
|--------|-----------|---------|-------------|
| **getById** | `id: int` | `CarEntry \| null` | Get entry by sequential number |
| **getAll** | — | `CarEntry[]` | Get all entries |
| **getHourlyPriceForParking** | `parkingId: int` | `float \| null` | Get hourly rate for a parking facility |
| **getCarsByTypeInDateRange** | `type: string, startDate: string, endDate: string` | `CarEntry[]` | Get cars by type in date range with duration and amount paid |
| **getCarsByProvinceInDateRange** | `province: string, startDate: string, endDate: string` | `CarEntry[]` | Get cars by province in date range with duration and amount due |

### GetHourlyPriceForParking

Returns the hourly rate for a specific parking facility. Used by other queries to calculate parking fees.

```javascript
// Node.js
const rate = await carEntryRepo.getHourlyPriceForParking(1);
console.log(`€${rate}/hour`);  // €3.50/hour
```

```python
# Python
rate = await car_entry_repo.get_hourly_price_for_parking(1)
print(f"€{rate}/hour")  # €3.50/hour
```

### GetCarsByTypeInDateRange

Lists cars of a specific type that entered during a date range. Includes entry/exit times and calculated amounts. **Uses `getHourlyPriceForParking` internally.**

**Returns:**
- sequential_number
- car_id
- car_type
- parking_id
- entry_date_time
- exit_date_time
- stay_duration_minutes (calculated)
- stay_duration_hours (calculated)
- hourly_rate (retrieved via getHourlyPriceForParking)
- amount_paid (calculated)

**Note:** Only includes completed sessions (exit_date_time IS NOT NULL)

```javascript
// Node.js
const entries = await carEntryRepo.getCarsByTypeInDateRange(
  'sedan',
  '2026-04-16',
  '2026-04-18'
);

entries.forEach(e => {
  console.log(
    `Car #${e.car_id}: ${e.entry_date_time} → ${e.exit_date_time} ` +
    `(${e.stay_duration_hours}h) - €${e.amount_paid}`
  );
});
```

```python
# Python
entries = await car_entry_repo.get_cars_by_type_in_date_range(
    'sedan',
    '2026-04-16',
    '2026-04-18'
)

for e in entries:
    print(
        f"Car #{e['car_id']}: {e['entry_date_time']} → {e['exit_date_time']} "
        f"({e['stay_duration_hours']}h) - €{e['amount_paid']}"
    )
```

### GetCarsByProvinceInDateRange

Lists cars that parked in a specific province during a date range. Includes parking details and calculated amounts. **Uses `getHourlyPriceForParking` internally.**

**Returns:**
- sequential_number
- car_id
- parking_id
- parking_name
- province_name
- entry_date_time
- exit_date_time
- stay_duration_minutes (calculated)
- stay_duration_hours (calculated)
- hourly_rate (from parking facility)
- amount_due (calculated)

**Note:** Only includes completed sessions (exit_date_time IS NOT NULL)

```javascript
// Node.js
const entries = await carEntryRepo.getCarsByProvinceInDateRange(
  'Madrid',
  '2026-04-16',
  '2026-04-18'
);

entries.forEach(e => {
  console.log(
    `${e.parking_name}: Car #${e.car_id} ` +
    `${e.entry_date_time} → ${e.exit_date_time} - €${e.amount_due}`
  );
});
```

```python
# Python
entries = await car_entry_repo.get_cars_by_province_in_date_range(
    'Madrid',
    '2026-04-16',
    '2026-04-18'
)

for e in entries:
    print(
        f"{e['parking_name']}: Car #{e['car_id']} "
        f"{e['entry_date_time']} → {e['exit_date_time']} - €{e['amount_due']}"
    )
```

---

## Usage Patterns

### Pattern 1: JSON Repository (Default)

Use JSON files for development/testing:

```javascript
// Node.js
const {
  JsonCarRepository,
  JsonParkingRepository,
  JsonCarEntryRepository
} = require('./repositories');

const carRepo = new JsonCarRepository('./prq_cars.json');
const parkingRepo = new JsonParkingRepository('./prq_parking.json');
const entryRepo = new JsonCarEntryRepository('./prq_car_entry.json', './prq_parking.json');
```

```python
# Python
from repositories import (
    JsonCarRepository,
    JsonParkingRepository,
    JsonCarEntryRepository
)

car_repo = JsonCarRepository('./prq_cars.json')
parking_repo = JsonParkingRepository('./prq_parking.json')
entry_repo = JsonCarEntryRepository('./prq_car_entry.json', './prq_parking.json')
```

### Pattern 2: Database Repository (Production)

Use database for production:

```javascript
// Node.js
const mysql = require('mysql2/promise');
const {
  DatabaseCarRepository,
  DatabaseParkingRepository,
  DatabaseCarEntryRepository
} = require('./repositories');

const pool = mysql.createPool({...});
const carRepo = new DatabaseCarRepository(pool);
const parkingRepo = new DatabaseParkingRepository(pool);
const entryRepo = new DatabaseCarEntryRepository(pool);
```

```python
# Python
from db_connection_python import DatabaseConnection
from repositories import (
    DatabaseCarRepository,
    DatabaseParkingRepository,
    DatabaseCarEntryRepository
)

db = DatabaseConnection()
db.connect()
car_repo = DatabaseCarRepository(db)
parking_repo = DatabaseParkingRepository(db)
entry_repo = DatabaseCarEntryRepository(db)
```

### Pattern 3: Switching Data Sources

```javascript
// Node.js - Easy to switch
let carRepo;

if (process.env.USE_DATABASE === 'true') {
  carRepo = new DatabaseCarRepository(pool);
} else {
  carRepo = new JsonCarRepository('./prq_cars.json');
}

// Business logic uses same interface regardless
const cars = await carRepo.getByYearRange(2020, 2022);
```

```python
# Python - Easy to switch
import os

if os.getenv('USE_DATABASE') == 'true':
    car_repo = DatabaseCarRepository(db)
else:
    car_repo = JsonCarRepository('./prq_cars.json')

# Business logic uses same interface regardless
cars = await car_repo.get_by_year_range(2020, 2022)
```

---

## Complex Query Examples

### Report: Revenue by Vehicle Type

```javascript
// Node.js
const entryRepo = new DatabaseCarEntryRepository(pool);

const entries = await entryRepo.getCarsByTypeInDateRange(
  'sedan',
  '2026-04-01',
  '2026-04-30'
);

const totalRevenue = entries.reduce((sum, e) => sum + e.amount_paid, 0);
console.log(`Total Revenue from Sedans: €${totalRevenue.toFixed(2)}`);
```

### Report: Occupancy by Province

```javascript
// Node.js
const parkingRepo = new DatabaseParkingRepository(pool);
const entryRepo = new DatabaseCarEntryRepository(pool);

const provinces = ['Madrid', 'Barcelona'];
for (const province of provinces) {
  const entries = await entryRepo.getCarsByProvinceInDateRange(
    province,
    '2026-04-01',
    '2026-04-30'
  );
  console.log(`${province}: ${entries.length} sessions`);
}
```

### Query: Find Long-Stay Sessions

```javascript
// Node.js
const entries = await entryRepo.getCarsByProvinceInDateRange(
  'Madrid',
  '2026-04-01',
  '2026-04-30'
);

const longStays = entries.filter(e => e.stay_duration_hours > 8);
console.log(`Long stays (>8 hours): ${longStays.length}`);
```

---

## Files Provided

| File | Description |
|------|-------------|
| `repositories.js` | Node.js interfaces and implementations |
| `repositories.py` | Python interfaces and implementations |
| `repositories-examples.js` | Node.js usage examples |
| `repositories-guide.md` | This comprehensive guide |

---

## Key Takeaways

1. ✅ **Use interfaces** - Define contracts that multiple implementations follow
2. ✅ **DRY principle** - Don't repeat query logic in multiple places
3. ✅ **Testability** - Mock repositories for unit testing
4. ✅ **Flexibility** - Switch data sources without changing business logic
5. ✅ **Maintainability** - Centralize all data access in repositories

---

## Summary

The repository pattern provides:
- **ICarRepository** - 7 methods for querying cars (by color, year, make, type, combined)
- **IParkingRepository** - 6 methods for querying parking (by province, name, rate, combined)
- **ICarEntryRepository** - 5 methods for querying entries (GetHourlyPriceForParking, cars by type in date range, cars by province in date range)

Both **JSON and Database** implementations available with identical interfaces!
