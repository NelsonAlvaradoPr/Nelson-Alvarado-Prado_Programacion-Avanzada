# 🚗 Vehicle Management MVC Application - Quick Setup Guide

## ⚡ 30-Second Installation

### Step 1: Install Dependencies
```bash
pip install -r requirements-mvc.txt
```

### Step 2: Ensure API is Running
```bash
# Option A: Python API
python flask_app.py

# Option B: Node.js API
npm start
```

### Step 3: Run MVC Application
```bash
python vehicle_mvc_app.py
```

### Step 4: Open Browser
```
http://localhost:8080
```

---

## 📋 System Requirements

- Python 3.8+
- pip package manager
- Running REST API server (on port 5000 or 3000)
- Web browser (Chrome, Firefox, Safari, Edge)

## 🛠️ Installation Steps

### 1. Install Required Packages

```bash
pip install -r requirements-mvc.txt
```

**Packages installed:**
- Flask 2.3.3 - Web framework
- Flask-CORS 4.0.0 - Cross-origin requests
- python-dotenv 1.0.0 - Environment variables
- requests 2.31.0 - HTTP client
- Werkzeug 2.3.7 - WSGI utility

### 2. Verify API Server is Running

Before starting the MVC application, ensure the API server is running on one of these ports:

**Check Python API (port 5000):**
```bash
curl http://localhost:5000/api/health
```

**Check Node.js API (port 3000):**
```bash
curl http://localhost:3000/api/health
```

Both should return `{"status": "healthy"}`

### 3. Configure Environment (Optional)

The MVC application has a default configuration that should work out of the box. If you need to change the API URL:

```bash
# Copy the example .env file
cp .env.mvc .env

# Edit .env with your text editor
# Change API_URL if your API is running on a different port/host
API_URL=http://localhost:5000/api
```

### 4. Run the MVC Application

```bash
python vehicle_mvc_app.py
```

Expected output:
```
🚗 Vehicle Management Application starting...
📡 API URL: http://localhost:5000/api
🌐 Web UI: http://localhost:8080
 * Running on http://0.0.0.0:8080
```

### 5. Access the Application

Open your web browser and go to:
```
http://localhost:8080
```

---

## ✅ Verification Checklist

After installation, verify everything is working:

- [ ] Python 3.8+ is installed (`python --version`)
- [ ] Dependencies installed successfully (`pip list | grep Flask`)
- [ ] API server is running (`curl http://localhost:5000/api/health`)
- [ ] MVC app starts without errors (`python vehicle_mvc_app.py`)
- [ ] Browser loads http://localhost:8080 without errors
- [ ] Search form displays correctly
- [ ] Can search vehicles by year range
- [ ] View/Edit/Delete buttons work

---

## 🚀 First Test Run

### 1. Search for Vehicles (2020-2023)
1. Navigate to http://localhost:8080
2. Set Minimum Year: **2020**
3. Set Maximum Year: **2023**
4. Click **Search**
5. Should see vehicles with years 2020-2023

### 2. View Vehicle Details
1. In the results table, click **View** on any vehicle
2. Should see full vehicle details
3. Verify color, year, make, and type are displayed

### 3. Create a New Vehicle
1. Click **Add Vehicle** in navigation or results table
2. Fill in:
   - Color: Blue
   - Year: 2024
   - Make: Toyota Corolla
   - Type: Sedan
3. Click **Create Vehicle**
4. Should be redirected to home page with success message

### 4. Edit a Vehicle
1. From the list, click **Edit** on any vehicle
2. Change the color to "Green"
3. Click **Update Vehicle**
4. Vehicle should be updated

### 5. Delete a Vehicle
1. Click **Delete** on any vehicle
2. Confirm in the dialog
3. Vehicle should be removed

---

## 🐛 Common Issues

### Issue: "ModuleNotFoundError: No module named 'flask'"

**Solution:**
```bash
pip install -r requirements-mvc.txt
```

### Issue: "Connection refused" or "Cannot connect to API"

