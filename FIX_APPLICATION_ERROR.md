# Fix Application Error - Keycloak

## Problem
Application Error page showing - likely due to:
1. Wrong hostname in `KC_HOSTNAME_ADMIN`
2. Container startup issues
3. Database connection problems

## Step 1: Fix Hostname

Your actual Web App URL is:
`wpdev-keycloack-budxfqh6cne6ezg4.canadacentral-01.azurewebsites.net`

But `KC_HOSTNAME_ADMIN` has wrong URL.

## Step 2: Check Logs First

1. Go to **Web App** → **"Log stream"**
2. Look for errors:
   - Database connection errors
   - Container startup errors
   - Hostname errors

## Step 3: Update Environment Variables

Go to **"Configuration"** → **"Application settings"** → **"Advanced edit"**

Replace the `KC_HOSTNAME_ADMIN` value with your correct URL:

```json
{
  "name": "KC_HOSTNAME_ADMIN",
  "value": "https://wpdev-keycloack-budxfqh6cne6ezg4.canadacentral-01.azurewebsites.net",
  "slotSetting": false
}
```

## Step 4: Add Missing Variables (If Needed)

You might also need to add startup command. Check **"General settings"** → **"Startup Command"** - leave it empty for production mode, or add `start` if needed.

## Step 5: Verify Database Connection

Make sure:
1. Database `keycloak` exists on PostgreSQL server
2. PostgreSQL firewall allows Azure services
3. Credentials are correct

## Complete Fixed JSON

Here's your complete JSON with corrected hostname:

```json
[
  {
    "name": "DOCKER_REGISTRY_SERVER_PASSWORD",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "DOCKER_REGISTRY_SERVER_URL",
    "value": "https://quay.io",
    "slotSetting": false
  },
  {
    "name": "DOCKER_REGISTRY_SERVER_USERNAME",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "KC_DB",
    "value": "postgres",
    "slotSetting": false
  },
  {
    "name": "KC_DB_PASSWORD",
    "value": "Nafees012!",
    "slotSetting": false
  },
  {
    "name": "KC_DB_SCHEMA",
    "value": "public",
    "slotSetting": false
  },
  {
    "name": "KC_DB_URL",
    "value": "jdbc:postgresql://key-cloack-server.postgres.database.azure.com:5432/keycloak?sslmode=require",
    "slotSetting": false
  },
  {
    "name": "KC_DB_USERNAME",
    "value": "keycloack_user@key-cloack-server",
    "slotSetting": false
  },
  {
    "name": "KC_HEALTH_ENABLED",
    "value": "true",
    "slotSetting": false
  },
  {
    "name": "KC_HOSTNAME_ADMIN",
    "value": "https://wpdev-keycloack-budxfqh6cne6ezg4.canadacentral-01.azurewebsites.net",
    "slotSetting": false
  },
  {
    "name": "KC_HOSTNAME_STRICT",
    "value": "false",
    "slotSetting": false
  },
  {
    "name": "KC_HOSTNAME_STRICT_BACKCHANNEL",
    "value": "false",
    "slotSetting": false
  },
  {
    "name": "KC_HOSTNAME_STRICT_HTTPS",
    "value": "false",
    "slotSetting": false
  },
  {
    "name": "KC_HTTP_ENABLED",
    "value": "true",
    "slotSetting": false
  },
  {
    "name": "KC_HTTP_RELATIVE_PATH",
    "value": "/",
    "slotSetting": false
  },
  {
    "name": "KC_METRICS_ENABLED",
    "value": "true",
    "slotSetting": false
  },
  {
    "name": "KC_PROXY",
    "value": "edge",
    "slotSetting": false
  },
  {
    "name": "KEYCLOAK_ADMIN",
    "value": "admin",
    "slotSetting": false
  },
  {
    "name": "KEYCLOAK_ADMIN_PASSWORD",
    "value": "admin",
    "slotSetting": false
  },
  {
    "name": "WEBSITES_ENABLE_APP_SERVICE_STORAGE",
    "value": "false",
    "slotSetting": false
  }
]
```

## After Updating

1. Click **"OK"** in Advanced edit
2. Click **"Save"** at top
3. Wait 2-3 minutes for restart
4. Check **"Log stream"** for errors
5. Try accessing the URL again

## If Still Not Working

Check logs for specific errors:
- Database connection failures
- Container startup failures
- Port binding issues
- Hostname validation errors

