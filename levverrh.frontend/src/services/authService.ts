import apiClient from './api';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterTenantRequest,
  AzureAdLoginRequest,
} from '../types/auth.types';
import { ApiResponse } from '../types/api.types';

/**
 * Serviço de Autenticação
 * Comunica com os endpoints /api/auth/* do backend
 */
class AuthService {
  private readonly endpoint = '/auth';

  /**
   * Login com email e senha
   */
  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      `${this.endpoint}/login`,
      credentials
    );
    return response.data;
  }

  /**
   * Registrar novo tenant (empresa) com usuário admin
   */
  async registerTenant(
    data: RegisterTenantRequest
  ): Promise<ApiResponse<LoginResponse>> {
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      `${this.endpoint}/register/tenant`,
      data
    );
    return response.data;
  }

  /**
   * Registrar novo usuário em tenant existente
   */
  async registerUser(data: RegisterRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      `${this.endpoint}/register`,
      data
    );
    return response.data;
  }

  /**
* Login com Azure AD (SSO)
   */
  async loginWithAzureAd(
    data: AzureAdLoginRequest
  ): Promise<ApiResponse<LoginResponse>> {
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
    `${this.endpoint}/login/azure`,
      data
    );
    return response.data;
  }

  /**
   * Armazena dados de autenticação no localStorage
   */
  saveAuthData(data: LoginResponse): void {
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data.user));
    localStorage.setItem('tenant', JSON.stringify(data.tenant));

    if (data.whiteLabel) {
      localStorage.setItem('whiteLabel', JSON.stringify(data.whiteLabel));
      this.applyWhiteLabel(data.whiteLabel);
    }
  }

  /**
   * Remove dados de autenticação
 */
  clearAuthData(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('tenant');
    localStorage.removeItem('whiteLabel');
  }

  /**
   * Verifica se usuário está autenticado
   */
  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  /**
   * Obtém dados do usuário logado
   */
  getCurrentUser() {
    const userJson = localStorage.getItem('user');
    return userJson ? JSON.parse(userJson) : null;
  }

  /**
   * Obtém dados do tenant
   */
  getCurrentTenant() {
    const tenantJson = localStorage.getItem('tenant');
    return tenantJson ? JSON.parse(tenantJson) : null;
  }

  /**
   * Aplica White Label (cores customizadas)
   */
  private applyWhiteLabel(whiteLabel: any): void {
    const root = document.documentElement;
    root.style.setProperty('--primary-color', whiteLabel.primaryColor);
    root.style.setProperty('--secondary-color', whiteLabel.secondaryColor);
    root.style.setProperty('--accent-color', whiteLabel.accentColor);
    root.style.setProperty('--background-color', whiteLabel.backgroundColor);
    root.style.setProperty('--text-color', whiteLabel.textColor);
    root.style.setProperty('--border-color', whiteLabel.borderColor);

    // Atualizar título da página
    if (whiteLabel.systemName) {
      document.title = whiteLabel.systemName;
    }

    // Atualizar favicon
    if (whiteLabel.faviconUrl) {
      const favicon = document.querySelector("link[rel~='icon']") as HTMLLinkElement;
      if (favicon) {
        favicon.href = whiteLabel.faviconUrl;
      }
    }
  }
}

export default new AuthService();
