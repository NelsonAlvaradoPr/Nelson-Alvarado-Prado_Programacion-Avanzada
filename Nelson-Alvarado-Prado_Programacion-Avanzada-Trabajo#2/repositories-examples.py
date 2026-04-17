"""
Repository Usage Examples - Python
Demonstrates how to use the repository pattern with JSON and Database sources
"""

import asyncio
import os
from repositories import (
    JsonCarRepository,
    JsonParkingRepository,
    JsonCarEntryRepository,
    DatabaseCarRepository,
    DatabaseParkingRepository,
    DatabaseCarEntryRepository
)
from db_connection_python import DatabaseConnection


# ================================================================
# JSON EXAMPLES
# ================================================================

async def demonstrate_json_repositories():
    """Demonstrate JSON-based repositories"""
    
    print('\n' + '=' * 80)
    print('JSON REPOSITORY EXAMPLES')
    print('=' * 80 + '\n')

    # Initialize JSON repositories
    car_repo = JsonCarRepository('prq_cars.json')
    parking_repo = JsonParkingRepository('prq_parking.json')
    car_entry_repo = JsonCarEntryRepository('prq_car_entry.json', 'prq_parking.json')

    # ================================================================
    # CAR QUERIES
    # ================================================================
    print('--- CAR QUERIES ---\n')

    # Get all cars
    all_cars = await car_repo.get_all()
    print(f'1. All cars ({len(all_cars)} total):')
    for car in all_cars:
        print(f"   • {car['color']} {car['make']} ({car['year']}) - {car['type']}")

    # Get cars by color
    red_cars = await car_repo.get_by_color('Red')
    print(f'\n2. Red cars ({len(red_cars)} found):')
    for car in red_cars:
        print(f"   • {car['make']} ({car['year']})")

    # Get cars by year range
    recent_cars = await car_repo.get_by_year_range(2020, 2022)
    print(f'\n3. Cars from 2020-2022 ({len(recent_cars)} found):')
    for car in recent_cars:
        print(f"   • {car['make']} ({car['year']})")

    # Get cars by type
    sedans = await car_repo.get_by_type('sedan')
    print(f'\n4. Sedans ({len(sedans)} found):')
    for car in sedans:
        print(f"   • {car['color']} {car['make']}")

    # Combined query
    filtered = await car_repo.get_by_color_and_year_range_and_make_and_type(
        None,      # No color filter
        2018, 2022,  # Year range
        None,      # No make filter
        '4x4'      # Type filter
    )
    print(f'\n5. 4x4 vehicles from 2018-2022 ({len(filtered)} found):')
    for car in filtered:
        print(f"   • {car['color']} {car['make']} ({car['year']})")

    # ================================================================
    # PARKING QUERIES
    # ================================================================
    print('\n--- PARKING QUERIES ---\n')

    # Get all parkings
    all_parkings = await parking_repo.get_all()
    print(f'1. All parking facilities ({len(all_parkings)} total):')
    for p in all_parkings:
        print(f"   • {p['name']} ({p['province_name']}) - €{p['hourly_rate']}/hour")

    # Get parkings by province
    madrid_parkings = await parking_repo.get_by_province_name('Madrid')
    print(f'\n2. Madrid parkings ({len(madrid_parkings)} found):')
    for p in madrid_parkings:
        print(f"   • {p['name']} - €{p['hourly_rate']}/hour")

    # Get parkings by rate range
    cheap_parkings = await parking_repo.get_by_hourly_rate_range(2.5, 3.0)
    print(f'\n3. Parkings €2.50-€3.00/hour ({len(cheap_parkings)} found):')
    for p in cheap_parkings:
        print(f"   • {p['name']} - €{p['hourly_rate']}/hour")

    # Combined query
    filtered_parkings = await parking_repo.get_by_province_and_name_and_hourly_rate_range(
        'Barcelona',  # Province
        None,         # No name filter
        2.0, 3.5      # Rate range
    )
    print(f'\n4. Barcelona parkings €2.00-€3.50/hour ({len(filtered_parkings)} found):')
    for p in filtered_parkings:
        print(f"   • {p['name']} - €{p['hourly_rate']}/hour")

    # ================================================================
    # CAR ENTRY QUERIES
    # ================================================================
    print('\n--- CAR ENTRY QUERIES ---\n')

    # Get hourly price for parking
    madrid_rate = await car_entry_repo.get_hourly_price_for_parking(1)
    print(f'1. Hourly rate for Parking ID 1: €{madrid_rate}')

    # Get cars by type in date range
    print(f'\n2. Sedan entries from 2026-04-16 to 2026-04-17:')
    sedan_entries = await car_entry_repo.get_cars_by_type_in_date_range(
        'sedan',
        '2026-04-16',
        '2026-04-17'
    )
    print(f'   Found {len(sedan_entries)} sedan sessions:')
    for entry in sedan_entries:
        print(
            f"   • Entry #{entry['sequential_number']}: "
            f"{entry['entry_date_time']} → {entry['exit_date_time']} "
            f"({entry['stay_duration_hours']}h) - €{entry['amount_paid']}"
        )

    # Get cars by province in date range
    print(f'\n3. Cars in Madrid from 2026-04-16 to 2026-04-17:')
    madrid_entries = await car_entry_repo.get_cars_by_province_in_date_range(
        'Madrid',
        '2026-04-16',
        '2026-04-17'
    )
    print(f'   Found {len(madrid_entries)} sessions:')
    for entry in madrid_entries:
        print(
            f"   • Entry #{entry['sequential_number']} at {entry['parking_name']}: "
            f"{entry['entry_date_time']} → {entry['exit_date_time']} - €{entry['amount_due']}"
        )


