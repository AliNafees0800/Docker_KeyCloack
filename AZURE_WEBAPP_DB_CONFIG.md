# Configure Azure Web App with docker-compose.yml Database Settings

## Your docker-compose.yml Settings

From your docker-compose.yml:
- **Database**: `postgres`
- **Database name**: `keycloak`
- **Username**: `postgres`
- **Password**: `123`
- **Schema**: `public`
- **Admin**: `admin` / `admin`

## Step 1: Get Your Azure PostgreSQL Server Details

You need:
1. **PostgreSQL server name** (e.g., `your-server-name.postgres.database.azure.com`)
2. **Database name**: `keycloak` (create this if it doesn't exist)
3. **Username format**: `postgres@your-server-name` (Azure format)
4. **Password**: Use `123` (same as docker-compose) or set a new one

## Step 2: Configure Environment Variables in Azure Web App

1. Go to your **Web App** (`wpdev-keycloack`) in Azure Portal
2. Navigate to **"Configuration"** → **"Application settings"**
3. Add/Update these environment variables:

### Database Configuration

```
KEYCLOAK_ADMIN = admin
KEYCLOAK_ADMIN_PASSWORD = admin
KC_DB = postgres
KC_DB_URL = jdbc:postgresql://YOUR-SERVER-NAME.postgres.database.azure.com:5432/keycloak?sslmode=require
KC_DB_USERNAME = postgres@YOUR-SERVER-NAME
KC_DB_PASSWORD = 123
KC_DB_SCHEMA = public
```

### Hostname & Proxy Settings (Same as docker-compose)

```
KC_HOSTNAME_STRICT = false
KC_HOSTNAME_STRICT_HTTPS = false
KC_HTTP_ENABLED = true
KC_PROXY = edge
```

### Health & Metrics (Same as docker-compose)

```
KC_HEALTH_ENABLED = true
KC_METRICS_ENABLED = true
```

## Step 3: Replace Placeholders

Replace `YOUR-SERVER-NAME` with your actual PostgreSQL server name:

**Example:**
- If your server is: `keycloak-postgres-12345.postgres.database.azure.com`
- Then:
  - `KC_DB_URL` = `jdbc:postgresql://keycloak-postgres-12345.postgres.database.azure.com:5432/keycloak?sslmode=require`
  - `KC_DB_USERNAME` = `postgres@keycloak-postgres-12345`

## Step 4: Startup Command (Optional)

If you want development mode (like docker-compose uses `start-dev`):

1. Go to **"Configuration"** → **"General settings"**
2. **Startup Command**: `start-dev`
3. Or leave empty for production mode (`start`)

## Step 5: Save and Restart

1. Click **"Save"** (app will restart automatically)
2. Wait 2-3 minutes for restart
3. Check your Web App URL

## Important Notes

### Differences from docker-compose.yml

1. **Database URL**: 
   - docker-compose: `host.docker.internal:5432`
   - Azure: `your-server-name.postgres.database.azure.com:5432`
   - Must add `?sslmode=require` for Azure

2. **Username format**:
   - docker-compose: `postgres`
   - Azure: `postgres@your-server-name` (Azure requires this format)

3. **Password**:
   - You can use the same password (`123`) or set a different one
   - Make sure PostgreSQL server uses the same password

4. **Port**:
   - docker-compose exposes `8081:8080`
   - Azure Web App automatically uses port `8080` (no mapping needed)

## Quick Copy-Paste (Replace YOUR-SERVER-NAME)

```
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://YOUR-SERVER-NAME.postgres.database.azure.com:5432/keycloak?sslmode=require
KC_DB_USERNAME=postgres@YOUR-SERVER-NAME
KC_DB_PASSWORD=123
KC_DB_SCHEMA=public
KC_HOSTNAME_STRICT=false
KC_HOSTNAME_STRICT_HTTPS=false
KC_HTTP_ENABLED=true
KC_PROXY=edge
KC_HEALTH_ENABLED=true
KC_METRICS_ENABLED=true
```

## Verify Database Connection

1. Make sure database `keycloak` exists in your PostgreSQL server
2. Make sure PostgreSQL firewall allows Azure services
3. Check **"Log stream"** in Web App for connection errors

## Security Note

⚠️ **For production**, change:
- `KEYCLOAK_ADMIN_PASSWORD` from `admin` to a strong password
- `KC_DB_PASSWORD` from `123` to a strong password


