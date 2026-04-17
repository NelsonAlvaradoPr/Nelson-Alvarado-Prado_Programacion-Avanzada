# Car Park REST API - Quick Reference Guide

## 🚀 Quick Start

```bash
# Node.js
npm install express cors body-parser dotenv mysql2
node api-server.js

# Python
pip install flask flask-cors python-dotenv mysql-connector-python
python flask_app.py
```

**Endpoints available at:**
- Node.js: `http://localhost:3000/api`
- Python: `http://localhost:5000/api`

---

## 📚 API Endpoints Summary

### 🚗 CARS (10 Endpoints)

| Operation | Method | Endpoint | Status |
|-----------|--------|----------|--------|
| **CREATE** | POST | `/api/cars` | 201 |
| Get All | GET | `/api/cars` | 200 |
| Get by ID | GET | `/api/cars/:id` | 200 |
| Get by Color | GET | `/api/cars/filter/color/:color` | 200 |
| Get by Make | GET | `/api/cars/filter/make/:make` | 200 |
| Get by Type | GET | `/api/cars/filter/type/:type` | 200 |
| Get by Year | GET | `/api/cars/filter/year-range?min=&max=` | 200 |
| Get Advanced | GET | `/api/cars/filter/advanced?color=&minYear=&maxYear=&make=&type=` | 200 |
| **UPDATE** | PUT | `/api/cars/:id` | 200 |
| **DELETE** | DELETE | `/api/cars/:id` | 200 |

**Create Request Body:**
```json
{
  "color": "string",
  "year": 2024,
  "make": "string",
  "type": "sedan|4x4|motorcycle"
}
```

---

### 🅿️ PARKING (9 Endpoints)

| Operation | Method | Endpoint | Status |
|-----------|--------|----------|--------|
| **CREATE** | POST | `/api/parking` | 201 |
| Get All | GET | `/api/parking` | 200 |
| Get by ID | GET | `/api/parking/:id` | 200 |
| Get by Province | GET | `/api/parking/filter/province/:province` | 200 |
| Get by Name | GET | `/api/parking/filter/name/:name` | 200 |
| Get by Rate | GET | `/api/parking/filter/rate-range?min=&max=` | 200 |
| Get Advanced | GET | `/api/parking/filter/advanced?province=&name=&minRate=&maxRate=` | 200 |
| **UPDATE** | PUT | `/api/parking/:id` | 200 |
| **DELETE** | DELETE | `/api/parking/:id` | 200 |

**Create Request Body:**
```json
{
  "province_name": "string",
  "name": "string",
  "hourly_rate": 3.50
}
```

---

### 🚙 CAR ENTRY (9 Endpoints)

| Operation | Method | Endpoint | Status |
|-----------|--------|----------|--------|
| **CREATE** | POST | `/api/car-entry` | 201 |
| Get All | GET | `/api/car-entry` | 200 |
| Get by ID | GET | `/api/car-entry/:id` | 200 |
| Get by Parking | GET | `/api/car-entry/filter/parking/:parkingId` | 200 |
| Get by Car Type | GET | `/api/car-entry/filter/car-type/:type?start=&end=` | 200 |
| Get by Province | GET | `/api/car-entry/filter/province/:province?start=&end=` | 200 |
| Get Hourly Price | GET | `/api/car-entry/price/:parkingId` | 200 |
| **UPDATE** | PUT | `/api/car-entry/:id` | 200 |
| **DELETE** | DELETE | `/api/car-entry/:id` | 200 |

**Create Request Body:**
```json
{
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2026-04-17T14:00:00",
  "exit_date_time": null
}
```

---

### 🔧 SYSTEM (2 Endpoints)

| Operation | Method | Endpoint |
|-----------|--------|----------|
| Health Check | GET | `/api/health` |
| List Endpoints | GET | `/api/endpoints` |

---

## 📋 Common cURL Examples

### Create Operations

