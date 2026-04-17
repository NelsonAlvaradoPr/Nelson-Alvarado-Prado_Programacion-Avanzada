"""
CRUD Operations Examples - Python
Demonstrates Insert, Update, Delete operations on all tables
"""

import asyncio
from crud_service import JsonCrudService, DatabaseCrudService
from repositories import JsonCarRepository, JsonParkingRepository, JsonCarEntryRepository
from db_connection_python import DatabaseConnection


def demonstrate_json_crud():
    """Demonstrate JSON CRUD operations"""
    
    print('\n' + '=' * 80)
    print('JSON CRUD OPERATIONS - CAR PARK DATABASE')
    print('=' * 80 + '\n')

    crud = JsonCrudService('.')

    # ================================================================
    # CAR CRUD OPERATIONS
    # ================================================================
    print('--- CAR OPERATIONS ---\n')

    # INSERT
    print('1. INSERT - Adding new car')
    insert_car_result = crud.insert_car({
        'color': 'Green',
        'year': 2023,
        'make': 'Audi A4',
        'type': 'sedan'
    })
    print(f"   Result: {'✓ Success' if insert_car_result['success'] else '✗ Failed'}")
    if insert_car_result['success']:
        print(f"   New Car ID: {insert_car_result['data']['id']}")
        print(f"   Data: {insert_car_result['data']}\n")

    # UPDATE
    print('2. UPDATE - Modifying car #1')
    update_car_result = crud.update_car(1, {
        'color': 'Dark Red',
        'year': 2020,
        'make': 'Toyota Corolla',
        'type': 'sedan'
    })
    print(f"   Result: {'✓ Success' if update_car_result['success'] else '✗ Failed'}")
    if update_car_result['success']:
        print(f"   Updated Data: {update_car_result['data']}\n")

    # DELETE
    print('3. DELETE - Removing car #6 (if it exists)')
    delete_car_result = crud.delete_car(6)
    print(f"   Result: {'✓ Success' if delete_car_result['success'] else '✗ Failed'}")
    if delete_car_result['success']:
        print(f"   Deleted: {delete_car_result['data']}\n")
    else:
        print(f"   Info: {delete_car_result['error']}\n")

    # ================================================================
    # PARKING CRUD OPERATIONS
    # ================================================================
    print('--- PARKING OPERATIONS ---\n')

    # INSERT
    print('1. INSERT - Adding new parking facility')
    insert_parking_result = crud.insert_parking({
        'province_name': 'Valencia',
        'name': 'Parking Centro Histórico',
        'hourly_rate': 2.25
    })
    print(f"   Result: {'✓ Success' if insert_parking_result['success'] else '✗ Failed'}")
    if insert_parking_result['success']:
        print(f"   New Parking ID: {insert_parking_result['data']['id']}")
        print(f"   Data: {insert_parking_result['data']}\n")

    # UPDATE
    print('2. UPDATE - Modifying parking #1 rate')
    update_parking_result = crud.update_parking(1, {
        'province_name': 'Madrid',
        'name': 'Parking Centro Plaza Mayor - Premium',
        'hourly_rate': 4.00
    })
    print(f"   Result: {'✓ Success' if update_parking_result['success'] else '✗ Failed'}")
    if update_parking_result['success']:
        print(f"   Updated Data: {update_parking_result['data']}\n")

    # DELETE
    print('3. DELETE - Attempting to delete parking #3 (if it exists)')
    delete_parking_result = crud.delete_parking(3)
    print(f"   Result: {'✓ Success' if delete_parking_result['success'] else '✗ Failed'}")
    if delete_parking_result['success']:
        print(f"   Deleted: {delete_parking_result['data']}\n")
    else:
        print(f"   Info: {delete_parking_result['error']}\n")

    # ================================================================
    # CAR ENTRY CRUD OPERATIONS
    # ================================================================
    print('--- CAR ENTRY OPERATIONS ---\n')

    # INSERT
    print('1. INSERT - Adding new car entry')
    insert_entry_result = crud.insert_car_entry({
        'parking_id': 1,
        'car_id': 3,
        'entry_date_time': '2026-04-17T14:00:00',
        'exit_date_time': '2026-04-17T17:30:00'
    })
    print(f"   Result: {'✓ Success' if insert_entry_result['success'] else '✗ Failed'}")
    if insert_entry_result['success']:
        print(f"   New Entry #: {insert_entry_result['data']['sequential_number']}")
        print(f"   Data: {insert_entry_result['data']}\n")

    # UPDATE
    print('2. UPDATE - Recording exit time for entry #3')
    update_entry_result = crud.update_car_entry(3, {
        'parking_id': 1,
        'car_id': 3,
        'entry_date_time': '2026-04-16 10:00:00',
        'exit_date_time': '2026-04-16 15:30:00'
    })
    print(f"   Result: {'✓ Success' if update_entry_result['success'] else '✗ Failed'}")
    if update_entry_result['success']:
        print(f"   Updated Data: {update_entry_result['data']}\n")

    # DELETE
    print('3. DELETE - Removing entry #16 (if it exists)')
    delete_entry_result = crud.delete_car_entry(16)
    print(f"   Result: {'✓ Success' if delete_entry_result['success'] else '✗ Failed'}")
    if delete_entry_result['success']:
        print(f"   Deleted: {delete_entry_result['data']}\n")
    else:
        print(f"   Info: {delete_entry_result['error']}\n")

    # ================================================================
    # VERIFICATION
    # ================================================================
    print('--- VERIFICATION ---\n')
    print('Current state of tables (post-CRUD operations):')

    car_repo = JsonCarRepository('./prq_cars.json')
    all_cars = car_repo.data
    print(f"  • Total Cars: {len(all_cars)}")

    parking_repo = JsonParkingRepository('./prq_parking.json')
    all_parkings = parking_repo.data
    print(f"  • Total Parkings: {len(all_parkings)}")

    entry_repo = JsonCarEntryRepository('./prq_car_entry.json', './prq_parking.json')
    all_entries = entry_repo.car_entry_data
    print(f"  • Total Entries: {len(all_entries)}\n")


