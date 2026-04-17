# Car Park REST API - Complete Guide

## 📋 Overview

This guide covers the complete REST APIs for all CRUD (Create, Read, Update, Delete) and query operations on the car park database. Both **Node.js (Express)** and **Python (Flask)** implementations are available.

---

## 🚀 Quick Start

### Node.js (Express)

```bash
# Install dependencies
npm install express cors body-parser dotenv mysql2

# Run the server
node api-server.js
```

Server runs on: `http://localhost:3000/api`

### Python (Flask)

```bash
# Install dependencies
pip install flask flask-cors python-dotenv mysql-connector-python

# Run the server
python flask_app.py
```

Server runs on: `http://localhost:5000/api`

---

## 🔍 API Endpoints Overview

### Cars Table - Endpoints

| Operation | Method | Endpoint |
|-----------|--------|----------|
| Create Car | POST | `/api/cars` |
| Get All Cars | GET | `/api/cars` |
| Get Car by ID | GET | `/api/cars/:id` |
| Get by Color | GET | `/api/cars/filter/color/:color` |
| Get by Make | GET | `/api/cars/filter/make/:make` |
| Get by Type | GET | `/api/cars/filter/type/:type` |
| Get by Year Range | GET | `/api/cars/filter/year-range?min=&max=` |
| Advanced Filter | GET | `/api/cars/filter/advanced?color=&minYear=&maxYear=&make=&type=` |
| Update Car | PUT | `/api/cars/:id` |
| Delete Car | DELETE | `/api/cars/:id` |

### Parking Table - Endpoints

| Operation | Method | Endpoint |
|-----------|--------|----------|
| Create Parking | POST | `/api/parking` |
| Get All Parking | GET | `/api/parking` |
| Get Parking by ID | GET | `/api/parking/:id` |
| Get by Province | GET | `/api/parking/filter/province/:province` |
| Get by Name | GET | `/api/parking/filter/name/:name` |
| Get by Rate Range | GET | `/api/parking/filter/rate-range?min=&max=` |
| Advanced Filter | GET | `/api/parking/filter/advanced?province=&name=&minRate=&maxRate=` |
| Update Parking | PUT | `/api/parking/:id` |
| Delete Parking | DELETE | `/api/parking/:id` |

### Car Entry Table - Endpoints

| Operation | Method | Endpoint |
|-----------|--------|----------|
| Create Entry | POST | `/api/car-entry` |
| Get All Entries | GET | `/api/car-entry` |
| Get Entry by ID | GET | `/api/car-entry/:id` |
| Get by Parking | GET | `/api/car-entry/filter/parking/:parkingId` |
| Get by Car Type | GET | `/api/car-entry/filter/car-type/:type?start=&end=` |
| Get by Province | GET | `/api/car-entry/filter/province/:province?start=&end=` |
| Get Hourly Price | GET | `/api/car-entry/price/:parkingId` |
| Update Entry | PUT | `/api/car-entry/:id` |
| Delete Entry | DELETE | `/api/car-entry/:id` |

---

## 🚗 CARS ENDPOINTS

### 1. Create a New Car

**Endpoint:** `POST /api/cars`

**Request Body:**
```json
{
  "color": "Blue",
  "year": 2023,
  "make": "Honda Civic",
  "type": "sedan"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 6,
    "color": "Blue",
    "year": 2023,
    "make": "Honda Civic",
    "type": "sedan"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

**Valid Car Types:** `sedan`, `4x4`, `motorcycle`

---

### 2. Get All Cars

**Endpoint:** `GET /api/cars`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2022,
      "make": "Toyota Corolla",
      "type": "sedan"
    },
    {
      "id": 2,
      "color": "Black",
      "year": 2021,
      "make": "BMW X5",
      "type": "4x4"
    }
  ],
  "count": 2,
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 3. Get Car by ID

**Endpoint:** `GET /api/cars/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "color": "Red",
    "year": 2022,
    "make": "Toyota Corolla",
    "type": "sedan"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "error": "Car with ID 999 not found",
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 4. Get Cars by Color

