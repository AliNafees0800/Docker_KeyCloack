# Azure Configuration Reference - Quick Copy/Paste Values

## PostgreSQL Connection String Format

When configuring Keycloak Container App, use these exact environment variable values:

### Required Environment Variables

```
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=<choose-a-secure-password>
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://<SERVER-NAME>:5432/keycloak?sslmode=require
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

## Important Notes

1. **PostgreSQL Username Format**: Azure requires `username@servername` format
   - If your server is `keycloak-postgres-12345`
   - Username should be: `postgres@keycloak-postgres-12345`

2. **SSL Mode**: Use `?sslmode=require` in the JDBC URL for secure connections

3. **Container Command**: 
   - For **production**: Use `start` (not `start-dev`)
   - For **development**: Use `start-dev`
   - In Azure Container Apps, you can set this in the container configuration under "Command" field

4. **Port**: Container Apps uses port `8080` (not 8081 like your local setup)

## Container App Settings Summary

- **Image**: `quay.io/phasetwo/phasetwo-keycloak:latest`
- **CPU**: Minimum `1.0` (recommend `2.0` for production)
- **Memory**: Minimum `2.0 Gi` (recommend `4.0 Gi` for production)
- **Target Port**: `8080`
- **Ingress**: Enabled, Accepting traffic from anywhere (or restrict as needed)

## PostgreSQL Settings Summary

- **Server Type**: Flexible Server
- **Version**: PostgreSQL 15 or 16
- **Compute**: Burstable B1ms (dev) or General Purpose (production)
- **Storage**: 32 GB minimum
- **Database Name**: `keycloak`
- **Firewall**: Allow Azure services (0.0.0.0 - 0.0.0.0) or use Private Endpoint

