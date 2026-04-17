# 🚗 Vehicle Management MVC - Visual Guide

## System Architecture Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                     WEB BROWSER                              │
│              http://localhost:8080                           │
└────────────────────────┬─────────────────────────────────────┘
                         │
                    HTTP Request
                         │
         ┌───────────────▼───────────────┐
         │   FLASK MVC APPLICATION       │
         │   (vehicle_mvc_app.py)        │
         └───────────────┬───────────────┘
                         │
         ┌───────────────┴───────────────┐
         │                               │
    ┌────▼────┐                    ┌────▼────┐
    │ ROUTES  │                    │  VIEWS  │
    │ (Logic) │                    │(HTML)   │
    └────┬────┘                    └────┬────┘
         │                              │
    CONTROLLER               templates/
    - GET /                 - base.html
    - POST /api/vehicles    - vehicles.html
    - PUT /api/vehicles/<id>- vehicle_detail.html
    - DELETE /api/vehicles/<id>- vehicle_form.html
         │                              │
         │                              │
    ┌────▼──────────────────────────────▼────┐
    │    VehicleAPIClient (MODEL)            │
    │    - API Requests                      │
    │    - Response Handling                 │
    │    - Error Management                  │
    └────┬─────────────────────────────────────┘
         │
         │   HTTP REST Calls
         │
    ┌────▼──────────────────────────────┐
    │   REST API SERVER                 │
    │ (flask_app.py or api-server.js)   │
    │                                   │
    │ GET    /api/cars/filter/year-range│
    │ GET    /api/cars/:id              │
    │ POST   /api/cars                  │
    │ PUT    /api/cars/:id              │
    │ DELETE /api/cars/:id              │
    └────┬──────────────────────────────┘
         │
         │
    ┌────▼──────────────────────────────┐
    │  DATABASE                         │
    │  - prq_cars.json (JSON)           │
    │  - PRQ_Cars (MySQL)               │
    └───────────────────────────────────┘
```

## Request/Response Flow

### Search Vehicles Request
```
┌─ USER ACTION ─────────────────────────────────────────┐
│                                                        │
│  1. User enters: Min Year = 2020, Max Year = 2023    │
│  2. User clicks "Search" button                      │
│                                                        │
└──────────────────┬─────────────────────────────────────┘
                   │
                   ▼
┌─ BROWSER ────────────────────────────────────────────┐
│                                                        │
│  Sends: GET /?min_year=2020&max_year=2023           │
│                                                        │
└──────────────────┬─────────────────────────────────────┘
                   │
                   ▼
┌─ FLASK ROUTE ────────────────────────────────────────┐
│                                                        │
│  @app.route('/')                                     │
│  def index():                                        │
│      vehicles = api_client.get_vehicles_by_year_range│
│      return render_template('vehicles.html',...)    │
│                                                        │
└──────────────────┬─────────────────────────────────────┘
                   │
                   ▼
┌─ API CLIENT ──────────────────────────────────────────┐
│                                                        │
│  requests.get('/api/cars/filter/year-range')         │
│  params = {'min': 2020, 'max': 2023}                │
│                                                        │
└──────────────────┬─────────────────────────────────────┘
                   │
                   ▼
┌─ REST API ────────────────────────────────────────────┐
│                                                        │
│  GET /api/cars/filter/year-range?min=2020&max=2023 │
│  Returns: {                                          │
│      "success": true,                                │
│      "data": [car1, car2, ...],                     │
│      "count": 5                                      │
│  }                                                    │
│                                                        │
└──────────────────┬─────────────────────────────────────┘
                   │
                   ▼
┌─ FLASK TEMPLATE ──────────────────────────────────────┐
│                                                        │
│  vehicles.html rendered with:                        │
│  - Search form                                       │
│  - Results table with 5 vehicles                     │
│  - Action buttons (View, Edit, Delete)              │
│                                                        │
└──────────────────┬─────────────────────────────────────┘
                   │
                   ▼
┌─ BROWSER DISPLAY ─────────────────────────────────────┐
│                                                        │
│  ┌─ Vehicles Found: 5 ──────────────────────┐       │
│  │ ID │ Color │ Year │ Make │ Type │ Actions│       │
│  ├────┼───────┼──────┼──────┼──────┼────────┤       │
│  │ 2  │ Black │ 2019 │ BMW  │ 4x4  │View... │       │
│  │ 3  │ White │ 2022 │ Yam  │ Moto │View... │       │
│  │ 4  │ Silv  │ 2021 │ MB   │ Sed  │View... │       │
│  │ 5  │ Blue  │ 2018 │ Land │ 4x4  │View... │       │
│  │ 6  │ Blue  │ 2023 │ Honda│ Sed  │View... │       │
│  └────┴───────┴──────┴──────┴──────┴────────┘       │
│                                                        │
└─────────────────────────────────────────────────────────┘
```

## CRUD Operation Flows

### CREATE (Add New Vehicle)
```
User clicks "Add Vehicle"
       ↓
