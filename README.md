# Keycloak Callback Handler

An Angular application that handles Keycloak authentication callbacks on port 3000.

## Features

- Handles Keycloak redirects with session_state, iss, and code parameters
- Validates callback data both locally and with backend API
- Exchanges authorization code for access tokens
- Parses and displays Keycloak issuer information
- Modern Angular 17 standalone components

## Installation

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm start
```

The app will be available at `http://localhost:3000`

## Usage

### Testing the Callback

Navigate to the callback URL with the required parameters:
```
http://localhost:3000/callback?session_state=676c1aac-f895-48f4-af99-dbe9365cf108&iss=http%3A%2F%2Flocalhost%3A8080%2Frealms%2FOkta-Broker&code=395b5b73-7b02-4e58-a358-668ae93a2510.676c1aac-f895-48f4-af99-dbe9365cf108.23aae05f-d069-4c92-83c9-e22189d9bf4e
```

### Backend Integration

The app includes a `KeycloakService` that can communicate with your backend API for validation. Update the `baseUrl` in `src/app/keycloak.service.ts` to point to your backend.

## Configuration

- **Port**: 3000 (configured in angular.json)
- **Backend URL**: Update in `src/app/keycloak.service.ts`
- **Client ID**: Update in the `exchangeCodeForToken` method

## Project Structure

```
src/
├── app/
│   ├── callback/          # Callback component
│   ├── home/             # Home component
│   ├── keycloak.service.ts # Keycloak service
│   ├── app.component.ts   # Main app component
│   └── app.routes.ts      # Routing configuration
├── assets/               # Static assets
├── index.html           # Main HTML file
├── main.ts              # App bootstrap
└── styles.css           # Global styles
```