```bash
# Create a Car
curl -X POST http://localhost:3000/api/cars \
  -H "Content-Type: application/json" \
  -d '{"color":"Blue","year":2023,"make":"Honda","type":"sedan"}'

# Create Parking
curl -X POST http://localhost:3000/api/parking \
  -H "Content-Type: application/json" \
  -d '{"province_name":"Madrid","name":"Centro","hourly_rate":3.50}'

# Create Car Entry
curl -X POST http://localhost:3000/api/car-entry \
  -H "Content-Type: application/json" \
  -d '{"parking_id":1,"car_id":1,"entry_date_time":"2026-04-17T10:00:00"}'
```

### Read Operations

```bash
# Get All
curl http://localhost:3000/api/cars
curl http://localhost:3000/api/parking
curl http://localhost:3000/api/car-entry

# Get by ID
curl http://localhost:3000/api/cars/1
curl http://localhost:3000/api/parking/1
curl http://localhost:3000/api/car-entry/1

# Get by Filter
curl http://localhost:3000/api/cars/filter/color/Blue
curl http://localhost:3000/api/cars/filter/type/sedan
curl http://localhost:3000/api/parking/filter/province/Madrid
curl "http://localhost:3000/api/cars/filter/year-range?min=2020&max=2025"
```

### Update Operations

```bash
# Update Car
curl -X PUT http://localhost:3000/api/cars/1 \
  -H "Content-Type: application/json" \
  -d '{"color":"Red","year":2024,"make":"Audi","type":"sedan"}'

# Update Parking
curl -X PUT http://localhost:3000/api/parking/1 \
  -H "Content-Type: application/json" \
  -d '{"province_name":"Barcelona","name":"Airport","hourly_rate":4.00}'

# Update Car Entry
curl -X PUT http://localhost:3000/api/car-entry/1 \
  -H "Content-Type: application/json" \
  -d '{"parking_id":1,"car_id":1,"entry_date_time":"2026-04-17T10:00:00","exit_date_time":"2026-04-17T14:00:00"}'
```

### Delete Operations

```bash
curl -X DELETE http://localhost:3000/api/cars/1
curl -X DELETE http://localhost:3000/api/parking/1
curl -X DELETE http://localhost:3000/api/car-entry/1
```

### System

```bash
curl http://localhost:3000/api/health
curl http://localhost:3000/api/endpoints
```

---

## 🔍 Query Parameters Guide

### Cars Advanced Filter
```
GET /api/cars/filter/advanced?color=Blue&minYear=2020&maxYear=2025&make=Honda&type=sedan
```

**Available Parameters:**
- `color` - Car color (string)
- `minYear` - Minimum year (number, default: 1900)
- `maxYear` - Maximum year (number, default: current year)
- `make` - Car make (string)
- `type` - Car type (sedan | 4x4 | motorcycle)

### Parking Advanced Filter
```
GET /api/parking/filter/advanced?province=Madrid&name=Centro&minRate=2.5&maxRate=4.0
```

**Available Parameters:**
- `province` - Province name (string)
- `name` - Parking name (string)
- `minRate` - Minimum hourly rate (float)
- `maxRate` - Maximum hourly rate (float)

### Car Entry Date Range
```
GET /api/car-entry/filter/car-type/sedan?start=2026-04-10&end=2026-04-20
GET /api/car-entry/filter/province/Madrid?start=2026-04-10&end=2026-04-20
```

**Available Parameters:**
- `start` - Start date (YYYY-MM-DD, default: 2020-01-01)
- `end` - End date (YYYY-MM-DD, default: today)

---

## 📝 Response Format

### Success Response (200/201)
```json
{
  "success": true,
  "data": { /* resource data */ },
  "count": 5,
  "timestamp": "2026-04-17T14:30:00"
}
```

### Error Response (4xx/5xx)
```json
{
  "success": false,
  "error": "Error message describing what went wrong",
  "timestamp": "2026-04-17T14:30:00"
}
```

---

## 🧪 Testing Endpoints

