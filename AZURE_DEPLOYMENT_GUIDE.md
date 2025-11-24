# Azure Portal Deployment Guide: Keycloak with PostgreSQL

This guide walks you through deploying Keycloak to Azure Container Apps with Azure Database for PostgreSQL, all through the Azure Portal.

## Prerequisites
- Azure subscription
- Basic understanding of Azure services

---

## Part 1: Create Azure Database for PostgreSQL

### Step 1: Create PostgreSQL Database
1. **Navigate to Azure Portal** → Search for "PostgreSQL" → Select **"Azure Database for PostgreSQL flexible servers"**
2. Click **"Create"**
3. Fill in the **Basics** tab:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or select existing (e.g., `keycloak-rg`)
   - **Server name**: `keycloak-postgres-{your-unique-id}` (must be globally unique)
   - **Region**: Choose closest to you (e.g., `East US`)
   - **PostgreSQL version**: `15` or `16` (recommended)
   - **Workload type**: `Development` (or `Production` if needed)
   - **Compute + storage**: 
     - **Compute tier**: `Burstable` (B1ms - 1 vCore, 2GB RAM) for dev, or `General Purpose` for production
     - **Storage**: `32 GB` minimum (can scale later)
   - **Administrator account**:
     - **Admin username**: `postgres` (or your choice)
     - **Password**: Create a strong password (save this!)
   - **Authentication method**: `PostgreSQL authentication only`

4. Click **"Next: Networking"**

### Step 2: Configure Networking
1. **Network connectivity**: Select **"Public access (allowed IP addresses)"**
2. **Firewall rules**:
   - Click **"Add current client IP address"** (to allow your IP)
   - Click **"Add 0.0.0.0 - 255.255.255.255"** (temporarily for setup, restrict later)
   - Or better: Add Azure services IP ranges later
3. Click **"Next: Security"**

### Step 3: Security Settings
1. **Enforce SSL connection**: `Disabled` (for development) or `Enabled` (for production)
2. Click **"Review + create"** → **"Create"**
3. Wait for deployment (5-10 minutes)

### Step 4: Create Database
1. Once deployed, go to your PostgreSQL server
2. Navigate to **"Databases"** in left menu
3. Click **"+ Create"**
4. **Database name**: `keycloak`
5. Click **"Create"**

### Step 5: Get Connection Details
1. Note down:
   - **Server name**: `keycloak-postgres-{id}.postgres.database.azure.com`
   - **Admin username**: `postgres@keycloak-postgres-{id}`
   - **Password**: (the one you created)
   - **Database name**: `keycloak`
   - **Port**: `5432`

---

## Part 2: Deploy Keycloak to Azure Container Apps

### Step 1: Create Container Apps Environment
1. **Navigate to Azure Portal** → Search for **"Container Apps"**
2. Click **"Create"**
3. Fill in **Basics**:
   - **Subscription**: Select your subscription
   - **Resource Group**: Same as PostgreSQL (e.g., `keycloak-rg`)
   - **Container Apps Environment name**: `keycloak-env`
   - **Region**: Same as PostgreSQL
   - **Zone redundancy**: `Disabled` (for dev) or `Enabled` (for production)
   - **Log Analytics workspace**: Create new or select existing
4. Click **"Next: Monitoring"** → **"Next: Tags"** → **"Review + create"** → **"Create"**
5. Wait for deployment (2-3 minutes)

### Step 2: Create Container App for Keycloak
1. In **Container Apps**, click **"+ Create"**
2. Fill in **Basics**:
   - **Container App name**: `keycloak-app`
   - **Resource Group**: Same as before
   - **Container Apps Environment**: Select `keycloak-env` (created above)
   - **Region**: Same as before
3. Click **"Next: Container"**

### Step 3: Configure Container
1. **Container image**: `quay.io/phasetwo/phasetwo-keycloak:latest`
2. Click **"Add"** to add the container
3. Expand the container configuration:
   - **CPU**: `1.0` (minimum, increase for production)
   - **Memory**: `2.0 Gi` (minimum, increase for production)
4. Scroll down to **Environment variables** → Click **"+ Add"** and add:

   ```
   KEYCLOAK_ADMIN = admin
   KEYCLOAK_ADMIN_PASSWORD = <your-secure-password>
   KC_DB = postgres
   KC_DB_URL = jdbc:postgresql://<your-postgres-server-name>:5432/keycloak?sslmode=require
   KC_DB_USERNAME = postgres@<your-postgres-server-name>
   KC_DB_PASSWORD = <your-postgres-password>
   KC_DB_SCHEMA = public
   KC_HOSTNAME_STRICT = false
   KC_HOSTNAME_STRICT_HTTPS = false
   KC_HTTP_ENABLED = true
   KC_PROXY = edge
   KC_HEALTH_ENABLED = true
   KC_METRICS_ENABLED = true
   ```

   **Important**: Replace placeholders:
   - `<your-postgres-server-name>`: Your PostgreSQL server name (e.g., `keycloak-postgres-12345.postgres.database.azure.com`)
   - `<your-postgres-password>`: Your PostgreSQL admin password
   - `<your-secure-password>`: A secure password for Keycloak admin

