import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface KeycloakCallbackData {
  session_state: string;
  iss: string;
  code: string;
}

export interface ValidationResult {
  isValid: boolean;
  message: string;
  tokenData?: any;
}

@Injectable({
  providedIn: 'root'
})
export class KeycloakService {
  private baseUrl = 'http://localhost:8080'; // Adjust this to your backend API URL

  constructor(private http: HttpClient) {}

  validateCallback(data: KeycloakCallbackData): Observable<ValidationResult> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<ValidationResult>(`${this.baseUrl}/api/validate-keycloak-callback`, data, { headers });
  }

  exchangeCodeForToken(data: KeycloakCallbackData): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded'
    });

    const body = new URLSearchParams();
    body.set('grant_type', 'authorization_code');
    body.set('client_id', 'your-client-id'); // Replace with your actual client ID
    body.set('code', data.code);
    body.set('redirect_uri', 'http://localhost:3000/callback');

    return this.http.post(`${this.baseUrl}/realms/Okta-Broker/protocol/openid-connect/token`, body.toString(), { headers });
  }

  validateSessionState(sessionState: string, code: string): boolean {
    // Basic validation - in a real app, you'd want more sophisticated validation
    return Boolean(sessionState && code && sessionState.length > 0 && code.length > 0);
  }

  parseIssuer(iss: string): { realm: string; baseUrl: string } | null {
    try {
      const decodedIss = decodeURIComponent(iss);
      const url = new URL(decodedIss);
      const pathParts = url.pathname.split('/');
      const realm = pathParts[pathParts.length - 1];
      
      return {
        realm,
        baseUrl: `${url.protocol}//${url.host}`
      };
    } catch (error) {
      console.error('Error parsing issuer URL:', error);
      return null;
    }
  }
}
