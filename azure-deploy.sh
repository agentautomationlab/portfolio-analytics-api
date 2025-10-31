#!/bin/bash

# Azure Free Tier Deployment Script for Portfolio Analytics API
# Uses only free tier resources

set -e

# Configuration
RESOURCE_GROUP="portfolio-analytics-rg"
LOCATION="eastus"
APP_NAME="portfolio-analytics-$(date +%s)"  # Unique name with timestamp
DB_NAME="portfolio-analytics-db-$(date +%s)"
PLAN_NAME="portfolio-analytics-plan"

echo "🚀 Deploying Portfolio Analytics API to Azure Free Tier"
echo "======================================================"

# Check if logged in
echo "📋 Checking Azure login status..."
az account show > /dev/null 2>&1 || {
    echo "❌ Not logged in to Azure. Run: az login"
    exit 1
}

# Create Resource Group
echo "📦 Creating resource group: $RESOURCE_GROUP"
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --output table

# Create App Service Plan (Free tier)
echo "🏗️  Creating App Service Plan (Free tier)..."
az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku F1 \
    --is-linux \
    --output table

# Create Web App
echo "🌐 Creating Web App: $APP_NAME"
az webapp create \
    --resource-group $RESOURCE_GROUP \
    --plan $PLAN_NAME \
    --name $APP_NAME \
    --runtime "DOTNET|8.0" \
    --output table

# Create PostgreSQL Flexible Server (Free tier equivalent)
echo "🗄️  Creating PostgreSQL Flexible Server..."
az postgres flexible-server create \
    --resource-group $RESOURCE_GROUP \
    --name $DB_NAME \
    --location $LOCATION \
    --admin-user dbadmin \
    --admin-password "SecurePass123!" \
    --tier Burstable \
    --sku-name Standard_B1ms \
    --storage-size 32 \
    --version 13 \
    --output table

# Create database
echo "📊 Creating analytics database..."
az postgres flexible-server db create \
    --resource-group $RESOURCE_GROUP \
    --server-name $DB_NAME \
    --database-name portfolio_analytics \
    --output table

# Configure firewall to allow Azure services
echo "🔒 Configuring firewall rules..."
az postgres flexible-server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --name $DB_NAME \
    --rule-name AllowAzureServices \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0 \
    --output table

# Get database connection string
echo "🔗 Getting database connection string..."
DB_HOST=$(az postgres flexible-server show \
    --resource-group $RESOURCE_GROUP \
    --name $DB_NAME \
    --query "fullyQualifiedDomainName" -o tsv)

CONNECTION_STRING="Host=$DB_HOST;Database=portfolio_analytics;Username=dbadmin;Password=SecurePass123!;SSL Mode=Require;"

# Configure app settings
echo "⚙️  Configuring application settings..."
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --settings \
        ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
        ASPNETCORE_ENVIRONMENT="Production" \
    --output table

# Enable CORS for your portfolio domain
echo "🌍 Configuring CORS..."
az webapp cors add \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --allowed-origins "*" \
    --output table

echo ""
echo "✅ Deployment Configuration Complete!"
echo "======================================"
echo ""
echo "📋 Resource Details:"
echo "  • Resource Group: $RESOURCE_GROUP"
echo "  • App Service: $APP_NAME"
echo "  • Database: $DB_NAME"
echo "  • URL: https://$APP_NAME.azurewebsites.net"
echo ""
echo "🚀 Next Steps:"
echo "  1. Deploy your code: az webapp deployment source config-zip"
echo "  2. Run EF migrations against cloud database"
echo "  3. Test API endpoints"
echo ""
echo "💰 Cost Estimate (Free Tier):"
echo "  • App Service (F1): FREE"
echo "  • PostgreSQL (B1ms): ~$12/month"
echo "  • Total: ~$12/month"
echo ""

# Save important info
cat > azure-deployment-info.txt << EOF
Azure Deployment Information
============================

Resource Group: $RESOURCE_GROUP
App Name: $APP_NAME
Database: $DB_NAME
Location: $LOCATION

API URL: https://$APP_NAME.azurewebsites.net

Database Connection:
$CONNECTION_STRING

Deployment Date: $(date)
EOF

echo "📝 Deployment info saved to: azure-deployment-info.txt"