5. Click **"Next: Ingress"**

### Step 4: Configure Ingress (Public Access)
1. **Ingress enabled**: `Enabled`
2. **Traffic**: `Accepting traffic from anywhere` (or restrict to specific IPs)
3. **Target port**: `8080`
4. **Transport**: `HTTP` (or `Auto` if you plan to add SSL later)
5. Click **"Next: Scaling"**

### Step 5: Configure Scaling
1. **Scale mode**: `Manual` (for dev) or `Automatic` (for production)
2. If Manual:
   - **Min replicas**: `1`
   - **Max replicas**: `1`
3. If Automatic:
   - **Min replicas**: `1`
   - **Max replicas**: `5`
   - Configure rules as needed
4. Click **"Next: Networking"** → **"Next: Tags"** → **"Review + create"** → **"Create"**
5. Wait for deployment (3-5 minutes)

---

## Part 3: Configure Networking & Security

### Step 1: Allow Container App to Access PostgreSQL
1. Go to your **PostgreSQL server**
2. Navigate to **"Networking"**
3. Under **Firewall rules**, add:
   - **Rule name**: `Allow-Azure-Services`
   - **Start IP address**: `0.0.0.0`
   - **End IP address**: `0.0.0.0`
   - Click **"Save"**
   
   **Note**: For better security, you can:
   - Use **Private Endpoint** (requires VNet integration)
   - Or restrict to specific Azure service tags

### Step 2: Get Container App URL
1. Go to your **Container App** (`keycloak-app`)
2. Copy the **Application URL** (e.g., `https://keycloak-app.xyz.azurecontainerapps.io`)
3. This is your Keycloak access URL

---

## Part 4: Access Keycloak

1. Open the **Application URL** in your browser
2. You should see the Keycloak welcome page
3. Click **"Administration Console"**
4. Login with:
   - **Username**: `admin`
   - **Password**: The password you set in `KEYCLOAK_ADMIN_PASSWORD`

---

## Part 5: Post-Deployment Configuration

### Enable HTTPS (Recommended)
1. In your **Container App**, go to **"Custom domains"**
2. Click **"+ Add"**
3. Add your custom domain
4. Configure SSL certificate (Azure-managed or your own)

### Update Environment Variables (if needed)
1. Go to **Container App** → **"Revision management"**
2. Click **"Create new revision"**
3. Update environment variables as needed
4. Deploy the new revision

### Monitor & Logs
1. **Logs**: Container App → **"Log stream"** or **"Logs"**
2. **Metrics**: Container App → **"Metrics"**
3. **Health**: Check **"Health probes"** if configured

---

## Important Notes

### Security Best Practices
1. **Change default passwords** immediately after first login
2. **Restrict PostgreSQL firewall** to specific IPs or use Private Endpoint
3. **Enable SSL** for PostgreSQL in production
4. **Use Azure Key Vault** for storing secrets (instead of environment variables)
5. **Enable HTTPS** for Keycloak
6. **Set up proper authentication** for Container Apps

### Cost Optimization
- Use **Burstable** PostgreSQL tier for development
- Use **Manual scaling** for Container Apps if traffic is predictable
- Consider **Azure Database for PostgreSQL - Flexible Server** with auto-pause for dev environments

### Troubleshooting
- **Can't connect to database**: Check PostgreSQL firewall rules
- **Container won't start**: Check logs in Container App → Log stream
- **Connection timeout**: Verify database credentials and network settings
- **Health check failures**: Ensure `KC_HEALTH_ENABLED=true` and port 8080 is correct

---

## Alternative Option: Web App for Containers (Recommended Alternative)

**Web App for Containers** is part of Azure App Service and is an excellent alternative to Container Apps. It offers:
- ✅ Built-in CI/CD integration
- ✅ Deployment slots (staging/production)
- ✅ Easy SSL certificate management
- ✅ More mature and feature-rich platform
- ✅ Better integration with other Azure services

### Deploy Keycloak using Web App for Containers

