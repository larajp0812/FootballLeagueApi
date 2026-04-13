# Deployment Instructions - Azure Static Web Apps

## Overview

This project is configured to deploy automatically to Azure Static Web Apps when you push to the `main` branch.

## Prerequisites

1. **Azure Subscription** - Create free account at https://azure.microsoft.com/free
2. **Azure Static Web Apps Resource** - Set up in Azure Portal
3. **GitHub Repository** - This repo with deployment workflow

## Step 1: Create Azure Static Web Apps Resource

### Using Azure Portal (easiest for beginners)

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **Create a resource**
3. Search for **"Static Web Apps"**
4. Click **Create**

### Configure Resource:

- **Subscription**: Select your subscription
- **Resource Group**: Create new (e.g., `football-league-rg`)
- **Name**: `football-league-frontend`
- **Plan**: Free (sufficient for coursework)
- **Region**: UK South (or closest to you)

## Step 2: Configure GitHub Integration

During **Static Web Apps** creation, you'll connect it to GitHub:

1. **Source Control**: GitHub
2. **Sign in** with your GitHub account
3. **Organization**: larajp0812
4. **Repository**: FootballLeagueApi
5. **Branch**: main
6. **Build Presets**: Custom
7. **App location**: `frontend`
8. **API location**: (Leave empty)
9. **Build output location**: `dist`

Azure will automatically create a workflow file in `.github/workflows/`.

## Step 3: Get Deployment Token

After Azure Static Web Apps is created:

1. Go to your Static Web App resource in Azure Portal
2. Click **Deployment tokens** in the left sidebar
3. Copy the token value

## Step 4: Add GitHub Secrets

In your GitHub repository:

1. Go to **Settings** > **Secrets and variables** > **Actions**
2. Click **New repository secret**
3. Add these secrets:

### Secret 1: AZURE_STATIC_WEB_APPS_TOKEN

- **Name**: `AZURE_STATIC_WEB_APPS_TOKEN`
- **Value**: Paste the token from Step 3
- Click **Add secret**

### Secret 2: AZURE_VITE_API_BASE_URL

- **Name**: `AZURE_VITE_API_BASE_URL`
- **Value**: Your backend API URL
  - Development: `https://localhost:7195`
  - Production: `https://your-backend-api.azurewebsites.net`
- Click **Add secret**

## Step 5: Configure Azure Environment

In Azure Portal > Your Static Web App > Configuration:

1. Click **Configuration**
2. Under **Application settings**, click **Add**
3. Name: `VITE_API_BASE_URL`
4. Value: Your backend API URL (production URL)
5. Click **Save**

## Step 6: Deploy

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
| App URL           | Azure Portal > Static Web App > Browse            |
| Backend API URL   | Azure Portal > Static Web App > Configuration     |
| Deployment token  | Azure Portal > Static Web App > Deployment tokens |
| GitHub secrets    | GitHub Settings > Secrets and variables           |
| Custom domain     | Azure Portal > Custom domains                     |

## Support

For issues:

1. Check GitHub Actions logs for build errors
2. Verify all secrets are set in GitHub
3. Verify VITE_API_BASE_URL in Azure Configuration
4. Check backend API is running and accessible
