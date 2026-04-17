"""
Vehicle Management MVC Application
Allows querying vehicles by year range with complete CRUD operations
"""

from flask import Flask, render_template, request, jsonify, redirect, url_for, flash
from flask_cors import CORS
import requests
import os
from dotenv import load_dotenv
from datetime import datetime

load_dotenv()

app = Flask(__name__)
app.secret_key = 'vehicle-mvc-secret-key-2024'
CORS(app)

# API Configuration
API_BASE_URL = os.getenv('API_URL', 'http://localhost:5000/api')
CURRENT_YEAR = datetime.now().year

# ================================================================
# API CLIENT (Model Layer)
# ================================================================

class VehicleAPIClient:
    """Client for interacting with the Vehicle REST API"""
    
    def __init__(self, base_url=API_BASE_URL):
        self.base_url = base_url
    
    def get_vehicles_by_year_range(self, min_year, max_year):
        """Fetch vehicles filtered by year range"""
        try:
            url = f"{self.base_url}/cars/filter/year-range"
            params = {'min': min_year, 'max': max_year}
            response = requests.get(url, params=params, timeout=5)
            response.raise_for_status()
            data = response.json()
            return data.get('data', []), None
        except requests.exceptions.RequestException as e:
            return None, f"Error fetching vehicles: {str(e)}"
    
    def get_vehicle_by_id(self, vehicle_id):
        """Fetch a single vehicle by ID"""
        try:
            url = f"{self.base_url}/cars/{vehicle_id}"
            response = requests.get(url, timeout=5)
            response.raise_for_status()
            data = response.json()
            return data.get('data'), None
        except requests.exceptions.RequestException as e:
            return None, f"Error fetching vehicle: {str(e)}"
    
    def create_vehicle(self, vehicle_data):
        """Create a new vehicle"""
        try:
            url = f"{self.base_url}/cars"
            response = requests.post(url, json=vehicle_data, timeout=5)
            response.raise_for_status()
            data = response.json()
            return data.get('data'), None
        except requests.exceptions.RequestException as e:
            return None, f"Error creating vehicle: {str(e)}"
    
    def update_vehicle(self, vehicle_id, vehicle_data):
        """Update an existing vehicle"""
        try:
            url = f"{self.base_url}/cars/{vehicle_id}"
            response = requests.put(url, json=vehicle_data, timeout=5)
            response.raise_for_status()
            data = response.json()
            return data.get('data'), None
        except requests.exceptions.RequestException as e:
            return None, f"Error updating vehicle: {str(e)}"
    
    def delete_vehicle(self, vehicle_id):
        """Delete a vehicle"""
        try:
            url = f"{self.base_url}/cars/{vehicle_id}"
            response = requests.delete(url, timeout=5)
            response.raise_for_status()
            return True, None
        except requests.exceptions.RequestException as e:
            return False, f"Error deleting vehicle: {str(e)}"

# Initialize API client
api_client = VehicleAPIClient()

# ================================================================
# CONTROLLER ROUTES
# ================================================================

@app.route('/')
def index():
    """Home page - display vehicle search form"""
    min_year = request.args.get('min_year', 2018, type=int)
    max_year = request.args.get('max_year', CURRENT_YEAR, type=int)
    
    vehicles, error = api_client.get_vehicles_by_year_range(min_year, max_year)
    
    if error:
        flash(f"⚠️ {error}", 'error')
        vehicles = []
    
    return render_template('vehicles.html',
                         vehicles=vehicles or [],
                         min_year=min_year,
                         max_year=max_year,
                         current_year=CURRENT_YEAR)

