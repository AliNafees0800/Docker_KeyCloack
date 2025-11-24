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

**If you see a "Dangerous site" warning in Chrome:**

This is a Chrome security warning, not a Keycloak error. Here's how to handle it:

**Option A: Bypass the warning to see what Keycloak shows**
1. Click **"Details"** or **"Advanced"** on the warning page
2. Click **"Proceed to [your-site] (unsafe)"** or **"Visit this site"**
3. This will let you see what Keycloak is actually displaying

**Option B: Use a different browser temporarily**
- Try Microsoft Edge or Firefox
- They may not show the same warning

**Option C: Fix the security warning (Recommended)**
- The warning appears because Chrome's Safe Browsing flagged the site
- This can happen with new Azure Web Apps before they're fully indexed
- After configuring the database and the site is working, the warning usually goes away
- You can also add a custom domain with proper SSL certificate (see Post-Deployment section)

**What you should see:**

**Without database configured:**
- Keycloak container should start and show an error page
- Error might say: "Database connection failed" or "Unable to connect to database"
- This confirms the container is running, but needs database connection

**With database configured:**
- Keycloak welcome page with "Administration Console" link
- You can login and use Keycloak normally

4. If you see the Keycloak welcome page, click **"Administration Console"**
5. Login with:
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

- **Chrome shows "Dangerous site" warning**:
  - This is a Chrome Safe Browsing warning, not a Keycloak error
  - **Quick fix**: Click "Details" → "Proceed to site (unsafe)" to see the actual page
  - **Why it happens**: New Azure Web Apps sometimes get flagged before they're indexed
  - **Permanent fix**: The warning usually disappears after a few days, or add a custom domain with SSL
  - **Alternative**: Use Microsoft Edge or Firefox temporarily
  - **Note**: The site is safe - it's your own Azure deployment

- **Can't connect to database**: 
  - Check PostgreSQL firewall rules (Step 11)
  - Verify connection string format in environment variables
  - Check PostgreSQL server is running
  - **Without database**: Keycloak will show an error page saying it can't connect to the database

- **Container won't start**: 
  - Check **"Log stream"** for errors
  - Verify all environment variables are set correctly
  - Check **"Always On"** is enabled
  - Look for container startup errors in logs

- **Connection timeout**: 
  - Verify database credentials
  - Check network connectivity
  - Ensure PostgreSQL server is accessible

- **Health check failures**: 
  - Ensure `KC_HEALTH_ENABLED=true`
  - Check port 8080 is correct
  - Verify container is running

- **Site shows error page instead of Keycloak**:
  - **Without database**: This is expected - Keycloak needs database connection
  - Check logs to see the specific error
  - Verify environment variables are set correctly
  - Ensure database is created and accessible

### Cost Optimization

- Use **Basic B1** tier for development/testing
- Use **Standard S1** or higher for production
- Consider **scaling down** during non-business hours
- Monitor usage in **Cost Management + Billing**

---

## Troubleshooting: Deployment Errors

### Error: "Could not find member 'tags' on object of type 'Subnet'"

**Problem**: This error occurs when Azure tries to create the database through the Web App wizard. It's a known bug in the Azure portal related to subnet tag handling in the deployment template.

**Why it happens**: When you select "Create a Database" in the wizard, Azure automatically creates a VNet and subnet. The deployment template tries to apply tags to the subnet, but subnets don't support tags in the way the template expects, causing the deployment to fail.

**Solutions to try** (in order):

#### Solution 1: Create Resource Group Without Tags
1. **Create a new Resource Group first** (don't create it during Web App creation)
   - Go to **Resource Groups** → **Create**
   - **Name**: `keycloak-rg`
   - **Region**: Your preferred region
   - **Important**: Don't add any tags to the resource group
2. **Then create Web App** using this existing resource group
   - In the Basics tab, select the existing resource group (don't create new)
   - Continue with database creation as normal

#### Solution 2: Try Different Region
Sometimes certain regions have updated templates that fix this bug:
1. Try a different Azure region (e.g., if you used East US, try West US 2)
2. Some regions may have newer deployment templates

#### Solution 3: Wait and Retry
1. Wait 15-30 minutes
2. Try the deployment again
3. Azure sometimes updates templates, and the issue may resolve

#### Solution 4: Use Azure CLI or PowerShell (Bypass Portal Bug)
If you're comfortable with command line, you can use Azure CLI to deploy, which uses different templates:

```bash
# This bypasses the portal bug entirely
az webapp create --resource-group keycloak-rg --plan keycloak-plan --name keycloak-app --deployment-container-image-name quay.io/phasetwo/phasetwo-keycloak:latest
```

#### Solution 5: Report to Azure Support
If none of the above work:
1. This is a confirmed bug in Azure Portal
2. Report it through Azure Support or Azure Feedback
3. Use the workaround (create database separately) in the meantime

**Recommended Workaround**: Create the database separately first, then create the Web App without the integrated database option. This is the most reliable approach and avoids the bug entirely.

#### Alternative Approach: Create Database Separately

**Step 1: Create PostgreSQL Database First**

1. **Navigate to Azure Portal** → Search for **"Azure Database for PostgreSQL flexible servers"**
2. Click **"Create"**
3. Fill in **Basics** tab:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new (e.g., `keycloak-rg`) or use existing
   - **Server name**: `keycloak-postgres-{your-unique-id}` (globally unique)
   - **Region**: Choose your region
   - **PostgreSQL version**: `15` or `16`
   - **Workload type**: `Development`
   - **Compute + storage**: 
     - **Compute tier**: `Burstable` → `B1ms` (1 vCore, 2GB RAM)
     - **Storage**: `32 GB`
   - **Administrator account**:
     - **Admin username**: `postgres`
     - **Password**: Create a strong password (save this!)
   - **Authentication method**: `PostgreSQL authentication only`

4. Click **"Next: Networking"**
   - **Network connectivity**: `Public access (allowed IP addresses)`
   - **Firewall rules**: 
     - Click **"Add current client IP address"**
     - Click **"Add 0.0.0.0 - 0.0.0.0"** (to allow Azure services - you can restrict later)
   - Click **"Save"**

5. Click **"Next: Security"** → **"Review + create"** → **"Create"**
6. Wait for deployment (5-10 minutes)

7. **Create the database**:
   - Go to your PostgreSQL server
   - Navigate to **"Databases"** → Click **"+ Create"**
   - **Database name**: `keycloak`
   - Click **"Create"**

**Step 2: Create Web App (Without Integrated Database)**

1. **Navigate to Azure Portal** → Search for **"Web App for Containers"**
2. Click **"Create"**
3. Follow **Step 1** from the main guide (Basics tab)
4. On **"Database"** tab: **DO NOT check "Create a Database"** ❌
   - Leave it unchecked
   - Click **"Next: Deployment"** (or **"Next: Container"**)
5. Follow **Step 3** (Container configuration) from the main guide
6. Continue with the rest of the steps
7. After deployment, configure environment variables (Step 9) using the database you created separately

**Note**: This approach avoids the subnet tags bug and gives you more control over the database configuration.

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
