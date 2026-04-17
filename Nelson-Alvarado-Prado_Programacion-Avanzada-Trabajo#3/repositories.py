"""
Repository Interfaces and Implementations - Python
Provides data access layer for car park tables with support for both JSON and Database sources
"""

import json
import os
from abc import ABC, abstractmethod
from datetime import datetime
from typing import List, Optional, Dict, Any
from car_parking_session import CarParkingSession


# ================================================================
# INTERFACES
# ================================================================

class IRepository(ABC):
    """Base Repository Interface"""

    @abstractmethod
    async def get_by_id(self, id: Any) -> Optional[Dict]:
        """Get record by primary key"""
        pass

    @abstractmethod
    async def get_all(self) -> List[Dict]:
        """Get all records"""
        pass


class ICarRepository(IRepository):
    """Car Repository Interface - PRQ_Cars queries"""

    @abstractmethod
    async def get_by_id(self, id: int) -> Optional[Dict]:
        """Get car by ID"""
        pass

    @abstractmethod
    async def get_all(self) -> List[Dict]:
        """Get all cars"""
        pass

    @abstractmethod
    async def get_by_color(self, color: str) -> List[Dict]:
        """Get cars by color (approximate match)"""
        pass

    @abstractmethod
    async def get_by_year_range(self, min_year: int, max_year: int) -> List[Dict]:
        """Get cars by year range"""
        pass

    @abstractmethod
    async def get_by_make(self, make: str) -> List[Dict]:
        """Get cars by manufacturer name (approximate match)"""
        pass

    @abstractmethod
    async def get_by_type(self, car_type: str) -> List[Dict]:
        """Get cars by type (sedan, 4x4, motorcycle)"""
        pass

    @abstractmethod
    async def get_by_color_and_year_range_and_make_and_type(
        self, color: Optional[str], min_year: int, max_year: int,
        make: Optional[str], car_type: Optional[str]
    ) -> List[Dict]:
        """Get cars by combined filters"""
        pass


class IParkingRepository(IRepository):
    """Parking Repository Interface - PRQ_Parking queries"""

    @abstractmethod
    async def get_by_id(self, id: int) -> Optional[Dict]:
        """Get parking by ID"""
        pass

    @abstractmethod
    async def get_all(self) -> List[Dict]:
        """Get all parking facilities"""
        pass

    @abstractmethod
    async def get_by_province_name(self, province_name: str) -> List[Dict]:
        """Get parking by province name (approximate match)"""
        pass

    @abstractmethod
    async def get_by_name(self, name: str) -> List[Dict]:
        """Get parking by name (approximate match)"""
        pass

    @abstractmethod
    async def get_by_hourly_rate_range(self, min_rate: float, max_rate: float) -> List[Dict]:
        """Get parking by hourly rate range"""
        pass

    @abstractmethod
    async def get_by_province_and_name_and_hourly_rate_range(
        self, province_name: Optional[str], name: Optional[str],
        min_rate: float, max_rate: float
    ) -> List[Dict]:
        """Get parking by combined filters"""
        pass


class ICarEntryRepository(IRepository):
    """Car Entry Repository Interface - PRQ_Car_Entry queries"""

    @abstractmethod
    async def get_by_id(self, sequential_number: int) -> Optional[Dict]:
        """Get car entry by sequential number"""
        pass

    @abstractmethod
    async def get_all(self) -> List[Dict]:
        """Get all car entries"""
        pass

    @abstractmethod
    async def get_hourly_price_for_parking(self, parking_id: int) -> Optional[float]:
        """Get hourly rate for a specific parking facility"""
        pass

    @abstractmethod
    async def get_cars_by_type_in_date_range(
        self, car_type: str, start_date: str, end_date: str
    ) -> List[Dict]:
        """Get cars by type that entered in date range with entry/exit times and amount paid"""
        pass

    @abstractmethod
    async def get_cars_by_province_in_date_range(
        self, province_name: str, start_date: str, end_date: str
    ) -> List[Dict]:
        """Get cars by province that entered in date range with entry/exit times and amount due"""
        pass


# ================================================================
# JSON IMPLEMENTATIONS
# ================================================================

class JsonCarRepository(ICarRepository):
    """JSON-based Car Repository"""

    def __init__(self, json_file_path: str):
        self.data = self._load_json(json_file_path)

    @staticmethod
    def _load_json(file_path: str) -> List[Dict]:
        """Load JSON file"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            print(f"Error loading JSON file {file_path}: {e}")
            return []

    async def get_by_id(self, id: int) -> Optional[Dict]:
        for car in self.data:
            if car.get('id') == id:
                return car
        return None

    async def get_all(self) -> List[Dict]:
        return self.data

    async def get_by_color(self, color: str) -> List[Dict]:
        return [c for c in self.data if color.lower() in c.get('color', '').lower()]

    async def get_by_year_range(self, min_year: int, max_year: int) -> List[Dict]:
        return [c for c in self.data if min_year <= c.get('year', 0) <= max_year]

    async def get_by_make(self, make: str) -> List[Dict]:
        return [c for c in self.data if make.lower() in c.get('make', '').lower()]

    async def get_by_type(self, car_type: str) -> List[Dict]:
        return [c for c in self.data if c.get('type') == car_type]

    async def get_by_color_and_year_range_and_make_and_type(
        self, color: Optional[str], min_year: int, max_year: int,
        make: Optional[str], car_type: Optional[str]
    ) -> List[Dict]:
        result = self.data
        
        if color:
            result = [c for c in result if color.lower() in c.get('color', '').lower()]
        
        result = [c for c in result if min_year <= c.get('year', 0) <= max_year]
        
        if make:
            result = [c for c in result if make.lower() in c.get('make', '').lower()]
        
        if car_type:
            result = [c for c in result if c.get('type') == car_type]
        
        return result


