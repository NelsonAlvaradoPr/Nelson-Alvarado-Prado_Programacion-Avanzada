"""
API Usage Examples - Python
Demonstrates how to use all REST API endpoints
"""

import requests
import json
from datetime import datetime, timedelta

BASE_URL = 'http://localhost:5000/api'

# ================================================================
# UTILITY FUNCTIONS
# ================================================================

def print_response(title, response):
    """Pretty print JSON responses"""
    print(f"\n{'='*60}")
    print(f"📋 {title}")
    print('='*60)
    print(json.dumps(response, indent=2, ensure_ascii=False))

def make_request(method, endpoint, data=None):
    """Make HTTP request and return JSON"""
    try:
        url = f"{BASE_URL}{endpoint}"
        if method == 'GET':
            response = requests.get(url)
        elif method == 'POST':
            response = requests.post(url, json=data)
        elif method == 'PUT':
            response = requests.put(url, json=data)
        elif method == 'DELETE':
            response = requests.delete(url)
        
        if response.status_code in [200, 201]:
            return response.json()
        else:
            print(f"❌ Error {response.status_code}: {response.text}")
            return None
    except Exception as e:
        print(f"❌ Request Error: {e}")
        return None

def get(endpoint):
    """Make GET request"""
    return make_request('GET', endpoint)

def post(endpoint, data):
    """Make POST request"""
    return make_request('POST', endpoint, data)

def put(endpoint, data):
    """Make PUT request"""
    return make_request('PUT', endpoint, data)

def delete(endpoint):
    """Make DELETE request"""
    return make_request('DELETE', endpoint)

# ================================================================
# CAR OPERATIONS EXAMPLES
# ================================================================

def example_create_car():
    """Example: Create a new car"""
    print("\n🚗 CREATING A NEW CAR")
    new_car = {
        'color': 'Silver',
        'year': 2024,
        'make': 'Audi A4',
        'type': 'sedan'
    }
    result = post('/cars', new_car)
    print_response('Create Car Response', result)
    return result['data']['id'] if result and result.get('success') else None

def example_get_all_cars():
    """Example: Get all cars"""
    print("\n🚗 GETTING ALL CARS")
    result = get('/cars')
    print_response('Get All Cars Response', result)
    return result['data'] if result else None

def example_get_car_by_id(car_id):
    """Example: Get car by ID"""
    print(f"\n🚗 GETTING CAR BY ID: {car_id}")
    result = get(f'/cars/{car_id}')
    print_response('Get Car by ID Response', result)

def example_get_cars_by_color(color):
    """Example: Get cars by color"""
    print(f"\n🚗 GETTING CARS BY COLOR: {color}")
    result = get(f'/cars/filter/color/{color}')
    print_response('Get Cars by Color Response', result)

def example_get_cars_by_make(make):
    """Example: Get cars by make"""
    print(f"\n🚗 GETTING CARS BY MAKE: {make}")
    result = get(f'/cars/filter/make/{make.replace(" ", "%20")}')
    print_response('Get Cars by Make Response', result)

def example_get_cars_by_type(car_type):
    """Example: Get cars by type"""
    print(f"\n🚗 GETTING CARS BY TYPE: {car_type}")
    result = get(f'/cars/filter/type/{car_type}')
    print_response('Get Cars by Type Response', result)

def example_get_cars_by_year_range(min_year, max_year):
    """Example: Get cars by year range"""
    print(f"\n🚗 GETTING CARS BY YEAR RANGE: {min_year}-{max_year}")
    result = get(f'/cars/filter/year-range?min={min_year}&max={max_year}')
    print_response('Get Cars by Year Range Response', result)

def example_get_cars_advanced_filter():
    """Example: Get cars with advanced filter"""
    print("\n🚗 GETTING CARS WITH ADVANCED FILTER")
    params = {
        'color': 'Red',
        'minYear': 2020,
        'maxYear': 2025,
        'type': 'sedan'
    }
    query_string = '&'.join([f"{k}={v}" for k, v in params.items()])
    result = get(f'/cars/filter/advanced?{query_string}')
    print_response('Get Cars Advanced Filter Response', result)

def example_update_car(car_id):
    """Example: Update a car"""
    print(f"\n🚗 UPDATING CAR: {car_id}")
    update_data = {
        'color': 'Gold',
        'year': 2024,
        'make': 'Audi A6',
        'type': 'sedan'
    }
    result = put(f'/cars/{car_id}', update_data)
    print_response('Update Car Response', result)