### JavaScript/Fetch
```javascript
// GET
fetch('http://localhost:3000/api/cars')
  .then(r => r.json())
  .then(d => console.log(d))

// POST
fetch('http://localhost:3000/api/cars', {
  method: 'POST',
  headers: {'Content-Type': 'application/json'},
  body: JSON.stringify({color: 'Blue', year: 2023, make: 'Honda', type: 'sedan'})
})
  .then(r => r.json())
  .then(d => console.log(d))
```

### Python/Requests
```python
import requests

# GET
r = requests.get('http://localhost:5000/api/cars')
print(r.json())

# POST
r = requests.post('http://localhost:5000/api/cars', json={
    'color': 'Blue', 'year': 2023, 'make': 'Honda', 'type': 'sedan'
})
print(r.json())
```

### Python/cURL (via terminal)
```bash
python api_examples.py  # Runs all examples
```

### JavaScript (via Node)
```bash
node api-examples.js  # Runs all examples
```

---

## ⚠️ HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK (Successful GET, PUT, DELETE) |
| 201 | Created (Successful POST) |
| 400 | Bad Request (Invalid data) |
| 404 | Not Found (Resource doesn't exist) |
| 500 | Server Error |

---

## 🔗 Valid Values

### Car Type
- `sedan`
- `4x4`
- `motorcycle`

### Date/Time Format
- ISO 8601: `2026-04-17T14:30:00` or `2026-04-17T14:30:00Z`
- Date only: `2026-04-17`

---

## 🚀 Running Examples

### All Examples (JavaScript)
```bash
npm install node-fetch
node api-examples.js
```

### All Examples (Python)
```bash
pip install requests
python api_examples.py
```

---

## 📞 API Documentation Files

| File | Purpose |
|------|---------|
| `API_REFERENCE.md` | Complete API documentation with examples |
| `API_SETUP_GUIDE.md` | Setup and deployment guide |
| `api-server.js` | Node.js Express server implementation |
| `flask_app.py` | Python Flask server implementation |
| `api-examples.js` | JavaScript example usage |
| `api_examples.py` | Python example usage |

---

## 🔐 Quick Port Reference

| Server | Port | URL |
|--------|------|-----|
| Express | 3000 | `http://localhost:3000/api` |
| Flask | 5000 | `http://localhost:5000/api` |

---

## 💡 Tips & Tricks

1. **Test Health First**: Always verify server is running
   ```bash
   curl http://localhost:3000/api/health
   ```

2. **Use Query Parameters**: Filter results on the server
   ```bash
   curl "http://localhost:3000/api/cars/filter/advanced?type=sedan&minYear=2020"
   ```

3. **Pretty Print JSON**: Use `jq` or Python for readable output
   ```bash
   curl http://localhost:3000/api/cars | jq .
   ```

4. **Save to File**: Redirect output to file
   ```bash
   curl http://localhost:3000/api/cars > cars.json
   ```

5. **Count Records**: Use the `count` field in responses
   ```bash
   curl http://localhost:3000/api/cars | jq '.count'
   ```

---

## 🆘 Common Issues

| Issue | Solution |
|-------|----------|
| Port already in use | Change PORT in .env or kill process |
| Module not found | Run `npm install` or `pip install` |
| JSON files not found | Ensure files exist in project directory |
| CORS error | CORS already enabled in both servers |
| Connection refused | Verify server is running on correct port |

---

## 📊 API Statistics

- **Total Endpoints**: 32
- **Total Operations**: 
  - CREATE: 3
  - READ: 17
  - UPDATE: 3
  - DELETE: 3
- **Supported Filters**: 8
- **Response Status Codes**: 4 main types (200, 201, 400, 404)

---

## 📞 Support

For detailed information, refer to:
1. **Full API Documentation**: See `API_REFERENCE.md`
2. **Setup Instructions**: See `API_SETUP_GUIDE.md`
3. **Code Examples**: See `api-examples.js` or `api_examples.py`

