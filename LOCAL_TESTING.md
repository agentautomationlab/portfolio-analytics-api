# Local Testing Guide

Complete guide to test your analytics system locally.

## Prerequisites

Make sure Docker Desktop is running!

## Step 1: Start Services

```bash
cd ~/Desktop/portfolio-analytics-api

# Stop any existing services
docker-compose down

# Start PostgreSQL and Grafana
docker-compose up -d

# Wait for services to be ready (about 10 seconds)
sleep 10

# Check if services are running
docker-compose ps
```

Expected output:
```
NAME                     STATUS    PORTS
portfolio-analytics-db       Up       0.0.0.0:5432->5432/tcp
portfolio-analytics-grafana  Up       0.0.0.0:3001->3000/tcp
```

## Step 2: Run Database Migrations

```bash
export PATH="$PATH:/Users/alpi/.dotnet/tools"
dotnet ef database update
```

## Step 3: Start the API

```bash
# In one terminal
dotnet run
```

You should see:
```
Now listening on: http://localhost:5213
```

## Step 4: Test the API

Open a **new terminal** and run:

```bash
# Test health endpoint
curl http://localhost:5213/api/analytics/health | jq .

# Send a test event
curl -X POST http://localhost:5213/api/analytics/track \
  -H "Content-Type: application/json" \
  -d '{
    "eventType": "pageview",
    "pageUrl": "https://myportfolio.com/test",
    "pageTitle": "Test Page",
    "device": "desktop",
    "browser": "Chrome",
    "os": "macOS"
  }' | jq .

# Get stats
curl http://localhost:5213/api/analytics/stats | jq .
```

## Step 5: Start Your Portfolio

Open **another terminal**:

```bash
cd ~/Desktop/portfolio
npm run dev
```

Then open: http://localhost:5173/portfolio/

## Step 6: Access Grafana

1. Open browser: http://localhost:3001
2. Login: `admin` / `admin`
3. Go to **Explore** in left menu
4. Select **PostgreSQL** datasource
5. Run this query:

```sql
SELECT 
  "EventType",
  "PageUrl",
  "Browser",
  "Device",
  "Timestamp"
FROM "AnalyticsEvents"
ORDER BY "Timestamp" DESC
LIMIT 10;
```

## Step 7: Generate Test Data

Run the test script:

```bash
chmod +x ~/Desktop/portfolio-analytics-api/test-analytics.sh
~/Desktop/portfolio-analytics-api/test-analytics.sh
```

## Quick Test - All in One

```bash
# Terminal 1: Start Docker
cd ~/Desktop/portfolio-analytics-api
docker-compose up -d
sleep 10

# Terminal 2: Start API  
cd ~/Desktop/portfolio-analytics-api
export PATH="$PATH:/Users/alpi/.dotnet/tools"
dotnet ef database update
dotnet run

# Terminal 3: Start Portfolio
cd ~/Desktop/portfolio
npm run dev

# Terminal 4: Test API
curl http://localhost:5213/api/analytics/health
```

## Verify Everything is Working

### Check 1: API is responding
```bash
curl http://localhost:5213/api/analytics/health
# Should return: {"status":"healthy","timestamp":"..."}
```

### Check 2: Database has data
```bash
docker exec portfolio-analytics-db psql -U postgres -d portfolio_analytics \
  -c "SELECT COUNT(*) FROM \"AnalyticsEvents\";"
```

### Check 3: Grafana is accessible
```bash
curl http://localhost:3001/login
# Should return HTML
```

### Check 4: Portfolio is tracking
1. Open http://localhost:5173/portfolio/
2. Open browser DevTools → Network tab
3. Look for POST request to `http://localhost:5213/api/analytics/track`
4. Should see status 200

## Troubleshooting

### Port Already in Use

If port 5213 is taken:
```bash
lsof -i :5213
kill -9 <PID>
```

If port 3001 is taken:
```bash
lsof -i :3001
kill -9 <PID>
```

### Docker Issues

```bash
# Restart Docker Desktop
# Then:
docker-compose down -v
docker-compose up -d
```

### API Not Connecting to Database

Check connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=portfolio_analytics;Username=postgres;Password=postgres_password"
}
```

### CORS Errors in Browser

The tracker should work from `http://localhost:5173`. If you see CORS errors, check `Program.cs` includes:
```csharp
"http://localhost:5173"
```

## Accessing Services

| Service | URL | Credentials |
|---------|-----|-------------|
| API | http://localhost:5213 | - |
| Swagger | http://localhost:5213/swagger | - |
| Grafana | http://localhost:3001 | admin/admin |
| Portfolio | http://localhost:5173/portfolio/ | - |
| PostgreSQL | localhost:5432 | postgres/postgres_password |

## Sample Grafana Queries

### Page Views Over Time
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

### Device Breakdown
```sql
SELECT 
  "Device",
  COUNT(*) as count
FROM "AnalyticsEvents"
GROUP BY "Device";
```

### Top Pages
```sql
SELECT 
  "PageUrl",
  COUNT(*) as views
FROM "AnalyticsEvents"
WHERE "EventType" = 'pageview'
GROUP BY "PageUrl"
ORDER BY views DESC
LIMIT 10;
```

### Browser Stats
```sql
SELECT 
  "Browser",
  "Os",
  COUNT(*) as sessions
FROM "AnalyticsEvents"
GROUP BY "Browser", "Os"
ORDER BY sessions DESC;
```

## Import Grafana Dashboard

1. In Grafana, click **+** → **Import Dashboard**
2. Click **Upload JSON file**
3. Select `grafana-dashboard.json`
4. Click **Import**

Your dashboard will automatically refresh every 10 seconds!

## Stop All Services

```bash
# Stop API (Ctrl+C in terminal)
# Stop Portfolio (Ctrl+C in terminal)

# Stop Docker services
cd ~/Desktop/portfolio-analytics-api
docker-compose down

# To remove all data
docker-compose down -v
```
