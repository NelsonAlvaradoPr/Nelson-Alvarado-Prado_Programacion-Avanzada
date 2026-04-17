-- Car Park Database Design
-- Created: 2026-04-17
-- Database schema with PRQ prefix for all tables

-- ================================================================
-- PRQ_Cars Table
-- Stores information about vehicles in the car park
-- ================================================================
CREATE TABLE PRQ_Cars (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    color VARCHAR(50) NOT NULL,
    year INT NOT NULL,
    make VARCHAR(100) NOT NULL,
    type ENUM('sedan', '4x4', 'motorcycle') NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ================================================================
-- PRQ_Parking Table
-- Stores information about parking facilities
-- ================================================================
CREATE TABLE PRQ_Parking (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    province_name VARCHAR(100) NOT NULL,
    name VARCHAR(150) NOT NULL,
    hourly_rate DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ================================================================
-- PRQ_Car_Entry Table
-- Logs car entries and exits from parking facilities
-- ================================================================
CREATE TABLE PRQ_Car_Entry (
    sequential_number INT AUTO_INCREMENT PRIMARY KEY,
    parking_id INT NOT NULL,
    car_id INT NOT NULL,
    entry_date_time DATETIME NOT NULL,
    exit_date_time DATETIME,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (parking_id) REFERENCES PRQ_Parking(ID) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (car_id) REFERENCES PRQ_Cars(ID) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_parking_id (parking_id),
    INDEX idx_car_id (car_id),
    INDEX idx_entry_date (entry_date_time),
    INDEX idx_exit_date (exit_date_time)
);


