# 🚗 Vehicle Management MVC Application

A complete MVC (Model-View-Controller) web application for managing vehicles with search, filtering, and CRUD operations. This application allows you to query vehicles by year range and perform complete maintenance operations (view, edit, delete).

## 📋 Features

✅ **Search by Year Range** - Filter vehicles using minimum and maximum year parameters
✅ **Complete CRUD Operations** - Create, Read, Update, and Delete vehicles
✅ **Responsive UI** - Bootstrap 5 styling for modern, mobile-friendly interface
✅ **API Integration** - Seamless integration with the existing REST API
✅ **Detailed Views** - View complete vehicle information with visual representation
✅ **Data Validation** - Client-side and server-side validation
✅ **User-Friendly Forms** - Intuitive forms for creating and editing vehicles
✅ **Action Buttons** - Easy-to-use buttons for View, Edit, and Delete operations

## 🏗️ Architecture

### Model Layer (`VehicleAPIClient`)
- Encapsulates all API interactions
- Methods for CRUD operations and filtering
- Error handling and request validation

### View Layer (HTML Templates)
- `base.html` - Base template with navigation and styling
- `vehicles.html` - Vehicle listing with search form and action buttons
- `vehicle_detail.html` - Detailed vehicle view
- `vehicle_form.html` - Form for creating/editing vehicles

### Controller Layer (Flask Routes)
- Search endpoint: `GET /` - Display search form and results
- Detail endpoint: `GET /vehicles/<id>` - View vehicle details
- Create endpoints: `GET /vehicles/new`, `POST /api/vehicles` - Create new vehicle
- Edit endpoints: `GET /vehicles/<id>/edit`, `PUT /api/vehicles/<id>` - Update vehicle
- Delete endpoint: `DELETE /api/vehicles/<id>` - Delete vehicle

## 🚀 Quick Start

### 1. Prerequisites
- Python 3.8 or higher
- pip (Python package manager)
- The REST API server running (either Node.js or Python)

### 2. Installation

#### Step 1: Install Dependencies
```bash
pip install -r requirements-mvc.txt
```

#### Step 2: Configure Environment
Copy and configure the environment file:
```bash
# Copy the example env file
cp .env.mvc .env

# Edit .env to set the correct API_URL (if different from default)
# Default: http://localhost:5000/api
```

#### Step 3: Run the MVC Application
```bash
python vehicle_mvc_app.py
```

The application will start on: **http://localhost:8080**

### 3. Ensure API is Running

Before running the MVC application, make sure the REST API server is running:

**Option A: Python API (Flask)**
```bash
python flask_app.py
# Server runs on: http://localhost:5000/api
```

**Option B: Node.js API (Express)**
```bash
npm start
# Server runs on: http://localhost:3000/api
```

## 📖 Usage Guide

### 1. Searching for Vehicles

1. Navigate to **http://localhost:8080**
2. Enter the **Minimum Year** and **Maximum Year** in the search form
3. Click the **Search** button
4. View the results in the table below

**Example:**
- Minimum Year: 2020
- Maximum Year: 2023
- Results: All vehicles manufactured between 2020 and 2023

### 2. Viewing Vehicle Details

1. From the vehicle list, click the **View** button (blue button)
2. You'll see detailed information about the selected vehicle
3. From this page, you can also Edit or Delete the vehicle

### 3. Creating a New Vehicle

#### Method 1: From Navigation
1. Click the **Add Vehicle** button in the navigation bar
2. Fill out the form with:
   - **Color** - Vehicle color (e.g., Blue, Red)
   - **Year** - Manufacturing year (1900 - current year)
   - **Make** - Vehicle make/model (e.g., Honda Civic)
   - **Type** - Select from: Sedan, 4x4, Motorcycle
3. Click **Create Vehicle**

#### Method 2: From Vehicle List
1. Click the **Add New Vehicle** button in the "Vehicles Found" card
2. Complete the form and submit

### 4. Editing a Vehicle

1. From the vehicle list or detail page, click the **Edit** button (yellow/warning button)
2. Modify the vehicle information
3. Click **Update Vehicle**

### 5. Deleting a Vehicle

1. Click the **Delete** button (red button) on either:
   - The vehicle list row
   - The vehicle detail page
2. Confirm the deletion in the modal dialog
3. The vehicle will be permanently deleted

## 📊 API Endpoints Used

The MVC application calls the following REST API endpoints:

| Operation | Method | Endpoint |
|-----------|--------|----------|
| Get vehicles by year range | GET | `/api/cars/filter/year-range?min=YYYY&max=YYYY` |
| Get single vehicle | GET | `/api/cars/:id` |
| Create vehicle | POST | `/api/cars` |
| Update vehicle | PUT | `/api/cars/:id` |
| Delete vehicle | DELETE | `/api/cars/:id` |

## 🎨 User Interface Components

### Navigation Bar
- **Logo** - Quick access to home page
- **Navigation Menu** - Links to Vehicles and Add Vehicle
- Responsive mobile menu