#### Step 1: Create Web App
1. **Navigate to Azure Portal** → Search for **"Web App for Containers"**
2. Click **"Create"**
3. Fill in **Basics** tab:
   - **Subscription**: Select your subscription
   - **Resource Group**: Same as PostgreSQL (e.g., `keycloak-rg`)
   - **Name**: `keycloak-app-{unique-id}` (must be globally unique, lowercase, alphanumeric)
   - **Publish**: `Container`
   - **Operating System**: `Linux`
   - **Region**: Same as PostgreSQL
   - **App Service Plan**: 
     - Click **"Create new"** or select existing
     - **Name**: `keycloak-plan`
     - **Pricing tier**: `Basic B1` (1 core, 1.75 GB RAM) for dev, or `Standard S1` for production
     - **Sku and size**: Select appropriate tier
   - **Zone redundancy**: `Disabled` (for dev)

4. Click **"Next: Deployment"**

#### Step 2: Configure Container
1. **Container type**: `Single Container`
2. **Image source**: `Docker Hub or other registries`
3. **Access type**: `Public`
4. **Image and tag**: `quay.io/phasetwo/phasetwo-keycloak:latest`
5. **Continuous Deployment**: `Enable` (optional, for auto-updates)
6. Click **"Next: Networking"**

#### Step 3: Networking (Optional)
1. Configure VNet integration if needed (for private endpoints)
2. Click **"Next: Monitoring"**

#### Step 4: Monitoring (Optional)
1. Enable Application Insights if desired
2. Click **"Next: Tags"** → **"Review + create"** → **"Create"**
3. Wait for deployment (3-5 minutes)

#### Step 5: Configure Environment Variables
1. Once deployed, go to your **Web App**
2. Navigate to **"Configuration"** in the left menu
3. Click **"+ New application setting"** and add each environment variable:

   ```
   KEYCLOAK_ADMIN = admin
   KEYCLOAK_ADMIN_PASSWORD = <your-secure-password>
   KC_DB = postgres
   KC_DB_URL = jdbc:postgresql://<your-postgres-server-name>:5432/keycloak?sslmode=require
   KC_DB_USERNAME = postgres@<your-postgres-server-name>
   KC_DB_PASSWORD = <your-postgres-password>
   KC_DB_SCHEMA = public
   KC_HOSTNAME_STRICT = false
   KC_HOSTNAME_STRICT_HTTPS = false
   KC_HTTP_ENABLED = true
   KC_PROXY = edge
   KC_HEALTH_ENABLED = true
   KC_METRICS_ENABLED = true
   ```

4. Click **"Save"** (this will restart the app)

#### Step 6: Configure Port (Important!)
1. In your **Web App**, go to **"Configuration"**
2. Click on **"General settings"** tab
3. Set **"Always On"**: `On` (prevents app from sleeping)
4. Set **"HTTP version"**: `2.0` (recommended)
5. Click **"Save"**

#### Step 7: Access Your App
1. Go to **"Overview"** in your Web App
2. Copy the **URL** (e.g., `https://keycloak-app-12345.azurewebsites.net`)
3. Access Keycloak at this URL

### Web App for Containers vs Container Apps

| Feature | Web App for Containers | Container Apps |
|---------|----------------------|----------------|
| **Maturity** | More mature, established | Newer service |
| **Deployment Slots** | ✅ Yes (staging/production) | ❌ No |
| **CI/CD Integration** | ✅ Excellent | ✅ Good |
| **SSL Certificates** | ✅ Easy management | ✅ Available |
| **Scaling** | ✅ Auto-scaling available | ✅ Auto-scaling |
| **Cost** | Pay per App Service Plan | Pay per usage |
| **Best For** | Traditional web apps, production | Microservices, serverless |

**Recommendation**: Use **Web App for Containers** if you want:
- Deployment slots for staging
- Easier SSL certificate management
- More traditional web app hosting experience
- Better integration with Azure DevOps

---

## Alternative: Using Azure Container Instances (Simpler but less scalable)

If you prefer a simpler setup without scaling features:

1. **Azure Portal** → Search **"Container Instances"**
2. Click **"Create"**
3. Fill in details similar to Container Apps
4. Add environment variables
5. Configure networking
6. Deploy

**Note**: Container Apps or Web App for Containers are recommended for better scalability and features.

---

## Summary

You now have:
- ✅ Azure Database for PostgreSQL (managed service)
- ✅ Keycloak running on Azure (choose one):
  - **Azure Container Apps** (serverless containers) - See Part 2
  - **Web App for Containers** (App Service) - See Alternative Option section
- ✅ Public access to Keycloak
- ✅ All configured through Azure Portal

**Which option to choose?**
- **Web App for Containers**: Better for traditional web apps, deployment slots, easier SSL management
- **Container Apps**: Better for microservices, serverless architecture, event-driven scaling

**Next Steps**:
- Configure your Keycloak realm
- Set up users and clients
- Configure custom domain and SSL
- Set up monitoring and alerts

