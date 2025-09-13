import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface KeycloakCallbackData {
  SessionState: string;
  Iss: string;
  Code: string;
}

export interface ValidationResult {
  IsValid: boolean;
  Message: string;
  TokenData?: any;
}

export interface TokenResponse {
  AccessToken: string;
  TokenType: string;
  ExpiresIn: number;
  RefreshToken: string;
  Scope: string;
}

export interface TokenValidationRequest {
  AccessToken: string;
}

export interface SessionStateValidationRequest {
  SessionState: string;
  Code: string;
}

export interface KeycloakUserInfo {
  Sub: string;
  Name: string;
  Email: string;
  PreferredUsername: string;
  EmailVerified: boolean;
  Roles: string[];
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

  validateToken(accessToken: string): Observable<{isValid: boolean}> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    const request: TokenValidationRequest = { AccessToken: accessToken };
    return this.http.post<{isValid: boolean}>(`${this.baseUrl}/api/validate-token`, request, { headers });
  }

  getUserInfo(accessToken: string): Observable<KeycloakUserInfo> {
    return this.http.get<KeycloakUserInfo>(`${this.baseUrl}/api/user-info?accessToken=${encodeURIComponent(accessToken)}`);
  }

  validateSessionState(sessionState: string, code: string): Observable<ValidationResult> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    const request: SessionStateValidationRequest = { SessionState: sessionState, Code: code };
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
