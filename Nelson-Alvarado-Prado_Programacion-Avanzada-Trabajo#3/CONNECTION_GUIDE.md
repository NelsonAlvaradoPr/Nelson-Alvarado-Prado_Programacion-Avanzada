# Car Park Database - Azure MySQL Cloud Connection Guide

## Overview
This guide helps you securely connect to the Azure MySQL cloud instance and query the car park database.

**Cloud Instance Details:**
- Host: `hecferme-mysql-2026.mysql.database.azure.com`
- Port: `3306`
- Username: `user01`
- Database: `car_park`

## Security Setup

### 1. Environment Variables (.env)
A `.env` file has been created to securely store your credentials:
- **Location:** `.env`
- **Contents:** Database host, port, user, password, and connection strings
- **IMPORTANT:** This file is in `.gitignore` and should NEVER be committed to git

### 2. Connection Strings Available
Three connection string formats are provided in `.env`:

#### MySQL URI Format
```
mysql://user01:MyVeryStr0ngPassword@hecferme-mysql-2026.mysql.database.azure.com:3306/car_park
```

#### Standard MySQL Format
```
user01:MyVeryStr0ngPassword@tcp(hecferme-mysql-2026.mysql.database.azure.com:3306)/car_park
```

#### JDBC Format (Java)
```
jdbc:mysql://hecferme-mysql-2026.mysql.database.azure.com:3306/car_park?user=user01&password=MyVeryStr0ngPassword
```

---

## Connection Methods

### Option 1: Node.js

**File:** `db-connection-nodejs.js`

**Setup:**
```bash
npm install mysql2 dotenv
```

**Usage:**
```javascript
const db = require('./db-connection-nodejs.js');

// Get all cars
const cars = await db.getAllCars();
console.log(cars);

// Get all parking spaces
const parking = await db.getAllParking();
console.log(parking);

// Get currently parked cars
const parkedCars = await db.getCurrentlyParkedCars();
console.log(parkedCars);
```

**Available Functions:**
- `getAllCars()` - Retrieve all vehicles
- `getAllParking()` - Retrieve all parking facilities
- `getAllCarEntries()` - Retrieve all entry/exit records
- `getCurrentlyParkedCars()` - Get vehicles with NULL exit times

---

### Option 2: Python

**File:** `db-connection-python.py`

**Setup:**
```bash
pip install mysql-connector-python python-dotenv
```

**Usage:**
```python
from db_connection_python import DatabaseConnection

db = DatabaseConnection()
if db.connect():
    # Get all cars
    cars = db.get_all_cars()
    print(cars)
    
    # Get currently parked cars
    parked = db.get_currently_parked_cars()
    print(parked)
    
    db.disconnect()
```

**Available Methods:**
- `get_all_cars()` - Retrieve all vehicles
- `get_all_parking()` - Retrieve all parking facilities
- `get_all_car_entries()` - Retrieve all entry/exit records
- `get_currently_parked_cars()` - Get vehicles with NULL exit times
- `get_cars_by_parking(parking_id)` - Get cars by parking facility

---

### Option 3: MySQL Command Line

Use the connection string directly:
```bash
mysql -h hecferme-mysql-2026.mysql.database.azure.com -u user01 -p car_park
```
When prompted, enter password: `MyVeryStr0ngPassword`

**Example Queries:**
```sql
-- Get all cars
SELECT * FROM PRQ_Cars;

-- Get all parking spaces
SELECT * FROM PRQ_Parking;

-- Get currently parked cars
SELECT * FROM PRQ_Car_Entry WHERE exit_date_time IS NULL;

-- Get cars by parking location
SELECT c.*, p.name, ce.entry_date_time 
FROM PRQ_Car_Entry ce
JOIN PRQ_Cars c ON ce.car_id = c.ID
JOIN PRQ_Parking p ON ce.parking_id = p.ID
WHERE p.ID = 1;
```

---

## Important Notes

⚠️ **Security:**
- Never commit `.env` to version control
- Keep credentials private
- Use HTTPS/SSL connections (enabled by default)

✅ **SSL Connection:**
- SSL is automatically enabled for Azure MySQL connections
- Both Node.js and Python implementations enforce SSL

📋 **Tables:**
- `PRQ_Cars` - Vehicle information (5 sample records)
- `PRQ_Parking` - Parking facilities (2 sample records)
- `PRQ_Car_Entry` - Entry/exit logs (15 sample records)

🔑 **Key Features:**
- NULL `exit_date_time` indicates vehicle is still parked
- Supports multiple parking sessions for the same vehicle
- Foreign key relationships enforce data integrity

---

## Troubleshooting

**Connection Timeout:**
- Verify internet connectivity
- Check firewall rules on Azure MySQL instance
- Ensure credentials are correct in `.env`

**SSL Certificate Issues:**
- Azure MySQL requires SSL connections
- Both Node.js and Python implementations handle this automatically

**Database Not Found:**
- Make sure you've created the `car_park` database
- Run `design-db.sql` and `insert-records.sql` first

---

## Test Data

The following test data is pre-loaded:

**5 Cars:**
1. Red Toyota Corolla (2020) - sedan
2. Black BMW X5 (2019) - 4x4
3. White Yamaha MT-07 (2022) - motorcycle
4. Silver Mercedes C-Class (2021) - sedan
5. Blue Land Rover Discovery (2018) - 4x4

**2 Parking Facilities:**
1. Madrid - Parking Centro Plaza Mayor (€3.50/hour)
2. Barcelona - Parking Diagonal Mar (€2.75/hour)

**15 Entry/Exit Records:**
- 8 records for Madrid parking
- 7 records for Barcelona parking
- 5 currently active (no exit time)

---

## Next Steps

1. ✅ Store `.env` securely on your local machine
2. ✅ Choose your preferred connection method (Node.js, Python, or CLI)
3. ✅ Install required dependencies
4. ✅ Test the connection with sample queries
5. ✅ Build your application on top of these examples