GET /vehicles/new → Display form
       ↓
User fills form and clicks "Create Vehicle"
       ↓
POST /api/vehicles with data
       ↓
API creates vehicle in database
       ↓
Redirect to home page with success message
```

### READ (View Details)
```
User clicks "View" button
       ↓
GET /vehicles/<id>
       ↓
Fetch vehicle data from API
       ↓
Display vehicle_detail.html with full information
```

### UPDATE (Edit Vehicle)
```
User clicks "Edit" button
       ↓
GET /vehicles/<id>/edit → Display pre-filled form
       ↓
User modifies fields and clicks "Update Vehicle"
       ↓
PUT /api/vehicles/<id> with new data
       ↓
API updates vehicle in database
       ↓
Redirect with success message
```

### DELETE (Remove Vehicle)
```
User clicks "Delete" button
       ↓
Display confirmation modal
       ↓
User confirms deletion
       ↓
DELETE /api/vehicles/<id>
       ↓
API removes vehicle from database
       ↓
Page refreshes showing updated list
```

## Page Navigation Map

```
                      HOME PAGE (/)
                      (Search & List)
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ▼                   ▼                   ▼
   DETAIL PAGE        EDIT FORM            CREATE FORM
   /vehicles/<id>   /vehicles/<id>/edit  /vehicles/new
        │                   │                   │
        │                   │                   │
        └───────────────────┼───────────────────┘
                            │
                            ▼
                      HOME PAGE
                    (Updated List)
```

## Component Structure

```
vehicle_mvc_app.py
│
├── VehicleAPIClient (Model)
│   ├── __init__(base_url)
│   ├── get_vehicles_by_year_range()
│   ├── get_vehicle_by_id()
│   ├── create_vehicle()
│   ├── update_vehicle()
│   └── delete_vehicle()
│
├── Routes (Controller)
│   ├── GET  / → index()
│   ├── GET  /vehicles/<id> → view_vehicle()
│   ├── GET  /vehicles/new → create_vehicle_page()
│   ├── GET  /vehicles/<id>/edit → edit_vehicle_page()
│   ├── GET  /api/vehicles/search → search_vehicles()
│   ├── POST /api/vehicles → api_create_vehicle()
│   ├── PUT  /api/vehicles/<id> → api_update_vehicle()
│   └── DELETE /api/vehicles/<id> → api_delete_vehicle()
│
└── Error Handlers
    ├── 404 → not_found()
    └── 500 → internal_error()

templates/
├── base.html (Layout template)
│   ├── Navigation Bar
│   ├── Flash Messages
│   ├── Main Content Block
│   └── Footer
│
├── vehicles.html (Extends base.html)
│   ├── Search Form
│   ├── Results Table
│   └── Delete Modal
│
├── vehicle_detail.html (Extends base.html)
│   ├── Vehicle Information
│   ├── Specifications Summary
│   └── Action Buttons
│
└── vehicle_form.html (Extends base.html)
    ├── Form Fields
    │   ├── Color Input
    │   ├── Year Input
    │   ├── Make Input
    │   └── Type Dropdown
    ├── Submit/Cancel Buttons
    └── Loading Modal
```

## Data Flow for Search Operation

```
User Input
│
├─ Min Year: 2020
├─ Max Year: 2023
│
▼
FORM SUBMISSION
│
▼
Flask Route: @app.route('/')
│
├─ Extract: min_year=2020, max_year=2023
│
▼
VehicleAPIClient.get_vehicles_by_year_range(2020, 2023)
│
├─ Build URL: /api/cars/filter/year-range
├─ Add params: min=2020&max=2023
│
▼
REST API: GET /api/cars/filter/year-range?min=2020&max=2023
│
▼
Database Query
│
├─ SELECT * FROM cars
├─ WHERE year >= 2020 AND year <= 2023
│
▼
Database Result
│
[
  {id: 2, color: 'Black', year: 2019, make: 'BMW X5', type: '4x4'},
  {id: 3, color: 'White', year: 2022, make: 'Yamaha MT-07', type: 'motorcycle'},
  {id: 4, color: 'Silver', year: 2021, make: 'Mercedes-Benz C-Class', type: 'sedan'},
  {id: 5, color: 'Blue', year: 2018, make: 'Land Rover Discovery', type: '4x4'},
  {id: 6, color: 'Blue', year: 2023, make: 'Honda', type: 'sedan'}
]

Note: Only vehicles with year 2022-2023 match the filter
Returned vehicles: 3, 4, 6