async def demonstrate_database_crud():
    """Demonstrate Database CRUD operations"""
    
    print('\n' + '=' * 80)
    print('DATABASE CRUD OPERATIONS - CAR PARK DATABASE')
    print('=' * 80 + '\n')

    db = DatabaseConnection()

    if not db.connect():
        print('✗ Failed to connect to database')
        return

    print('✓ Database connection established\n')

    try:
        crud = DatabaseCrudService(db)

        # ================================================================
        # CAR CRUD OPERATIONS
        # ================================================================
        print('--- CAR OPERATIONS ---\n')

        # INSERT
        print('1. INSERT - Adding new car')
        insert_car_result = await crud.insert_car({
            'color': 'Purple',
            'year': 2023,
            'make': 'Volkswagen Golf',
            'type': 'sedan'
        })
        print(f"   Result: {'✓ Success' if insert_car_result['success'] else '✗ Failed'}")
        if insert_car_result['success']:
            print(f"   ID: {insert_car_result['data']['id']}\n")

        # UPDATE
        print('2. UPDATE - Modifying car #1')
        update_car_result = await crud.update_car(1, {
            'color': 'Burgundy',
            'year': 2021,
            'make': 'Toyota Corolla SE',
            'type': 'sedan'
        })
        print(f"   Result: {'✓ Success' if update_car_result['success'] else '✗ Failed'}\n")

        # ================================================================
        # PARKING CRUD OPERATIONS
        # ================================================================
        print('--- PARKING OPERATIONS ---\n')

        # INSERT
        print('1. INSERT - Adding new parking')
        insert_parking_result = await crud.insert_parking({
            'province_name': 'Seville',
            'name': 'Parking Torre del Oro',
            'hourly_rate': 2.50
        })
        print(f"   Result: {'✓ Success' if insert_parking_result['success'] else '✗ Failed'}")
        if insert_parking_result['success']:
            print(f"   ID: {insert_parking_result['data']['id']}\n")

        # UPDATE
        print('2. UPDATE - Modifying parking #1')
        update_parking_result = await crud.update_parking(1, {
            'province_name': 'Madrid',
            'name': 'Parking Centro Renovado',
            'hourly_rate': 3.75
        })
        print(f"   Result: {'✓ Success' if update_parking_result['success'] else '✗ Failed'}\n")

        # ================================================================
        # CAR ENTRY CRUD OPERATIONS
        # ================================================================
        print('--- CAR ENTRY OPERATIONS ---\n')

        # INSERT
        print('1. INSERT - Adding new entry')
        insert_entry_result = await crud.insert_car_entry({
            'parking_id': 1,
            'car_id': 2,
            'entry_date_time': '2026-04-17 15:00:00',
            'exit_date_time': '2026-04-17 18:00:00'
        })
        print(f"   Result: {'✓ Success' if insert_entry_result['success'] else '✗ Failed'}")
        if insert_entry_result['success']:
            print(f"   Sequential #: {insert_entry_result['data']['sequential_number']}\n")

        # UPDATE
        print('2. UPDATE - Recording exit time for entry #1')
        update_entry_result = await crud.update_car_entry(1, {
            'parking_id': 1,
            'car_id': 1,
            'entry_date_time': '2026-04-16 08:00:00',
            'exit_date_time': '2026-04-16 13:00:00'
        })
        print(f"   Result: {'✓ Success' if update_entry_result['success'] else '✗ Failed'}\n")

    except Exception as error:
        print(f'Error: {error}')
    finally:
        db.disconnect()


async def main():
    """Run all examples"""
    
    try:
        # Run JSON examples
        demonstrate_json_crud()

        # Run database examples (optional - uncomment if database available)
        # await demonstrate_database_crud()

    except Exception as error:
        print(f'Error: {error}')


if __name__ == '__main__':
    asyncio.run(main())