**Solution:**
1. Check API server is running:
   ```bash
   curl http://localhost:5000/api/cars
   ```
2. If not running, start it:
   ```bash
   python flask_app.py
   ```
3. Verify `.env` has correct `API_URL`

### Issue: Port 8080 already in use

**Solution:**
Edit `vehicle_mvc_app.py` line at the bottom:
```python
app.run(debug=True, port=9090, host='0.0.0.0')  # Change 8080 to 9090
```

### Issue: Browser shows "Cannot GET /"

**Solution:**
1. Clear browser cache (Ctrl+Shift+Del)
2. Verify API is responding:
   ```bash
   curl http://localhost:5000/api/cars
   ```
3. Check console for errors (F12)

---

## 📁 Project Files

```
vehicle_mvc_app.py          # Main Flask application
templates/
  ├── base.html             # Base template with styling
  ├── vehicles.html         # Vehicle listing page
  ├── vehicle_detail.html   # Vehicle details page
  └── vehicle_form.html     # Create/Edit form
static/                     # (Optional) Static CSS/JS files
.env.mvc                    # Environment template
requirements-mvc.txt        # Python dependencies
MVC_README.md              # Complete documentation
SETUP_GUIDE.md             # This file
```

---

## 🎯 Next Steps

1. **Customize Styling** - Edit CSS in `templates/base.html`
2. **Add Features** - Extend `VehicleAPIClient` class for new features
3. **Create Dashboard** - Add statistics and analytics
4. **Export Data** - Add CSV/PDF export functionality
5. **Advanced Filtering** - Add more filter options

---

## 📞 Support Resources

- **API Documentation**: See `API_REFERENCE.md`
- **REST API**: See `DELIVERY_SUMMARY.md`
- **Flask Documentation**: https://flask.palletsprojects.com/
- **Bootstrap Documentation**: https://getbootstrap.com/docs/5.3/

---

## ⚙️ Environment Variables Reference

| Variable | Default | Description |
|----------|---------|-------------|
| `API_URL` | `http://localhost:5000/api` | REST API base URL |
| `FLASK_ENV` | `development` | Flask environment mode |
| `FLASK_DEBUG` | `True` | Enable debug mode |
| `PORT` | `8080` | MVC application port |
| `HOST` | `0.0.0.0` | MVC application host |

---

## 🔄 Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Web Browser                          │
│              http://localhost:8080                      │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│            Flask MVC Application                        │
│  (vehicle_mvc_app.py)                                   │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Controller Layer (Routes)                          │ │
│  │  - GET  / (search & list)                           │ │
│  │  - GET  /vehicles/<id> (detail)                     │ │
│  │  - GET  /vehicles/new (create form)                 │ │
│  │  - POST /api/vehicles (create)                      │ │
│  │  - PUT  /api/vehicles/<id> (update)                 │ │
│  │  - DELETE /api/vehicles/<id> (delete)               │ │
│  └────────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Model Layer (VehicleAPIClient)                     │ │
│  │  - get_vehicles_by_year_range()                     │ │
│  │  - get_vehicle_by_id()                              │ │
│  │  - create_vehicle()                                 │ │
│  │  - update_vehicle()                                 │ │
│  │  - delete_vehicle()                                 │ │
│  └────────────────────────────────────────────────────┘ │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│           REST API Server                               │
│  (flask_app.py or api-server.js)                        │
│  http://localhost:5000/api (Python)                     │
│  http://localhost:3000/api (Node.js)                    │
│                                                          │
│  - GET  /cars                                           │
│  - GET  /cars/:id                                       │
│  - GET  /cars/filter/year-range?min=&max=              │
│  - POST /cars                                           │
│  - PUT  /cars/:id                                       │
│  - DELETE /cars/:id                                     │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│          Database (JSON or MySQL)                       │
│  - prq_cars.json or PRQ_Cars table                      │
└─────────────────────────────────────────────────────────┘
```

---

**Ready to go!** 🚀

Your MVC application is now set up and ready to use. Start with the Quick Setup section at the top of this guide.