class JsonParkingRepository(IParkingRepository):
    """JSON-based Parking Repository"""

    def __init__(self, json_file_path: str):
        self.data = self._load_json(json_file_path)

    @staticmethod
    def _load_json(file_path: str) -> List[Dict]:
        """Load JSON file"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            print(f"Error loading JSON file {file_path}: {e}")
            return []

    async def get_by_id(self, id: int) -> Optional[Dict]:
        for parking in self.data:
            if parking.get('id') == id:
                return parking
        return None

    async def get_all(self) -> List[Dict]:
        return self.data

    async def get_by_province_name(self, province_name: str) -> List[Dict]:
        return [p for p in self.data 
                if province_name.lower() in p.get('province_name', '').lower()]

    async def get_by_name(self, name: str) -> List[Dict]:
        return [p for p in self.data if name.lower() in p.get('name', '').lower()]

    async def get_by_hourly_rate_range(self, min_rate: float, max_rate: float) -> List[Dict]:
        return [p for p in self.data 
                if min_rate <= p.get('hourly_rate', 0) <= max_rate]

    async def get_by_province_and_name_and_hourly_rate_range(
        self, province_name: Optional[str], name: Optional[str],
        min_rate: float, max_rate: float
    ) -> List[Dict]:
        result = self.data
        
        if province_name:
            result = [p for p in result 
                     if province_name.lower() in p.get('province_name', '').lower()]
        
        if name:
            result = [p for p in result if name.lower() in p.get('name', '').lower()]
        
        result = [p for p in result if min_rate <= p.get('hourly_rate', 0) <= max_rate]
        
        return result


class JsonCarEntryRepository(ICarEntryRepository):
    """JSON-based Car Entry Repository"""

    def __init__(self, car_entry_json_path: str, parking_json_path: str):
        self.car_entry_data = self._load_json(car_entry_json_path)
        self.parking_repository = JsonParkingRepository(parking_json_path)

    @staticmethod
    def _load_json(file_path: str) -> List[Dict]:
        """Load JSON file"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            print(f"Error loading JSON file {file_path}: {e}")
            return []

    async def get_by_id(self, sequential_number: int) -> Optional[Dict]:
        for entry in self.car_entry_data:
            if entry.get('sequential_number') == sequential_number:
                return entry
        return None

    async def get_all(self) -> List[Dict]:
        return self.car_entry_data

    async def get_hourly_price_for_parking(self, parking_id: int) -> Optional[float]:
        parking = await self.parking_repository.get_by_id(parking_id)
        return parking.get('hourly_rate') if parking else None

    async def get_cars_by_type_in_date_range(
        self, car_type: str, start_date: str, end_date: str
    ) -> List[Dict]:
        start_dt = datetime.fromisoformat(start_date.replace('Z', '+00:00'))
        end_dt = datetime.fromisoformat(end_date.replace('Z', '+00:00'))

        result = []
        for entry in self.car_entry_data:
            if entry.get('exit_date_time') is None:
                continue
            
            entry_time = datetime.fromisoformat(
                entry.get('entry_date_time', '').replace('Z', '+00:00')
            )
            
            if start_dt <= entry_time <= end_dt:
                session = CarParkingSession(entry)
                hourly_rate = await self.get_hourly_price_for_parking(entry.get('parking_id'))
                
                result.append({
                    'sequential_number': entry.get('sequential_number'),
                    'car_id': entry.get('car_id'),
                    'car_type': car_type,
                    'parking_id': entry.get('parking_id'),
                    'entry_date_time': entry.get('entry_date_time'),
                    'exit_date_time': entry.get('exit_date_time'),
                    'stay_duration_minutes': session.stay_duration_minutes,
                    'stay_duration_hours': session.stay_duration_hours,
                    'hourly_rate': hourly_rate,
                    'amount_paid': session.total_amount_due
                })
        
        return result

    async def get_cars_by_province_in_date_range(
        self, province_name: str, start_date: str, end_date: str
    ) -> List[Dict]:
        start_dt = datetime.fromisoformat(start_date.replace('Z', '+00:00'))
        end_dt = datetime.fromisoformat(end_date.replace('Z', '+00:00'))

        # Get all parkings in province
        parkings = await self.parking_repository.get_by_province_name(province_name)
        parking_ids = [p.get('id') for p in parkings]

        result = []
        for entry in self.car_entry_data:
            if entry.get('exit_date_time') is None:
                continue
            
            if entry.get('parking_id') not in parking_ids:
                continue
            
            entry_time = datetime.fromisoformat(
                entry.get('entry_date_time', '').replace('Z', '+00:00')
            )
            
            if start_dt <= entry_time <= end_dt:
                session = CarParkingSession(entry)
                parking = await self.parking_repository.get_by_id(entry.get('parking_id'))
                
                result.append({
                    'sequential_number': entry.get('sequential_number'),
                    'car_id': entry.get('car_id'),
                    'parking_id': entry.get('parking_id'),
                    'parking_name': parking.get('name') if parking else None,
                    'province_name': parking.get('province_name') if parking else None,
                    'entry_date_time': entry.get('entry_date_time'),
                    'exit_date_time': entry.get('exit_date_time'),
                    'stay_duration_minutes': session.stay_duration_minutes,
                    'stay_duration_hours': session.stay_duration_hours,
                    'hourly_rate': parking.get('hourly_rate') if parking else None,
                    'amount_due': session.total_amount_due
                })
        
        return result


