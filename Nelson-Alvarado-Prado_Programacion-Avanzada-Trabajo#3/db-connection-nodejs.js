// Node.js - MySQL Connection Example
// Install dependencies: npm install mysql2 dotenv

const mysql = require('mysql2/promise');
require('dotenv').config();

const pool = mysql.createPool({
  host: process.env.DB_HOST,
  port: process.env.DB_PORT,
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  database: process.env.DB_NAME,
  ssl: 'REQUIRED',
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0
});

// Example: Query PRQ_Cars table
async function getAllCars() {
  try {
    const connection = await pool.getConnection();
    const [rows] = await connection.execute('SELECT * FROM PRQ_Cars');
    connection.release();
    return rows;
  } catch (error) {
    console.error('Database error:', error);
  }
}

// Example: Query PRQ_Parking table
async function getAllParking() {
  try {
    const connection = await pool.getConnection();
    const [rows] = await connection.execute('SELECT * FROM PRQ_Parking');
    connection.release();
    return rows;
  } catch (error) {
    console.error('Database error:', error);
  }
}

// Example: Query PRQ_Car_Entry table
async function getAllCarEntries() {
  try {
    const connection = await pool.getConnection();
    const [rows] = await connection.execute('SELECT * FROM PRQ_Car_Entry');
    connection.release();
    return rows;
  } catch (error) {
    console.error('Database error:', error);
  }
}

// Example: Get currently parked cars
async function getCurrentlyParkedCars() {
  try {
    const connection = await pool.getConnection();
    const [rows] = await connection.execute(`
      SELECT 
        c.ID, c.color, c.year, c.make, c.type,
        p.name as parking_name,
        ce.entry_date_time
      FROM PRQ_Car_Entry ce
      JOIN PRQ_Cars c ON ce.car_id = c.ID
      JOIN PRQ_Parking p ON ce.parking_id = p.ID
      WHERE ce.exit_date_time IS NULL
    `);
    connection.release();
    return rows;
  } catch (error) {
    console.error('Database error:', error);
  }
}

// Export functions
module.exports = {
  getAllCars,
  getAllParking,
  getAllCarEntries,
  getCurrentlyParkedCars
};