def example_delete_car(car_id):
    """Example: Delete a car"""
    print(f"\n🚗 DELETING CAR: {car_id}")
    result = delete(f'/cars/{car_id}')
    print_response('Delete Car Response', result)

# ================================================================
# PARKING OPERATIONS EXAMPLES
# ================================================================

def example_create_parking():
    """Example: Create a new parking facility"""
    print("\n🅿️  CREATING A NEW PARKING FACILITY")
    new_parking = {
        'province_name': 'Murcia',
        'name': 'Parking Aeropuerto',
        'hourly_rate': 3.75
    }
    result = post('/parking', new_parking)
    print_response('Create Parking Response', result)
    return result['data']['id'] if result and result.get('success') else None

def example_get_all_parking():
    """Example: Get all parking facilities"""
    print("\n🅿️  GETTING ALL PARKING FACILITIES")
    result = get('/parking')
    print_response('Get All Parking Response', result)
    return result['data'] if result else None

def example_get_parking_by_id(parking_id):
    """Example: Get parking by ID"""
    print(f"\n🅿️  GETTING PARKING BY ID: {parking_id}")
    result = get(f'/parking/{parking_id}')
    print_response('Get Parking by ID Response', result)

def example_get_parking_by_province(province):
    """Example: Get parking by province"""
    print(f"\n🅿️  GETTING PARKING BY PROVINCE: {province}")
    result = get(f'/parking/filter/province/{province}')
    print_response('Get Parking by Province Response', result)

def example_get_parking_by_name(name):
    """Example: Get parking by name"""
    print(f"\n🅿️  GETTING PARKING BY NAME: {name}")
    result = get(f'/parking/filter/name/{name.replace(" ", "%20")}')
    print_response('Get Parking by Name Response', result)

def example_get_parking_by_rate_range(min_rate, max_rate):
    """Example: Get parking by rate range"""
    print(f"\n🅿️  GETTING PARKING BY RATE RANGE: €{min_rate}-€{max_rate}")
    result = get(f'/parking/filter/rate-range?min={min_rate}&max={max_rate}')
    print_response('Get Parking by Rate Range Response', result)

def example_get_parking_advanced_filter():
    """Example: Get parking with advanced filter"""
    print("\n🅿️  GETTING PARKING WITH ADVANCED FILTER")
    params = {
        'province': 'Madrid',
        'minRate': 3.0,
        'maxRate': 4.5
    }
    query_string = '&'.join([f"{k}={v}" for k, v in params.items()])
    result = get(f'/parking/filter/advanced?{query_string}')
    print_response('Get Parking Advanced Filter Response', result)

def example_update_parking(parking_id):
    """Example: Update a parking facility"""
    print(f"\n🅿️  UPDATING PARKING: {parking_id}")
    update_data = {
        'province_name': 'Murcia',
        'name': 'Parking Aeropuerto Premium',
        'hourly_rate': 4.50
    }
    result = put(f'/parking/{parking_id}', update_data)
    print_response('Update Parking Response', result)

def example_delete_parking(parking_id):
    """Example: Delete a parking facility"""
    print(f"\n🅿️  DELETING PARKING: {parking_id}")
    result = delete(f'/parking/{parking_id}')
    print_response('Delete Parking Response', result)

# ================================================================
# CAR ENTRY OPERATIONS EXAMPLES
# ================================================================

def example_create_car_entry():
    """Example: Create a new car entry"""
    print("\n🚙 CREATING A NEW CAR ENTRY")
    new_entry = {
        'parking_id': 1,
        'car_id': 1,
        'entry_date_time': '2026-04-17T10:00:00',
        'exit_date_time': None
    }
    result = post('/car-entry', new_entry)
    print_response('Create Car Entry Response', result)
    return result['data']['sequential_number'] if result and result.get('success') else None

def example_get_all_car_entries():
    """Example: Get all car entries"""
    print("\n🚙 GETTING ALL CAR ENTRIES")
    result = get('/car-entry')
    print_response('Get All Car Entries Response', result)
    return result['data'] if result else None

def example_get_car_entry_by_id(entry_id):
    """Example: Get car entry by ID"""
    print(f"\n🚙 GETTING CAR ENTRY BY ID: {entry_id}")
    result = get(f'/car-entry/{entry_id}')
    print_response('Get Car Entry by ID Response', result)