▼
API Response
│
{
  "success": true,
  "data": [
    {id: 3, color: 'White', year: 2022, ...},
    {id: 4, color: 'Silver', year: 2021, ...},
    {id: 6, color: 'Blue', year: 2023, ...}
  ],
  "count": 3
}

▼
Render Template
│
├─ Pass vehicles list
├─ Pass min_year and max_year
├─ Pass current_year
│
▼
HTML Template renders
│
├─ Search form populated with user input
├─ Results table with 3 vehicles
├─ Each vehicle has View, Edit, Delete buttons
│
▼
Browser Display
│
[Show to user]
```

## File Dependencies

```
vehicle_mvc_app.py
│
├── Imports:
│   ├── flask (Flask web framework)
│   ├── requests (HTTP client for API calls)
│   ├── dotenv (Environment variables)
│   └── datetime (Date/time handling)
│
├── Loads templates from:
│   ├── templates/base.html
│   ├── templates/vehicles.html
│   ├── templates/vehicle_detail.html
│   └── templates/vehicle_form.html
│
├── Uses environment from:
│   └── .env (API_URL configuration)
│
└── Depends on API server:
    └── http://localhost:5000/api or :3000/api

templates/base.html
│
├── Loads external CSS:
│   ├── Bootstrap 5 CDN
│   └── Font Awesome Icons CDN
│
├── Loads external JS:
│   └── Bootstrap 5 Bundle
│
└── Included by all other templates

templates/vehicles.html
│
├── Extends base.html
│
└── Uses JavaScript:
    ├── delete event handler
    ├── form validation
    └── API delete request

templates/vehicle_detail.html
│
├── Extends base.html
│
└── Uses JavaScript:
    ├── delete confirmation
    └── API delete request

templates/vehicle_form.html
│
├── Extends base.html
│
└── Uses JavaScript:
    ├── form submission
    ├── validation
    ├── API POST/PUT
    └── loading modal
```

## Request Methods Summary

```
┌─────────────────────────────────────────────────────────┐
│                   REST API METHODS                      │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  GET (Retrieve)                                         │
│  - No body                                              │
│  - Safe (doesn't modify)                                │
│  - Idempotent (same result each time)                   │
│  Example: GET /api/cars/filter/year-range?min=2020    │
│                                                          │
│  POST (Create)                                          │
│  - Has body with data                                   │
│  - Not safe (creates new resource)                      │
│  - Not idempotent (creates new each time)               │
│  Example: POST /api/cars {color, year, make, type}    │
│                                                          │
│  PUT (Update)                                           │
│  - Has body with data                                   │
│  - Not safe (modifies resource)                         │
│  - Idempotent (same result each time)                   │
│  Example: PUT /api/cars/4 {updated fields}            │
│                                                          │
│  DELETE (Remove)                                        │
│  - No body                                              │
│  - Not safe (deletes resource)                          │
│  - Idempotent (same result each time)                   │
│  Example: DELETE /api/cars/4                           │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

## Environment Setup

```
┌─────────────────────────────────────────────────────────┐
│                   .env FILE                             │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  API_URL=http://localhost:5000/api                     │
│  FLASK_ENV=development                                  │
│  FLASK_DEBUG=True                                       │
│  PORT=8080                                              │
│  HOST=0.0.0.0                                           │
│                                                          │
└─────────────────────────────────────────────────────────┘

These variables control:
- Where the Flask app looks for the API
- Debug mode (development/production)
- Port the web app runs on
- Host binding (0.0.0.0 = accessible from anywhere)
```

## Deployment Architecture

```
Production Setup:
┌────────────────────────────────────────┐
│      Internet / Users                  │
└─────────────────────┬──────────────────┘
                      │
┌─────────────────────▼──────────────────┐
│    Reverse Proxy (Nginx/Apache)        │
│    - Port 80 (HTTP) / 443 (HTTPS)     │
│    - Load balancing                    │
└─────────────────────┬──────────────────┘
                      │
┌─────────────────────▼──────────────────┐
│   Gunicorn / WSGI Server               │
│   - Running vehicle_mvc_app            │
│   - Multiple worker processes          │
└─────────────────────┬──────────────────┘
                      │
┌─────────────────────▼──────────────────┐
│   Flask MVC Application                │
│   - Routes and logic                   │
│   - Template rendering                 │
└─────────────────────┬──────────────────┘
                      │
┌─────────────────────▼──────────────────┐
│   REST API Server                      │
│   - API endpoints                      │
│   - Business logic                     │
└─────────────────────┬──────────────────┘
                      │
┌─────────────────────▼──────────────────┐
│   Database                             │
│   - MySQL or JSON                      │
└────────────────────────────────────────┘
```

---

This visual guide provides a complete overview of the system architecture, data flows, and component relationships in the Vehicle Management MVC Application.
