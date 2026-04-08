# Azure Static Web Apps Deployment Guide

## Prerequisites

- Azure subscription
- Azure Static Web Apps resource created in Azure portal
- GitHub account with the repository

## Quick Setup Steps

### 1. Create Azure Static Web Apps Resource

1. Go to [Azure Portal](https://portal.azure.com)
2. Click "Create a resource"
3. Search for "Static Web Apps"
4. Click "Create"
5. Fill in the details:
   - **Name**: `football-league-frontend` (or your choice)
   - **Resource Group**: Create new or select existing
   - **Region**: Select closest to your users (e.g., UK South, West Europe)
   - **Pricing**: Free tier works for development

### 2. Configure GitHub Integration

During Static Web Apps creation:
1. **Source**: GitHub
2. Sign in with your GitHub account
3. Select your repository: `larajp0812/FootballLeagueApi`
4. **Branch**: `main`
5. **Build Presets**: Select "Custom"
6. **App location**: `frontend`
7. **API location**: (leave empty)
8. **Build output location**: `dist`

### 3. configure Environment Variables

In Azure Portal > Your Static Web App > Configuration:

```
VITE_API_BASE_URL = https://your-dotnet-backend-api.com
```

Example: `https://footballleagueapi.azurewebsites.net`

### 4. GitHub Actions Workflow

Azure Static Web Apps automatically creates a GitHub Actions workflow in `.github/workflows/`.

The workflow will:
- Trigger on pushes to `main` branch
- Install dependencies
- Run `npm run build` from the `frontend` directory
- Deploy the `frontend/dist` folder to Azure

### 5. Deployment

1. Push to main branch:
```bash
git push origin main
```

2. GitHub Actions will automatically:
   - Build the frontend
   - Run tests (if configured)
   - Deploy to Azure Static Web Apps

3. Monitor deployment in:
   - GitHub: Actions tab
   - Azure: Deployment Center in Static Web App resource

## Post-Deployment

### 1. Configure Custom Domain (Optional)

In Azure Portal > Static Web App > Custom domains:
- Add your custom domain (e.g., `footballleague.com`)
- Point DNS records to Azure

### 2. Enable HTTPS (Automatic)

Azure automatically provisions SSL certificates via Let's Encrypt.

### 3. Set Production Backend URL

Update firewall/CORS rules on your backend API to allow requests from:
- `https://<static-app-name>.azurestaticapps.net`
- Your custom domain (if configured)

### 4. Monitor Performance

Use Azure Portal:
- **Application Insights** for performance monitoring
- **Azure Monitor** for uptime and availability

## Environment-Specific URLs

After deployment, your app will be available at:
- `https://<static-app-name>.azurestaticapps.net` (auto-generated)
- Your custom domain (if configured)

## Troubleshooting

### Build Fails

Check GitHub Actions logs:
1. Go to repository > Actions
2. Click latest workflow run
3. Look for build errors

Common issues:
- Missing dependencies: Run `npm install` locally and commit `package-lock.json`
- Node version mismatch: Specify Node version in workflow
- Environment variables: Ensure `VITE_API_BASE_URL` is set

### API Calls Fail After Deployment

1. Verify backend is running and accessible
2. Check CORS settings on backend
3. Verify `VITE_API_BASE_URL` environment variable is correct
4. Check browser console for detailed error messages

### "Not Found" After Navigation

Static Web Apps automatically handles SPA routing. If 404 errors occur:
- Verify `staticwebapp.config.json` exists and is correct
- Rules should rewrite all routes to `index.html`

## Scaling & Performance

- **CDN**: Azure Static Web Apps includes global CDN
- **Caching**: Static assets are cached at edge locations
- **Free tier includes**: 100 GB bandwidth per month

## Costs

- **Free tier**: Perfect for development and testing
- **Standard tier**: For production (pay per request + bandwidth)

Pricing: https://azure.microsoft.com/en-us/pricing/details/app-service/static/

## Continuous Deployment

Once GitHub workflow is set up:
- Any push to `main` triggers automatic build and deploy
- Pull requests get preview deployments (if enabled)
- Rollback to previous versions from Azure Portal

## Security

- HTTPS enabled by default
- DDoS protection included
- Azure Defender available for advanced threat protection

## Additional Resources

- [Azure Static Web Apps Docs](https://learn.microsoft.com/en-us/azure/static-web-apps/)
- [GitHub Actions Starter Workflows](https://github.com/Azure/static-web-apps-deploy)
- [SPA Routing Configuration](https://learn.microsoft.com/en-us/azure/static-web-apps/configuration)
