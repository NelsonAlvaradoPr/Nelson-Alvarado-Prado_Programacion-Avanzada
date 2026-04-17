"""
Practical Integration Example - Python
Shows how to integrate CarParkingSession with database queries
"""

import json
from datetime import datetime
from car_parking_session import CarParkingSession
from db_connection_python import DatabaseConnection


class ParkingReportGenerator:
    """Generate parking reports with calculated session fields"""
    
    def __init__(self):
        self.db = DatabaseConnection()
        self.sessions = []
    
    def load_data(self):
        """Connect to database and load all car entry data"""
        if not self.db.connect():
            print("Failed to connect to database")
            return False
        
        # Get all entries with parking rate information
        query = """
            SELECT 
                ce.sequential_number,
                ce.parking_id,
                ce.car_id,
                ce.entry_date_time,
                ce.exit_date_time,
                p.hourly_rate,
                p.name as parking_name,
                c.make,
                c.year,
                c.color,
                c.type
            FROM PRQ_Car_Entry ce
            JOIN PRQ_Parking p ON ce.parking_id = p.ID
            JOIN PRQ_Cars c ON ce.car_id = c.ID
            ORDER BY ce.entry_date_time DESC
        """
        
        cursor = self.db.connection.cursor(dictionary=True)
        cursor.execute(query)
        results = cursor.fetchall()
        cursor.close()
        
        # Create session objects
        self.sessions = [CarParkingSession(row) for row in results]
        print(f"✓ Loaded {len(self.sessions)} parking sessions\n")
        return True
    
    def print_summary(self):
        """Print executive summary"""
        print("=" * 80)
        print("PARKING FACILITY REPORT SUMMARY")
        print("=" * 80)
        
        total_sessions = len(self.sessions)
        completed = len([s for s in self.sessions if s.has_exited])
        active = len([s for s in self.sessions if not s.has_exited])
        
        print(f"\nTotal Sessions: {total_sessions}")
        print(f"  • Completed: {completed}")
        print(f"  • Active (Still Parked): {active}")
        
        # Calculate revenue
        total_revenue = sum(s.total_amount_due for s in self.sessions if s.total_amount_due)
        avg_duration = sum(s.stay_duration_hours for s in self.sessions if s.stay_duration_hours) / completed if completed > 0 else 0
        
        print(f"\nRevenue:")
        print(f"  • Total Revenue: €{total_revenue:.2f}")
        print(f"  • Average Duration: {avg_duration:.2f} hours")
        print(f"  • Avg Revenue per Session: €{(total_revenue / completed):.2f}" if completed > 0 else "  • Avg Revenue per Session: N/A")
        print()
    
    def print_completed_sessions(self):
        """Print all completed sessions with details"""
        completed = [s for s in self.sessions if s.has_exited]
        
        print("=" * 80)
        print("COMPLETED PARKING SESSIONS")
        print("=" * 80)
        print(f"\nTotal Completed: {len(completed)}\n")
        
        # Table header
        print(f"{'#':<4} {'Car':<25} {'Duration':<12} {'Amount Due':<12} {'Rate':<8}")
        print("-" * 80)
        
        for session in completed:
            car_info = f"{session.to_dict().get('make', 'Unknown')[:20]}"
            duration = f"{session.stay_duration_hours}h" if session.stay_duration_hours else "N/A"
            amount = f"€{session.total_amount_due:.2f}" if session.total_amount_due else "N/A"
            rate = f"€{session.hourly_rate:.2f}/h"
            
            print(f"{session.sequential_number:<4} {car_info:<25} {duration:<12} {amount:<12} {rate:<8}")
        
        print()
    
    def print_active_sessions(self):
        """Print currently active sessions"""
        active = [s for s in self.sessions if not s.has_exited]
        
        if not active:
            print("No active sessions\n")
            return
        
        print("=" * 80)
        print("ACTIVE PARKING SESSIONS (Currently Parked)")
        print("=" * 80)
        print(f"\nTotal Active: {len(active)}\n")
        
        # Table header
        print(f"{'#':<4} {'Car':<25} {'Parked Since':<20} {'Rate':<8}")
        print("-" * 80)
        
        for session in active:
            car_info = f"{session.to_dict().get('make', 'Unknown')[:20]}"
            entry_time = session.entry_date_time.strftime('%Y-%m-%d %H:%M:%S')
            rate = f"€{session.hourly_rate:.2f}/h"
            
            print(f"{session.sequential_number:<4} {car_info:<25} {entry_time:<20} {rate:<8}")
        
        print()
    
    def print_high_value_sessions(self, min_amount=20):
        """Print sessions with high parking fees"""
        high_value = [s for s in self.sessions 
                     if s.total_amount_due and s.total_amount_due >= min_amount]
        
        print("=" * 80)
        print(f"HIGH VALUE SESSIONS (€{min_amount}+)")
        print("=" * 80)
        print(f"\nSessions: {len(high_value)}\n")
        
        # Sort by amount descending
        high_value.sort(key=lambda s: s.total_amount_due or 0, reverse=True)
        
        print(f"{'#':<4} {'Car':<25} {'Duration':<12} {'Amount':<12}")
        print("-" * 80)
        
        for session in high_value:
            car_info = f"{session.to_dict().get('make', 'Unknown')[:20]}"
            duration = f"{session.stay_duration_hours}h"
            amount = f"€{session.total_amount_due:.2f}"
            
            print(f"{session.sequential_number:<4} {car_info:<25} {duration:<12} {amount:<12}")
        
        print()
    
    def print_long_stay_sessions(self, min_hours=5):
        """Print sessions with long parking duration"""
        long_stays = [s for s in self.sessions 
                     if s.stay_duration_hours and s.stay_duration_hours >= min_hours]
        
        print("=" * 80)
        print(f"LONG STAY SESSIONS ({min_hours}+ hours)")
        print("=" * 80)
        print(f"\nSessions: {len(long_stays)}\n")
        
        # Sort by duration descending
        long_stays.sort(key=lambda s: s.stay_duration_hours or 0, reverse=True)
        
        print(f"{'#':<4} {'Car':<25} {'Duration':<12} {'Amount Due':<12}")
        print("-" * 80)
        
        for session in long_stays:
            car_info = f"{session.to_dict().get('make', 'Unknown')[:20]}"
            duration = f"{session.stay_duration_hours}h"
            amount = f"€{session.total_amount_due:.2f}"
            
            print(f"{session.sequential_number:<4} {car_info:<25} {duration:<12} {amount:<12}")
        
        print()
    
    def export_to_json(self, filename='parking_report.json'):
        """Export all sessions to JSON file"""
        data = [session.to_dict() for session in self.sessions]
        
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, default=str)
        
        print(f"✓ Report exported to {filename}\n")
    
    def export_revenue_report(self, filename='revenue_report.json'):
        """Export revenue analysis to JSON"""
        completed = [s for s in self.sessions if s.has_exited]
        
        report = {
            'generated_at': datetime.now().isoformat(),
            'summary': {
                'total_sessions': len(self.sessions),
                'completed': len(completed),
                'active': len(self.sessions) - len(completed),
                'total_revenue': sum(s.total_amount_due for s in completed if s.total_amount_due),
                'average_duration_hours': sum(s.stay_duration_hours for s in completed if s.stay_duration_hours) / len(completed) if completed else 0,
                'average_revenue_per_session': sum(s.total_amount_due for s in completed if s.total_amount_due) / len(completed) if completed else 0
            },
            'sessions': [s.to_dict() for s in self.sessions]
        }
        
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(report, f, indent=2, default=str)
        
        print(f"✓ Revenue report exported to {filename}\n")
    
    def close(self):
        """Close database connection"""
        self.db.disconnect()


# Main execution
if __name__ == '__main__':
    print("\n" + "=" * 80)
    print("PARKING MANAGEMENT SYSTEM - REPORT GENERATOR")
    print("=" * 80 + "\n")
    
    generator = ParkingReportGenerator()
    
    if not generator.load_data():
        exit(1)
    
    # Generate reports
    generator.print_summary()
    generator.print_completed_sessions()
    generator.print_active_sessions()
    generator.print_high_value_sessions(min_amount=15)
    generator.print_long_stay_sessions(min_hours=4)
    
    # Export data
    generator.export_to_json()
    generator.export_revenue_report()
    
    # Close connection
    generator.close()
    
    print("=" * 80)
    print("✓ Report generation complete!")
    print("=" * 80 + "\n")
