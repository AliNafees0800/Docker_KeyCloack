import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface KeycloakCallbackData {
  session_state: string;
  iss: string;
  code: string;
}

export interface ValidationResult {
  is_valid: boolean;
  message: string;
  token_data?: any;
}

export interface TokenResponse {
  access_token: string;
  token_type: string;
  expires_in: number;
  refresh_token: string;
  scope: string;
  id_token?: string;
  session_state?: string;
  'not-before-policy'?: number;
}

export interface TokenValidationRequest {
  access_token: string;
}

export interface SessionStateValidationRequest {
  session_state: string;
  code: string;
}

export interface KeycloakUserInfo {
  sub: string;
  name: string;
  email: string;
  preferred_username: string;
  email_verified: boolean;
  roles: string[];
}

@Injectable({
  providedIn: 'root'
})
export class KeycloakService {
  private baseUrl = 'https://localhost:7093'; // Adjust this to your backend API URL

  constructor(private http: HttpClient) {}

  validateCallback(data: KeycloakCallbackData): Observable<ValidationResult> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<ValidationResult>(`${this.baseUrl}/api/validate-keycloak-callback`, data, { headers });
  }

  exchangeCodeForToken(data: KeycloakCallbackData): Observable<TokenResponse> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<TokenResponse>(`${this.baseUrl}/api/exchange-code-for-token`, data, { headers });
  }

  validateToken(accessToken: string): Observable<{is_valid: boolean}> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    const request: TokenValidationRequest = { access_token: accessToken };
    return this.http.post<{is_valid: boolean}>(`${this.baseUrl}/api/validate-token`, request, { headers });
  }

  getUserInfo(accessToken: string): Observable<KeycloakUserInfo> {
    return this.http.get<KeycloakUserInfo>(`${this.baseUrl}/api/user-info?accessToken=${encodeURIComponent(accessToken)}`);
  }

  validateSessionState(sessionState: string, code: string): Observable<ValidationResult> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    const request: SessionStateValidationRequest = { session_state: sessionState, code: code };
    return this.http.post<ValidationResult>(`${this.baseUrl}/api/validate-session-state`, request, { headers });
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