# ================================================================
# DATABASE EXAMPLES
# ================================================================

async def demonstrate_database_repositories():
    """Demonstrate Database-based repositories"""
    
    print('\n' + '=' * 80)
    print('DATABASE REPOSITORY EXAMPLES')
    print('=' * 80 + '\n')

    # Create database connection
    db = DatabaseConnection()

    if not db.connect():
        print('✗ Failed to connect to database')
        return

    print('✓ Database connection established\n')

    try:
        # Initialize database repositories
        car_repo = DatabaseCarRepository(db)
        parking_repo = DatabaseParkingRepository(db)
        car_entry_repo = DatabaseCarEntryRepository(db)

        # ================================================================
        # CAR QUERIES
        # ================================================================
        print('--- CAR QUERIES ---\n')

        # Get all cars
        all_cars = await car_repo.get_all()
        print(f'1. All cars ({len(all_cars)} total)')

        # Get cars by make
        toyotas = await car_repo.get_by_make('Toyota')
        print(f'2. Toyota cars ({len(toyotas)} found)')

        # Get cars by year range
        recent_cars = await car_repo.get_by_year_range(2020, 2022)
        print(f'3. Cars from 2020-2022 ({len(recent_cars)} found)')

        # Combined query
        filtered = await car_repo.get_by_color_and_year_range_and_make_and_type(
            None, 2018, 2022, None, 'sedan'
        )
        print(f'4. Sedans from 2018-2022 ({len(filtered)} found)')

        # ================================================================
        # PARKING QUERIES
        # ================================================================
        print('\n--- PARKING QUERIES ---\n')

        # Get all parkings
        all_parkings = await parking_repo.get_all()
        print(f'1. All parking facilities ({len(all_parkings)} total)')

        # Get parkings by province
        madrid_parkings = await parking_repo.get_by_province_name('Madrid')
        print(f'2. Madrid parkings ({len(madrid_parkings)} found)')

        # Get parkings by rate range
        rated_parkings = await parking_repo.get_by_hourly_rate_range(2.0, 3.5)
        print(f'3. Parkings €2.00-€3.50/hour ({len(rated_parkings)} found)')

        # ================================================================
        # CAR ENTRY QUERIES
        # ================================================================
        print('\n--- CAR ENTRY QUERIES ---\n')

        # Get hourly price
        rate = await car_entry_repo.get_hourly_price_for_parking(1)
        print(f'1. Hourly rate for Parking ID 1: €{rate}')

        # Get cars by type in date range
        type_entries = await car_entry_repo.get_cars_by_type_in_date_range(
            'sedan',
            '2026-04-16',
            '2026-04-18'
        )
        print(f'2. Sedan entries in date range ({len(type_entries)} found)')
        if type_entries:
            entry = type_entries[0]
            print(f'   Sample: {entry["entry_date_time"]} → {entry["exit_date_time"]} - €{entry["amount_paid"]}')

        # Get cars by province in date range
        province_entries = await car_entry_repo.get_cars_by_province_in_date_range(
            'Madrid',
            '2026-04-16',
            '2026-04-18'
        )
        print(f'3. Madrid entries in date range ({len(province_entries)} found)')
        if province_entries:
            entry = province_entries[0]
            print(f'   Sample: {entry["parking_name"]} - €{entry["amount_due"]}')

    finally:
        db.disconnect()


