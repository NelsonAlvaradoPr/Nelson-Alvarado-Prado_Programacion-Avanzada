# Python - MySQL Connection Example
# Install dependencies: pip install mysql-connector-python python-dotenv

import mysql.connector
from mysql.connector import Error
import os
from dotenv import load_dotenv

# Load environment variables from .env file
load_dotenv()

class DatabaseConnection:
    def __init__(self):
        self.connection = None
        self.host = os.getenv('DB_HOST')
        self.port = os.getenv('DB_PORT')
        self.user = os.getenv('DB_USER')
        self.password = os.getenv('DB_PASSWORD')
        self.database = os.getenv('DB_NAME')
    
    def connect(self):
        """Establish connection to MySQL database"""
        try:
            self.connection = mysql.connector.connect(
                host=self.host,
                port=int(self.port),
                user=self.user,
                password=self.password,
                database=self.database,
                ssl_disabled=False,
                use_pure=True
            )
            if self.connection.is_connected():
                print("Successfully connected to MySQL database")
                return True
        except Error as e:
            print(f"Error while connecting to MySQL: {e}")
            return False
    
    def disconnect(self):
        """Close database connection"""
        if self.connection and self.connection.is_connected():
            self.connection.close()
            print("MySQL connection closed")
    
    def get_all_cars(self):
        """Retrieve all cars from PRQ_Cars table"""
        try:
            cursor = self.connection.cursor(dictionary=True)
            cursor.execute("SELECT * FROM PRQ_Cars")
            result = cursor.fetchall()
            cursor.close()
            return result
        except Error as e:
            print(f"Database error: {e}")
            return None
    
    def get_all_parking(self):
        """Retrieve all parking spaces from PRQ_Parking table"""
        try:
            cursor = self.connection.cursor(dictionary=True)
            cursor.execute("SELECT * FROM PRQ_Parking")
            result = cursor.fetchall()
            cursor.close()
            return result
        except Error as e:
            print(f"Database error: {e}")
            return None
    
    def get_all_car_entries(self):
        """Retrieve all car entry/exit records from PRQ_Car_Entry table"""
        try:
            cursor = self.connection.cursor(dictionary=True)
            cursor.execute("SELECT * FROM PRQ_Car_Entry")
            result = cursor.fetchall()
            cursor.close()
            return result
        except Error as e:
            print(f"Database error: {e}")
            return None
    
    def get_currently_parked_cars(self):
        """Get vehicles currently parked (exit_date_time is NULL)"""
        try:
            cursor = self.connection.cursor(dictionary=True)
            query = """
                SELECT 
                    c.ID, c.color, c.year, c.make, c.type,
                    p.name as parking_name,
                    ce.entry_date_time
                FROM PRQ_Car_Entry ce
                JOIN PRQ_Cars c ON ce.car_id = c.ID
                JOIN PRQ_Parking p ON ce.parking_id = p.ID
                WHERE ce.exit_date_time IS NULL
            """
            cursor.execute(query)
            result = cursor.fetchall()
            cursor.close()
            return result
        except Error as e:
            print(f"Database error: {e}")
            return None
    
    def get_cars_by_parking(self, parking_id):
        """Get all cars that have used a specific parking facility"""
        try:
            cursor = self.connection.cursor(dictionary=True)
            query = """
                SELECT DISTINCT
                    c.ID, c.color, c.year, c.make, c.type,
                    COUNT(ce.sequential_number) as visits
                FROM PRQ_Car_Entry ce
                JOIN PRQ_Cars c ON ce.car_id = c.ID
                WHERE ce.parking_id = %s
                GROUP BY c.ID
            """
            cursor.execute(query, (parking_id,))
            result = cursor.fetchall()
            cursor.close()
            return result
        except Error as e:
            print(f"Database error: {e}")
            return None


# Example usage
if __name__ == "__main__":
    db = DatabaseConnection()
    
    if db.connect():
        # Query all tables
        print("\n--- All Cars ---")
        cars = db.get_all_cars()
        for car in cars if cars else []:
            print(car)
        
        print("\n--- All Parking Spaces ---")
        parking = db.get_all_parking()
        for p in parking if parking else []:
            print(p)
        
        print("\n--- All Car Entries ---")
        entries = db.get_all_car_entries()
        for entry in entries if entries else []:
            print(entry)
        
        print("\n--- Currently Parked Cars ---")
        parked = db.get_currently_parked_cars()
        for car in parked if parked else []:
            print(car)
        
        db.disconnect()
