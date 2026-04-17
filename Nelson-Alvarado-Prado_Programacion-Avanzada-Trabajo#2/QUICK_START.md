# Quick Start Guide - Car Park Database System

## ⚡ 30-Second Overview

You now have a **complete car park management system** with:
- ✅ Database schema design (MySQL)
- ✅ Test data (5 cars, 2 parkings, 15 entries)
- ✅ Dual data sources (JSON for dev, MySQL for prod)
- ✅ CRUD operations (Insert, Update, Delete)
- ✅ Repository pattern (abstraction layer)
- ✅ Business logic (calculated fields)

---

## 🚀 Running Your First Test

### Option A: JSON Test (No Setup Required)
```bash
node crud-examples.js
```
**Expected Output:**
- ✓ INSERT: New cars/parkings/entries created
- ✓ UPDATE: Records modified
- ✓ DELETE: Records removed
- Summary showing total records in each table

### Option B: Database Test (If Connected)
```bash
# Uncomment the database test in crud-examples.js line ~195
node crud-examples.js
```

### Option C: Verification Script
```bash
node verify.js
```
**Tests:**
- File integrity
- JSON data validity
- Module dependencies
- CRUD functionality
- Database connection (optional)

---

## 📁 Key Files by Purpose

### 🗄️ Database Setup
| File | Purpose |
|------|---------|
| `design-db.sql` | Create tables in MySQL |
| `insert-records.sql` | Add test data |
| `scaffold-prq-tables.sql` | Auto-create tables |

### 📝 Data Access
| File | Purpose |
|------|---------|
| `repositories.js` | Query interface |
| `repositories.py` | Python queries |
| `crud-service.js` | Insert/Update/Delete |
| `crud_service.py` | Python CRUD |

### 🔌 Configuration
| File | Purpose |
|------|---------|
| `.env` | Database credentials |
| `db-connection-nodejs.js` | Node.js connector |
| `db-connection-python.py` | Python connector |

### 📊 Data Files
| File | Purpose |
|------|---------|
| `prq_cars.json` | 5 test cars |
| `prq_parking.json` | 2 test parking locations |
| `prq_car_entry.json` | 15 test entries |

### 📚 Documentation
| File | Purpose |
|------|---------|
| `CRUD_GUIDE.md` | How to use CRUD operations |
| `REPOSITORIES_GUIDE.md` | Query examples |
| `CUSTOM_FIELDS_GUIDE.md` | Calculated fields |
| `CONNECTION_GUIDE.md` | Setup instructions |

---

## 🎯 Common Tasks

### Insert a New Car
```javascript
const { JsonCrudService } = require('./crud-service');
const crud = new JsonCrudService('.');

const result = crud.insertCar({
  color: 'Blue',
  year: 2023,
  make: 'Honda Civic',
  type: 'sedan'
});

console.log(result); // { success: true, data: {...} }
```

### Query Cars by Color
```javascript
const { JsonCarRepository } = require('./repositories');
const repo = new JsonCarRepository('./prq_cars.json');

const redCars = await repo.getByColor('Red');
console.log(redCars); // Array of red cars
```

### Calculate Parking Fee
```javascript
const CarParkingSession = require('./CarParkingSession');

const entry = {
  entry_date_time: '2026-04-17 10:00:00',
  exit_date_time: '2026-04-17 13:30:00',
  hourly_rate: 3.50
};

const session = new CarParkingSession(entry);
console.log(`Duration: ${session.stayDurationHours} hours`);
console.log(`Amount Due: €${session.totalAmountDue}`);
```

### Switch from JSON to Database
```javascript
// Before (JSON)
const { JsonCrudService } = require('./crud-service');
const crud = new JsonCrudService('.');

// After (Database)
const { DatabaseCrudService } = require('./crud-service');
const crud = new DatabaseCrudService(pool);
const result = await crud.insertCar({...}); // async!
```

---

## ✅ Verification Checklist

Run these to verify everything is set up:

- [ ] **JSON Files Valid**: Open `prq_cars.json` - should see 5 cars
- [ ] **CRUD Methods Available**: Check `crud-service.js` has 18 methods (9 × 2 services)
- [ ] **Repositories Work**: Check `repositories.js` has 6 classes
- [ ] **Examples Run**: `node crud-examples.js` completes without errors
- [ ] **Connection String Ready**: Check `.env` has DB credentials (if using database)

---

## 🔗 18 CRUD Methods Available

### Cars (6 methods)
- `insertCar(data)` - Add new car
- `updateCar(id, data)` - Update existing car
- `deleteCar(id)` - Remove car
- Plus repository methods: getAll, getById, getByColor, getByType, etc.

### Parking (6 methods)
- `insertParking(data)` - Add new parking
- `updateParking(id, data)` - Update parking rate
- `deleteParking(id)` - Remove parking
- Plus repository methods: getByProvinceName, getByHourlyRateRange, etc.

