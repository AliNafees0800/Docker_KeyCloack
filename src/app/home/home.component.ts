import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  template: `
    <div class="login-container">
      <div class="login-card">
        <h2>Welcome to Keycloak Callback Handler</h2>
        <p>Please login to continue</p>
        
        <div class="login-section">
          <a href="http://localhost:8080/realms/Okta-Broker/protocol/openid-connect/auth?client_id=okta-client&redirect_uri=http://localhost:3000/callback&response_type=code&scope=openid&kc_idp_hint=oidc-okta" 
             class="login-button">
            Login with Okta
          </a>
        </div>

        <div class="divider">
          <span>OR</span>
        </div>

        <div class="test-section">
          <p>Test the callback functionality directly:</p>
          <button (click)="testCallback()" class="test-button">Test Callback</button>
        </div>

        <div class="info-section">
          <details>
            <summary>Callback URL Format</summary>
            <code>http://localhost:3000/callback?session_state=676c1aac-f895-48f4-af99-dbe9365cf108&iss=http%3A%2F%2Flocalhost%3A8080%2Frealms%2FOkta-Broker&code=395b5b73-7b02-4e58-a358-668ae93a2510.676c1aac-f895-48f4-af99-dbe9365cf108.23aae05f-d069-4c92-83c9-e22189d9bf4e</code>
          </details>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 60vh;
    }

    .login-card {
      background: white;
      padding: 40px;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.1);
      text-align: center;
      max-width: 500px;
      width: 100%;
    }

    .login-section {
      margin: 30px 0;
    }

    .login-button {
      display: inline-block;
      background: linear-gradient(135deg, #007bff, #0056b3);
      color: white;
      text-decoration: none;
      padding: 15px 30px;
      border-radius: 8px;
      font-weight: 600;
      font-size: 16px;
      transition: all 0.3s ease;
      box-shadow: 0 2px 10px rgba(0,123,255,0.3);
    }

    .login-button:hover {
      background: linear-gradient(135deg, #0056b3, #004085);
      transform: translateY(-2px);
      box-shadow: 0 4px 15px rgba(0,123,255,0.4);
    }

    .divider {
      margin: 30px 0;
      position: relative;
      text-align: center;
    }

    .divider::before {
      content: '';
      position: absolute;
      top: 50%;
      left: 0;
      right: 0;
      height: 1px;
      background: #e9ecef;
    }

    .divider span {
      background: white;
      padding: 0 20px;
      color: #6c757d;
      font-weight: 500;
    }

    .test-section {
      margin: 20px 0;
    }

    .test-button {
      background: #6c757d;
      color: white;
      border: none;
      padding: 10px 20px;
      border-radius: 6px;
      cursor: pointer;
      font-size: 14px;
      transition: background 0.3s ease;
    }

    .test-button:hover {
      background: #5a6268;
    }

    .info-section {
      margin-top: 30px;
      text-align: left;
    }

    .info-section details {
      background: #f8f9fa;
      padding: 15px;
      border-radius: 6px;
      border: 1px solid #e9ecef;
    }

    .info-section summary {
      cursor: pointer;
      font-weight: 500;
      margin-bottom: 10px;
    }

    .info-section code {
      display: block;
      background: #e9ecef;
      padding: 10px;
      border-radius: 4px;
      font-size: 12px;
      word-break: break-all;
      margin-top: 10px;
    }
  `]
})
export class HomeComponent {
  constructor(private router: Router) {}

  testCallback() {
    const testUrl = '/callback?session_state=676c1aac-f895-48f4-af99-dbe9365cf108&iss=http%3A%2F%2Flocalhost%3A8080%2Frealms%2FOkta-Broker&code=395b5b73-7b02-4e58-a358-668ae93a2510.676c1aac-f895-48f4-af99-dbe9365cf108.23aae05f-d069-4c92-83c9-e22189d9bf4e';
    this.router.navigateByUrl(testUrl);
  }
}
