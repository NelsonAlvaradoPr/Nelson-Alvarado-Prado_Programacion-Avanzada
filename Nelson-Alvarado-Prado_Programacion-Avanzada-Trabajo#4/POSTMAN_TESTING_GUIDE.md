# 🧪 Postman Testing Guide for REST APIs

## Table of Contents
1. [Installation & Setup](#installation--setup)
2. [Creating Collections](#creating-collections)
3. [Testing Each Endpoint](#testing-each-endpoint)
4. [Using Environment Variables](#using-environment-variables)
5. [Pre-request Scripts & Tests](#pre-request-scripts--tests)
6. [Importing Collections](#importing-collections)
7. [Troubleshooting](#troubleshooting)

---

## Installation & Setup

### Step 1: Download Postman
1. Go to [https://www.postman.com/downloads/](https://www.postman.com/downloads/)
2. Download for **Windows**
3. Run the installer and follow prompts
4. Launch Postman

### Step 2: Create Workspace
1. Click **"Create New"** or **"New Workspace"**
2. Name it: `Parking Lot API`
3. Click **Create Workspace**

### Step 3: Start Your Server
Before testing, start one of your API servers:

**Node.js:**
```bash
npm install
npm start
# Server runs on http://localhost:3000
```

**Python:**
```bash
pip install -r requirements.txt
python flask_app.py
# Server runs on http://localhost:5000
```

---

## Creating Collections

### Method 1: Create Manually (Recommended for Learning)

#### Step 1: Create New Collection
1. Click **"Collections"** (left sidebar)
2. Click **"+"** to create new collection
3. Name it: `Parking Lot API - Node.js` (or `- Python`)
4. Click **Create**

#### Step 2: Create Folders
Right-click on collection → **Add Folder** and create:
- `🚗 Cars`
- `🅿️ Parking`
- `🚙 Car Entry`
- `🔧 System`

#### Step 3: Add Requests to Each Folder

---

## Testing Each Endpoint

### 🔧 System Endpoints (Start Here!)

#### 1. Health Check
```
Method: GET
URL: http://localhost:3000/api/health
```

**Steps:**
1. Right-click `System` folder → **Add Request**
2. Name: `Health Check`
3. Method: `GET`
4. URL: `http://localhost:3000/api/health`
5. Click **Send**

**Expected Response:**
```json
{
  "success": true,
  "message": "API is running",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 2. List All Endpoints
```
Method: GET
URL: http://localhost:3000/api/endpoints
```

**Steps:**
1. Create new request in `System` folder
2. Name: `List All Endpoints`
3. Method: `GET`
4. URL: `http://localhost:3000/api/endpoints`
5. Click **Send**

---

### 🚗 Cars Endpoints

#### CREATE - Add New Car
```
Method: POST
URL: http://localhost:3000/api/cars
```

**Body (JSON):**
```json
{
  "color": "Blue",
  "year": 2023,
  "make": "Honda",
  "type": "sedan"
}
```

**Steps:**
1. Right-click `🚗 Cars` → **Add Request**
2. Name: `Create Car`
3. Method: `POST`
4. URL: `http://localhost:3000/api/cars`
5. Go to **Body** tab
6. Select **raw** → **JSON**
7. Paste the JSON above
8. Click **Send**

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "color": "Blue",
    "year": 2023,
    "make": "Honda",
    "type": "sedan"
  },
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### READ - Get All Cars
```
Method: GET
URL: http://localhost:3000/api/cars
```

**Steps:**
1. Create new request: `Get All Cars`
2. Method: `GET`
3. URL: `http://localhost:3000/api/cars`
4. Click **Send**

#### READ - Get Car by ID
```
Method: GET
URL: http://localhost:3000/api/cars/1
```

**Steps:**
1. Create new request: `Get Car by ID`
2. Method: `GET`
3. URL: `http://localhost:3000/api/cars/1`
4. Click **Send**

#### FILTER - Get Cars by Color
```
Method: GET
URL: http://localhost:3000/api/cars/filter/color/Blue
```

**Steps:**
1. Create new request: `Get Cars by Color`
2. Method: `GET`
3. URL: `http://localhost:3000/api/cars/filter/color/Blue`
4. Try different colors: `Red`, `Silver`, `Black`, etc.
5. Click **Send**

#### FILTER - Get Cars by Make
```
Method: GET
URL: http://localhost:3000/api/cars/filter/make/Honda
```

#### FILTER - Get Cars by Type
```
Method: GET
URL: http://localhost:3000/api/cars/filter/type/sedan
```

#### FILTER - Get Cars by Year Range
```
Method: GET
URL: http://localhost:3000/api/cars/filter/year-range?min=2020&max=2025
```

**In Postman:**
1. Create new request: `Get Cars by Year Range`
2. Method: `GET`
3. URL: `http://localhost:3000/api/cars`
4. Go to **Params** tab
5. Add parameters:
   - Key: `filter` | Value: `year-range`
   - Key: `min` | Value: `2020`
   - Key: `max` | Value: `2025`
6. Or just paste full URL with query string

#### FILTER - Advanced Filter
```
Method: GET
URL: http://localhost:3000/api/cars/filter/advanced?color=Blue&type=sedan&minYear=2020
```

**In Postman:**
1. Create new request: `Advanced Car Filter`
2. Method: `GET`
3. Go to **Params** tab
4. Add:
   - Key: `color` | Value: `Blue`
   - Key: `type` | Value: `sedan`
   - Key: `minYear` | Value: `2020`

#### UPDATE - Modify Car
```
Method: PUT
URL: http://localhost:3000/api/cars/1
```

**Body (JSON):**
```json
{
  "color": "Red",
  "year": 2024,
  "make": "Audi",
  "type": "sedan"
}
```

**Steps:**
1. Create new request: `Update Car`
2. Method: `PUT`
3. URL: `http://localhost:3000/api/cars/1`
4. Body → raw → JSON
5. Paste JSON above
6. Click **Send**

#### DELETE - Remove Car
```
Method: DELETE
URL: http://localhost:3000/api/cars/1
```

**Steps:**
1. Create new request: `Delete Car`
2. Method: `DELETE`
3. URL: `http://localhost:3000/api/cars/1`
4. Click **Send**

---

### 🅿️ Parking Endpoints

#### CREATE - Add Parking Location
```
Method: POST
URL: http://localhost:3000/api/parking
```

**Body (JSON):**
```json
{
  "province_name": "Quebec",
  "name": "Downtown Parking",
  "hourly_rate": 15.50
}
```

**Steps:**
1. Right-click `🅿️ Parking` → **Add Request**
2. Name: `Create Parking`
3. Method: `POST`
4. URL: `http://localhost:3000/api/parking`
5. Body → raw → JSON
6. Paste JSON
7. Click **Send**

#### READ - Get All Parking
```
Method: GET
URL: http://localhost:3000/api/parking
```

#### READ - Get Parking by ID
```
Method: GET
URL: http://localhost:3000/api/parking/1
```

#### FILTER - Get Parking by Province
```
Method: GET
URL: http://localhost:3000/api/parking/filter/province/Quebec
```

#### FILTER - Get Parking by Name
```
Method: GET
URL: http://localhost:3000/api/parking/filter/name/Downtown
```

#### FILTER - Get Parking by Rate Range
```
Method: GET
URL: http://localhost:3000/api/parking/filter/rate-range?min=10&max=20
```

#### FILTER - Advanced Parking Filter
```
Method: GET
URL: http://localhost:3000/api/parking/filter/advanced?province=Quebec&maxRate=20
```

#### UPDATE - Modify Parking
```
Method: PUT
URL: http://localhost:3000/api/parking/1
```

**Body (JSON):**
```json
{
  "province_name": "Quebec",
  "name": "Downtown Plaza",
  "hourly_rate": 18.50
}
```

#### DELETE - Remove Parking
```
Method: DELETE
URL: http://localhost:3000/api/parking/1
```

---

### 🚙 Car Entry Endpoints

#### CREATE - Record Car Entry
```
Method: POST
URL: http://localhost:3000/api/car-entry
```

**Body (JSON):**
```json
{
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2024-01-15T08:30:00Z",
  "exit_date_time": null
}
```

**Steps:**
1. Right-click `🚙 Car Entry` → **Add Request**
2. Name: `Create Car Entry`
3. Method: `POST`
4. URL: `http://localhost:3000/api/car-entry`
5. Body → raw → JSON
6. Paste JSON
7. Click **Send**

#### READ - Get All Entries
```
Method: GET
URL: http://localhost:3000/api/car-entry
```

#### READ - Get Entry by ID
```
Method: GET
URL: http://localhost:3000/api/car-entry/1
```

#### FILTER - Get Entries by Parking
```
Method: GET
URL: http://localhost:3000/api/car-entry/filter/parking/1
```

#### FILTER - Get Entries by Car Type
```
Method: GET
URL: http://localhost:3000/api/car-entry/filter/car-type/sedan
```

#### FILTER - Get Entries by Province
```
Method: GET
URL: http://localhost:3000/api/car-entry/filter/province/Quebec
```

#### FILTER - Get Hourly Price
```
Method: GET
URL: http://localhost:3000/api/car-entry/price/1
```

#### UPDATE - Modify Entry
```
Method: PUT
URL: http://localhost:3000/api/car-entry/1
```

**Body (JSON):**
```json
{
  "parking_id": 1,
  "car_id": 1,
  "entry_date_time": "2024-01-15T08:30:00Z",
  "exit_date_time": "2024-01-15T12:30:00Z"
}
```

#### DELETE - Remove Entry
```
Method: DELETE
URL: http://localhost:3000/api/car-entry/1
```

---

## Using Environment Variables

### Why Use Environment Variables?
- Switch between servers (localhost:3000 → localhost:5000)
- Change base URLs easily
- Share collections with team

### Step 1: Create Environment
1. Click **Environments** (left sidebar)
2. Click **"+"** to create new environment
3. Name: `Local - Node.js`

### Step 2: Add Variables
In the table, add:

| Variable | Initial Value | Current Value |
|----------|---------------|---------------|
| `base_url` | `http://localhost:3000` | `http://localhost:3000` |
| `api_path` | `/api` | `/api` |

### Step 3: Save Environment
1. Click **Save**
2. Close the environment editor

### Step 4: Use in Requests
1. Open any request
2. In URL field, use: `{{base_url}}{{api_path}}/cars`
3. Instead of: `http://localhost:3000/api/cars`

### Step 5: Switch Environments
1. Top-right corner, click environment dropdown
2. Select `Local - Node.js` or create `Local - Python`
3. All requests automatically use new base URL

---

## Pre-request Scripts & Tests

### Add Pre-request Script
**Example: Add Timestamp**

1. Open any request
2. Click **Pre-request Script** tab
3. Add:
```javascript
pm.environment.set("timestamp", new Date().toISOString());
```

### Add Tests
**Example: Verify Response is Success**

1. Open any request
2. Click **Tests** tab
3. Add:
```javascript
pm.test("Response status is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response success is true", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.success).to.equal(true);
});

pm.test("Response has data", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.data).to.exist;
});
```

4. Send request - tests run automatically
5. Check **Test Results** tab for pass/fail

---

## Importing Collections

### Option 1: Import from JSON (If Available)

1. Click **Import** button (top-left)
2. Choose **Upload Files**
3. Select collection JSON file
4. Click **Import**

### Option 2: Manual Creation Checklist

Use this checklist to create all requests:

```
🚗 CARS (10 endpoints)
  ✅ Create Car (POST)
  ✅ Get All Cars (GET)
  ✅ Get Car by ID (GET)
  ✅ Get Cars by Color (GET)
  ✅ Get Cars by Make (GET)
  ✅ Get Cars by Type (GET)
  ✅ Get Cars by Year Range (GET)
  ✅ Advanced Car Filter (GET)
  ✅ Update Car (PUT)
  ✅ Delete Car (DELETE)

🅿️ PARKING (9 endpoints)
  ✅ Create Parking (POST)
  ✅ Get All Parking (GET)
  ✅ Get Parking by ID (GET)
  ✅ Get Parking by Province (GET)
  ✅ Get Parking by Name (GET)
  ✅ Get Parking by Rate Range (GET)
  ✅ Advanced Parking Filter (GET)
  ✅ Update Parking (PUT)
  ✅ Delete Parking (DELETE)

🚙 CAR ENTRY (9 endpoints)
  ✅ Create Car Entry (POST)
  ✅ Get All Entries (GET)
  ✅ Get Entry by ID (GET)
  ✅ Get Entries by Parking (GET)
  ✅ Get Entries by Car Type (GET)
  ✅ Get Entries by Province (GET)
  ✅ Get Hourly Price (GET)
  ✅ Update Entry (PUT)
  ✅ Delete Entry (DELETE)

🔧 SYSTEM (2 endpoints)
  ✅ Health Check (GET)
  ✅ List All Endpoints (GET)
```

---

## Testing Workflows

### Workflow 1: Complete CRUD Cycle

```
1. Health Check → Verify API is running
   GET /api/health
   
2. Create Car → Get ID (e.g., 1)
   POST /api/cars
   
3. Read Car → Verify it exists
   GET /api/cars/1
   
4. Update Car → Change values
   PUT /api/cars/1
   
5. Read Again → Verify updates
   GET /api/cars/1
   
6. Delete Car → Remove it
   DELETE /api/cars/1
   
7. Verify Gone → Confirm deletion
   GET /api/cars/1 (should error)
```

### Workflow 2: Complete Parking Entry Scenario

```
1. Create Parking Location
   POST /api/parking
   
2. Create Car
   POST /api/cars
   
3. Record Car Entry
   POST /api/car-entry
   (with parking_id and car_id from above)
   
4. Query Entry by Parking
   GET /api/car-entry/filter/parking/1
   
5. Get Parking Price
   GET /api/car-entry/price/1
   
6. Update Entry (add exit time)
   PUT /api/car-entry/1
   
7. Delete Entry
   DELETE /api/car-entry/1
```

---

## Testing Different Scenarios

### Scenario 1: Filter Operations

**Test all car filters:**
```
GET /api/cars/filter/color/Blue
GET /api/cars/filter/make/Honda
GET /api/cars/filter/type/sedan
GET /api/cars/filter/year-range?min=2020&max=2025
GET /api/cars/filter/advanced?color=Blue&type=sedan
```

**Test all parking filters:**
```
GET /api/parking/filter/province/Quebec
GET /api/parking/filter/name/Downtown
GET /api/parking/filter/rate-range?min=10&max=20
GET /api/parking/filter/advanced?province=Quebec
```

### Scenario 2: Error Handling

**Test error responses:**
```
GET /api/cars/999         (non-existent car)
POST /api/cars (invalid body)
PUT /api/cars/abc         (invalid ID)
DELETE /api/parking/999   (non-existent)
```

**Expected Error Response:**
```json
{
  "success": false,
  "message": "Error message here",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

---

## Troubleshooting

### Common Issues

#### Issue: "Connection refused"
```
Error: connect ECONNREFUSED 127.0.0.1:3000
```
**Solution:**
- Verify server is running
- Check correct port (3000 for Node, 5000 for Python)
- Ensure no firewall blocking

#### Issue: "Invalid JSON"
```
Error: Unexpected token < in JSON at position 0
```
**Solution:**
- Check Body tab has **raw** selected
- Verify **JSON** format selected (not Text)
- Check JSON syntax (use validator if unsure)

#### Issue: "CORS Error"
```
Error: Access to XMLHttpRequest blocked by CORS policy
```
**Solution:**
- Server already has CORS enabled
- Verify API is running properly
- Try in different browser tab

#### Issue: "URL not found (404)"
```
Error: "success": false, "message": "Not found"
```
**Solution:**
- Check URL spelling
- Verify endpoint exists
- Confirm ID exists (e.g., GET /api/cars/1 - does car 1 exist?)

### Debug Mode

**Enable Postman Console:**
1. **View** → **Show Postman Console**
2. Bottom panel opens
3. Shows all requests/responses in detail
4. Check for error messages

---

## Tips & Tricks

### 1. Use Collections Runner
- Select collection
- Click **Run** button
- Executes all requests sequentially
- Great for regression testing

### 2. Use Postman Mock Server
- Right-click collection → **Mock Collection**
- Useful for testing without running real server

### 3. Save Responses as Examples
- Send request
- **Save Response** → **Save as Example**
- Helpful for documentation

### 4. Export Collection
- Right-click collection → **Export**
- Share with team
- Version control friendly

### 5. Generate Code
- Open request
- Click **Code** button (right side)
- Select language (JavaScript, Python, cURL, etc.)
- Copy generated code

---

## Quick Reference

### Switch Servers

**Node.js Server:**
```
Base URL: http://localhost:3000
Port: 3000
Start: npm start
```

**Python Server:**
```
Base URL: http://localhost:5000
Port: 5000
Start: python flask_app.py
```

### Response Format
```json
{
  "success": true|false,
  "data": {},
  "count": 5,
  "timestamp": "ISO-8601"
}
```

### HTTP Methods
- **GET** - Read data
- **POST** - Create data
- **PUT** - Update data
- **DELETE** - Remove data

---

## Next Steps

1. ✅ Download and install Postman
2. ✅ Create collection with folders
3. ✅ Add 32 endpoints (use checklist above)
4. ✅ Test each endpoint with sample data
5. ✅ Create environment variables
6. ✅ Add pre-request scripts and tests
7. ✅ Run collection tests
8. ✅ Export collection for backup

---

## Still Need Help?

- 📖 Read [API_REFERENCE.md](API_REFERENCE.md) for endpoint details
- 🚀 Check [API_SETUP_GUIDE.md](API_SETUP_GUIDE.md) for server setup
- 🧪 Review [api-examples.js](api-examples.js) for usage patterns
- 💬 Postman Help: [https://learning.postman.com/docs/](https://learning.postman.com/docs/)

---

**Happy Testing! 🚀**