### Search Form
- Minimum and Maximum year inputs
- Search button with validation
- Real-time form validation

### Vehicle List Table
- Vehicle ID with visual indicator
- Color with color preview box
- Year display
- Make/Model
- Type badge with icons
- Action buttons (View, Edit, Delete)
- Empty state message when no results

### Vehicle Detail Page
- Complete vehicle information
- Visual color representation
- Specifications summary with badges
- Action buttons for Edit and Delete
- Back to list navigation

### Vehicle Form
- Intuitive form layout
- Field validation
- Vehicle type dropdown selector
- Color and make input fields
- Year range validation
- Success/error feedback
- Loading state during submission

## 🔧 Configuration

### Change API URL

Edit `.env` file:
```bash
# Default (Python API)
API_URL=http://localhost:5000/api

# Alternative (Node.js API)
API_URL=http://localhost:3000/api
```

### Change MVC Server Port

Edit the app.run() call in `vehicle_mvc_app.py`:
```python
app.run(debug=True, port=8080, host='0.0.0.0')
```

Change `8080` to your desired port.

## 📁 Project Structure

```
.
├── vehicle_mvc_app.py           # Main Flask application (Controller)
├── templates/                    # HTML templates (View)
│   ├── base.html                # Base template with styling
│   ├── vehicles.html            # Vehicle list with search
│   ├── vehicle_detail.html      # Single vehicle view
│   └── vehicle_form.html        # Create/edit vehicle form
├── static/                       # Static files (CSS, JS)
├── .env.mvc                     # Environment configuration template
├── requirements-mvc.txt         # Python dependencies
└── MVC_README.md               # This file
```

## 🐛 Troubleshooting

### Issue: "Error fetching vehicles" or API connection error

**Solution:**
1. Verify the API server is running (port 5000 or 3000)
2. Check the `API_URL` in `.env` matches your API server
3. Ensure CORS is enabled on the API server
4. Try accessing the API directly: `curl http://localhost:5000/api/cars`

### Issue: Form submission not working

**Solution:**
1. Open browser developer console (F12)
2. Check for JavaScript errors
3. Verify all form fields are filled correctly
4. Ensure API server is responding to POST requests

### Issue: Vehicles not appearing in search results

**Solution:**
1. Verify vehicles exist in the database with matching year range
2. Try a wider year range (e.g., 2000-2025)
3. Check API response: `curl 'http://localhost:5000/api/cars/filter/year-range?min=2000&max=2025'`

### Issue: Delete button not working

**Solution:**
1. Check browser console for errors
2. Verify DELETE method is allowed in API CORS configuration
3. Confirm the vehicle ID is valid
4. Check API server logs

## 📝 Example Workflow

1. **Start API Server**
   ```bash
   # In terminal 1
   python flask_app.py
   ```

2. **Start MVC Application**
   ```bash
   # In terminal 2
   python vehicle_mvc_app.py
   ```

3. **Open Browser**
   - Navigate to: http://localhost:8080

4. **Search for Vehicles**
   - Set Year Range: 2020-2023
   - Click Search

5. **Manage Vehicles**
   - Click View to see details
   - Click Edit to modify
   - Click Delete to remove

## 🚀 Advanced Usage

### Using with Different API Servers

**Switch to Node.js API:**
```bash
# Terminal 1: Start Node.js API
npm start

# Terminal 2: Update .env
API_URL=http://localhost:3000/api

# Terminal 3: Start MVC app
python vehicle_mvc_app.py
```

**Switch to Python API:**
```bash
# Terminal 1: Start Python API
python flask_app.py

# Terminal 2: MVC app uses default API_URL
python vehicle_mvc_app.py
```

### Extending the Application

To add new features:

1. **Add new API methods** in `VehicleAPIClient` class
2. **Add new routes** in Flask (Controller)
3. **Create new templates** (View)
4. **Update navigation** to link to new pages

## 📊 Sample Data

The application comes with sample data in `prq_cars.json`:
- 5 vehicles with IDs 2-6
- Years ranging from 2018-2023
- Colors: Black, White, Silver, Blue
- Types: Sedan, 4x4, Motorcycle
- Makes: BMW, Yamaha, Mercedes-Benz, Land Rover, Honda

## ✅ Testing Checklist

- [ ] API server is running
- [ ] MVC application starts without errors
- [ ] Search form loads correctly
- [ ] Search by year range works
- [ ] Vehicle list displays properly
- [ ] View button shows vehicle details
- [ ] Edit form loads and updates vehicle
- [ ] Delete function removes vehicle
- [ ] Create new vehicle works
- [ ] Responsive design on mobile

## 📞 Support

For issues or questions:
1. Check the Troubleshooting section
2. Review API documentation in `API_REFERENCE.md`
3. Check browser console for JavaScript errors
4. Verify API server logs for errors

## 📄 License

This project is part of the Car Park Management System.

---

**Version:** 1.0.0  
**Last Updated:** 2024  
**MVC Framework:** Flask (Python)  
**Frontend:** Bootstrap 5, HTML5, JavaScript