def example_get_entries_by_parking(parking_id):
    """Example: Get car entries by parking"""
    print(f"\n🚙 GETTING ENTRIES FOR PARKING: {parking_id}")
    result = get(f'/car-entry/filter/parking/{parking_id}')
    print_response('Get Entries by Parking Response', result)

def example_get_entries_by_car_type(car_type):
    """Example: Get car entries by car type in date range"""
    print(f"\n🚙 GETTING ENTRIES FOR CAR TYPE: {car_type}")
    params = {
        'start': '2026-04-10',
        'end': '2026-04-20'
    }
    query_string = '&'.join([f"{k}={v}" for k, v in params.items()])
    result = get(f'/car-entry/filter/car-type/{car_type}?{query_string}')
    print_response('Get Entries by Car Type Response', result)

def example_get_entries_by_province(province):
    """Example: Get car entries by province in date range"""
    print(f"\n🚙 GETTING ENTRIES FOR PROVINCE: {province}")
    params = {
        'start': '2026-04-10',
        'end': '2026-04-20'
    }
    query_string = '&'.join([f"{k}={v}" for k, v in params.items()])
    result = get(f'/car-entry/filter/province/{province}?{query_string}')
    print_response('Get Entries by Province Response', result)

def example_get_hourly_price(parking_id):
    """Example: Get hourly price for parking"""
    print(f"\n🚙 GETTING HOURLY PRICE FOR PARKING: {parking_id}")
    result = get(f'/car-entry/price/{parking_id}')
    print_response('Get Hourly Price Response', result)

def example_update_car_entry(entry_id):
    """Example: Update a car entry"""
    print(f"\n🚙 UPDATING CAR ENTRY: {entry_id}")
    update_data = {
        'parking_id': 1,
        'car_id': 1,
        'entry_date_time': '2026-04-17T10:00:00',
        'exit_date_time': '2026-04-17T13:00:00'
    }
    result = put(f'/car-entry/{entry_id}', update_data)
    print_response('Update Car Entry Response', result)

def example_delete_car_entry(entry_id):
    """Example: Delete a car entry"""
    print(f"\n🚙 DELETING CAR ENTRY: {entry_id}")
    result = delete(f'/car-entry/{entry_id}')
    print_response('Delete Car Entry Response', result)

# ================================================================
# SYSTEM ENDPOINTS EXAMPLES
# ================================================================

def example_health_check():
    """Example: Health check"""
    print("\n💚 HEALTH CHECK")
    result = get('/health')
    print_response('Health Check Response', result)

def example_get_endpoints():
    """Example: Get all endpoints"""
    print("\n📋 GET ALL ENDPOINTS")
    result = get('/endpoints')
    print_response('Endpoints List', result)

# ================================================================
# MAIN EXECUTION
# ================================================================

def run_all_examples():
    """Run all examples"""
    try:
        print("""
╔════════════════════════════════════════╗
║   Car Park REST API - Python           ║
║           Usage Examples               ║
╚════════════════════════════════════════╝
        """)

        # System endpoints
        example_health_check()
        example_get_endpoints()

        # Car operations
        new_car_id = example_create_car()
        example_get_all_cars()
        if new_car_id:
            example_get_car_by_id(new_car_id)
        example_get_cars_by_color('Red')
        example_get_cars_by_make('Toyota Corolla')
        example_get_cars_by_type('sedan')
        example_get_cars_by_year_range(2020, 2025)
        example_get_cars_advanced_filter()
        if new_car_id:
            example_update_car(new_car_id)

        # Parking operations
        new_parking_id = example_create_parking()
        example_get_all_parking()
        if new_parking_id:
            example_get_parking_by_id(new_parking_id)
        example_get_parking_by_province('Madrid')
        example_get_parking_by_rate_range(2.0, 4.0)
        example_get_parking_advanced_filter()
        if new_parking_id:
            example_update_parking(new_parking_id)

        # Car entry operations
        new_entry_id = example_create_car_entry()
        example_get_all_car_entries()
        if new_entry_id:
            example_get_car_entry_by_id(new_entry_id)
        example_get_entries_by_parking(1)
        example_get_entries_by_car_type('sedan')
        example_get_entries_by_province('Madrid')
        example_get_hourly_price(1)
        if new_entry_id:
            example_update_car_entry(new_entry_id)

        print("""
╔════════════════════════════════════════╗
║        Examples Completed! ✅           ║
╚════════════════════════════════════════╝
        """)

    except Exception as error:
        print(f"❌ Error running examples: {error}")

if __name__ == '__main__':
    run_all_examples()