**Endpoint:** `GET /api/cars/filter/color/Red`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2022,
      "make": "Toyota Corolla",
      "type": "sedan"
    }
  ],
  "count": 1,
  "filter": {
    "color": "Red"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 5. Get Cars by Make

**Endpoint:** `GET /api/cars/filter/make/Toyota%20Corolla`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2022,
      "make": "Toyota Corolla",
      "type": "sedan"
    }
  ],
  "count": 1,
  "filter": {
    "make": "Toyota Corolla"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 6. Get Cars by Type

**Endpoint:** `GET /api/cars/filter/type/sedan`

**Valid Types:** `sedan`, `4x4`, `motorcycle`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2022,
      "make": "Toyota Corolla",
      "type": "sedan"
    }
  ],
  "count": 1,
  "filter": {
    "type": "sedan"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 7. Get Cars by Year Range

**Endpoint:** `GET /api/cars/filter/year-range?min=2020&max=2023`

**Query Parameters:**
- `min`: Minimum year (default: 1900)
- `max`: Maximum year (default: current year)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2022,
      "make": "Toyota Corolla",
      "type": "sedan"
    }
  ],
  "count": 1,
  "filter": {
    "minYear": 2020,
    "maxYear": 2023
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 8. Get Cars with Advanced Filter

**Endpoint:** `GET /api/cars/filter/advanced?color=Red&minYear=2020&maxYear=2025&make=Toyota&type=sedan`

**Query Parameters:**
- `color`: Car color (optional)
- `minYear`: Minimum year (default: 1900)
- `maxYear`: Maximum year (default: current year)
- `make`: Car make (optional)
- `type`: Car type - `sedan`, `4x4`, `motorcycle` (optional)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2022,
      "make": "Toyota Corolla",
      "type": "sedan"
    }
  ],
  "count": 1,
  "filters": {
    "color": "Red",
    "minYear": 2020,
    "maxYear": 2025,
    "make": "Toyota",
    "type": "sedan"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 9. Update a Car

**Endpoint:** `PUT /api/cars/1`

**Request Body:**
```json
{
  "color": "Navy Blue",
  "year": 2023,
  "make": "Toyota Camry",
  "type": "sedan"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "color": "Navy Blue",
    "year": 2023,
    "make": "Toyota Camry",
    "type": "sedan"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 10. Delete a Car

**Endpoint:** `DELETE /api/cars/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "color": "Navy Blue",
    "year": 2023,
    "make": "Toyota Camry",
    "type": "sedan"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

## 🅿️ PARKING ENDPOINTS

### 1. Create a New Parking Facility

**Endpoint:** `POST /api/parking`

**Request Body:**
```json
{
  "province_name": "Valencia",
  "name": "Parking Centro",
  "hourly_rate": 2.50
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 3,
    "province_name": "Valencia",
    "name": "Parking Centro",
    "hourly_rate": 2.50
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 2. Get All Parking Facilities

**Endpoint:** `GET /api/parking`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "province_name": "Madrid",
      "name": "Parking Centro",
      "hourly_rate": 3.50
    },
    {
      "id": 2,
      "province_name": "Barcelona",
      "name": "Parking Estación",
      "hourly_rate": 2.75
    }
  ],
  "count": 2,
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 3. Get Parking Facility by ID

**Endpoint:** `GET /api/parking/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "province_name": "Madrid",
    "name": "Parking Centro",
    "hourly_rate": 3.50
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 4. Get Parking Facilities by Province

**Endpoint:** `GET /api/parking/filter/province/Madrid`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "province_name": "Madrid",
      "name": "Parking Centro",
      "hourly_rate": 3.50
    }
  ],
  "count": 1,
  "filter": {
    "province": "Madrid"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 5. Get Parking Facilities by Name

**Endpoint:** `GET /api/parking/filter/name/Parking%20Centro`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "province_name": "Madrid",
      "name": "Parking Centro",
      "hourly_rate": 3.50
    }
  ],
  "count": 1,
  "filter": {
    "name": "Parking Centro"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 6. Get Parking Facilities by Rate Range

**Endpoint:** `GET /api/parking/filter/rate-range?min=2.0&max=4.0`

**Query Parameters:**
- `min`: Minimum hourly rate (default: 0)
- `max`: Maximum hourly rate (default: 9999)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "province_name": "Madrid",
      "name": "Parking Centro",
      "hourly_rate": 3.50
    },
    {
      "id": 2,
      "province_name": "Barcelona",
      "name": "Parking Estación",
      "hourly_rate": 2.75
    }
  ],
  "count": 2,
  "filter": {
    "minRate": 2.0,
    "maxRate": 4.0
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 7. Get Parking Facilities with Advanced Filter

**Endpoint:** `GET /api/parking/filter/advanced?province=Madrid&name=Centro&minRate=3.0&maxRate=4.0`

**Query Parameters:**
- `province`: Province name (optional)
- `name`: Parking name (optional)
- `minRate`: Minimum hourly rate (default: 0)
- `maxRate`: Maximum hourly rate (default: 9999)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "province_name": "Madrid",
      "name": "Parking Centro",
      "hourly_rate": 3.50
    }
  ],
  "count": 1,
  "filters": {
    "province": "Madrid",
    "name": "Centro",
    "minRate": 3.0,
    "maxRate": 4.0
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 8. Update a Parking Facility

**Endpoint:** `PUT /api/parking/1`

**Request Body:**
```json
{
  "province_name": "Madrid",
  "name": "Parking Centro Premium",
  "hourly_rate": 4.00
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "province_name": "Madrid",
    "name": "Parking Centro Premium",
    "hourly_rate": 4.00
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 9. Delete a Parking Facility

**Endpoint:** `DELETE /api/parking/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "province_name": "Madrid",
    "name": "Parking Centro Premium",
    "hourly_rate": 4.00
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

## 🚙 CAR ENTRY ENDPOINTS

### 1. Create a New Car Entry

**Endpoint:** `POST /api/car-entry`

**Request Body:**
```json
{
  "parking_id": 1,
  "car_id": 2,
  "entry_date_time": "2026-04-17T14:00:00",
  "exit_date_time": null
}
```

**Note:** `exit_date_time` is optional for active sessions.

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "sequential_number": 16,
    "parking_id": 1,
    "car_id": 2,
    "entry_date_time": "2026-04-17T14:00:00",
    "exit_date_time": null
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 2. Get All Car Entries

**Endpoint:** `GET /api/car-entry`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "sequential_number": 1,
      "parking_id": 1,
      "car_id": 1,
      "entry_date_time": "2026-04-17T09:00:00",
      "exit_date_time": "2026-04-17T11:30:00"
    },
    {
      "sequential_number": 2,
      "parking_id": 1,
      "car_id": 2,
      "entry_date_time": "2026-04-17T14:00:00",
      "exit_date_time": null
    }
  ],
  "count": 2,
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 3. Get Car Entry by ID

