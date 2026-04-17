# Running the Database Scaffold

This guide explains how to run the scaffold against your Azure MySQL cloud instance using the connection already established in VS Code.

## File: `scaffold-prq-tables.sql`

This SQL script connects to your cloud database and generates a comprehensive scaffold of all PRQ tables, including:
- ✅ Table structures (columns, types, constraints)
- ✅ Record counts and sample data
- ✅ Foreign key relationships
- ✅ Index information
- ✅ Parking facility analysis
- ✅ Currently parked vehicles report
- ✅ CREATE TABLE statements for documentation

## How to Run the Scaffold

### Option 1: Using VS Code SQL Query Editor (RECOMMENDED)

1. **Open the SQL file:**
   - Open `scaffold-prq-tables.sql` in VS Code

2. **Execute the script:**
   - Right-click in the editor → "Execute Query" or press `Ctrl+Shift+E`
   - Or select all text (`Ctrl+A`) and run the query

3. **View Results:**
   - Results will appear in the "Query Results" panel
   - Scroll through each section to see:
     - PRQ_Parking table structure
     - PRQ_Car_Entry table structure
     - PRQ_Cars table structure
     - Relationships and indexes
     - Data analysis and currently parked vehicles

### Option 2: Using Database Explorer Extension

1. **In VS Code left sidebar:**
   - Click the Database Explorer icon
   - Right-click on your cloud connection
   - Select "New Query"
   - Paste the contents of `scaffold-prq-tables.sql`
   - Execute the query

### Option 3: Using MySQL CLI (if installed)

```bash
mysql -h hecferme-mysql-2026.mysql.database.azure.com -u user01 -p car_park < scaffold-prq-tables.sql
```

When prompted, enter password: `MyVeryStr0ngPassword`

---

## Understanding the Scaffold Output

### Section 1: Tables Found
Lists all PRQ tables with:
- Table name
- Row count
- Creation date
- Storage information

### Section 2-4: Table Structures
For each table (PRQ_Parking, PRQ_Car_Entry, PRQ_Cars):
- Column names and data types
- Nullable/Required status
- Primary/Foreign keys
- Auto-increment fields
- Sample data

### Section 5: Foreign Key Relationships
Shows all relationships between tables:
- Which columns reference which tables
- Referential integrity constraints

### Section 6: Indexes
Lists all indexes for performance optimization:
- Index names
- Columns indexed
- Uniqueness

### Section 7: Analysis Queries
- **Parking Facility Analysis:** How many cars and entries per parking location
- **Currently Parked Vehicles:** Vehicles still in the parking (NULL exit time)

### Section 8: CREATE TABLE Statements
Exact DDL statements for recreating the tables, useful for:
- Documentation
- Database migration
- Backup/recovery procedures

---

## What to Expect

**Sample Output Structure:**

```
====================================================
Current Database: car_park
Connected User: user01@...

=== TABLES FOUND ===
PRQ_Parking     (2 records)
PRQ_Car_Entry   (15 records)
PRQ_Cars        (5 records)

=== PRQ_Parking TABLE STRUCTURE ===
ID              INT         NOT NULL  PRI   auto_increment
province_name   VARCHAR(100) NOT NULL
name            VARCHAR(150) NOT NULL
hourly_rate     DECIMAL(10,2) NOT NULL

Total Records: 2

=== CURRENTLY PARKED VEHICLES ===
Car ID  Vehicle                           Parking              Hours Parked
3       White Yamaha MT-07 (2022)        Parking Centro       6
5       Blue Land Rover Discovery (2018) Parking Centro       5
...
```

---

## Troubleshooting

### Connection Error
- Verify your .env file has correct credentials
- Check internet connectivity to Azure
- Ensure firewall allows MySQL connections

### Tables Not Found
- Make sure you've run `design-db.sql` first to create the schema
- Verify you're connected to the correct database (`car_park`)

### No Data Displayed
- Run `insert-records.sql` to populate sample data
- Execute `SELECT COUNT(*) FROM PRQ_Cars;` to verify data exists

---

## Next Steps

After running the scaffold, you can:

1. **Generate Reports:** Use the query results for documentation
2. **Verify Data:** Confirm all tables and relationships are correct
3. **Performance Analysis:** Check indexes and query execution plans
4. **Export Results:** Copy results to documentation or spreadsheets
5. **Run Additional Queries:** Use the analysis as basis for custom queries

---

## Sample Queries From the Scaffold

### Get All Cars in Madrid Parking
```sql
SELECT c.*, p.name 
FROM PRQ_Car_Entry ce
JOIN PRQ_Cars c ON ce.car_id = c.ID
JOIN PRQ_Parking p ON ce.parking_id = p.ID
WHERE p.ID = 1 AND ce.exit_date_time IS NULL;
```

### Calculate Parking Revenue
```sql
SELECT 
  p.name,
  p.hourly_rate,
  COUNT(ce.sequential_number) as 'Total Entries',
  ROUND(SUM(HOUR(TIMEDIFF(ce.exit_date_time, ce.entry_date_time)) * p.hourly_rate), 2) as 'Revenue'
FROM PRQ_Parking p
JOIN PRQ_Car_Entry ce ON p.ID = ce.parking_id
WHERE ce.exit_date_time IS NOT NULL
GROUP BY p.ID, p.name, p.hourly_rate;
```

---

**Status:** Ready to execute
**File:** `scaffold-prq-tables.sql`
**Connection:** `hecferme-mysql-2026.mysql.database.azure.com`
