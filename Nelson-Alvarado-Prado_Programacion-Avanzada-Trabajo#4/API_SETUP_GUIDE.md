# REST API Setup & Deployment Guide

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Node.js Express Setup](#nodejs-express-setup)
3. [Python Flask Setup](#python-flask-setup)
4. [Database Configuration](#database-configuration)
5. [Running the Servers](#running-the-servers)
6. [Testing the APIs](#testing-the-apis)
7. [Troubleshooting](#troubleshooting)
8. [Environment Variables](#environment-variables)

---

## 📦 Prerequisites

### General Requirements

- **Node.js** (v14 or higher) and npm OR **Python** (3.7 or higher)
- **Git** (optional, for version control)
- **Postman** or **Insomnia** (optional, for testing)
- Access to the parking database files (prq_cars.json, prq_parking.json, prq_car_entry.json)

### Verify Installations

**Node.js:**
```bash
node --version
npm --version
```

**Python:**
```bash
python --version
pip --version
```

---

## 🚀 Node.js Express Setup

### Step 1: Install Dependencies

Navigate to the project directory and install Express and required packages:

```bash
cd path/to/project

npm install express cors body-parser dotenv mysql2/promise
```

**Alternative: Install one by one**

```bash
npm install express
npm install cors
npm install body-parser
npm install dotenv
npm install mysql2
```

### Step 2: Create .env File (Optional)

Create a `.env` file in the project root for configuration:

```env
PORT=3000
USE_DATABASE=false
NODE_ENV=development

# Database settings (if using database instead of JSON)
DB_HOST=your-mysql-host.mysql.database.azure.com
DB_USER=your-username
DB_PASSWORD=your-password
DB_NAME=parking_db
DB_PORT=3306
```

### Step 3: Verify Project Structure

Ensure all required files exist in the project directory:

```
├── api-server.js                    ✅ Main API server file
├── crud-service.js                  ✅ CRUD operations
├── repositories.js                  ✅ Data access layer
├── prq_cars.json                    ✅ Cars data
├── prq_parking.json                 ✅ Parking data
├── prq_car_entry.json              ✅ Car entry data
├── .env                             ✅ Configuration (optional)
└── package.json                     ✅ Dependencies (will be created)
```

### Step 4: Start the Express Server

```bash
node api-server.js
```

**Expected Output:**

```
╔════════════════════════════════════════╗
║   Car Park REST API Server Running     ║
╠════════════════════════════════════════╣
║ 🚗 Base URL: http://localhost:3000/api   ║
║ 📊 Storage: JSON Files             ║
║ 📝 API Docs: http://localhost:3000/api/endpoints ║
║ 💚 Health: http://localhost:3000/api/health      ║
╚════════════════════════════════════════╝
```

### Step 5: Verify Server is Running

Open a new terminal and run:

```bash
curl http://localhost:3000/api/health
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "status": "healthy",
    "storage": "json",
    "version": "1.0.0"
  },
  "timestamp": "2026-04-17T14:30:00.000Z"
}
```

---

## 🐍 Python Flask Setup

### Step 1: Create Virtual Environment (Recommended)

**Windows:**
```bash
python -m venv venv
venv\Scripts\activate
```

**macOS/Linux:**
```bash
python3 -m venv venv
source venv/bin/activate
```

### Step 2: Install Dependencies

```bash
pip install flask flask-cors python-dotenv mysql-connector-python requests
```

**Alternative: Install one by one**
```bash
pip install flask
pip install flask-cors
pip install python-dotenv
pip install mysql-connector-python
pip install requests  # For testing examples
```

### Step 3: Create .env File (Optional)

Create a `.env` file in the project root:

```env
PORT=5000
USE_DATABASE=false
FLASK_ENV=development

# Database settings (if using database instead of JSON)
DB_HOST=your-mysql-host.mysql.database.azure.com
DB_USER=your-username
DB_PASSWORD=your-password
DB_NAME=parking_db
DB_PORT=3306
```

### Step 4: Verify Project Structure

Ensure all required files exist:

```
├── flask_app.py                     ✅ Main Flask app
├── crud_service.py                  ✅ CRUD operations
├── repositories.py                  ✅ Data access layer
├── prq_cars.json                    ✅ Cars data
├── prq_parking.json                 ✅ Parking data
├── prq_car_entry.json              ✅ Car entry data
├── .env                             ✅ Configuration (optional)
└── requirements.txt                 ✅ Dependencies list
```

### Step 5: Create requirements.txt (Optional)

For easier dependency management:

```bash
pip freeze > requirements.txt
```

Or manually create `requirements.txt`:

```txt
Flask==2.3.0
flask-cors==4.0.0
python-dotenv==1.0.0
mysql-connector-python==8.0.33
requests==2.31.0
```

### Step 6: Start the Flask Server

```bash
python flask_app.py
```

**Expected Output:**

```
╔════════════════════════════════════════╗
║   Car Park REST API Server Running     ║
╠════════════════════════════════════════╣
║ 🚗 Base URL: http://localhost:5000/api   ║
║ 📊 Storage: JSON Files            ║
║ 📝 API Docs: http://localhost:5000/api/endpoints ║
║ 💚 Health: http://localhost:5000/api/health      ║
╚════════════════════════════════════════╝
```

### Step 7: Verify Server is Running

Open a new terminal and run:

```bash
curl http://localhost:5000/api/health
```

---

## 🗄️ Database Configuration

### Using JSON Files (Default)

No additional setup needed! The APIs will automatically use:
- `prq_cars.json`
- `prq_parking.json`
- `prq_car_entry.json`

### Using Azure MySQL Database

#### Step 1: Update .env File

```env
USE_DATABASE=true
DB_HOST=your-server.mysql.database.azure.com
DB_USER=your-username@your-server
DB_PASSWORD=your-password
DB_NAME=parking_db
DB_PORT=3306
```

#### Step 2: Initialize Database Schema

Run the database setup scripts:

```bash
# Using MySQL CLI
mysql -h your-host -u username -p < design-db.sql
mysql -h your-host -u username -p < insert-records.sql
```

#### Step 3: Update Server Code

The servers will automatically detect `USE_DATABASE=true` and connect to MySQL.

**For Node.js (api-server.js):**
The code includes TODO comments where you need to add database initialization.

**For Python (flask_app.py):**
The code includes TODO comments where you need to add database initialization.

---

## ▶️ Running the Servers

### Start Express Server (Port 3000)

```bash
# Terminal 1
node api-server.js
```

### Start Flask Server (Port 5000)

```bash
# Terminal 2 (with venv activated)
python flask_app.py
```

### Run Both Simultaneously

**Using npm scripts (package.json):**

```json
{
  "scripts": {
    "start": "node api-server.js",
    "dev": "nodemon api-server.js"
  }
}
```

Then run:
```bash
npm start
```

**Using Python with Gunicorn (Production):**

```bash
pip install gunicorn
gunicorn -w 4 -b 0.0.0.0:5000 flask_app:app
```

---

## 🧪 Testing the APIs

### Using cURL

**Health Check:**
```bash
curl http://localhost:3000/api/health
```

**Get All Cars:**
```bash
curl http://localhost:3000/api/cars
```

**Create a Car:**
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

### Using Postman

1. Open Postman
2. Create a new collection called "Car Park API"
3. Add requests for each endpoint
4. Set variables:
   - `base_url`: http://localhost:3000/api
   - `car_id`: 1
   - `parking_id`: 1
5. Save and run requests

### Using Python Examples

Run the Python example file:

```bash
python api_examples.py
```

This will execute all example operations.

### Using Node.js Examples

Install dependencies and run:

```bash
npm install node-fetch
node api-examples.js
```

---

## 🐛 Troubleshooting

### Port Already in Use

**Error:** `Error: listen EADDRINUSE: address already in use :::3000`

**Solution:**
```bash
# Find and kill process using port 3000
# Windows
netstat -ano | findstr :3000
taskkill /PID <PID> /F

# macOS/Linux
lsof -i :3000
kill -9 <PID>
```

### Module Not Found

**Error:** `Error: Cannot find module 'express'`

**Solution:**
```bash
npm install  # Install all dependencies from package.json
```

**Error:** `ModuleNotFoundError: No module named 'flask'`

**Solution:**
```bash
pip install flask  # Install missing module
```

### JSON Files Not Found

**Error:** `Error loading ./prq_cars.json`

**Solution:**
1. Verify JSON files exist in project directory
2. Check file permissions
3. Ensure correct working directory:
   ```bash
   cd path/to/project
   ```

### CORS Issues

**Error:** `Access to XMLHttpRequest at 'http://localhost:3000/api/cars' from origin 'http://localhost:3001' has been blocked by CORS policy`

**Solution:** CORS is already enabled in both servers, but verify:

**Express (api-server.js):**
```javascript
app.use(cors());
```

**Flask (flask_app.py):**
```python
CORS(app)
```

### Connection Refused

**Error:** `Error: connect ECONNREFUSED 127.0.0.1:3000`

**Solution:**
1. Ensure server is running
2. Check if correct port is being used
3. Verify firewall settings

---

## 🔐 Environment Variables

### .env File Example

```env
# Server Configuration
PORT=3000
FLASK_PORT=5000
NODE_ENV=development
FLASK_ENV=development

# Database Configuration
USE_DATABASE=false
DB_HOST=localhost
DB_PORT=3306
DB_USER=root
DB_PASSWORD=password
DB_NAME=parking_db

# API Configuration
API_TIMEOUT=5000
CORS_ORIGIN=*
LOG_LEVEL=debug
```

### How to Load .env

**Node.js:**
```javascript
require('dotenv').config();
const port = process.env.PORT || 3000;
```

**Python:**
```python
from dotenv import load_dotenv
import os

load_dotenv()
port = int(os.getenv('FLASK_PORT', 5000))
```

---

## 📊 Performance Optimization

### Node.js Optimization

**Use Clustering:**
```bash
npm install cluster
```

**Update api-server.js:**
```javascript
const cluster = require('cluster');
const os = require('os');

if (cluster.isMaster) {
  const numCPUs = os.cpus().length;
  for (let i = 0; i < numCPUs; i++) {
    cluster.fork();
  }
}
```

### Python Optimization

**Use Gunicorn with Multiple Workers:**
```bash
gunicorn -w 4 -b 0.0.0.0:5000 flask_app:app
```

**Use Gevent for async operations:**
```bash
pip install gevent gunicorn[gevent]
gunicorn -k gevent -w 4 flask_app:app
```

---

## 🚀 Deployment Options

### Heroku

**Node.js:**
```bash
heroku login
heroku create car-park-api
git push heroku main
```

**Python:**
```bash
heroku create car-park-api-py
heroku config:set FLASK_ENV=production
git push heroku main
```

### Docker

**Dockerfile for Node.js:**
```dockerfile
FROM node:16
WORKDIR /app
COPY . .
RUN npm install
EXPOSE 3000
CMD ["node", "api-server.js"]
```

**Dockerfile for Python:**
```dockerfile
FROM python:3.9
WORKDIR /app
COPY . .
RUN pip install -r requirements.txt
EXPOSE 5000
CMD ["gunicorn", "-b", "0.0.0.0:5000", "flask_app:app"]
```

### Azure App Service

**Node.js:**
1. Create App Service
2. Connect to GitHub
3. Deploy from branch
4. Set environment variables in Application settings

**Python:**
1. Create App Service (Python 3.9)
2. Connect to GitHub
3. Set startup command: `gunicorn -b 0.0.0.0:8000 flask_app:app`

---

## 📝 Summary Checklist

- [ ] Node.js/Python installed and verified
- [ ] Dependencies installed (`npm install` or `pip install`)
- [ ] .env file created with proper configuration
- [ ] JSON data files present in project directory
- [ ] Express server started successfully (Port 3000)
- [ ] Flask server started successfully (Port 5000)
- [ ] Health check endpoint responds (http://localhost:PORT/api/health)
- [ ] Sample requests tested with cURL or Postman
- [ ] Example scripts run successfully
- [ ] Endpoints documentation reviewed

---

## 🎓 Next Steps

1. **Test All Operations**: Use `api_examples.py` or `api-examples.js`
2. **Integrate with Frontend**: Use the API endpoints in your web application
3. **Set Up Database**: Configure Azure MySQL for production
4. **Monitor Performance**: Use APM tools like New Relic or DataDog
5. **Implement Authentication**: Add JWT or OAuth2 security
6. **Scale Infrastructure**: Deploy to cloud with load balancing

