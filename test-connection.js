const { Client } = require('pg');

const client = new Client({
  host: 'mypostgresdb-alpi2.postgres.database.azure.com',
  port: 5432,
  database: 'portfolio_analytics',
  user: 'dbadmin',
  password: 'Simple123',
  ssl: {
    rejectUnauthorized: false
  }
});

async function testConnection() {
  try {
    await client.connect();
    console.log('Connected successfully!');
    
    const result = await client.query('SELECT NOW()');
    console.log('Current time from database:', result.rows[0].now);
    
    const tables = await client.query(`
      SELECT tablename 
      FROM pg_tables 
      WHERE schemaname = 'public'
    `);
    console.log('\nTables in database:');
    tables.rows.forEach(row => console.log('-', row.tablename));
    
    await client.end();
  } catch (err) {
    console.error('Connection error:', err.message);
  }
}

testConnection();