# Portfolio Analytics API

A custom analytics system built with C# ASP.NET Core, PostgreSQL, and Grafana for tracking comprehensive website analytics data.

## Features

- 📊 Real-time event tracking (page views, clicks, scrolls)
- 🌍 Geographic data collection (IP-based)
- 📱 Device, browser, and OS detection
- 🎯 UTM parameter tracking for campaign analytics
- 🔄 Session management
- ⚡ Performance metrics (page load times)
- 📈 RESTful API for querying analytics data
- 🎨 Grafana-ready PostgreSQL database

## Architecture

```
Portfolio Website (React) 
    ↓ (tracker.js)
Analytics API (C# ASP.NET Core)
    ↓
PostgreSQL Database
    ↓
Grafana Dashboards
```

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL 14+
- (Optional) Grafana for visualization

### 1. Database Setup

```bash
# Install PostgreSQL (macOS)
brew install postgresql@14
brew services start postgresql@14

# Create database
psql postgres
CREATE DATABASE portfolio_analytics;
CREATE USER postgres WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE portfolio_analytics TO postgres;
\q
```

### 2. Configure Connection String

Update `appsettings.json` with your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=portfolio_analytics;Username=postgres;Password=your_password"
  }
}
```

### 3. Run Migrations

```bash
cd portfolio-analytics-api

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
```

API will be available at: `https://localhost:5000`

### 5. Add Tracker to Your Portfolio

Copy `tracker.js` to your portfolio project and include it in your `index.html`:

```html
<script src="/tracker.js"></script>
```

Or in your React app, add to `public/index.html`:

```html
<script>
  // Update API URL
  const ANALYTICS_API = 'https://your-api-domain.com/api/analytics/track';
</script>
<script src="/tracker.js"></script>
```

## API Endpoints

### Track Event
```http
POST /api/analytics/track
Content-Type: application/json

{
  "eventType": "pageview",
  "pageUrl": "https://yoursite.com/about",
  "pageTitle": "About Me",
  "sessionId": "uuid",
  "device": "desktop",
  "browser": "Chrome",
  "os": "macOS"
}
```

### Get Stats
```http
GET /api/analytics/stats?from=2025-10-01&to=2025-10-30
```

Response:
```json
{
  "totalEvents": 1523,
  "uniqueSessions": 342,
  "pageViews": 891,
  "topPages": [...],
  "topReferrers": [...],
  "deviceBreakdown": [...]
}
```

### Health Check
```http
GET /api/analytics/health
```

## Data Collected

| Field | Description |
|-------|-------------|
| EventType | Type of event (pageview, click, scroll, etc.) |
| PageUrl | Full URL of the page |
| PageTitle | Title of the page |
| Referrer | Where the visitor came from |
| UtmSource/Medium/Campaign | Marketing campaign tracking |
| SessionId | Unique session identifier |
| IpAddress | Client IP address |
| Country/City | Geographic location (optional) |
| UserAgent | Browser user agent string |
| Browser/Os/Device | Parsed browser, OS, and device type |
| ScreenResolution | Screen dimensions |
| ViewportWidth/Height | Browser viewport size |
| LoadTime | Page load time in milliseconds |
| CustomData | JSON object for custom tracking data |

## Grafana Integration

### Connect PostgreSQL to Grafana

1. Install Grafana: `brew install grafana`
2. Start Grafana: `brew services start grafana`
3. Open http://localhost:3000 (admin/admin)
4. Add PostgreSQL data source:
   - Host: `localhost:5432`
   - Database: `portfolio_analytics`
   - User: `postgres`
   - SSL Mode: `disable`

### Example Queries

**Page Views Over Time:**
```sql
SELECT 
  DATE_TRUNC('day', "Timestamp") as time,
  COUNT(*) as pageviews
FROM "AnalyticsEvents"
WHERE "EventType" = 'pageview'
  AND "Timestamp" >= NOW() - INTERVAL '30 days'
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

**Unique Visitors by Day:**
```sql
SELECT 
  DATE_TRUNC('day', "Timestamp") as time,
  COUNT(DISTINCT "SessionId") as unique_visitors
FROM "AnalyticsEvents"
WHERE "Timestamp" >= NOW() - INTERVAL '30 days'
GROUP BY time
ORDER BY time;
```

## Custom Event Tracking

Track custom events from your portfolio:

```javascript
// Track button click
document.getElementById('contact-btn').addEventListener('click', () => {
  window.portfolioAnalytics.track('button_click', {
    customData: { button: 'contact' }
  });
});

// Track form submission
form.addEventListener('submit', () => {
  window.portfolioAnalytics.track('form_submit', {
    customData: { form: 'contact' }
  });
});
```

## Deployment

### Deploy API to Azure/AWS

1. Update CORS origins in `Program.cs`
2. Configure production connection string
3. Deploy using:
   - Azure App Service
   - AWS Elastic Beanstalk
   - Docker container

### Environment Variables

```bash
ConnectionStrings__DefaultConnection="Host=prod-db;Database=analytics;..."
ASPNETCORE_ENVIRONMENT=Production
```

## Security Considerations

- Configure CORS properly for production
- Use HTTPS only in production
- Protect database credentials
- Consider rate limiting for the tracking endpoint
- Implement API key authentication if needed

## License

MIT License - Feel free to use for your portfolio!