# ================================================================
# COMPLEX QUERY EXAMPLES
# ================================================================

async def complex_query_examples():
    """Demonstrate complex queries and data analysis"""
    
    print('\n' + '=' * 80)
    print('COMPLEX QUERY EXAMPLES')
    print('=' * 80 + '\n')

    # Use JSON repos for demo
    car_entry_repo = JsonCarEntryRepository('prq_car_entry.json', 'prq_parking.json')

    # Example 1: Calculate revenue by vehicle type
    print('1. Revenue Analysis by Vehicle Type\n')
    
    for car_type in ['sedan', '4x4', 'motorcycle']:
        entries = await car_entry_repo.get_cars_by_type_in_date_range(
            car_type,
            '2026-04-01',
            '2026-04-30'
        )
        
        if entries:
            total_revenue = sum(e['amount_paid'] for e in entries if e['amount_paid'])
            avg_duration = sum(e['stay_duration_hours'] for e in entries if e['stay_duration_hours']) / len(entries) if entries else 0
            
            print(f'   {car_type.upper()}:')
            print(f'      Sessions: {len(entries)}')
            print(f'      Total Revenue: €{total_revenue:.2f}')
            print(f'      Avg Duration: {avg_duration:.2f} hours\n')

    # Example 2: Occupancy by province
    print('2. Occupancy Analysis by Province\n')
    
    for province in ['Madrid', 'Barcelona']:
        entries = await car_entry_repo.get_cars_by_province_in_date_range(
            province,
            '2026-04-01',
            '2026-04-30'
        )
        
        if entries:
            total_revenue = sum(e['amount_due'] for e in entries if e['amount_due'])
            print(f'   {province.upper()}:')
            print(f'      Sessions: {len(entries)}')
            print(f'      Total Revenue: €{total_revenue:.2f}\n')

    # Example 3: Find long-stay sessions
    print('3. Long-Stay Sessions (>4 hours)\n')
    
    entries = await car_entry_repo.get_cars_by_province_in_date_range(
        'Madrid',
        '2026-04-01',
        '2026-04-30'
    )
    
    long_stays = [e for e in entries if e['stay_duration_hours'] and e['stay_duration_hours'] > 4]
    print(f'   Found {len(long_stays)} long-stay sessions:')
    for entry in sorted(long_stays, key=lambda x: x['stay_duration_hours'], reverse=True)[:3]:
        print(f'      • Entry #{entry["sequential_number"]}: {entry["stay_duration_hours"]}h - €{entry["amount_due"]}')


# ================================================================
# MAIN EXECUTION
# ================================================================

async def main():
    """Run all examples"""
    
    try:
        # Run JSON examples
        await demonstrate_json_repositories()

        # Run complex queries
        await complex_query_examples()

        # Run database examples (optional - uncomment if database available)
        # await demonstrate_database_repositories()

    except Exception as error:
        print(f'Error: {error}')


if __name__ == '__main__':
    asyncio.run(main())