**Endpoint:** `GET /api/car-entry/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "sequential_number": 1,
    "parking_id": 1,
    "car_id": 1,
    "entry_date_time": "2026-04-17T09:00:00",
    "exit_date_time": "2026-04-17T11:30:00"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 4. Get Car Entries by Parking Facility

**Endpoint:** `GET /api/car-entry/filter/parking/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "sequential_number": 1,
      "parking_id": 1,
      "car_id": 1,
      "entry_date_time": "2026-04-17T09:00:00",
      "exit_date_time": "2026-04-17T11:30:00"
    }
  ],
  "count": 1,
  "filter": {
    "parkingId": 1
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 5. Get Car Entries by Car Type in Date Range

**Endpoint:** `GET /api/car-entry/filter/car-type/sedan?start=2026-04-15&end=2026-04-20`

**Query Parameters:**
- `start`: Start date (default: 2020-01-01) - format: YYYY-MM-DD
- `end`: End date (default: today) - format: YYYY-MM-DD

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "sequential_number": 1,
      "parking_id": 1,
      "car_id": 1,
      "entry_date_time": "2026-04-17T09:00:00",
      "exit_date_time": "2026-04-17T11:30:00"
    }
  ],
  "count": 1,
  "filters": {
    "carType": "sedan",
    "startDate": "2026-04-15",
    "endDate": "2026-04-20"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 6. Get Car Entries by Province in Date Range

**Endpoint:** `GET /api/car-entry/filter/province/Madrid?start=2026-04-15&end=2026-04-20`

**Query Parameters:**
- `start`: Start date (default: 2020-01-01) - format: YYYY-MM-DD
- `end`: End date (default: today) - format: YYYY-MM-DD

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "sequential_number": 1,
      "parking_id": 1,
      "car_id": 1,
      "entry_date_time": "2026-04-17T09:00:00",
      "exit_date_time": "2026-04-17T11:30:00"
    }
  ],
  "count": 1,
  "filters": {
    "province": "Madrid",
    "startDate": "2026-04-15",
    "endDate": "2026-04-20"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 7. Get Hourly Price for Parking Facility

**Endpoint:** `GET /api/car-entry/price/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "hourlyPrice": 3.50
  },
  "parkingId": "1",
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 8. Update a Car Entry

**Endpoint:** `PUT /api/car-entry/1`

**Request Body:**
```json
{
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2026-04-17T09:00:00",
  "exit_date_time": "2026-04-17T12:00:00"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "sequential_number": 1,
    "parking_id": 1,
    "car_id": 1,
    "entry_date_time": "2026-04-17T09:00:00",
    "exit_date_time": "2026-04-17T12:00:00"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### 9. Delete a Car Entry

**Endpoint:** `DELETE /api/car-entry/1`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "sequential_number": 1,
    "parking_id": 1,
    "car_id": 1,
    "entry_date_time": "2026-04-17T09:00:00",
    "exit_date_time": "2026-04-17T12:00:00"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

## 🛠️ SYSTEM ENDPOINTS

### Health Check

**Endpoint:** `GET /api/health`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "status": "healthy",
    "storage": "json",
    "version": "1.0.0"
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

### List All Endpoints

**Endpoint:** `GET /api/endpoints`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "cars": {
      "create": "POST /api/cars",
      "getAll": "GET /api/cars",
      "getById": "GET /api/cars/:id",
      "getByColor": "GET /api/cars/filter/color/:color",
      "getByMake": "GET /api/cars/filter/make/:make",
      "getByType": "GET /api/cars/filter/type/:type",
      "getByYearRange": "GET /api/cars/filter/year-range?min=YYYY&max=YYYY",
      "advancedFilter": "GET /api/cars/filter/advanced?...",
      "update": "PUT /api/cars/:id",
      "delete": "DELETE /api/cars/:id"
    },
    "parking": { /* ... */ },
    "carEntry": { /* ... */ },
    "system": {
      "health": "GET /api/health",
      "endpoints": "GET /api/endpoints"
    }
  },
  "timestamp": "2026-04-17T14:30:00"
}
```

---

## 📝 Common Response Formats

### Success Response

```json
{
  "success": true,
  "data": {},
  "count": 1,
  "filters": {},
  "timestamp": "2026-04-17T14:30:00"
}
```

### Error Response

```json
{
  "success": false,
  "error": "Error message describing what went wrong",
  "timestamp": "2026-04-17T14:30:00"
}
```

---

## 🔗 cURL Examples

### Create a Car
```bash
curl -X POST http://localhost:3000/api/cars \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Blue",
    "year": 2023,
    "make": "Honda Civic",
    "type": "sedan"
  }'
