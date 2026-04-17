"""
Car Park REST API Server - Python with Flask
Provides complete REST endpoints for all CRUD and query operations

Usage:
    pip install flask flask-cors python-dotenv mysql-connector-python
    python flask_app.py

Base URL: http://localhost:5000/api
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
from datetime import datetime
import os
import json
from dotenv import load_dotenv

from crud_service import JsonCrudService, DatabaseCrudService
from repositories import (
    JsonCarRepository, JsonParkingRepository, JsonCarEntryRepository,
    DatabaseCarRepository, DatabaseParkingRepository, DatabaseCarEntryRepository
)

load_dotenv()

app = Flask(__name__)
CORS(app)
PORT = int(os.getenv('PORT', 5000))
USE_DATABASE = os.getenv('USE_DATABASE', 'false').lower() == 'true'

# Initialize services and repositories
if USE_DATABASE:
    print('🗄️  Initializing database connections...')
    # TODO: Initialize with database connection
    crud_service = JsonCrudService('.')
    car_repository = JsonCarRepository('./prq_cars.json')
    parking_repository = JsonParkingRepository('./prq_parking.json')
    car_entry_repository = JsonCarEntryRepository('./prq_car_entry.json')
else:
    print('📄 Initializing JSON file storage...')
    crud_service = JsonCrudService('.')
    car_repository = JsonCarRepository('./prq_cars.json')
    parking_repository = JsonParkingRepository('./prq_parking.json')
    car_entry_repository = JsonCarEntryRepository('./prq_car_entry.json')

# ================================================================
# UTILITY FUNCTIONS
# ================================================================

def success_response(data=None, count=None, status_code=200, **kwargs):
    """Create a standardized success response"""
    response = {
        'success': True,
        'data': data,
        'timestamp': datetime.now().isoformat()
    }
    if count is not None:
        response['count'] = count
    response.update(kwargs)
    return jsonify(response), status_code

def error_response(error, status_code=400):
    """Create a standardized error response"""
    return jsonify({
        'success': False,
        'error': error,
        'timestamp': datetime.now().isoformat()
    }), status_code

# ================================================================
# API ENDPOINTS - CARS
# ================================================================

# ---- CAR CREATE ----
@app.route('/api/cars', methods=['POST'])
def create_car():
    """
    Insert a new car
    Body: { color, year, make, type }
    """
    try:
        data = request.get_json()
        if not data:
            return error_response('Request body cannot be empty', 400)
        
        result = crud_service.insert_car(data)
        if result['success']:
            return success_response(result['data'], status_code=201)
        else:
            return error_response(result['error'], 400)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (ALL) ----
@app.route('/api/cars', methods=['GET'])
def get_all_cars():
    """Retrieve all cars"""
    try:
        cars = car_repository.get_all()
        return success_response(cars, count=len(cars))
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (BY ID) ----
@app.route('/api/cars/<int:car_id>', methods=['GET'])
def get_car_by_id(car_id):
    """Retrieve a specific car by ID"""
    try:
        car = car_repository.get_by_id(car_id)
        if not car:
            return error_response(f'Car with ID {car_id} not found', 404)
        return success_response(car)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (BY COLOR) ----
@app.route('/api/cars/filter/color/<color>', methods=['GET'])
def get_cars_by_color(color):
    """Retrieve cars by color"""
    try:
        cars = car_repository.get_by_color(color)
        return success_response(cars, count=len(cars), filter={'color': color})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (BY MAKE) ----
@app.route('/api/cars/filter/make/<make>', methods=['GET'])
def get_cars_by_make(make):
    """Retrieve cars by make"""
    try:
        cars = car_repository.get_by_make(make)
        return success_response(cars, count=len(cars), filter={'make': make})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (BY TYPE) ----
@app.route('/api/cars/filter/type/<car_type>', methods=['GET'])
def get_cars_by_type(car_type):
    """Retrieve cars by type (sedan, 4x4, motorcycle)"""
    try:
        cars = car_repository.get_by_type(car_type)
        return success_response(cars, count=len(cars), filter={'type': car_type})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (BY YEAR RANGE) ----
@app.route('/api/cars/filter/year-range', methods=['GET'])
def get_cars_by_year_range():
    """Retrieve cars by year range"""
    try:
        min_year = int(request.args.get('min', 1900))
        max_year = int(request.args.get('max', datetime.now().year))
        cars = car_repository.get_by_year_range(min_year, max_year)
        return success_response(cars, count=len(cars), 
                              filter={'minYear': min_year, 'maxYear': max_year})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR READ (ADVANCED FILTER) ----
@app.route('/api/cars/filter/advanced', methods=['GET'])
def get_cars_advanced_filter():
    """Retrieve cars with multiple filters"""
    try:
        color = request.args.get('color')
        min_year = int(request.args.get('minYear', 1900))
        max_year = int(request.args.get('maxYear', datetime.now().year))
        make = request.args.get('make')
        car_type = request.args.get('type')
        
        cars = car_repository.get_by_color_and_year_range_and_make_and_type(
            color, min_year, max_year, make, car_type
        )
        filters = {
            'color': color,
            'minYear': min_year,
            'maxYear': max_year,
            'make': make,
            'type': car_type
        }
        return success_response(cars, count=len(cars), filters=filters)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR UPDATE ----
@app.route('/api/cars/<int:car_id>', methods=['PUT'])
def update_car(car_id):
    """Update a car"""
    try:
        data = request.get_json()
        if not data:
            return error_response('Request body cannot be empty', 400)
        
        result = crud_service.update_car(car_id, data)
        if result['success']:
            return success_response(result['data'])
        else:
            return error_response(result['error'], 404)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR DELETE ----
@app.route('/api/cars/<int:car_id>', methods=['DELETE'])
def delete_car(car_id):
    """Delete a car"""
    try:
        result = crud_service.delete_car(car_id)
        if result['success']:
            return success_response(result['data'])
        else:
            return error_response(result['error'], 404)
    except Exception as e:
        return error_response(str(e), 500)

# ================================================================
# API ENDPOINTS - PARKING
# ================================================================

# ---- PARKING CREATE ----
@app.route('/api/parking', methods=['POST'])
def create_parking():
    """Insert a new parking facility"""
    try:
        data = request.get_json()
        if not data:
            return error_response('Request body cannot be empty', 400)
        
        result = crud_service.insert_parking(data)
        if result['success']:
            return success_response(result['data'], status_code=201)
        else:
            return error_response(result['error'], 400)
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING READ (ALL) ----
@app.route('/api/parking', methods=['GET'])
def get_all_parking():
    """Retrieve all parking facilities"""
    try:
        parking = parking_repository.get_all()
        return success_response(parking, count=len(parking))
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING READ (BY ID) ----
@app.route('/api/parking/<int:parking_id>', methods=['GET'])
def get_parking_by_id(parking_id):
    """Retrieve a specific parking facility by ID"""
    try:
        parking = parking_repository.get_by_id(parking_id)
        if not parking:
            return error_response(f'Parking with ID {parking_id} not found', 404)
        return success_response(parking)
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING READ (BY PROVINCE) ----
@app.route('/api/parking/filter/province/<province>', methods=['GET'])
def get_parking_by_province(province):
    """Retrieve parking facilities by province"""
    try:
        parking = parking_repository.get_by_province_name(province)
        return success_response(parking, count=len(parking), 
                              filter={'province': province})
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING READ (BY NAME) ----
@app.route('/api/parking/filter/name/<name>', methods=['GET'])
def get_parking_by_name(name):
    """Retrieve parking facilities by name"""
    try:
        parking = parking_repository.get_by_name(name)
        return success_response(parking, count=len(parking), 
                              filter={'name': name})
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING READ (BY HOURLY RATE RANGE) ----
@app.route('/api/parking/filter/rate-range', methods=['GET'])
def get_parking_by_rate_range():
    """Retrieve parking facilities by hourly rate range"""
    try:
        min_rate = float(request.args.get('min', 0))
        max_rate = float(request.args.get('max', 9999))
        parking = parking_repository.get_by_hourly_rate_range(min_rate, max_rate)
        return success_response(parking, count=len(parking),
                              filter={'minRate': min_rate, 'maxRate': max_rate})
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING READ (ADVANCED FILTER) ----
@app.route('/api/parking/filter/advanced', methods=['GET'])
def get_parking_advanced_filter():
    """Retrieve parking facilities with multiple filters"""
    try:
        province = request.args.get('province')
        name = request.args.get('name')
        min_rate = float(request.args.get('minRate', 0))
        max_rate = float(request.args.get('maxRate', 9999))
        
        parking = parking_repository.get_by_province_and_name_and_hourly_rate_range(
            province, name, min_rate, max_rate
        )
        filters = {
            'province': province,
            'name': name,
            'minRate': min_rate,
            'maxRate': max_rate
        }
        return success_response(parking, count=len(parking), filters=filters)
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING UPDATE ----
@app.route('/api/parking/<int:parking_id>', methods=['PUT'])
def update_parking(parking_id):
    """Update a parking facility"""
    try:
        data = request.get_json()
        if not data:
            return error_response('Request body cannot be empty', 400)
        
        result = crud_service.update_parking(parking_id, data)
        if result['success']:
            return success_response(result['data'])
        else:
            return error_response(result['error'], 404)
    except Exception as e:
        return error_response(str(e), 500)

# ---- PARKING DELETE ----
@app.route('/api/parking/<int:parking_id>', methods=['DELETE'])
def delete_parking(parking_id):
    """Delete a parking facility"""
    try:
        result = crud_service.delete_parking(parking_id)
        if result['success']:
            return success_response(result['data'])
        else:
            return error_response(result['error'], 404)
    except Exception as e:
        return error_response(str(e), 500)

# ================================================================
# API ENDPOINTS - CAR ENTRY
# ================================================================

# ---- CAR ENTRY CREATE ----
@app.route('/api/car-entry', methods=['POST'])
def create_car_entry():
    """Insert a new car entry"""
    try:
        data = request.get_json()
        if not data:
            return error_response('Request body cannot be empty', 400)
        
        result = crud_service.insert_car_entry(data)
        if result['success']:
            return success_response(result['data'], status_code=201)
        else:
            return error_response(result['error'], 400)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY READ (ALL) ----
@app.route('/api/car-entry', methods=['GET'])
def get_all_car_entries():
    """Retrieve all car entries"""
    try:
        entries = car_entry_repository.get_all()
        return success_response(entries, count=len(entries))
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY READ (BY ID) ----
@app.route('/api/car-entry/<int:entry_id>', methods=['GET'])
def get_car_entry_by_id(entry_id):
    """Retrieve a specific car entry by sequential number"""
    try:
        entry = car_entry_repository.get_by_id(entry_id)
        if not entry:
            return error_response(f'Car entry with ID {entry_id} not found', 404)
        return success_response(entry)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY READ (BY PARKING ID) ----
@app.route('/api/car-entry/filter/parking/<int:parking_id>', methods=['GET'])
def get_entries_by_parking(parking_id):
    """Retrieve car entries for a specific parking facility"""
    try:
        entries = car_entry_repository.get_car_entries_by_parking_id(parking_id)
        return success_response(entries, count=len(entries),
                              filter={'parkingId': parking_id})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY READ (BY CAR TYPE IN DATE RANGE) ----
@app.route('/api/car-entry/filter/car-type/<car_type>', methods=['GET'])
def get_entries_by_car_type(car_type):
    """Retrieve car entries by car type in a date range"""
    try:
        start = request.args.get('start', '2020-01-01')
        end = request.args.get('end', datetime.now().strftime('%Y-%m-%d'))
        
        entries = car_entry_repository.get_cars_by_type_in_date_range(car_type, start, end)
        return success_response(entries, count=len(entries),
                              filters={'carType': car_type, 'startDate': start, 'endDate': end})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY READ (BY PROVINCE IN DATE RANGE) ----
@app.route('/api/car-entry/filter/province/<province>', methods=['GET'])
def get_entries_by_province(province):
    """Retrieve car entries by province in a date range"""
    try:
        start = request.args.get('start', '2020-01-01')
        end = request.args.get('end', datetime.now().strftime('%Y-%m-%d'))
        
        entries = car_entry_repository.get_cars_by_province_in_date_range(province, start, end)
        return success_response(entries, count=len(entries),
                              filters={'province': province, 'startDate': start, 'endDate': end})
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY READ (HOURLY PRICE FOR PARKING) ----
@app.route('/api/car-entry/price/<int:parking_id>', methods=['GET'])
def get_hourly_price(parking_id):
    """Get hourly price for a specific parking facility"""
    try:
        price = car_entry_repository.get_hourly_price_for_parking(parking_id)
        return success_response({'hourlyPrice': price}, parkingId=parking_id)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY UPDATE ----
@app.route('/api/car-entry/<int:entry_id>', methods=['PUT'])
def update_car_entry(entry_id):
    """Update a car entry"""
    try:
        data = request.get_json()
        if not data:
            return error_response('Request body cannot be empty', 400)
        
        result = crud_service.update_car_entry(entry_id, data)
        if result['success']:
            return success_response(result['data'])
        else:
            return error_response(result['error'], 404)
    except Exception as e:
        return error_response(str(e), 500)

# ---- CAR ENTRY DELETE ----
@app.route('/api/car-entry/<int:entry_id>', methods=['DELETE'])
def delete_car_entry(entry_id):
    """Delete a car entry"""
    try:
        result = crud_service.delete_car_entry(entry_id)
        if result['success']:
            return success_response(result['data'])
        else:
            return error_response(result['error'], 404)
    except Exception as e:
        return error_response(str(e), 500)

# ================================================================
# HEALTH & INFO ENDPOINTS
# ================================================================

@app.route('/api/health', methods=['GET'])
def health_check():
    """Server health check"""
    return success_response({
        'status': 'healthy',
        'storage': 'database' if USE_DATABASE else 'json',
        'version': '1.0.0'
    })

@app.route('/api/endpoints', methods=['GET'])
def list_endpoints():
    """List all available endpoints"""
    endpoints = {
        'cars': {
            'create': 'POST /api/cars',
            'getAll': 'GET /api/cars',
            'getById': 'GET /api/cars/<id>',
            'getByColor': 'GET /api/cars/filter/color/<color>',
            'getByMake': 'GET /api/cars/filter/make/<make>',
            'getByType': 'GET /api/cars/filter/type/<type>',
            'getByYearRange': 'GET /api/cars/filter/year-range?min=YYYY&max=YYYY',
            'advancedFilter': 'GET /api/cars/filter/advanced?color=&minYear=&maxYear=&make=&type=',
            'update': 'PUT /api/cars/<id>',
            'delete': 'DELETE /api/cars/<id>'
        },
        'parking': {
            'create': 'POST /api/parking',
            'getAll': 'GET /api/parking',
            'getById': 'GET /api/parking/<id>',
            'getByProvince': 'GET /api/parking/filter/province/<province>',
            'getByName': 'GET /api/parking/filter/name/<name>',
            'getByRateRange': 'GET /api/parking/filter/rate-range?min=&max=',
            'advancedFilter': 'GET /api/parking/filter/advanced?province=&name=&minRate=&maxRate=',
            'update': 'PUT /api/parking/<id>',
            'delete': 'DELETE /api/parking/<id>'
        },
        'carEntry': {
            'create': 'POST /api/car-entry',
            'getAll': 'GET /api/car-entry',
            'getById': 'GET /api/car-entry/<id>',
            'getByParking': 'GET /api/car-entry/filter/parking/<parkingId>',
            'getByCarType': 'GET /api/car-entry/filter/car-type/<type>?start=&end=',
            'getByProvince': 'GET /api/car-entry/filter/province/<province>?start=&end=',
            'getHourlyPrice': 'GET /api/car-entry/price/<parkingId>',
            'update': 'PUT /api/car-entry/<id>',
            'delete': 'DELETE /api/car-entry/<id>'
        },
        'system': {
            'health': 'GET /api/health',
            'endpoints': 'GET /api/endpoints'
        }
    }
    return success_response(endpoints)

# 404 Handler
@app.errorhandler(404)
def not_found(error):
    """Handle 404 errors"""
    return error_response('Endpoint not found', 404)

# 500 Handler
@app.errorhandler(500)
def internal_error(error):
    """Handle 500 errors"""
    return error_response('Internal server error', 500)

# ================================================================
# START SERVER
# ================================================================

if __name__ == '__main__':
    print(f"""
╔════════════════════════════════════════╗
║   Car Park REST API Server Running     ║
╠════════════════════════════════════════╣
║ 🚗 Base URL: http://localhost:{PORT}/api   ║
║ 📊 Storage: {'Database' if USE_DATABASE else 'JSON Files'}            ║
║ 📝 API Docs: http://localhost:{PORT}/api/endpoints ║
║ 💚 Health: http://localhost:{PORT}/api/health      ║
╚════════════════════════════════════════╝
    """)
    app.run(host='0.0.0.0', port=PORT, debug=True)
