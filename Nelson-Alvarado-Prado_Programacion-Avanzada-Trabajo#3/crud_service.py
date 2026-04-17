"""
CRUD Service for Car Park Database - Python
Handles Insert, Update, Delete operations on all tables
Supports both JSON and Database sources
"""

import json
import os
from typing import Dict, Any, Optional, List, Tuple


# ================================================================
# JSON CRUD SERVICE
# ================================================================

class JsonCrudService:
    """Handle CRUD operations on JSON files"""

    def __init__(self, data_dir: str = '.'):
        self.data_dir = data_dir
        self.cars_file = os.path.join(data_dir, 'prq_cars.json')
        self.parking_file = os.path.join(data_dir, 'prq_parking.json')
        self.car_entry_file = os.path.join(data_dir, 'prq_car_entry.json')

    @staticmethod
    def _load_json(file_path: str) -> List[Dict]:
        """Load JSON file safely"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            print(f"Error loading {file_path}: {e}")
            return []

    @staticmethod
    def _save_json(file_path: str, data: List[Dict]) -> bool:
        """Save JSON file safely"""
        try:
            with open(file_path, 'w', encoding='utf-8') as f:
                json.dump(data, f, indent=2, ensure_ascii=False)
            return True
        except Exception as e:
            print(f"Error saving {file_path}: {e}")
            return False

    @staticmethod
    def _get_next_id(data: List[Dict]) -> int:
        """Get next ID for a table"""
        if not data:
            return 1
        max_id = max(item.get('id') or item.get('sequential_number') or 0 for item in data)
        return max_id + 1

    # ================================================================
    # CAR OPERATIONS
    # ================================================================

    def insert_car(self, car_data: Dict) -> Dict[str, Any]:
        """Insert a new car"""
        cars = self._load_json(self.cars_file)
        new_car = {
            'id': self._get_next_id(cars),
            'color': car_data.get('color'),
            'year': car_data.get('year'),
            'make': car_data.get('make'),
            'type': car_data.get('type')
        }
        cars.append(new_car)
        if self._save_json(self.cars_file, cars):
            return {'success': True, 'data': new_car}
        return {'success': False, 'error': 'Failed to save car'}

    def update_car(self, car_id: int, car_data: Dict) -> Dict[str, Any]:
        """Update an existing car"""
        cars = self._load_json(self.cars_file)
        car_index = next((i for i, c in enumerate(cars) if c['id'] == car_id), -1)
        if car_index == -1:
            return {'success': False, 'error': f'Car with ID {car_id} not found'}
        
        cars[car_index] = {'id': car_id, **car_data}
        if self._save_json(self.cars_file, cars):
            return {'success': True, 'data': cars[car_index]}
        return {'success': False, 'error': 'Failed to update car'}

    def delete_car(self, car_id: int) -> Dict[str, Any]:
        """Delete a car"""
        cars = self._load_json(self.cars_file)
        car_index = next((i for i, c in enumerate(cars) if c['id'] == car_id), -1)
        if car_index == -1:
            return {'success': False, 'error': f'Car with ID {car_id} not found'}
        
        deleted_car = cars[car_index]
        cars.pop(car_index)
        if self._save_json(self.cars_file, cars):
            return {'success': True, 'data': deleted_car}
        return {'success': False, 'error': 'Failed to delete car'}

    # ================================================================
    # PARKING OPERATIONS
    # ================================================================

    def insert_parking(self, parking_data: Dict) -> Dict[str, Any]:
        """Insert a new parking facility"""
        parkings = self._load_json(self.parking_file)
        new_parking = {
            'id': self._get_next_id(parkings),
            'province_name': parking_data.get('province_name'),
            'name': parking_data.get('name'),
            'hourly_rate': parking_data.get('hourly_rate')
        }
        parkings.append(new_parking)
        if self._save_json(self.parking_file, parkings):
            return {'success': True, 'data': new_parking}
        return {'success': False, 'error': 'Failed to save parking'}

    def update_parking(self, parking_id: int, parking_data: Dict) -> Dict[str, Any]:
        """Update an existing parking facility"""
        parkings = self._load_json(self.parking_file)
        parking_index = next((i for i, p in enumerate(parkings) if p['id'] == parking_id), -1)
        if parking_index == -1:
            return {'success': False, 'error': f'Parking with ID {parking_id} not found'}
        
        parkings[parking_index] = {'id': parking_id, **parking_data}
        if self._save_json(self.parking_file, parkings):
            return {'success': True, 'data': parkings[parking_index]}
        return {'success': False, 'error': 'Failed to update parking'}

    def delete_parking(self, parking_id: int) -> Dict[str, Any]:
        """Delete a parking facility"""
        parkings = self._load_json(self.parking_file)
        parking_index = next((i for i, p in enumerate(parkings) if p['id'] == parking_id), -1)
        if parking_index == -1:
            return {'success': False, 'error': f'Parking with ID {parking_id} not found'}
        
        deleted_parking = parkings[parking_index]
        parkings.pop(parking_index)
        if self._save_json(self.parking_file, parkings):
            return {'success': True, 'data': deleted_parking}
        return {'success': False, 'error': 'Failed to delete parking'}

    # ================================================================
    # CAR ENTRY OPERATIONS
    # ================================================================

    def insert_car_entry(self, entry_data: Dict) -> Dict[str, Any]:
        """Insert a new car entry"""
        entries = self._load_json(self.car_entry_file)
        new_entry = {
            'sequential_number': self._get_next_id(entries),
            'parking_id': entry_data.get('parking_id'),
            'car_id': entry_data.get('car_id'),
            'entry_date_time': entry_data.get('entry_date_time'),
            'exit_date_time': entry_data.get('exit_date_time')
        }
        entries.append(new_entry)
        if self._save_json(self.car_entry_file, entries):
            return {'success': True, 'data': new_entry}
        return {'success': False, 'error': 'Failed to save car entry'}

    def update_car_entry(self, sequential_number: int, entry_data: Dict) -> Dict[str, Any]:
        """Update an existing car entry"""
        entries = self._load_json(self.car_entry_file)
        entry_index = next((i for i, e in enumerate(entries) if e['sequential_number'] == sequential_number), -1)
        if entry_index == -1:
            return {'success': False, 'error': f'Car entry #{sequential_number} not found'}
        
        entries[entry_index] = {'sequential_number': sequential_number, **entry_data}
        if self._save_json(self.car_entry_file, entries):
            return {'success': True, 'data': entries[entry_index]}
        return {'success': False, 'error': 'Failed to update car entry'}

    def delete_car_entry(self, sequential_number: int) -> Dict[str, Any]:
        """Delete a car entry"""
        entries = self._load_json(self.car_entry_file)
        entry_index = next((i for i, e in enumerate(entries) if e['sequential_number'] == sequential_number), -1)
        if entry_index == -1:
            return {'success': False, 'error': f'Car entry #{sequential_number} not found'}
        
        deleted_entry = entries[entry_index]
        entries.pop(entry_index)
        if self._save_json(self.car_entry_file, entries):
            return {'success': True, 'data': deleted_entry}
        return {'success': False, 'error': 'Failed to delete car entry'}


# ================================================================
# DATABASE CRUD SERVICE
# ================================================================

class DatabaseCrudService:
    """Handle CRUD operations on Database"""

    def __init__(self, db_connection):
        self.db = db_connection

    # ================================================================
    # CAR OPERATIONS
    # ================================================================

    async def insert_car(self, car_data: Dict) -> Dict[str, Any]:
        """Insert a new car"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute(
                'INSERT INTO PRQ_Cars (color, year, make, type) VALUES (%s, %s, %s, %s)',
                (car_data.get('color'), car_data.get('year'), car_data.get('make'), car_data.get('type'))
            )
            self.db.connection.commit()
            new_id = cursor.lastrowid
            cursor.close()
            return {'success': True, 'data': {'id': new_id, **car_data}}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    async def update_car(self, car_id: int, car_data: Dict) -> Dict[str, Any]:
        """Update an existing car"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute(
                'UPDATE PRQ_Cars SET color = %s, year = %s, make = %s, type = %s WHERE ID = %s',
                (car_data.get('color'), car_data.get('year'), car_data.get('make'), car_data.get('type'), car_id)
            )
            self.db.connection.commit()
            cursor.close()
            return {'success': True, 'data': {'id': car_id, **car_data}}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    async def delete_car(self, car_id: int) -> Dict[str, Any]:
        """Delete a car"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute('SELECT * FROM PRQ_Cars WHERE ID = %s', (car_id,))
            car = cursor.fetchone()
            if not car:
                cursor.close()
                return {'success': False, 'error': f'Car with ID {car_id} not found'}
            
            cursor.execute('DELETE FROM PRQ_Cars WHERE ID = %s', (car_id,))
            self.db.connection.commit()
            cursor.close()
            return {'success': True, 'data': car}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    # ================================================================
    # PARKING OPERATIONS
    # ================================================================

    async def insert_parking(self, parking_data: Dict) -> Dict[str, Any]:
        """Insert a new parking facility"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute(
                'INSERT INTO PRQ_Parking (province_name, name, hourly_rate) VALUES (%s, %s, %s)',
                (parking_data.get('province_name'), parking_data.get('name'), parking_data.get('hourly_rate'))
            )
            self.db.connection.commit()
            new_id = cursor.lastrowid
            cursor.close()
            return {'success': True, 'data': {'id': new_id, **parking_data}}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    async def update_parking(self, parking_id: int, parking_data: Dict) -> Dict[str, Any]:
        """Update an existing parking facility"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute(
                'UPDATE PRQ_Parking SET province_name = %s, name = %s, hourly_rate = %s WHERE ID = %s',
                (parking_data.get('province_name'), parking_data.get('name'), parking_data.get('hourly_rate'), parking_id)
            )
            self.db.connection.commit()
            cursor.close()
            return {'success': True, 'data': {'id': parking_id, **parking_data}}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    async def delete_parking(self, parking_id: int) -> Dict[str, Any]:
        """Delete a parking facility"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute('SELECT * FROM PRQ_Parking WHERE ID = %s', (parking_id,))
            parking = cursor.fetchone()
            if not parking:
                cursor.close()
                return {'success': False, 'error': f'Parking with ID {parking_id} not found'}
            
            cursor.execute('DELETE FROM PRQ_Parking WHERE ID = %s', (parking_id,))
            self.db.connection.commit()
            cursor.close()
            return {'success': True, 'data': parking}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    # ================================================================
    # CAR ENTRY OPERATIONS
    # ================================================================

    async def insert_car_entry(self, entry_data: Dict) -> Dict[str, Any]:
        """Insert a new car entry"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute(
                'INSERT INTO PRQ_Car_Entry (parking_id, car_id, entry_date_time, exit_date_time) VALUES (%s, %s, %s, %s)',
                (entry_data.get('parking_id'), entry_data.get('car_id'), entry_data.get('entry_date_time'), 
                 entry_data.get('exit_date_time'))
            )
            self.db.connection.commit()
            new_id = cursor.lastrowid
            cursor.close()
            return {'success': True, 'data': {'sequential_number': new_id, **entry_data}}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    async def update_car_entry(self, sequential_number: int, entry_data: Dict) -> Dict[str, Any]:
        """Update an existing car entry"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute(
                'UPDATE PRQ_Car_Entry SET parking_id = %s, car_id = %s, entry_date_time = %s, exit_date_time = %s WHERE sequential_number = %s',
                (entry_data.get('parking_id'), entry_data.get('car_id'), entry_data.get('entry_date_time'),
                 entry_data.get('exit_date_time'), sequential_number)
            )
            self.db.connection.commit()
            cursor.close()
            return {'success': True, 'data': {'sequential_number': sequential_number, **entry_data}}
        except Exception as e:
            return {'success': False, 'error': str(e)}

    async def delete_car_entry(self, sequential_number: int) -> Dict[str, Any]:
        """Delete a car entry"""
        try:
            cursor = self.db.connection.cursor(dictionary=True)
            cursor.execute('SELECT * FROM PRQ_Car_Entry WHERE sequential_number = %s', (sequential_number,))
            entry = cursor.fetchone()
            if not entry:
                cursor.close()
                return {'success': False, 'error': f'Car entry #{sequential_number} not found'}
            
            cursor.execute('DELETE FROM PRQ_Car_Entry WHERE sequential_number = %s', (sequential_number,))
            self.db.connection.commit()
            cursor.close()
            return {'success': True, 'data': entry}
        except Exception as e:
            return {'success': False, 'error': str(e)}