# ================================================================
# DATABASE IMPLEMENTATIONS
# ================================================================

class DatabaseCarRepository(ICarRepository):
    """Database-based Car Repository"""

    def __init__(self, db_connection):
        self.db = db_connection

    async def get_by_id(self, id: int) -> Optional[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Cars WHERE ID = %s', (id,))
        result = cursor.fetchone()
        cursor.close()
        return result

    async def get_all(self) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Cars')
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_color(self, color: str) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Cars WHERE color LIKE %s', (f'%{color}%',))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_year_range(self, min_year: int, max_year: int) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Cars WHERE year >= %s AND year <= %s',
                      (min_year, max_year))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_make(self, make: str) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Cars WHERE make LIKE %s', (f'%{make}%',))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_type(self, car_type: str) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Cars WHERE type = %s', (car_type,))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_color_and_year_range_and_make_and_type(
        self, color: Optional[str], min_year: int, max_year: int,
        make: Optional[str], car_type: Optional[str]
    ) -> List[Dict]:
        query = 'SELECT * FROM PRQ_Cars WHERE 1=1'
        params = []

        if color:
            query += ' AND color LIKE %s'
            params.append(f'%{color}%')
        
        query += ' AND year >= %s AND year <= %s'
        params.extend([min_year, max_year])

        if make:
            query += ' AND make LIKE %s'
            params.append(f'%{make}%')
        
        if car_type:
            query += ' AND type = %s'
            params.append(car_type)

        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute(query, params)
        result = cursor.fetchall()
        cursor.close()
        return result


