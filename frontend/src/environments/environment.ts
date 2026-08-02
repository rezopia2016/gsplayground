export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:5001/api',
  azureAd: {
    clientId: '<shipping-platform-ui-app-registration-client-id>',
    authority: 'https://login.microsoftonline.com/<eurofins-azure-ad-tenant-id>',
    apiScope: 'api://shipping-platform-api/access_as_user'
  }
};
