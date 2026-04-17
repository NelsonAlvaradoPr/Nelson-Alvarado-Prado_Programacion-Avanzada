"""
CarParkingSession Class - Python
Represents a car parking session with calculated fields:
- stay_duration_minutes: Total time parked in minutes
- stay_duration_hours: Total time parked in hours (decimal)
- total_amount_due: Parking fee based on duration and hourly rate

All calculated fields return None if exit_date_time is not set
"""

from datetime import datetime
from typing import Optional


class CarParkingSession:
    """
    Represents a car parking session with automatically calculated fields
    """

    def __init__(self, data: dict):
        """
        Initialize a CarParkingSession instance
        
        Args:
            data (dict): Dictionary containing session data with keys:
                - sequential_number: int
                - parking_id: int
                - car_id: int
                - entry_date_time: str (ISO format datetime)
                - exit_date_time: str or None (ISO format datetime)
                - hourly_rate: float
        """
        self.sequential_number = data.get('sequential_number')
        self.parking_id = data.get('parking_id')
        self.car_id = data.get('car_id')
        
        # Parse datetime strings
        if isinstance(data.get('entry_date_time'), str):
            self.entry_date_time = datetime.fromisoformat(
                data.get('entry_date_time').replace('Z', '+00:00')
            )
        else:
            self.entry_date_time = data.get('entry_date_time')
        
        exit_time = data.get('exit_date_time')
        if exit_time and isinstance(exit_time, str):
            self.exit_date_time = datetime.fromisoformat(
                exit_time.replace('Z', '+00:00')
            )
        else:
            self.exit_date_time = exit_time
        
        self.hourly_rate = data.get('hourly_rate')

    @property
    def stay_duration_minutes(self) -> Optional[int]:
        """
        Calculate stay duration in minutes
        
        Returns:
            int: Duration in minutes if vehicle has exited, None otherwise
        """
        if self.exit_date_time is None:
            return None
        
        time_diff = self.exit_date_time - self.entry_date_time
        duration_minutes = int(time_diff.total_seconds() / 60)
        return duration_minutes

    @property
    def stay_duration_hours(self) -> Optional[float]:
        """
        Calculate stay duration in hours (decimal format)
        
        Returns:
            float: Duration in hours (rounded to 2 decimals) if vehicle has exited, None otherwise
        """
        if self.exit_date_time is None:
            return None
        
        minutes = self.stay_duration_minutes
        hours = round(minutes / 60, 2)
        return hours

    @property
    def total_amount_due(self) -> Optional[float]:
        """
        Calculate total amount due
        Formula: stay_duration_hours × hourly_rate
        
        Returns:
            float: Amount due (rounded to 2 decimals) if vehicle has exited, None otherwise
        """
        if self.exit_date_time is None:
            return None
        
        hours = self.stay_duration_hours
        amount = round(hours * self.hourly_rate, 2)
        return amount

    @property
    def has_exited(self) -> bool:
        """
        Check if the vehicle has exited
        
        Returns:
            bool: True if exit_date_time is set, False otherwise
        """
        return self.exit_date_time is not None

    @property
    def status(self) -> str:
        """
        Get vehicle status
        
        Returns:
            str: 'EXITED' if vehicle has exited, 'ACTIVE' otherwise
        """
        return 'EXITED' if self.has_exited else 'ACTIVE'

    def to_dict(self) -> dict:
        """
        Return object with all session data and calculated fields
        
        Returns:
            dict: Complete session information including calculated fields
        """
        return {
            'sequential_number': self.sequential_number,
            'parking_id': self.parking_id,
            'car_id': self.car_id,
            'entry_date_time': self.entry_date_time.isoformat() if self.entry_date_time else None,
            'exit_date_time': self.exit_date_time.isoformat() if self.exit_date_time else None,
            'stay_duration_minutes': self.stay_duration_minutes,
            'stay_duration_hours': self.stay_duration_hours,
            'total_amount_due': self.total_amount_due,
            'status': self.status,
            'hourly_rate': self.hourly_rate
        }

    def to_json(self) -> dict:
        """
        Return JSON-serializable representation
        
        Returns:
            dict: Same as to_dict()
        """
        return self.to_dict()

    def __str__(self) -> str:
        """
        Return formatted string representation
        """
        exit_str = self.exit_date_time.strftime('%Y-%m-%d %H:%M:%S') if self.exit_date_time else 'Not exited'
        return (
            f"CarParkingSession #{self.sequential_number} - Car {self.car_id} at Parking {self.parking_id}\n"
            f"    Status: {self.status}\n"
            f"    Entry: {self.entry_date_time.strftime('%Y-%m-%d %H:%M:%S')}\n"
            f"    Exit: {exit_str}\n"
            f"    Duration: {self.stay_duration_minutes} minutes ({self.stay_duration_hours} hours)\n"
            f"    Amount Due: €{self.total_amount_due}"
        )

    def __repr__(self) -> str:
        """
        Return object representation
        """
        return f"CarParkingSession(seq={self.sequential_number}, car={self.car_id}, parking={self.parking_id}, status={self.status})"


# Example usage and helper function
def create_session_from_db(db_row: dict) -> CarParkingSession:
    """
    Factory function to create a CarParkingSession from database query results
    
    Args:
        db_row (dict): Database row with PRQ_Car_Entry and PRQ_Parking data
        
    Returns:
        CarParkingSession: Initialized session object
    """
    return CarParkingSession({
        'sequential_number': db_row.get('sequential_number'),
        'parking_id': db_row.get('parking_id'),
        'car_id': db_row.get('car_id'),
        'entry_date_time': db_row.get('entry_date_time'),
        'exit_date_time': db_row.get('exit_date_time'),
        'hourly_rate': db_row.get('hourly_rate')
    })


if __name__ == '__main__':
    # Test example 1: Completed session
    session1 = CarParkingSession({
        'sequential_number': 1,
        'parking_id': 1,
        'car_id': 1,
        'entry_date_time': '2026-04-16 08:00:00',
        'exit_date_time': '2026-04-16 12:30:00',
        'hourly_rate': 3.50
    })
    
    print("Test 1: Completed Session")
    print(session1)
    print(f"Session as dict: {session1.to_dict()}\n")
    
    # Test example 2: Active session (not exited)
    session2 = CarParkingSession({
        'sequential_number': 3,
        'parking_id': 1,
        'car_id': 3,
        'entry_date_time': '2026-04-16 10:00:00',
        'exit_date_time': None,
        'hourly_rate': 3.50
    })
    
    print("Test 2: Active Session (Not Exited)")
    print(session2)
    print(f"Stay Duration Minutes: {session2.stay_duration_minutes}")
    print(f"Stay Duration Hours: {session2.stay_duration_hours}")
    print(f"Total Amount Due: {session2.total_amount_due}\n")
    
    # Test example 3: Different rates
    session3 = CarParkingSession({
        'sequential_number': 12,
        'parking_id': 2,
        'car_id': 2,
        'entry_date_time': '2026-04-17 08:30:00',
        'exit_date_time': '2026-04-17 13:00:00',
        'hourly_rate': 2.75
    })
    
    print("Test 3: Barcelona Parking (Lower Rate)")
    print(session3)
    print(f"Session as dict: {session3.to_dict()}")
