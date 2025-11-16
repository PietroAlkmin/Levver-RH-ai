import { Configuration, PopupRequest } from '@azure/msal-browser';

/**
 * Configuração do Microsoft Authentication Library (MSAL)
 * Para integração com Azure AD / Microsoft Entra ID
 */

// 🔧 Configuração do Azure AD (você precisa registrar o app no Azure Portal)
export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_CLIENT_ID || 'YOUR_CLIENT_ID_HERE', // Application (client) ID
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_TENANT_ID || 'common'}`, // common permite qualquer conta Microsoft
    redirectUri: import.meta.env.VITE_AZURE_REDIRECT_URI || 'http://localhost:5173', // Deve estar registrado no Azure
    postLogoutRedirectUri: import.meta.env.VITE_AZURE_REDIRECT_URI || 'http://localhost:5173',
  },
  cache: {
    cacheLocation: 'localStorage', // ou 'sessionStorage'
    storeAuthStateInCookie: false, // true se precisar suportar IE11
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        
        switch (level) {
          case 0: // Error
            console.error(message);
            break;
          case 1: // Warning
            console.warn(message);
            break;
          case 2: // Info
            console.info(message);
            break;
          case 3: // Verbose
            console.debug(message);
            break;
        }
      },
      logLevel: import.meta.env.DEV ? 3 : 1, // Verbose em dev, Warning em prod
    },
  },
};

/**
 * Escopos solicitados ao Azure AD
 * - openid: Identificador único do usuário
 * - profile: Nome, foto, etc
 * - email: Email do usuário
 */
export const loginRequest: PopupRequest = {
  scopes: ['openid', 'profile', 'email', 'User.Read'], // User.Read = Microsoft Graph API
};

/**
 * Endpoints protegidos
 * Se você quiser chamar APIs da Microsoft (Graph, etc)
 */
export const protectedResources = {
  graphMe: {
    endpoint: 'https://graph.microsoft.com/v1.0/me',
    scopes: ['User.Read'],
  },
  graphPhoto: {
    endpoint: 'https://graph.microsoft.com/v1.0/me/photo/$value',
    scopes: ['User.Read'],
  },
};
