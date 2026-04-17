-- ================================================================
-- Car Park Database Scaffold - PRQ Tables
-- Generates comprehensive information about all PRQ tables
-- Execute this script in VS Code's SQL Query Editor
-- ================================================================

-- Show database and user info
SELECT 'Car Park Database Scaffold' as INFO, NOW() as 'Generated At';
SELECT DATABASE() as 'Current Database', USER() as 'Connected User';

-- ================================================================
-- STEP 1: List all PRQ tables
-- ================================================================
SELECT 
  '=== TABLES FOUND ===' as section;

SELECT 
  TABLE_NAME,
  TABLE_TYPE,
  TABLE_ROWS,
  DATA_FREE as 'Data Free (bytes)',
  CREATION_TIME
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME LIKE 'PRQ%'
ORDER BY TABLE_NAME;

-- ================================================================
-- STEP 2: Detailed structure of PRQ_Parking table
-- ================================================================
SELECT '=== PRQ_Parking TABLE STRUCTURE ===' as section;

SELECT 
  COLUMN_NAME,
  COLUMN_TYPE,
  IS_NULLABLE,
  COLUMN_KEY,
  EXTRA,
  COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'PRQ_Parking'
ORDER BY ORDINAL_POSITION;

-- Record count
SELECT CONCAT('Total Records: ', COUNT(*)) as 'PRQ_Parking Data'
FROM PRQ_Parking;

-- Sample data
SELECT '--- Sample Data ---' as info;
SELECT * FROM PRQ_Parking;

-- ================================================================
-- STEP 3: Detailed structure of PRQ_Car_Entry table
-- ================================================================
SELECT '=== PRQ_Car_Entry TABLE STRUCTURE ===' as section;

SELECT 
  COLUMN_NAME,
  COLUMN_TYPE,
  IS_NULLABLE,
  COLUMN_KEY,
  EXTRA,
  COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'PRQ_Car_Entry'
ORDER BY ORDINAL_POSITION;

-- Record count
SELECT CONCAT('Total Records: ', COUNT(*)) as 'PRQ_Car_Entry Data'
FROM PRQ_Car_Entry;

-- Sample data
SELECT '--- Sample Data (First 5 records) ---' as info;
SELECT * FROM PRQ_Car_Entry LIMIT 5;

-- ================================================================
-- STEP 4: Detailed structure of PRQ_Cars table
-- ================================================================
SELECT '=== PRQ_Cars TABLE STRUCTURE ===' as section;

SELECT 
  COLUMN_NAME,
  COLUMN_TYPE,
  IS_NULLABLE,
  COLUMN_KEY,
  EXTRA,
  COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'PRQ_Cars'
ORDER BY ORDINAL_POSITION;

-- Record count
SELECT CONCAT('Total Records: ', COUNT(*)) as 'PRQ_Cars Data'
FROM PRQ_Cars;

-- Sample data
SELECT '--- Sample Data ---' as info;
SELECT * FROM PRQ_Cars;

-- ================================================================
-- STEP 5: Foreign Key Relationships
-- ================================================================
SELECT '=== FOREIGN KEY RELATIONSHIPS ===' as section;

SELECT 
  CONSTRAINT_NAME,
  TABLE_NAME,
  COLUMN_NAME,
  REFERENCED_TABLE_NAME,
  REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME LIKE 'PRQ%'
  AND REFERENCED_TABLE_NAME IS NOT NULL
ORDER BY TABLE_NAME, CONSTRAINT_NAME;

-- ================================================================
-- STEP 6: Indexes
-- ================================================================
SELECT '=== INDEXES ===' as section;

SELECT 
  TABLE_NAME,
  INDEX_NAME,
  COLUMN_NAME,
  SEQ_IN_INDEX,
  NON_UNIQUE
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME LIKE 'PRQ%'
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

-- ================================================================
-- STEP 7: Complex Queries for Analysis
-- ================================================================
SELECT '=== PARKING FACILITY ANALYSIS ===' as section;

-- Cars per parking facility
SELECT 
  p.ID,
  p.name as 'Parking Name',
  p.province_name,
  COUNT(DISTINCT ce.car_id) as 'Unique Cars',
  COUNT(ce.sequential_number) as 'Total Entries',
  SUM(CASE WHEN ce.exit_date_time IS NULL THEN 1 ELSE 0 END) as 'Currently Parked'
FROM PRQ_Parking p
LEFT JOIN PRQ_Car_Entry ce ON p.ID = ce.parking_id
GROUP BY p.ID, p.name, p.province_name;

-- Currently parked vehicles with details
SELECT '=== CURRENTLY PARKED VEHICLES ===' as section;

SELECT 
  ce.sequential_number,
  c.ID as 'Car ID',
  CONCAT(c.color, ' ', c.make, ' (', c.year, ')') as 'Vehicle',
  c.type,
  p.name as 'Parking',
  p.province_name,
  ce.entry_date_time,
  TIMESTAMPDIFF(HOUR, ce.entry_date_time, NOW()) as 'Hours Parked'
FROM PRQ_Car_Entry ce
JOIN PRQ_Cars c ON ce.car_id = c.ID
JOIN PRQ_Parking p ON ce.parking_id = p.ID
WHERE ce.exit_date_time IS NULL
ORDER BY ce.entry_date_time;

-- ================================================================
-- STEP 8: Generate CREATE TABLE Statements
-- ================================================================
SELECT '=== GENERATE CREATE TABLE STATEMENTS ===' as section;

-- This will show you the exact CREATE TABLE commands
SHOW CREATE TABLE PRQ_Parking\G
SHOW CREATE TABLE PRQ_Car_Entry\G
SHOW CREATE TABLE PRQ_Cars\G

-- ================================================================
-- SCAFFOLD COMPLETE
-- ================================================================
SELECT CONCAT('✓ Scaffold completed at ', NOW()) as 'Status';
