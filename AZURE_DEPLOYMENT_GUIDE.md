# Azure Portal Deployment Guide: Keycloak with Web App for Containers

This guide walks you through deploying Keycloak to Azure Web App for Containers with integrated PostgreSQL database, all through the Azure Portal.

## Prerequisites
- Azure subscription
- Basic understanding of Azure services

---

## Step-by-Step Deployment Guide

### Step 1: Create Web App - Basics Tab

1. **Navigate to Azure Portal** → Search for **"Web App for Containers"**
2. Click **"Create"**
3. Fill in **Basics** tab:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new (e.g., `keycloak-rg`) or select existing
   - **Name**: `keycloak-app-{unique-id}` (must be globally unique, lowercase, alphanumeric only)
   - **Publish**: `Container`
   - **Operating System**: `Linux`
   - **Region**: Choose closest to you (e.g., `East US`)
   - **App Service Plan**: 
     - Click **"Create new"**
     - **Name**: `keycloak-plan`
     - **Pricing tier**: `Basic B1` (1 core, 1.75 GB RAM) for dev, or `Standard S1` for production
     - **Sku and size**: Select appropriate tier
   - **Zone redundancy**: `Disabled` (for dev)

4. Click **"Next: Database"**

---

### Step 2: Configure Database (Integrated PostgreSQL)

