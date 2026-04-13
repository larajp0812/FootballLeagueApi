# Frontend Deployment Guide - Azure Static Web Apps

A simple step-by-step guide to deploy your React frontend to Azure Static Web Apps.

## What You'll Need

- Azure account (free at https://azure.microsoft.com/free)
- GitHub repository (already have this)
- About 10 minutes

## Step 1: Create Azure Static Web App

1. Go to **[Azure Portal](https://portal.azure.com)**

2. Click **Create a resource** (or search for "Static Web Apps")

3. Click **Create** on Static Web Apps

4. Fill out the form:
   - **Subscription**: Your Azure subscription
   - **Resource Group**: Create new → Name it `football-league` (or your choice)
   - **Name**: `football-league-frontend` (can be any unique name)
   - **Hosting plan**: Free
   - **Region**: Pick closest to you (e.g., East US)

5. Click **Sign in with GitHub** and authorize Azure to access your account

6. Fill in GitHub details:
   - **Organization**: `larajp0812`
   - **Repository**: `FootballLeagueApi`
   - **Branch**: `main`

7. Click **Next: Build** and fill in build details:
   - **Build Presets**: React
   - **App location**: `frontend`
   - **Api location**: Leave blank
   - **Output location**: `dist`

8. Click **Review + create** → **Create**

Wait 5-10 minutes for Azure to finish creating your app. You'll see a green checkmark when done.

## Step 2: Get Your Deployment Token

1. After creation, go to your Static Web App resource

2. In the left sidebar, click **Deployment token**

3. Click **Copy** to copy the token (this is your secret key for deployments)

Keep this token safe! You'll use it next.

## Step 3: Add Deployment Token to GitHub

1. Go to **[GitHub](https://github.com/larajp0812/FootballLeagueApi)**

2. Click **Settings** → **Secrets and variables** → **Actions**

3. Click **New repository secret**

4. Fill in:
   - **Name**: `AZURE_STATIC_WEB_APPS_API_TOKEN`
   - **Value**: Paste the token you copied in Step 2

5. Click **Add secret**

## Step 4: Update Frontend Environment Variable

Your frontend needs to know where the backend API is located.

In the Azure Portal > Your Static Web App:

1. Click **Configuration** in the left sidebar

2. Click **Environment variables**

3. Click **Create new environment variable**

4. Fill in:
   - **Name**: `VITE_API_BASE_URL`
   - **Value**: `https://football-league-backend.azurewebsites.net` (your backend URL)
   - **Environment**: Production

5. Click **Add**

## Step 5: Push Code to GitHub

The GitHub Action will automatically deploy when you push to main:

```bash
git push origin main
```

## Step 6: Wait for Deployment

1. Go to GitHub repository → **Actions** tab

2. You'll see a workflow running (frontend CI/CD)

3. Wait for it to turn green ✓ (about 2-5 minutes)

4. Once complete, click the workflow to see deployment details

## Step 7: View Your Deployed App

Your new app URL appears in multiple places:

**Option A - Azure Portal**:
1. Go to your Static Web App resource
2. Click **Overview**
3. Click the **URL** link

**Option B - GitHub Actions**:
1. Go to the completed workflow in GitHub Actions
2. Look for "deployment successful" message with the URL

**Your app will be at**: `https://<your-app-name>.azurestaticapps.net`

## Verify It Works

1. Open your app URL in browser
2. Check if the page loads
3. Try clicking around - does it work?

If it doesn't work, check:
- Backend is deployed and running
- `VITE_API_BASE_URL` is set correctly in Azure Configuration
- No errors in browser console (F12)

## Update Your App

From now on, every time you push to GitHub main branch:

```bash
git add .
git commit -m "your message"
git push origin main
```

The deployment happens automatically! Just wait 2-5 minutes for it to complete.

## Troubleshooting

### App loads but shows blank page
- Check browser console: F12 → Console tab → look for errors
- Verify backend is running
- Check `VITE_API_BASE_URL` environment variable is correct

### Frontend loads but API calls fail
- Backend might not be deployed
- Check CORS is configured in backend (should allow `*.azurestaticapps.net`)
- Verify backend URL is correct

### Deployment shows "failed" in GitHub Actions
- Click the failed job to see error messages
- Common issues:
  - Build output location not `dist`
  - React build settings wrong
  - Node version mismatch

### Want to check deployment status
- GitHub: **Actions** tab → click latest workflow
- Azure: **Deployments** tab in Static Web App resource

## Quick Reference

| What | Where |
|------|-------|
| Your app URL | Azure Portal > Overview > URL |
| Deployment token | Azure Portal > Deployment token |
| GitHub secrets | GitHub > Settings > Secrets and variables > Actions |
| Environment variables | Azure Portal > Configuration > Environment variables |
| Deployment history | GitHub > Actions tab |
| Build logs | GitHub > Actions > click workflow |

## Need Help?

1. Check GitHub Actions logs: GitHub > Actions > Your workflow > Click failed job
2. Check Azure deployment: Azure Portal > Your Static Web App > Deployments
3. Check browser console: F12 > Console tab for frontend errors
