# Docker Setup Guide

This guide will help you run PostgreSQL and Grafana using Docker containers.

## Prerequisites

- Docker Desktop installed and running
- Docker Compose (included with Docker Desktop)

## Quick Start

### 1. Start PostgreSQL and Grafana

```bash
# Start all services
docker-compose up -d

# Check if services are running
docker-compose ps

# View logs
docker-compose logs -f
```

### 2. Verify PostgreSQL is Running

```bash
# Connect to PostgreSQL
docker exec -it portfolio-analytics-db psql -U postgres -d portfolio_analytics

# Inside PostgreSQL shell, test connection
\l  # List databases
\q  # Quit
```

### 3. Run Database Migrations

```bash
# Install EF Core tools if not already installed
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

### 4. Start the API

```bash
# Update connection string in appsettings.json (already configured)
dotnet run
```

API will be available at: `http://localhost:5000`

### 5. Access Grafana

1. Open browser: http://localhost:3000
2. Login with:
   - Username: `admin`
   - Password: `admin`
3. PostgreSQL datasource is auto-configured!

## Docker Commands

### Stop Services
```bash
docker-compose stop
```

### Start Existing Services
```bash
docker-compose start
```

### Restart Services
```bash
docker-compose restart
```

### Stop and Remove Containers
```bash
docker-compose down
```

### Stop and Remove Everything (including data)
```bash
docker-compose down -v
```

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f postgres
docker-compose logs -f grafana
```

## Connection Details

### PostgreSQL
- **Host:** localhost
- **Port:** 5432
- **Database:** portfolio_analytics
- **Username:** postgres
- **Password:** postgres_password

### Grafana
- **URL:** http://localhost:3000
- **Username:** admin
- **Password:** admin

## Data Persistence

Data is persisted in Docker volumes:
- `postgres_data` - PostgreSQL database files
- `grafana_data` - Grafana dashboards and settings

Your data will survive container restarts!

## Troubleshooting

### Port Already in Use
If ports 5432 or 3000 are already in use:

1. Stop existing services:
```bash
brew services stop postgresql
brew services stop grafana
```

2. Or change ports in `docker-compose.yml`:
```yaml
ports:
  - "5433:5432"  # Use 5433 instead of 5432
  - "3001:3000"  # Use 3001 instead of 3000
```

### Reset Database
```bash
docker-compose down -v
docker-compose up -d
dotnet ef database update
```

### Check Container Health
```bash
docker-compose ps
docker inspect portfolio-analytics-db
```

## Grafana Dashboard Setup

### Import Example Queries

Once logged into Grafana:

1. Go to **Explore** → Select **PostgreSQL** datasource
2. Try these queries:

**Page Views Over Time:**
```sql
SELECT 
  DATE_TRUNC('hour', "Timestamp") as time,
  COUNT(*) as pageviews
FROM "AnalyticsEvents"
WHERE "EventType" = 'pageview'
  AND "Timestamp" >= NOW() - INTERVAL '24 hours'
GROUP BY time
ORDER BY time;
```

**Top Pages:**
```sql
SELECT 
  "PageUrl",
  COUNT(*) as views
FROM "AnalyticsEvents"
WHERE "EventType" = 'pageview'
  AND "Timestamp" >= NOW() - INTERVAL '7 days'
GROUP BY "PageUrl"
ORDER BY views DESC
LIMIT 10;
```

**Device Breakdown:**
```sql
SELECT 
  "Device",
  COUNT(*) as count
FROM "AnalyticsEvents"
WHERE "Timestamp" >= NOW() - INTERVAL '7 days'
GROUP BY "Device";
```

### Create Dashboard

1. Click **+** → **Dashboard**
2. Add Panel
3. Select PostgreSQL datasource
4. Paste SQL query
5. Configure visualization (Time series, Bar chart, etc.)
6. Save dashboard

## Production Deployment

For production, update security settings:

1. Change passwords in `.env` file
2. Use secrets management (Azure Key Vault, AWS Secrets Manager)
3. Enable PostgreSQL SSL
4. Use reverse proxy (nginx) for Grafana
5. Set up backups

## Backup & Restore

### Backup Database
```bash
docker exec portfolio-analytics-db pg_dump -U postgres portfolio_analytics > backup.sql
```

### Restore Database
```bash
cat backup.sql | docker exec -i portfolio-analytics-db psql -U postgres portfolio_analytics
```

## Next Steps

1. ✅ Start Docker containers
2. ✅ Run migrations
3. ✅ Start the API
4. ✅ Access Grafana
5. 📊 Create your first dashboard!
