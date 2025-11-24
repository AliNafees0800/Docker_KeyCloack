# Azure Configuration Reference - Quick Copy/Paste Values

## Quick Setup Guide

This reference is for deploying Keycloak using **Web App for Containers** with **integrated PostgreSQL database** (created through the Web App wizard).

---

## Database Configuration in Wizard

When you're on the **"Database"** tab in the Web App creation wizard:

- ✅ **Check "Create a Database"**
- **Engine**: `PostgreSQL - Flexible Server`
- **Server name**: `keycloak-postgres-{your-unique-id}` (lowercase, alphanumeric, hyphens only)
- **Database name**: `keycloak`

---

## Environment Variables for Web App

After deployment, add these in **Web App → Configuration → Application settings**:

### Required Environment Variables

```
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=<choose-a-secure-password>
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://<SERVER-NAME>.postgres.database.azure.com:5432/keycloak?sslmode=require
KC_DB_USERNAME=postgres@<SERVER-NAME>
KC_DB_PASSWORD=<your-postgres-password>
KC_DB_SCHEMA=public
KC_HOSTNAME_STRICT=false
KC_HOSTNAME_STRICT_HTTPS=false
KC_HTTP_ENABLED=true
KC_PROXY=edge
KC_HEALTH_ENABLED=true
KC_METRICS_ENABLED=true
```

### Example (Replace with your actual values)

```
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=MySecureKeycloakPass123!
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://keycloak-postgres-abc123.postgres.database.azure.com:5432/keycloak?sslmode=require
KC_DB_USERNAME=postgres@keycloak-postgres-abc123
KC_DB_PASSWORD=MyPostgresPass456!
KC_DB_SCHEMA=public
KC_HOSTNAME_STRICT=false
KC_HOSTNAME_STRICT_HTTPS=false
KC_HTTP_ENABLED=true
KC_PROXY=edge
KC_HEALTH_ENABLED=true
KC_METRICS_ENABLED=true
```

---

## Important Notes

### 1. PostgreSQL Username Format
Azure requires `username@servername` format:
- If your server is `keycloak-postgres-12345`
- Username should be: `postgres@keycloak-postgres-12345`
- **Note**: Use the server name WITHOUT the `.postgres.database.azure.com` suffix

### 2. Server Name in Connection String
- **In JDBC URL**: Use full FQDN: `keycloak-postgres-12345.postgres.database.azure.com`
- **In Username**: Use short name: `postgres@keycloak-postgres-12345`

### 3. SSL Mode
Always use `?sslmode=require` in the JDBC URL for secure connections to Azure PostgreSQL.

### 4. Container Command
- The container image defaults to production mode
- If you need development mode, you can add a startup command in **Configuration → General settings → Startup Command**: `start-dev`
- For production, no command needed (uses default `start`)

### 5. Port Configuration
- Web App for Containers automatically uses port `8080`
- No manual port configuration needed
- The container exposes port 8080, and Azure App Service handles the routing

---

## Web App for Containers Settings

### Container Configuration
- **Image**: `quay.io/phasetwo/phasetwo-keycloak:latest`
- **Container type**: `Single Container`
- **Image source**: `Docker Hub or other registries`
- **Access type**: `Public`

### App Service Plan
- **Dev/Test**: `Basic B1` (1 core, 1.75 GB RAM)
- **Production**: `Standard S1` or higher (1 core, 1.75 GB RAM minimum)

### General Settings (Important!)
- **Always On**: `On` ⚠️ **CRITICAL** - Prevents app from sleeping
- **HTTP version**: `2.0` (recommended)
- **ARR affinity**: `On` (for session persistence)

### URL Format
- Your app will be accessible at: `https://your-app-name.azurewebsites.net`

---

## PostgreSQL Settings (Auto-Configured)

When you create the database through the Web App wizard:

- **Server Type**: Flexible Server (automatically selected)
- **Version**: Latest PostgreSQL version
- **Database Name**: `keycloak` (or what you specified)
- **Firewall**: Azure services access should be automatically configured
- **SSL**: Enabled by default (use `sslmode=require` in connection string)

### Getting Database Password

If you need to find or reset the PostgreSQL password:

1. Go to your **PostgreSQL server** in Azure Portal
2. Navigate to **"Settings"** → **"Connection security"** or **"Reset password"**
3. You can reset the password if needed
4. Update the `KC_DB_PASSWORD` environment variable in your Web App after resetting

---

## Troubleshooting Connection Issues

### Can't Connect to Database?

1. **Check Firewall Rules**:
   - Go to PostgreSQL server → **"Networking"** or **"Connection security"**
   - Ensure **"Allow Azure services and resources"** is set to `Yes`

2. **Verify Connection String**:
   - Check `KC_DB_URL` has correct server name (with full domain)
   - Check `KC_DB_USERNAME` has correct format (`postgres@servername`)
   - Verify `KC_DB_PASSWORD` is correct

3. **Check Logs**:
   - Go to Web App → **"Log stream"** to see real-time errors
   - Look for database connection errors

### Container Won't Start?

1. **Check Environment Variables**:
   - Ensure all required variables are set
   - Check for typos in variable names
   - Verify no extra spaces in values

2. **Check Always On**:
   - Go to **Configuration → General settings**
   - Ensure **Always On** is set to `On`

3. **View Logs**:
   - Check **"Log stream"** for startup errors
   - Review **"Diagnose and solve problems"** in Web App

---

## Security Checklist

- [ ] Changed default Keycloak admin password
- [ ] Changed PostgreSQL admin password (if using default)
- [ ] Enabled **Always On** in Web App settings
- [ ] Verified PostgreSQL firewall allows only Azure services
- [ ] Using SSL for database connection (`sslmode=require`)
- [ ] Set up custom domain with SSL certificate
- [ ] Enabled Application Insights for monitoring (optional but recommended)

---

## Cost Estimation (Approximate)

### Development/Testing
- **App Service Plan (Basic B1)**: ~$13/month
- **PostgreSQL Flexible Server (Burstable B1ms)**: ~$12/month
- **Total**: ~$25/month

### Production
- **App Service Plan (Standard S1)**: ~$70/month
- **PostgreSQL Flexible Server (General Purpose)**: ~$100+/month
- **Total**: ~$170+/month

*Prices vary by region and are subject to change. Check Azure pricing calculator for current rates.*
