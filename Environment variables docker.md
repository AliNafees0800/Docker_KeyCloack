
1. Create web app for container
   add container 
   https://quay.io as other registery url
   phasetwo/phasetwo-keycloak:latest as image and tags 
   start-dev as command
2. Create azure database for postgresql app
3. keycloack_user
   Nafees012!
4. 
5. 
   
6. 

Following environment variables need to be set

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



