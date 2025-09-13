import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { KeycloakService, KeycloakCallbackData, ValidationResult } from '../keycloak.service';

@Component({
  selector: 'app-callback',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div>
      <h2>Keycloak Callback Handler</h2>
      
      <div *ngIf="loading" class="status info">
        Processing callback...
      </div>

      <div *ngIf="!loading">
        <div *ngIf="callbackData" class="param-list">
          <h3>Received Parameters:</h3>
          <div class="param-item"><strong>Session State:</strong> {{ callbackData.session_state }}</div>
          <div class="param-item"><strong>Issuer:</strong> {{ callbackData.iss }}</div>
          <div class="param-item"><strong>Code:</strong> {{ callbackData.code }}</div>
        </div>

        <div *ngIf="validationResult" class="status" [ngClass]="validationResult.is_valid ? 'success' : 'error'">
          <strong>Validation Result:</strong> {{ validationResult.message }}
        </div>

        <div *ngIf="parsedIssuer" class="param-list">
          <h3>Parsed Issuer Information:</h3>
          <div class="param-item"><strong>Realm:</strong> {{ parsedIssuer.realm }}</div>
          <div class="param-item"><strong>Base URL:</strong> {{ parsedIssuer.baseUrl }}</div>
        </div>

        <div *ngIf="error" class="status error">
          <strong>Error:</strong> {{ error }}
        </div>

        <div *ngIf="tokenData" class="param-list">
          <h3>Token Information:</h3>
          <div class="param-item"><strong>Access Token:</strong> {{ tokenData.access_token ? 'Received' : 'Not available' }}</div>
          <div class="param-item"><strong>Token Type:</strong> {{ tokenData.token_type }}</div>
          <div class="param-item"><strong>Expires In:</strong> {{ tokenData.expires_in }} seconds</div>
          <div class="param-item"><strong>Scope:</strong> {{ tokenData.scope }}</div>
          <div class="param-item"><strong>Session State:</strong> {{ tokenData.session_state }}</div>
        </div>

        <div *ngIf="userInfo" class="param-list">
          <h3>User Information:</h3>
          <div class="param-item"><strong>Subject:</strong> {{ userInfo.sub }}</div>
          <div class="param-item"><strong>Name:</strong> {{ userInfo.name }}</div>
          <div class="param-item"><strong>Email:</strong> {{ userInfo.email }}</div>
          <div class="param-item"><strong>Username:</strong> {{ userInfo.preferred_username }}</div>
          <div class="param-item"><strong>Email Verified:</strong> {{ userInfo.email_verified ? 'Yes' : 'No' }}</div>
          <div class="param-item"><strong>Roles:</strong> {{ userInfo.roles?.join(', ') || 'None' }}</div>
        </div>

        <div class="actions">
          <button (click)="validateWithBackend()" [disabled]="!callbackData || validating">
            {{ validating ? 'Validating...' : 'Validate with Backend' }}
          </button>
          <button (click)="exchangeCodeForToken()" [disabled]="!callbackData || exchanging">
            {{ exchanging ? 'Exchanging...' : 'Exchange Code for Token' }}
          </button>
          <button (click)="validateToken()" [disabled]="!tokenData?.access_token || validatingToken">
            {{ validatingToken ? 'Validating Token...' : 'Validate Token' }}
          </button>
          <button (click)="getUserInfo()" [disabled]="!tokenData?.access_token || gettingUserInfo">
            {{ gettingUserInfo ? 'Getting User Info...' : 'Get User Info' }}
          </button>
          <button (click)="validateSessionState()" [disabled]="!callbackData || validatingSession">
            {{ validatingSession ? 'Validating Session...' : 'Validate Session State' }}
          </button>
          <button (click)="goHome()">Go Home</button>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class CallbackComponent implements OnInit {
  callbackData: KeycloakCallbackData | null = null;
  validationResult: ValidationResult | null = null;
  parsedIssuer: { realm: string; baseUrl: string } | null = null;
  tokenData: any = null;
  userInfo: any = null;
  loading = true;
  validating = false;
  exchanging = false;
  validatingToken = false;
  gettingUserInfo = false;
  validatingSession = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private keycloakService: KeycloakService
  ) {}

  ngOnInit() {
    this.parseUrlParameters();
  }

  private parseUrlParameters() {
    this.route.queryParams.subscribe(params => {
      const sessionState = params['session_state'];
      const iss = params['iss'];
      const code = params['code'];

      if (sessionState && iss && code) {
        this.callbackData = {
          session_state: sessionState,
          iss: iss,
          code: code
        };

        // Parse issuer information
        this.parsedIssuer = this.keycloakService.parseIssuer(iss);

        // Basic validation
        const isValid = sessionState && code && sessionState.length > 0 && code.length > 0;
        this.validationResult = {
          is_valid: isValid,
          message: isValid ? 'Basic validation passed' : 'Invalid session state or code'
        };

        this.loading = false;
      } else {
        this.error = 'Missing required parameters: session_state, iss, or code';
        this.loading = false;
      }
    });
  }

  validateWithBackend() {
    if (!this.callbackData) return;

    this.validating = true;
    this.error = null;

    this.keycloakService.validateCallback(this.callbackData).subscribe({
      next: (result) => {
        this.validationResult = result;
        this.validating = false;
      },
      error: (error) => {
        this.error = `Backend validation failed: ${error.message || error}`;
        this.validating = false;
      }
    });
  }

  exchangeCodeForToken() {
    if (!this.callbackData) return;

    this.exchanging = true;
    this.error = null;

    this.keycloakService.exchangeCodeForToken(this.callbackData).subscribe({
      next: (result) => {
        this.tokenData = result;
        this.exchanging = false;
      },
      error: (error) => {
        this.error = `Token exchange failed: ${error.message || error}`;
        this.exchanging = false;
      }
    });
  }

  validateToken() {
    if (!this.tokenData?.access_token) return;

    this.validatingToken = true;
    this.error = null;

    this.keycloakService.validateToken(this.tokenData.access_token).subscribe({
      next: (result) => {
        this.validationResult = {
          is_valid: result.is_valid,
          message: result.is_valid ? 'Token is valid' : 'Token is invalid'
        };
        this.validatingToken = false;
      },
      error: (error) => {
        this.error = `Token validation failed: ${error.message || error}`;
        this.validatingToken = false;
      }
    });
  }

  getUserInfo() {
    if (!this.tokenData?.access_token) return;

    this.gettingUserInfo = true;
    this.error = null;

    this.keycloakService.getUserInfo(this.tokenData.access_token).subscribe({
      next: (result) => {
        this.userInfo = result;
        this.gettingUserInfo = false;
      },
      error: (error) => {
        this.error = `Get user info failed: ${error.message || error}`;
        this.gettingUserInfo = false;
      }
    });
  }

  validateSessionState() {
    if (!this.callbackData) return;

    this.validatingSession = true;
    this.error = null;

    this.keycloakService.validateSessionState(this.callbackData.session_state, this.callbackData.code).subscribe({
      next: (result) => {
        this.validationResult = result;
        this.validatingSession = false;
      },
      error: (error) => {
        this.error = `Session state validation failed: ${error.message || error}`;
        this.validatingSession = false;
      }
    });
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
