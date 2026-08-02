import { Injectable } from '@angular/core';
import { PublicClientApplication, AccountInfo, InteractionRequiredAuthError } from '@azure/msal-browser';
import { environment } from '../../../environments/environment';

/**
 * Thin wrapper around MSAL for Azure AD SSO — the same tenant D365 users already authenticate
 * against (BR-064/UR-062), so the shipping platform introduces no separate credential store.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly msalInstance = new PublicClientApplication({
    auth: {
      clientId: environment.azureAd.clientId,
      authority: environment.azureAd.authority,
      redirectUri: '/'
    },
    cache: { cacheLocation: 'sessionStorage' }
  });

  private initialized = false;

  private async ensureInitialized(): Promise<void> {
    if (!this.initialized) {
      await this.msalInstance.initialize();
      this.initialized = true;
    }
  }

  async login(): Promise<void> {
    await this.ensureInitialized();
    await this.msalInstance.loginRedirect({ scopes: [environment.azureAd.apiScope] });
  }

  logout(): void {
    void this.msalInstance.logoutRedirect();
  }

  getActiveAccount(): AccountInfo | null {
    return this.msalInstance.getActiveAccount() ?? this.msalInstance.getAllAccounts()[0] ?? null;
  }

  async acquireApiToken(): Promise<string | null> {
    await this.ensureInitialized();
    const account = this.getActiveAccount();
    if (!account) {
      return null;
    }

    try {
      const result = await this.msalInstance.acquireTokenSilent({ scopes: [environment.azureAd.apiScope], account });
      return result.accessToken;
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        await this.msalInstance.acquireTokenRedirect({ scopes: [environment.azureAd.apiScope] });
      }
      return null;
    }
  }
}
