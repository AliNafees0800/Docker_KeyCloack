# Quick Fix: Keycloak Not Showing (Database Missing)

## Problem
Keycloak shows blank page or nothing - this is because **Keycloak requires a database to start**.

## Quick Fix Steps

### 1. Check Logs First
- Go to **Web App** → **"Log stream"**
- Look for database connection errors
- This confirms the issue

### 2. Create Database (If Missing)

**Option A: You already have PostgreSQL server**
1. Go to your **PostgreSQL server** in Azure Portal
2. Click **"Databases"** → **"+ Create"**
3. **Database name**: `keycloak`
4. Click **"Create"**

**Option B: You need to create PostgreSQL server**
1. Search for **"Azure Database for PostgreSQL flexible servers"**
2. Click **"Create"**
3. Fill in:
   - **Server name**: `keycloak-postgres-{unique-id}`
   - **Resource Group**: Same as your Web App
   - **Region**: Same as your Web App
   - **PostgreSQL version**: `15` or `16`
   - **Compute**: `Burstable` → `B1ms` (1 vCore, 2GB RAM)
   - **Storage**: `32 GB`
   - **Admin username**: `postgres`
   - **Password**: Create strong password (save it!)
4. **Networking**: 
   - Select **"Public access"**
   - Add firewall rule: `0.0.0.0 - 0.0.0.0` (to allow Azure services)
5. Click **"Create"** and wait 5-10 minutes
6. Then create database `keycloak` (see Option A)

### 3. Configure Environment Variables

1. Go to **Web App** → **"Configuration"** → **"Application settings"**
2. Add these variables (click **"+ New application setting"** for each):

```
KEYCLOAK_ADMIN = admin
KEYCLOAK_ADMIN_PASSWORD = YourSecurePassword123!
KC_DB = postgres
KC_DB_URL = jdbc:postgresql://YOUR-SERVER-NAME.postgres.database.azure.com:5432/keycloak?sslmode=require
KC_DB_USERNAME = postgres@YOUR-SERVER-NAME
KC_DB_PASSWORD = YOUR-POSTGRES-PASSWORD
KC_DB_SCHEMA = public
KC_HOSTNAME_STRICT = false
KC_HOSTNAME_STRICT_HTTPS = false
KC_HTTP_ENABLED = true
KC_PROXY = edge
KC_HEALTH_ENABLED = true
KC_METRICS_ENABLED = true
```

**Replace:**
- `YOUR-SERVER-NAME`: Your PostgreSQL server name (without `.postgres.database.azure.com`)
- `YOUR-POSTGRES-PASSWORD`: Your PostgreSQL admin password

3. Click **"Save"** (app will restart)

### 4. Verify PostgreSQL Firewall

1. Go to **PostgreSQL server** → **"Networking"**
2. Ensure **"Allow Azure services and resources"** = `Yes`
3. If not, enable it and **"Save"**

### 5. Check General Settings

1. Go to **Web App** → **"Configuration"** → **"General settings"**
2. Ensure **"Always On"** = `On`
3. Click **"Save"**

### 6. Wait and Test

1. Wait 2-3 minutes for restart
2. Go to your Web App URL
3. You should see Keycloak welcome page

## Still Not Working?

### Check Logs
- **Web App** → **"Log stream"** - see real-time errors
- Look for specific error messages

### Common Issues

**"Connection refused"**
- PostgreSQL firewall not allowing Azure services
- Fix: Enable "Allow Azure services" in PostgreSQL Networking

**"Authentication failed"**
- Wrong username/password
- Fix: Check username format is `postgres@servername` (not just `postgres`)

**"Database does not exist"**
- Database `keycloak` not created
- Fix: Create database in PostgreSQL server

**"Container keeps restarting"**
- Check all environment variables are set correctly
- Check logs for specific error

## Need Help?

If you're stuck, check:
1. **Log stream** - what errors do you see?
2. **PostgreSQL server** - does it exist? Is database created?
3. **Environment variables** - are they all set correctly?


