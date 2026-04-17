-- Car Park Database - Insert Records
-- Created: 2026-04-17
-- Populates PRQ_Cars, PRQ_Parking, and PRQ_Car_Entry tables

-- ================================================================
-- INSERT PARKING SPACES (2 records)
-- ================================================================

INSERT INTO PRQ_Parking (province_name, name, hourly_rate) VALUES
('Madrid', 'Parking Centro Plaza Mayor', 3.50),
('Barcelona', 'Parking Diagonal Mar', 2.75);


-- ================================================================
-- INSERT CARS (5 records)
-- ================================================================

INSERT INTO PRQ_Cars (color, year, make, type) VALUES
('Red', 2020, 'Toyota Corolla', 'sedan'),
('Black', 2019, 'BMW X5', '4x4'),
('White', 2022, 'Yamaha MT-07', 'motorcycle'),
('Silver', 2021, 'Mercedes-Benz C-Class', 'sedan'),
('Blue', 2018, 'Land Rover Discovery', '4x4');


-- ================================================================
-- INSERT CAR ENTRY/EXIT RECORDS (15 records)
-- Note: NULL exit_date_time indicates vehicle is still parked
-- ================================================================

INSERT INTO PRQ_Car_Entry (parking_id, car_id, entry_date_time, exit_date_time) VALUES
-- Parking 1 records (Madrid)
(1, 1, '2026-04-16 08:00:00', '2026-04-16 12:30:00'),
(1, 2, '2026-04-16 09:15:00', '2026-04-16 18:45:00'),
(1, 3, '2026-04-16 10:00:00', NULL),
(1, 4, '2026-04-17 07:30:00', '2026-04-17 09:00:00'),
(1, 5, '2026-04-17 08:45:00', NULL),
(1, 1, '2026-04-17 10:15:00', '2026-04-17 14:30:00'),
(1, 2, '2026-04-17 11:00:00', NULL),
(1, 3, '2026-04-17 12:30:00', '2026-04-17 16:15:00'),

-- Parking 2 records (Barcelona)
(2, 4, '2026-04-16 06:45:00', '2026-04-16 10:30:00'),
(2, 5, '2026-04-16 11:20:00', '2026-04-16 15:45:00'),
(2, 1, '2026-04-17 06:00:00', NULL),
(2, 2, '2026-04-17 08:30:00', '2026-04-17 13:00:00'),
(2, 3, '2026-04-17 09:45:00', '2026-04-17 11:30:00'),
(2, 4, '2026-04-17 10:00:00', NULL),
(2, 5, '2026-04-17 14:15:00', '2026-04-17 17:45:00');
