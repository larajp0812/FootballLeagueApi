# Azure Deployment Guide

This guide explains how to deploy the Football League API to Microsoft Azure.

## Prerequisites

1. **Azure Subscription** - Free trial or paid subscription
2. **Azure CLI** - Download from https://aka.ms/installazurecliwindows
3. **Docker** (optional) - For container deployment
4. **Git** - Repository access

## Deployment Options

### Option 1: Deploy via Azure Portal (Simplest)

1. Go to [Azure Portal](https://portal.azure.com)
2. Search for "App Service" and click "Create"
3. Fill in the form:
   - **Name**: `football-league-api` (globally unique)
   - **Runtime stack**: .NET 8 (Linux)
   - **Region**: East US or nearest location
   - **App Service Plan**: Create new (B1 or B2 for production)
4. Click "Review + Create" then "Create"
5. Once created, go to "Deployment Center"
6. Choose "GitHub" as source
7. Connect your GitHub repository
8. Configure the build settings
9. Click "Save" to start automatic deployment

### Option 2: Deploy via Azure CLI (Recommended for Automation)

1. Login to Azure:
   ```bash
   az login
   ```

2. Create a resource group:
   ```bash
   az group create --name football-league-rg --location eastus
   ```

3. Deploy using the ARM template:
   ```bash
   az deployment group create \
     --resource-group football-league-rg \
     --template-file azure-deploy-template.json \
     --parameters \
       webAppName=football-league-api-unique \
       environment=production \
       jwtKey="YourSecureJWTKeyAtLeast32CharactersLong!"
   ```

4. Deploy the application code:
   ```bash
   az webapp up \
     --resource-group football-league-rg \
     --name football-league-api-unique \
     --runtime DOTNET:8.0 \
     --os-type linux
   ```

### Option 3: Deploy via Azure DevOps Pipeline

1. Create an Azure DevOps project at https://dev.azure.com
2. Connect to your GitHub repository
3. Upload `azure-pipelines.yml` to your repository root
4. Create a new Pipeline pointing to `azure-pipelines.yml`
5. Configure Azure subscription in pipeline settings
6. Create environments for "Staging" and "Production"
7. Run the pipeline - it will automatically:
   - Build the code
   - Run tests
   - Deploy to Staging
   - (On main branch) Deploy to Production

## Environment Configuration

### Application Settings (Set in Azure Portal or via CLI)

```bash
# Via Azure CLI
az webapp config appsettings set \
  --resource-group football-league-rg \
  --name football-league-api-unique \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    Jwt__Key="your-secure-jwt-key" \
    Jwt__Issuer="FootballLeagueApi" \
    Jwt__Audience="FootballLeagueApiUsers" \
    Jwt__ExpiresMinutes="60"
```

### Connection Strings

For SQLite (file-based):
```
Data Source=/home/site/wwwroot/league.db
```

For Azure SQL Database (recommended for production):
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=FootballLeagueDb;Persist Security Info=False;User ID=sqladmin;Password=YourPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Post-Deployment Steps

1. **Enable HTTPS**:
   ```bash
   az webapp update \
     --resource-group football-league-rg \
     --name football-league-api-unique \
     --set httpsOnly=true
   ```

2. **Configure Custom Domain** (optional):
   - Go to App Service > Custom domains
   - Add your domain and verify ownership
   - Update DNS settings

3. **Enable Monitoring**:
   - In Azure Portal, go to Application Insights
   - Enable for your App Service
   - Monitor logs, performance, and errors

4. **Set up Continuous Deployment**:
   - In Deployment Center, configure automatic deployment from GitHub
   - Choose branch to deploy (main/develop)

5. **Test the API**:
   ```bash
   # Health check
   curl https://your-app-name.azurewebsites.net/health
   
   # Swagger UI
   https://your-app-name.azurewebsites.net/swagger
   ```

## Troubleshooting

### App Won't Start
- Check logs: `az webapp log tail --resource-group football-league-rg --name your-app-name`
- Verify environment variables are set in Application Settings
- Check if Runtime Stack is set to .NET 8.0

### Database Issues
- SQLite path must be writable: `/home/site/wwwroot/`
- For Azure SQL, verify firewall rules allow connections
- Run migrations after deployment:
  ```bash
  dotnet ef database update --connection "YourConnectionString"
  ```

### Authentication Issues
- Verify JWT__Key is set (minimum 32 characters)
- Check token expiry in production settings
- Verify CORS policy in appsettings.json

## Scaling & Performance

### For Higher Traffic:
1. Scale up App Service Plan (B2 → S1 → P1)
2. Enable autoscaling based on metrics
3. Migrate to Azure SQL Database for better performance
4. Use Azure CDN for static content
5. Enable Application Caching

### Monitoring:
- Application Insights: Real-time monitoring
- Log Analytics: Centralized logging
- Azure Monitor: Performance metrics

## Cost Optimization

- **Development**: Use F1 Free tier
- **Staging**: Use B1 Basic tier (~$0.06/hour)
- **Production**: Use B2 Standard tier or higher with autoscaling
- **Database**: Use Azure SQL Database (pay-per-use) instead of SQLite for production
- **Backup**: Enable automatic backups for SQL Database

## Security Checklist

- [ ] HTTPS enabled (Azure enforces by default)
- [ ] JWT secret stored in Key Vault (not in config)
- [ ] Database credentials in Key Vault or Managed Identity
- [ ] Network security rules configured
- [ ] SQL injection prevention via EF Core
- [ ] CORS policy restricted to trusted origins
- [ ] Rate limiting implemented
- [ ] Logging enabled for audits

## Cleanup (Delete Resources)

When you're done testing:
```bash
az group delete --resource-group football-league-rg
```

This deletes all resources in the group to avoid unexpected charges.

## Support & Documentation

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service)
- [Azure DevOps Pipelines](https://docs.microsoft.com/azure/devops/pipelines)
- [Azure ARM Templates](https://docs.microsoft.com/azure/azure-resource-manager)
- [.NET on Azure](https://azure.microsoft.com/solutions/dotnet)