@app.route('/api/vehicles/search', methods=['GET'])
def search_vehicles():
    """API endpoint for searching vehicles by year range"""
    try:
        min_year = request.args.get('min_year', 1900, type=int)
        max_year = request.args.get('max_year', CURRENT_YEAR, type=int)
        
        vehicles, error = api_client.get_vehicles_by_year_range(min_year, max_year)
        
        if error:
            return jsonify({'success': False, 'error': error}), 400
        
        return jsonify({
            'success': True,
            'data': vehicles,
            'count': len(vehicles) if vehicles else 0
        })
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/vehicles/<int:vehicle_id>')
def view_vehicle(vehicle_id):
    """View details of a single vehicle"""
    vehicle, error = api_client.get_vehicle_by_id(vehicle_id)
    
    if error or not vehicle:
        flash(f"⚠️ {error or 'Vehicle not found'}", 'error')
        return redirect(url_for('index'))
    
    return render_template('vehicle_detail.html', vehicle=vehicle)

@app.route('/vehicles/new')
def create_vehicle_page():
    """Display form to create a new vehicle"""
    vehicle_types = ['sedan', '4x4', 'motorcycle']
    return render_template('vehicle_form.html',
                         vehicle=None,
                         types=vehicle_types,
                         current_year=CURRENT_YEAR)

@app.route('/vehicles/<int:vehicle_id>/edit')
def edit_vehicle_page(vehicle_id):
    """Display form to edit an existing vehicle"""
    vehicle, error = api_client.get_vehicle_by_id(vehicle_id)
    
    if error or not vehicle:
        flash(f"⚠️ {error or 'Vehicle not found'}", 'error')
        return redirect(url_for('index'))
    
    vehicle_types = ['sedan', '4x4', 'motorcycle']
    return render_template('vehicle_form.html',
                         vehicle=vehicle,
                         types=vehicle_types,
                         current_year=CURRENT_YEAR)

@app.route('/api/vehicles', methods=['POST'])
def api_create_vehicle():
    """API endpoint to create a new vehicle"""
    try:
        data = request.get_json()
        
        # Validate required fields
        required_fields = ['color', 'year', 'make', 'type']
        if not all(field in data for field in required_fields):
            return jsonify({
                'success': False,
                'error': f'Missing required fields: {", ".join(required_fields)}'
            }), 400
        
        vehicle, error = api_client.create_vehicle(data)
        
        if error:
            return jsonify({'success': False, 'error': error}), 400
        
        return jsonify({
            'success': True,
            'message': 'Vehicle created successfully',
            'data': vehicle
        }), 201
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/api/vehicles/<int:vehicle_id>', methods=['PUT'])
def api_update_vehicle(vehicle_id):
    """API endpoint to update a vehicle"""
    try:
        data = request.get_json()
        
        if not data:
            return jsonify({
                'success': False,
                'error': 'Request body cannot be empty'
            }), 400
        
        vehicle, error = api_client.update_vehicle(vehicle_id, data)
        
        if error:
            return jsonify({'success': False, 'error': error}), 400
        
        return jsonify({
            'success': True,
            'message': 'Vehicle updated successfully',
            'data': vehicle
        })
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/api/vehicles/<int:vehicle_id>', methods=['DELETE'])
def api_delete_vehicle(vehicle_id):
    """API endpoint to delete a vehicle"""
    try:
        success, error = api_client.delete_vehicle(vehicle_id)
        
        if error:
            return jsonify({'success': False, 'error': error}), 400
        
        return jsonify({
            'success': True,
            'message': 'Vehicle deleted successfully'
        })
    except Exception as e:
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/health')
def health():
    """Health check endpoint"""
    return jsonify({
        'status': 'healthy',
        'service': 'Vehicle MVC Application',
        'api_url': API_BASE_URL
    })

# ================================================================
# ERROR HANDLERS
# ================================================================

@app.errorhandler(404)
def not_found(error):
    """Handle 404 errors"""
    flash('❌ Page not found', 'error')
    return redirect(url_for('index'))

@app.errorhandler(500)
def internal_error(error):
    """Handle 500 errors"""
    flash('❌ Internal server error', 'error')
    return redirect(url_for('index'))

if __name__ == '__main__':
    print(f"🚗 Vehicle Management Application starting...")
    print(f"📡 API URL: {API_BASE_URL}")
    print(f"🌐 Web UI: http://localhost:8080")
    app.run(debug=True, port=8080, host='0.0.0.0')