1. **Create a Database**: ✅ **Check this box** (this is what you're doing!)

2. **Database** section (will be enabled once you check the box):
   - **Engine***: Select **"PostgreSQL - Flexible Server"**
   - **Server name***: Enter a unique name (e.g., `keycloak-postgres-{your-id}`)
     - Must be globally unique
     - Lowercase, alphanumeric, and hyphens only
   - **Database name***: Enter `keycloak` (or your preferred name)

3. **Cache** section:
   - **Create an Azure Cache for Redis**: Leave **unchecked** (not needed for Keycloak)

4. Click **"Next: Deployment"**

**Note**: Azure will automatically create a VNet and networking resources for the database. This is normal and expected.

---

### Step 3: Configure Container

You're now on the **"Container"** tab. Here's what to configure:

1. **Sidecar support**: Leave **OFF** (toggle should be grey/off position)
   - This is for advanced scenarios, not needed for Keycloak

2. **Image Source***: Select **"Other container registries"** ⚠️ **IMPORTANT**
   - By default, "Quickstart" is selected (showing NGINX sample)
   - You MUST change this to **"Other container registries"** to use your Keycloak image

3. After selecting "Other container registries", you'll see **"Docker hub options"** section. Configure these fields:

   - **Access Type**: Keep **"Public"** selected ✅
   
   - **Registry server URL***: Change from `https://index.docker.io` to:
     ```
     https://quay.io
     ```
     ⚠️ **IMPORTANT**: The Keycloak image is on Quay.io, not Docker Hub, so you must change this URL!
   
   - **Image and tag***: Enter:
     ```
     phasetwo/phasetwo-keycloak:latest
     ```
     (Note: Don't include `quay.io/` here, just the image path)
   
   - **Startup Command**: Leave **empty** (for production mode)
     - If you need development mode, you can add: `start-dev`
     - But for production, leave it empty to use the default `start` command

4. Click **"Next: Networking"**

---

### Step 4: Networking (Optional)

1. Configure VNet integration if needed (for private endpoints)
2. For most cases, you can leave defaults
3. Click **"Next: Monitor + secure"**

---

### Step 5: Monitoring (Optional)

1. **Application Insights**: 
   - Enable if you want monitoring (recommended for production)
   - Or leave disabled for now
2. Click **"Next: Tags"**

---

### Step 6: Tags (Optional)

1. Add tags if needed for organization
2. Click **"Review + create"**

---

### Step 7: Review and Create

1. Review all your settings
2. Click **"Create"**
3. Wait for deployment (5-10 minutes)
   - This will create:
     - Web App
     - App Service Plan
     - PostgreSQL Flexible Server
     - Database
     - VNet and networking resources

---

### Step 8: Get Database Connection Details

After deployment completes:

1. Go to your **Web App** (the one you just created)
2. Navigate to **"Configuration"** in the left menu
3. Look for connection strings or go to **"Overview"** → Click on the **PostgreSQL server** link
4. Or navigate directly to your **PostgreSQL server**:
   - Go to **"All resources"** in Azure Portal
   - Find your PostgreSQL server (name you entered in Step 2)
   - Go to **"Connection strings"** or **"Settings"** → **"Connection security"**

**Note down these details:**
- **Server name**: `your-server-name.postgres.database.azure.com`
- **Database name**: `keycloak` (or what you entered)
- **Username**: `postgres@your-server-name` (Azure format)
- **Password**: You'll need to check the PostgreSQL server settings or reset it

---

### Step 9: Configure Environment Variables

1. In your **Web App**, go to **"Configuration"** in the left menu
2. Click on **"Application settings"** tab
3. Click **"+ New application setting"** and add each of these:

   ```
   KEYCLOAK_ADMIN = admin
   KEYCLOAK_ADMIN_PASSWORD = <choose-a-secure-password>
   KC_DB = postgres
   KC_DB_URL = jdbc:postgresql://<your-server-name>.postgres.database.azure.com:5432/keycloak?sslmode=require
   KC_DB_USERNAME = postgres@<your-server-name>
   KC_DB_PASSWORD = <your-postgres-password>
   KC_DB_SCHEMA = public
   KC_HOSTNAME_STRICT = false
   KC_HOSTNAME_STRICT_HTTPS = false
   KC_HTTP_ENABLED = true
   KC_PROXY = edge
   KC_HEALTH_ENABLED = true
   KC_METRICS_ENABLED = true
   ```

   **Replace these placeholders:**
   - `<your-server-name>`: The server name you entered (without `.postgres.database.azure.com`)
   - `<your-postgres-password>`: The PostgreSQL admin password
   - `<choose-a-secure-password>`: A secure password for Keycloak admin

4. Click **"Save"** (this will restart your app)

---

### Step 10: Configure General Settings

1. Still in **"Configuration"**, click on **"General settings"** tab
2. Set these important settings:
   - **Always On**: `On` ⚠️ **IMPORTANT** - Prevents app from sleeping
   - **HTTP version**: `2.0` (recommended)
   - **ARR affinity**: `On` (recommended for session persistence)
3. Click **"Save"**

---

### Step 11: Configure PostgreSQL Firewall (If Needed)

If your Web App can't connect to the database:

1. Go to your **PostgreSQL server** (from All resources)
2. Navigate to **"Networking"** or **"Connection security"**
3. Under **Firewall rules**, ensure:
   - **Allow Azure services and resources to access this server**: `Yes` ✅
   - Or add a specific rule for Azure services
4. Click **"Save"**

**Note**: Since you created the database through the Web App wizard, Azure should have configured this automatically, but verify if you have connection issues.

---

### Step 12: Access Keycloak

1. Go to your **Web App** → **"Overview"**
2. Copy the **URL** (e.g., `https://keycloak-app-12345.azurewebsites.net`)
3. Open this URL in your browser
4. You should see the Keycloak welcome page
5. Click **"Administration Console"**
6. Login with:
   - **Username**: `admin`
   - **Password**: The password you set in `KEYCLOAK_ADMIN_PASSWORD`

---

## Post-Deployment Configuration

### Enable HTTPS with Custom Domain (Recommended)

1. In your **Web App**, go to **"Custom domains"**
2. Click **"+ Add custom domain"**
3. Add your domain name
4. Configure SSL certificate:
   - Use **App Service Managed Certificate** (free, automatic)
   - Or upload your own certificate
5. Follow the DNS configuration steps

### View Logs

1. Go to **"Log stream"** in your Web App for real-time logs
2. Or go to **"Logs"** → **"App Service logs"** for historical logs
3. Enable **"Application Logging"** if needed

### Monitor Performance

1. Go to **"Metrics"** to see performance data
2. Set up **Alerts** if needed
3. Use **Application Insights** if you enabled it

---

## Important Notes

### Security Best Practices

1. **Change default passwords** immediately after first login
2. **Restrict PostgreSQL firewall** to Azure services only (already done if configured correctly)
3. **Enable SSL** for PostgreSQL (already enabled with `sslmode=require`)
4. **Use Azure Key Vault** for storing secrets (advanced - can migrate later)
5. **Enable HTTPS** for Keycloak (use custom domain with SSL)
6. **Set strong passwords** for both Keycloak admin and PostgreSQL

### Troubleshooting

- **Can't connect to database**: 
  - Check PostgreSQL firewall rules (Step 11)
  - Verify connection string format in environment variables
  - Check PostgreSQL server is running

- **Container won't start**: 
  - Check **"Log stream"** for errors
  - Verify all environment variables are set correctly
  - Check **"Always On"** is enabled

- **Connection timeout**: 
  - Verify database credentials
  - Check network connectivity
  - Ensure PostgreSQL server is accessible

- **Health check failures**: 
  - Ensure `KC_HEALTH_ENABLED=true`
  - Check port 8080 is correct
  - Verify container is running

### Cost Optimization

- Use **Basic B1** tier for development/testing
- Use **Standard S1** or higher for production
- Consider **scaling down** during non-business hours
- Monitor usage in **Cost Management + Billing**

---

## Summary

You now have:
- ✅ Azure Web App for Containers running Keycloak
- ✅ Azure Database for PostgreSQL (Flexible Server) - created automatically
- ✅ Database configured and connected
- ✅ Public access to Keycloak
- ✅ All configured through Azure Portal

**Next Steps**:
- Configure your Keycloak realm
- Set up users and clients
- Configure custom domain and SSL certificate
- Set up monitoring and alerts
- Change default admin password

---

## Quick Reference: Environment Variables

When setting up in Step 9, use this format:

```
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=YourSecurePassword123!
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://your-server-name.postgres.database.azure.com:5432/keycloak?sslmode=require
KC_DB_USERNAME=postgres@your-server-name
KC_DB_PASSWORD=YourPostgresPassword456!
KC_DB_SCHEMA=public
KC_HOSTNAME_STRICT=false
KC_HOSTNAME_STRICT_HTTPS=false
KC_HTTP_ENABLED=true
KC_PROXY=edge
KC_HEALTH_ENABLED=true
KC_METRICS_ENABLED=true
```

**Remember**: Replace `your-server-name` with the actual server name you entered in the Database tab!
