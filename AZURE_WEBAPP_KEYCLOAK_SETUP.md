# Azure Web App Setup with Keycloak Container

## Prerequisites

- Azure PostgreSQL server created (refer to `AZURE_POSTGRESQL_SETUP.md`)
- PostgreSQL database named `keycloak` created
- Access to Azure Portal

---

## Step 1: Create Web App Resource

1. In Azure dashboard, click on **Create a resource**
2. From the list of resources, click on **Web App**
3. This will navigate you to the **Create Web App** page

---

## Step 2: Project Details

- Select **Subscription**
- Select **existing resource group** or add a **new one**

---

## Step 3: Instance Details

### Basic Configuration
- Add **Web app name**
- ✅ Keep checked: **Secure unique default hostname on** checkbox
- In **Publish**, select **Container**
- In **Operating System**, select **Linux**
- Select **Region** as required

---

## Step 4: Pricing Plans

- In **Pricing plans**, select **B1** (1 CPU and 1.75GB RAM VM)

---

## Step 5: Zone Redundancy

- In **Zone redundancy**, keep **disabled** for development environments

---

## Step 6: Database Configuration

- In the **Database** tab, **do NOT** select **Create a Database**
- Leave this section as is (we'll connect to the existing PostgreSQL database)

---

## Step 7: Container Configuration

### Container Settings
- In **Container**, keep **Sidecar support** ✅ checked

### Image Source
- In **Image Source**, keep **Other container registries** selected

### Container Registry Details
- In **Name**, maybe keep **main**
- In **Docker hub options**, select **Access Type** as **Public**
- In **Registry server URL**, add: `https://quay.io`
- In **Image and tag**, select: `phasetwo/phasetwo-keycloak:latest`
- Keep the **Port** as **80**
- In **Startup Command**, add: `start-dev`

---

## Step 8: Networking

- In **Networking**, keep **Enable public access** ✅ **on**

---

## Step 9: Monitor + Secure

- Keep **disabled**:
  - ✅ **Enable Application Insights**
  - ✅ **Enable Defender for App Service**

---

## Step 10: Tags and Review

- Add appropriate **tags**
- Click **Review and create**
- Review all settings
- Click **Create**

---

## Step 11: Configure Environment Variables

### Access Environment Variables

Once the web app is created:
1. **Access the resource** in Azure Portal
2. Scroll from the left side menu to **Settings > Environment Variables**

### Add Environment Variables

Add the following environment variables:

| Variable Name | Value | Description |
|--------------|-------|-------------|
| `DOCKER_REGISTRY_SERVER_URL` | `https://quay.io` | Docker registry URL |
| `DOCKER_REGISTRY_SERVER_USERNAME` | `` (empty) | Docker registry username (leave empty for public) |
| `DOCKER_REGISTRY_SERVER_PASSWORD` | `` (empty) | Docker registry password (leave empty for public) |
| `KC_DB` | `postgres` | Database type |
| `KC_DB_URL` | `jdbc:postgresql://pgsql-keycloak.postgres.database.azure.com:5432/keycloak` | Database connection URL |
| `KC_DB_URL_PROPERTIES` | `sslmode=require` | Database SSL mode |
| `KC_DB_USERNAME` | `keycloack_user` | Database username |
| `KC_DB_PASSWORD` | `Nafees012!` | Database password |
| `KC_DB_SCHEMA` | `public` | Database schema |
| `KEYCLOAK_ADMIN` | `admin` | Keycloak admin username |
| `KEYCLOAK_ADMIN_PASSWORD` | `admin` | Keycloak admin password |
| `KC_LOG_LEVEL` | `DEBUG` | Logging level |
| `KC_PROXY` | `edge` | Proxy configuration |
| `KC_HOSTNAME` | `https://wpdev-key-cloak-cbfxdedmbbc4dcac.canadacentral-01.azurewebsites.net` | Keycloak hostname |
| `KC_HOSTNAME_ADMIN` | `https://wpdev-key-cloak-cbfxdedmbbc4dcac.canadacentral-01.azurewebsites.net` | Keycloak admin hostname |
| `KC_HOSTNAME_STRICT` | `false` | Hostname strict mode |
| `KC_HOSTNAME_STRICT_BACKCHANNEL` | `false` | Backchannel hostname strict mode |
| `KC_HTTP_ENABLED` | `true` | Enable HTTP |
| `KC_HTTP_RELATIVE_PATH` | `/` | HTTP relative path |
| `KC_HEALTH_ENABLED` | `true` | Enable health checks |
| `KC_METRICS_ENABLED` | `true` | Enable metrics |
| `WEBSITES_ENABLE_APP_SERVICE_STORAGE` | `false` | Disable app service storage |

### Environment Variables JSON Format

If you need to add these programmatically, here's the JSON format:

```json
[
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
    "name": "DOCKER_REGISTRY_SERVER_PASSWORD",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "KC_DB",
    "value": "postgres",
    "slotSetting": false
  },
  {
    "name": "KC_DB_URL",
    "value": "jdbc:postgresql://pgsql-keycloak.postgres.database.azure.com:5432/keycloak",
    "slotSetting": false
  },
  {
    "name": "KC_DB_URL_PROPERTIES",
    "value": "sslmode=require",
    "slotSetting": false
  },
  {
    "name": "KC_DB_USERNAME",
    "value": "keycloack_user",
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
    "name": "KC_LOG_LEVEL",
    "value": "DEBUG",
    "slotSetting": false
  },
  {
    "name": "KC_PROXY",
    "value": "edge",
    "slotSetting": false
  },
  {
    "name": "KC_HOSTNAME",
    "value": "https://wpdev-key-cloak-cbfxdedmbbc4dcac.canadacentral-01.azurewebsites.net",
    "slotSetting": false
  },
  {
    "name": "KC_HOSTNAME_ADMIN",
    "value": "https://wpdev-key-cloak-cbfxdedmbbc4dcac.canadacentral-01.azurewebsites.net",
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
    "name": "KC_HEALTH_ENABLED",
    "value": "true",
    "slotSetting": false
  },
  {
    "name": "KC_METRICS_ENABLED",
    "value": "true",
    "slotSetting": false
  },
  {
    "name": "WEBSITES_ENABLE_APP_SERVICE_STORAGE",
    "value": "false",
    "slotSetting": false
  }
]
```

---

## Important Notes

### Database Connection Details

- **`pgsql-keycloak.postgres.database.azure.com`** in `KC_DB_URL` is the **database host name**
- **`5432`** is the **port**
- **`keycloak`** (after the port) is the **database name**

### Hostname Configuration

- **`KC_HOSTNAME`** and **`KC_HOSTNAME_ADMIN`** will be the **domain for accessing Keycloak and logging in**
- Update these values with your actual web app URL after deployment
- The example URL format: `https://wpdev-key-cloak-cbfxdedmbbc4dcac.canadacentral-01.azurewebsites.net`

### Other Configuration

- Rest of the variables are self-explanatory
- Make sure to update database credentials (`KC_DB_USERNAME`, `KC_DB_PASSWORD`) to match your PostgreSQL setup
- Update Keycloak admin credentials (`KEYCLOAK_ADMIN`, `KEYCLOAK_ADMIN_PASSWORD`) as needed

---

## Step 12: Production Setup (Next Steps)

### Custom Domain and Certificate

For production environments, you'll need to:

1. **Add your own domain** to the web app
2. **Configure SSL certificate** for secure access
3. Azure will provide **DNS information** which you can add in **GoDaddy** (or your domain registrar)
4. That's all - once DNS is configured, your custom domain will be active

### Steps to Add Custom Domain

1. Navigate to your Web App resource
2. Go to **Settings > Custom domains**
3. Click **Add custom domain**
4. Enter your domain name
5. Follow the DNS configuration instructions
6. Add the provided DNS records in GoDaddy
7. Configure SSL/TLS certificate (Azure can provide managed certificates)

---

## Summary

After completing these steps:
- ✅ Web App is created with Keycloak container
- ✅ Connected to PostgreSQL database
- ✅ Environment variables configured
- ✅ Ready for development/testing
- 🔄 Next: Configure custom domain and SSL for production