class DatabaseParkingRepository(IParkingRepository):
    """Database-based Parking Repository"""

    def __init__(self, db_connection):
        self.db = db_connection

    async def get_by_id(self, id: int) -> Optional[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Parking WHERE ID = %s', (id,))
        result = cursor.fetchone()
        cursor.close()
        return result

    async def get_all(self) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Parking')
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_province_name(self, province_name: str) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Parking WHERE province_name LIKE %s',
                      (f'%{province_name}%',))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_name(self, name: str) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Parking WHERE name LIKE %s', (f'%{name}%',))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_hourly_rate_range(self, min_rate: float, max_rate: float) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Parking WHERE hourly_rate >= %s AND hourly_rate <= %s',
                      (min_rate, max_rate))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_by_province_and_name_and_hourly_rate_range(
        self, province_name: Optional[str], name: Optional[str],
        min_rate: float, max_rate: float
    ) -> List[Dict]:
        query = 'SELECT * FROM PRQ_Parking WHERE 1=1'
        params = []

        if province_name:
            query += ' AND province_name LIKE %s'
            params.append(f'%{province_name}%')
        
        if name:
            query += ' AND name LIKE %s'
            params.append(f'%{name}%')
        
        query += ' AND hourly_rate >= %s AND hourly_rate <= %s'
        params.extend([min_rate, max_rate])

        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute(query, params)
        result = cursor.fetchall()
        cursor.close()
        return result


class DatabaseCarEntryRepository(ICarEntryRepository):
    """Database-based Car Entry Repository"""

    def __init__(self, db_connection):
        self.db = db_connection
        self.parking_repository = DatabaseParkingRepository(db_connection)

    async def get_by_id(self, sequential_number: int) -> Optional[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Car_Entry WHERE sequential_number = %s',
                      (sequential_number,))
        result = cursor.fetchone()
        cursor.close()
        return result

    async def get_all(self) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM PRQ_Car_Entry')
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_hourly_price_for_parking(self, parking_id: int) -> Optional[float]:
        parking = await self.parking_repository.get_by_id(parking_id)
        return parking.get('hourly_rate') if parking else None

    async def get_cars_by_type_in_date_range(
        self, car_type: str, start_date: str, end_date: str
    ) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        query = """
            SELECT 
              ce.sequential_number,
              ce.car_id,
              c.type as car_type,
              ce.parking_id,
              ce.entry_date_time,
              ce.exit_date_time,
              TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) as stay_duration_minutes,
              ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60, 2) as stay_duration_hours,
              p.hourly_rate,
              ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60 * p.hourly_rate, 2) as amount_paid
            FROM PRQ_Car_Entry ce
            JOIN PRQ_Cars c ON ce.car_id = c.ID
            JOIN PRQ_Parking p ON ce.parking_id = p.ID
            WHERE c.type = %s 
              AND ce.entry_date_time >= %s 
              AND ce.entry_date_time <= %s
              AND ce.exit_date_time IS NOT NULL
            ORDER BY ce.entry_date_time DESC
        """
        cursor.execute(query, (car_type, start_date, end_date))
        result = cursor.fetchall()
        cursor.close()
        return result

    async def get_cars_by_province_in_date_range(
        self, province_name: str, start_date: str, end_date: str
    ) -> List[Dict]:
        cursor = self.db.connection.cursor(dictionary=True)
        query = """
            SELECT 
              ce.sequential_number,
              ce.car_id,
              ce.parking_id,
              p.name as parking_name,
              p.province_name,
              ce.entry_date_time,
              ce.exit_date_time,
              TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) as stay_duration_minutes,
              ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60, 2) as stay_duration_hours,
              p.hourly_rate,
              ROUND(TIMESTAMPDIFF(MINUTE, ce.entry_date_time, ce.exit_date_time) / 60 * p.hourly_rate, 2) as amount_due
            FROM PRQ_Car_Entry ce
            JOIN PRQ_Parking p ON ce.parking_id = p.ID
            WHERE p.province_name LIKE %s 
              AND ce.entry_date_time >= %s 
              AND ce.entry_date_time <= %s
              AND ce.exit_date_time IS NOT NULL
            ORDER BY ce.entry_date_time DESC
        """
        cursor.execute(query, (f'%{province_name}%', start_date, end_date))
        result = cursor.fetchall()
        cursor.close()
        return result
