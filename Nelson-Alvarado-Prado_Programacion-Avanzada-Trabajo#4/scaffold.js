// Database Scaffold Generator for Car Park Tables
// Connects to Azure MySQL and generates schema documentation for PRQ tables

const mysql = require('mysql2/promise');
require('dotenv').config();
const fs = require('fs');

class DatabaseScaffold {
  constructor() {
    this.pool = null;
    this.scaffold = [];
  }

  async connect() {
    try {
      this.pool = mysql.createPool({
        host: process.env.DB_HOST,
        port: process.env.DB_PORT,
        user: process.env.DB_USER,
        password: process.env.DB_PASSWORD,
        database: process.env.DB_NAME,
        ssl: 'REQUIRED',
        waitForConnections: true,
        connectionLimit: 5,
        queueLimit: 0
      });

      const connection = await this.pool.getConnection();
      console.log('✓ Successfully connected to Azure MySQL instance');
      console.log(`  Host: ${process.env.DB_HOST}`);
      console.log(`  Database: ${process.env.DB_NAME}\n`);
      connection.release();
      return true;
    } catch (error) {
      console.error('✗ Connection failed:', error.message);
      return false;
    }
  }

  async getTables() {
    try {
      const connection = await this.pool.getConnection();
      const [tables] = await connection.execute(
        `SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
         WHERE TABLE_SCHEMA = ? AND TABLE_NAME LIKE 'PRQ%' 
         ORDER BY TABLE_NAME`,
        [process.env.DB_NAME]
      );
      connection.release();
      return tables.map(t => t.TABLE_NAME);
    } catch (error) {
      console.error('Error fetching tables:', error.message);
      return [];
    }
  }

  async getTableStructure(tableName) {
    try {
      const connection = await this.pool.getConnection();
      const [columns] = await connection.execute(
        `SELECT COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_KEY, EXTRA, COLUMN_DEFAULT
         FROM INFORMATION_SCHEMA.COLUMNS 
         WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ?
         ORDER BY ORDINAL_POSITION`,
        [process.env.DB_NAME, tableName]
      );

      const [constraints] = await connection.execute(
        `SELECT CONSTRAINT_NAME, COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
         FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
         WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ? AND REFERENCED_TABLE_NAME IS NOT NULL`,
        [process.env.DB_NAME, tableName]
      );

      connection.release();
      return { columns, constraints };
    } catch (error) {
      console.error(`Error fetching structure for ${tableName}:`, error.message);
      return { columns: [], constraints: [] };
    }
  }