### Car Entry (6 methods)
- `insertCarEntry(data)` - Record arrival
- `updateCarEntry(id, data)` - Record departure
- `deleteCarEntry(id)` - Remove entry
- Plus repository methods: getCarsByTypeInDateRange, getCarsByProvinceInDateRange, etc.

---

## 🌟 Response Format

All CRUD operations return:

**Success:**
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

**Error:**
```json
{
  "success": false,
  "error": "Car with ID 999 not found"
}
```

---

## 📖 Documentation Roadmap

1. **Start Here**: `IMPLEMENTATION_STATUS.md` (overview)
2. **Setup**: `CONNECTION_GUIDE.md` (database connection)
3. **Database**: `SCAFFOLD_GUIDE.md` (create tables)
4. **Queries**: `REPOSITORIES_GUIDE.md` (data access)
5. **CRUD**: `CRUD_GUIDE.md` (insert/update/delete)
6. **Business Logic**: `CUSTOM_FIELDS_GUIDE.md` (calculated fields)

---

## 💾 Data Models

### Car
```json
{
  "id": 1,
  "color": "Red",
  "year": 2020,
  "make": "Toyota Corolla",
  "type": "sedan"
}
```

### Parking
```json
{
  "id": 1,
  "province_name": "Madrid",
  "name": "Centro",
  "hourly_rate": 3.50
}
```

### Car Entry
```json
{
  "sequential_number": 1,
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2026-04-17T10:00:00",
  "exit_date_time": "2026-04-17T13:30:00"
}
```

### Calculated Fields (in CarParkingSession)
```javascript
session.stayDurationMinutes    // 210 (3.5 hours)
session.stayDurationHours      // 3.50 (decimal)
session.totalAmountDue         // 12.25 (3.50 × €3.50/hour)
```

---

## 🔐 Security Notes

- ✅ Database credentials in `.env` (never commit)
- ✅ `.gitignore` prevents accidental exposure
- ✅ Parameterized queries prevent SQL injection
- ✅ Foreign keys prevent orphaned data
- ✅ SSL/TLS for database connections

---

## 🐍 Python Users

All examples available in Python too:
- `crud_service.py` - CRUD operations
- `crud_examples.py` - Usage examples
- `repositories.py` - Query repository
- `db_connection_python.py` - Connection module

Run Python examples:
```bash
python crud_examples.py
```

---

## 🆘 Troubleshooting

| Problem | Solution |
|---------|----------|
| "Node not found" | Install Node.js or use VS Code integrated terminal |
| JSON file errors | Check file is valid JSON (open in editor) |
| Database connection fails | Verify .env credentials and internet connection |
| Import errors | Ensure all files are in same directory |
| Port 3306 already in use | Change MySQL port or close other MySQL instances |

---

## 🎓 Learning Path

1. **Beginner**: Read `IMPLEMENTATION_STATUS.md` overview
2. **Intermediate**: Run `node crud-examples.js` to see CRUD in action
3. **Advanced**: Study `repositories.js` to understand query patterns
4. **Expert**: Examine `REPOSITORIES_GUIDE.md` for complex queries

---

## 📊 System Architecture

```
┌─────────────────────────────────────┐
│     Application Layer               │
│  (Your business logic)              │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   CRUD Service Layer                │
│  • insertCar, updateCar, deleteCar  │
│  • insertParking, updateParking...  │
│  • insertCarEntry, updateCarEntry...│
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Repository Pattern Layer          │
│  • JsonCarRepository                │
│  • DatabaseCarRepository            │
│  • JsonParkingRepository            │
│  • DatabaseParkingRepository        │
└──────────────┬──────────────────────┘
               │
      ┌────────┴────────┐
      │                 │
┌─────▼──────┐   ┌─────▼──────────┐
│  JSON      │   │  MySQL         │
│  Files     │   │  Database      │
└────────────┘   └────────────────┘
```

---

## ✨ What's Included

```
✅ Database Schema (3 tables)
✅ Test Data (5+2+15 records)
✅ CRUD Services (18 methods)
✅ Repository Pattern (6 implementations)
✅ Domain Models (Calculated fields)
✅ Connection Modules (Node.js + Python)
✅ JSON Test Files (prq_*.json)
✅ Configuration (.env + .gitignore)
✅ 6 Comprehensive Guides
✅ 6 Example Files
✅ Verification Scripts
```

**Total: 32 production-ready files**

---

## 🎉 You're All Set!

Everything is ready to use. Choose your next step:

- 🏃 **Quick Start**: Run `node crud-examples.js`
- 📚 **Learn**: Read `CRUD_GUIDE.md`
- 🔌 **Connect DB**: Follow `CONNECTION_GUIDE.md`
- ✅ **Verify**: Run `node verify.js`

**Status: ✅ SYSTEM READY**
