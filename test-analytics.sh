#!/bin/bash

echo "🧪 Testing Portfolio Analytics System"
echo "======================================"
echo ""

# Test 1: Check API Health
echo "1️⃣ Testing API Health..."
health=$(curl -s http://localhost:5213/api/analytics/health)
if [ $? -eq 0 ]; then
    echo "✅ API is healthy: $health"
else
    echo "❌ API is not responding"
    exit 1
fi
echo ""

# Test 2: Send test pageview events
echo "2️⃣ Sending test analytics events..."
for i in {1..5}; do
    curl -s -X POST http://localhost:5213/api/analytics/track \
      -H "Content-Type: application/json" \
      -d "{
        \"eventType\": \"pageview\",
        \"pageUrl\": \"https://myportfolio.com/page$i\",
        \"pageTitle\": \"Test Page $i\",
        \"device\": \"desktop\",
        \"browser\": \"Chrome\",
        \"os\": \"macOS\",
        \"screenResolution\": \"1920x1080\",
        \"viewportWidth\": 1920,
        \"viewportHeight\": 1080
      }" > /dev/null
    echo "   ✅ Sent pageview $i"
done
echo ""

# Test 3: Send different device types
echo "3️⃣ Testing different devices..."
devices=("mobile" "tablet" "desktop")
for device in "${devices[@]}"; do
    curl -s -X POST http://localhost:5213/api/analytics/track \
      -H "Content-Type: application/json" \
      -d "{
        \"eventType\": \"pageview\",
        \"pageUrl\": \"https://myportfolio.com/home\",
        \"pageTitle\": \"Home\",
        \"device\": \"$device\",
        \"browser\": \"Safari\",
        \"os\": \"iOS\"
      }" > /dev/null
    echo "   ✅ Sent $device pageview"
done
echo ""

# Test 4: Send custom events
echo "4️⃣ Testing custom events..."
curl -s -X POST http://localhost:5213/api/analytics/track \
  -H "Content-Type: application/json" \
  -d '{
    "eventType": "button_click",
    "pageUrl": "https://myportfolio.com/contact",
    "pageTitle": "Contact",
    "customData": {"button": "contact_form"}
  }' > /dev/null
echo "   ✅ Sent button_click event"

curl -s -X POST http://localhost:5213/api/analytics/track \
  -H "Content-Type: application/json" \
  -d '{
    "eventType": "scroll",
    "pageUrl": "https://myportfolio.com/about",
    "customData": {"depth": 75}
  }' > /dev/null
echo "   ✅ Sent scroll event"
echo ""

# Test 5: Get statistics
echo "5️⃣ Fetching analytics statistics..."
stats=$(curl -s http://localhost:5213/api/analytics/stats | jq .)
echo "$stats"
echo ""

# Test 6: Query database directly
echo "6️⃣ Querying PostgreSQL database..."
docker exec portfolio-analytics-db psql -U postgres -d portfolio_analytics -c \
  "SELECT \"EventType\", COUNT(*) as count FROM \"AnalyticsEvents\" GROUP BY \"EventType\" ORDER BY count DESC;"
echo ""

echo "✨ All tests completed!"
echo ""
echo "📊 Next steps:"
echo "   1. Open Grafana: http://localhost:3001 (admin/admin)"
echo "   2. Browse your portfolio: http://localhost:5173/portfolio/"
echo "   3. Check API stats: http://localhost:5213/api/analytics/stats"
