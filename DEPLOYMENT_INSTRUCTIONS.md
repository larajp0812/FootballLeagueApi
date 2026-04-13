# Deployment Instructions - Azure App Service

## Overview

This project is configured to deploy automatically to Azure App Service when you push to the `main` branch.

## Prerequisites

1. **Azure Subscription** - Create free account at https://azure.microsoft.com/free
2. **Azure App Service Resources** - Frontend and backend App Services created in Azure Portal
3. **GitHub Repository** - This repo with deployment workflows

## Step 1: Create Azure App Service Resources

### Frontend App Service (football-league-frontend)

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **Create a resource**
3. Search for **"Web App"**
4. Click **Create**
5. **Basics** tab:
   - **Subscription**: Your Azure subscription
   - **Resource Group**: Create new or use existing
   - **Name**: `football-league-frontend` (must be globally unique)
   - **Publish**: Code
   - **Runtime stack**: Node 22 LTS
   - **Operating System**: Linux
   - **Region**: Choose closest to you
6. **App Service Plan**:
   - Create new plan or use existing (Free tier available)
7. Click **Review + create** then **Create**

### Backend App Service (football-league-backend)

1. Repeat the above steps for the backend
2. **Basics** tab:
   - **Name**: `football-league-backend` (must be globally unique)
   - **Runtime stack**: .NET 8 (LTS)
   - **Operating System**: Linux

## Step 2: Get Publish Profiles

For Azure App Service deployment, you need publish profiles (not deployment tokens):

### Frontend Publish Profile (football-league-frontend)

1. Go to your **Azure App Service** resource named `football-league-frontend`
2. Click **Deployment Center** in the left sidebar
3. Click **FTPS credentials** tab
4. Click **Download publish profile**
5. Save the `.PublishSettings` file

### Backend Publish Profile (football-league-backend)

1. Go to your **Azure App Service** resource for the backend
2. Click **Deployment Center** in the left sidebar
3. Click **FTPS credentials** tab
4. Click **Download publish profile**
5. Save the `.PublishSettings` file

## Step 3: Add GitHub Secrets

In your GitHub repository:

1. Go to **Settings** > **Secrets and variables** > **Actions**
2. Click **New repository secret**
3. Add these secrets:

### Secret 1: AZURE_WEBAPP_PUBLISH_PROFILE

- **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
- **Value**: Open the frontend `.PublishSettings` file and copy the entire XML content
- Click **Add secret**

### Secret 2: AZURE_WEBAPP_PUBLISH_PROFILE_BACKEND

- **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE_BACKEND`
- **Value**: Open the backend `.PublishSettings` file and copy the entire XML content
- Click **Add secret**

### Secret 3: AZURE_VITE_API_BASE_URL

- **Name**: `AZURE_VITE_API_BASE_URL`
- **Value**: Your backend API URL
  - Production: `https://your-backend-app-service.azurewebsites.net`
- Click **Add secret**

## Step 4: Configure Azure Environment

In Azure Portal > Your Frontend App Service (`football-league-frontend`) > Configuration:

1. Click **Configuration**
2. Under **Application settings**, click **Add**
3. Name: `VITE_API_BASE_URL`
4. Value: Your backend API URL (production URL)
5. Click **Save**

In Azure Portal > Your Backend App Service > Configuration:

1. Add any environment variables your backend needs (database connection strings, etc.)
2. Make sure CORS allows requests from your frontend URL

## Step 5: Deploy

### Automatic Deployment

Simply push to main:

```bash
git add .
git commit -m "Deploy to Azure"
git push origin main
```

The GitHub Actions workflow will automatically:

1. Checkout code
2. Run lint checks
3. Run tests
4. Build production bundle
5. Deploy to Azure

### Monitor Deployment

1. **GitHub**: Go to repository > **Actions** tab
2. Click the latest workflow run
3. Monitor build and deployment progress

## Step 7: Verify Deployment

After deployment completes:

1. Go to Azure Portal > Your Static Web App
2. Click **Browse** to open the app
3. Or use the URL: `https://<app-name>.azurestaticapps.net`

## Troubleshooting

### Build Fails in GitHub Actions

Check logs:

1. GitHub > Settings > Secrets - verify all secrets are set
2. Repository > Actions tab - click failed workflow
3. Look for error messages in Build step

Common issues:

- Missing `AZURE_STATIC_WEB_APPS_TOKEN` secret
- Build output location not set to `dist`
- Node version incompatibility

### API Calls Fail After Deployment

1. Verify `VITE_API_BASE_URL` is set correctly in Azure Configuration
2. Backend CORS must allow requests from `*.azurestaticapps.net`
3. Backend must be accessible from Azure (check firewall rules)

### Can't Access Deployed App

1. Wait 5-10 minutes after deployment completes
2. Check browser cache - press `Ctrl+Shift+Delete` and clear cache
3. Verify app is using HTTPS: `https://...` not `http://...`

## Environment Variables

### Development

```
VITE_API_BASE_URL=https://localhost:7195
```

### Production (Azure)

```
VITE_API_BASE_URL=https://your-production-backend.azurewebsites.net
```

## Custom Domain (Optional)

1. Azure Portal > Static Web App > Custom domains
2. Add your domain
3. Follow DNS configuration steps
4. SSL certificate auto-provisioned

## Monitoring

### View Logs

Azure Portal > Static Web App > Logs (using Azure Monitor)

### Performance

Use Azure Application Insights for:

- Page load times
- Error rates
- User analytics

## Scaling & Costs

- **Free tier**: 100GB bandwidth/month, unlimited builds
- **Standard tier**: Pay-per-request + bandwidth
- No minimum costs for free tier

## Disable Auto-Deploy (If Needed)

To stop automatic deployments:

1. GitHub > Remove the workflow file OR
2. Disable workflow in GitHub Actions settings

## Rollback to Previous Version

Azure Portal > Deployments > Select previous deployment > Redeploy

## Further Resources

- [Azure Static Web Apps Docs](https://learn.microsoft.com/azure/static-web-apps/)
- [SPA Routing Guide](https://learn.microsoft.com/azure/static-web-apps/configuration)
- [Azure CLI Deployment](https://learn.microsoft.com/azure/static-web-apps/deployment-environments)

## Quick Reference

| Task              | Location                                          |
| ----------------- | ------------------------------------------------- |
| Deployment status | GitHub Actions tab                                |
| Frontend App URL  | Azure Portal > App Service > Browse                |
| Backend API URL   | Azure Portal > App Service > Browse                |
| Publish Profile   | Azure Portal > App Service > Deployment Center    |
| GitHub secrets    | GitHub Settings > Secrets and variables           |
| Custom domain     | Azure Portal > Custom domains                     |

## Support

For issues:

1. Check GitHub Actions logs for build errors
2. Verify all secrets are set in GitHub
3. Verify VITE_API_BASE_URL in Azure Configuration
4. Check backend API is running and accessible