  async generateScaffold() {
    const tables = await this.getTables();
    
    if (tables.length === 0) {
      console.log('No PRQ tables found in database.');
      return;
    }

    console.log(`Found ${tables.length} table(s) starting with PRQ:\n`);
    console.log('═'.repeat(80));

    let scaffoldOutput = `# Car Park Database Scaffold\n`;
    scaffoldOutput += `Generated: ${new Date().toISOString()}\n`;
    scaffoldOutput += `Database: ${process.env.DB_NAME}\n`;
    scaffoldOutput += `Instance: ${process.env.DB_HOST}\n\n`;

    for (const tableName of tables) {
      const { columns, constraints } = await this.getTableStructure(tableName);
      
      // Console output
      console.log(`\n📋 TABLE: ${tableName}`);
      console.log('─'.repeat(80));

      // Generate CREATE TABLE statement
      let createTableSQL = `CREATE TABLE ${tableName} (\n`;
      
      const columnDefinitions = columns.map(col => {
        let def = `  ${col.COLUMN_NAME} ${col.COLUMN_TYPE}`;
        
        if (col.IS_NULLABLE === 'NO') {
          def += ' NOT NULL';
        }
        
        if (col.COLUMN_DEFAULT !== null) {
          def += ` DEFAULT ${col.COLUMN_DEFAULT}`;
        }
        
        if (col.COLUMN_KEY === 'PRI') {
          def += ' PRIMARY KEY';
        }
        
        if (col.EXTRA === 'auto_increment') {
          def += ' AUTO_INCREMENT';
        }
        
        return def;
      });

      // Add foreign key constraints
      if (constraints.length > 0) {
        constraints.forEach(fk => {
          columnDefinitions.push(
            `  FOREIGN KEY (${fk.COLUMN_NAME}) REFERENCES ${fk.REFERENCED_TABLE_NAME}(${fk.REFERENCED_COLUMN_NAME})`
          );
        });
      }

      createTableSQL += columnDefinitions.join(',\n');
      createTableSQL += '\n);';

      // Console display
      console.log('\nColumns:');
      columns.forEach(col => {
        const nullable = col.IS_NULLABLE === 'YES' ? 'NULL' : 'NOT NULL';
        const key = col.COLUMN_KEY ? `[${col.COLUMN_KEY}]` : '';
        console.log(`  • ${col.COLUMN_NAME.padEnd(25)} ${col.COLUMN_TYPE.padEnd(20)} ${nullable.padEnd(10)} ${key}`);
      });

      if (constraints.length > 0) {
        console.log('\nForeign Keys:');
        constraints.forEach(fk => {
          console.log(`  • ${fk.CONSTRAINT_NAME}: ${fk.COLUMN_NAME} → ${fk.REFERENCED_TABLE_NAME}(${fk.REFERENCED_COLUMN_NAME})`);
        });
      }

      // Get row count
      try {
        const connection = await this.pool.getConnection();
        const [countResult] = await connection.execute(`SELECT COUNT(*) as count FROM ${tableName}`);
        const rowCount = countResult[0].count;
        console.log(`\nRecords: ${rowCount}`);
        connection.release();
      } catch (error) {
        console.log('Records: Error fetching count');
      }

      // Add to scaffold file output
      scaffoldOutput += `\n## ${tableName}\n\n`;
      scaffoldOutput += '```sql\n';
      scaffoldOutput += createTableSQL + '\n';
      scaffoldOutput += '```\n\n';
      scaffoldOutput += '**Columns:**\n';
      columns.forEach(col => {
        scaffoldOutput += `- \`${col.COLUMN_NAME}\` (${col.COLUMN_TYPE}) - ${col.IS_NULLABLE === 'YES' ? 'nullable' : 'required'}\n`;
      });

      if (constraints.length > 0) {
        scaffoldOutput += '\n**Foreign Keys:**\n';
        constraints.forEach(fk => {
          scaffoldOutput += `- \`${fk.COLUMN_NAME}\` references \`${fk.REFERENCED_TABLE_NAME}(${fk.REFERENCED_COLUMN_NAME})\`\n`;
        });
      }

      scaffoldOutput += '\n';
      this.scaffold.push({ tableName, createTableSQL });
    }

    console.log('\n' + '═'.repeat(80) + '\n');

    return scaffoldOutput;
  }

  async saveScaffold(filename, content) {
    try {
      fs.writeFileSync(filename, content, 'utf-8');
      console.log(`✓ Scaffold saved to: ${filename}`);
      return true;
    } catch (error) {
      console.error('Error saving scaffold:', error.message);
      return false;
    }
  }

  async disconnect() {
    if (this.pool) {
      await this.pool.end();
      console.log('✓ Database connection closed');
    }
  }
}

// Main execution
async function main() {
  const scaffold = new DatabaseScaffold();

  console.log('\n🔗 Connecting to Azure MySQL Cloud Instance...\n');

  if (!(await scaffold.connect())) {
    process.exit(1);
  }

  console.log('📊 Scaffolding PRQ tables...\n');
  const scaffoldContent = await scaffold.generateScaffold();

  if (scaffoldContent) {
    // Save to markdown file
    const markdownFile = './scaffold-prq-tables.md';
    await scaffold.saveScaffold(markdownFile, scaffoldContent);

    // Also save CREATE TABLE statements
    let sqlContent = `-- Car Park Database Scaffold - SQL Statements\n`;
    sqlContent += `-- Generated: ${new Date().toISOString()}\n\n`;
    scaffold.scaffold.forEach(table => {
      sqlContent += table.createTableSQL + '\n\n';
    });

    const sqlFile = './scaffold-prq-tables.sql';
    await scaffold.saveScaffold(sqlFile, sqlContent);
  }

  await scaffold.disconnect();
  console.log('\n✓ Scaffold completed successfully!\n');
}

main().catch(error => {
  console.error('Fatal error:', error);
  process.exit(1);
});