```

### Get All Cars
```bash
curl http://localhost:3000/api/cars
```

### Get Cars by Color
```bash
curl http://localhost:3000/api/cars/filter/color/Blue
```

### Update a Car
```bash
curl -X PUT http://localhost:3000/api/cars/1 \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Navy Blue",
    "year": 2023,
    "make": "Honda Accord",
    "type": "sedan"
  }'
```

### Delete a Car
```bash
curl -X DELETE http://localhost:3000/api/cars/1
```

### Get Parking with Advanced Filter
```bash
curl "http://localhost:3000/api/parking/filter/advanced?province=Madrid&minRate=3&maxRate=5"
```

### Get Car Entries by Date Range
```bash
curl "http://localhost:3000/api/car-entry/filter/car-type/sedan?start=2026-04-15&end=2026-04-20"
```

---

## ⚠️ HTTP Status Codes

| Status | Meaning |
|--------|---------|
| 200 | OK - Successful GET, PUT, DELETE |
| 201 | Created - Successful POST |
| 400 | Bad Request - Invalid data or missing body |
| 404 | Not Found - Resource doesn't exist |
| 500 | Internal Server Error - Server error |

---

## 🧪 Testing the APIs

### Using Postman
1. Import the endpoints listed above
2. Set up variables for `base_url`, `car_id`, `parking_id`, etc.
3. Create test collections for each resource

### Using JavaScript/Fetch
```javascript
// Create a car
async function createCar() {
  const response = await fetch('http://localhost:3000/api/cars', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      color: 'Blue',
      year: 2023,
      make: 'Honda Civic',
      type: 'sedan'
    })
  });
  return response.json();
}

// Get all cars
async function getAllCars() {
  const response = await fetch('http://localhost:3000/api/cars');
  return response.json();
}
```

### Using Python/Requests
```python
import requests

# Create a car
response = requests.post('http://localhost:5000/api/cars', json={
    'color': 'Blue',
    'year': 2023,
    'make': 'Honda Civic',
    'type': 'sedan'
})
print(response.json())

# Get all cars
response = requests.get('http://localhost:5000/api/cars')
print(response.json())
```

---

## 📊 Complete Operation Summary

### Total Endpoints: 32

**Cars:** 10 endpoints
- 1 CREATE
- 6 READ (getAll, getById, getByColor, getByMake, getByType, getByYearRange, advanced)
- 1 UPDATE
- 1 DELETE

**Parking:** 9 endpoints
- 1 CREATE
- 5 READ (getAll, getById, getByProvince, getByName, getByRateRange, advanced)
- 1 UPDATE
- 1 DELETE

**Car Entry:** 9 endpoints
- 1 CREATE
- 5 READ (getAll, getById, getByParking, getByCarType, getByProvince, getHourlyPrice)
- 1 UPDATE
- 1 DELETE

**System:** 2 endpoints
- Health check
- Endpoint listing

---

## 🚀 Next Steps

1. **Start the server** (Node.js or Python)
2. **Visit the health endpoint** to verify it's running
3. **Get a list of all endpoints** using the endpoints endpoint
4. **Test basic CRUD operations** on each resource
5. **Integrate with your frontend** application

---

## 📞 Support & Troubleshooting

- **Port already in use?** Change PORT in .env or environment variables
- **Database connection error?** Ensure database is running and credentials are correct
- **Missing dependencies?** Install required packages (express, flask, cors, etc.)
- **API not responding?** Check server logs for error messages

