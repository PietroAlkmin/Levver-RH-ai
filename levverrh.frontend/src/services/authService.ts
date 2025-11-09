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
 * Servi�o de Autentica��o
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
   * Registrar novo tenant (empresa) com usu�rio admin
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
   * Registrar novo usu�rio em tenant existente
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
    const url = `${this.endpoint}/login/azure`;
    console.log('🔹 authService.loginWithAzureAd - URL:', url);
    console.log('🔹 authService.loginWithAzureAd - Data:', data);
    
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      url,
      data
    );
    return response.data;
  }

  /**
   * Completar setup de tenant criado via SSO
   * (Admin já logado precisa preencher dados da empresa)
   */
  async completeTenantSetup(data: {
    nomeEmpresa: string;
    cnpj: string;
    emailEmpresa: string;
    telefoneEmpresa?: string;
    enderecoEmpresa?: string;
  }): Promise<ApiResponse<LoginResponse>> {
    console.log('🔹 authService.completeTenantSetup - Chamando endpoint...');
    console.log('🔹 authService.completeTenantSetup - Data (camelCase):', data);
    console.log('🔹 authService.completeTenantSetup - Token:', localStorage.getItem('token')?.substring(0, 50) + '...');
    
    // Converter para PascalCase esperado pelo backend
    const payload = {
      NomeEmpresa: data.nomeEmpresa,
      Cnpj: data.cnpj,
      EmailEmpresa: data.emailEmpresa,
      TelefoneEmpresa: data.telefoneEmpresa || null,
      EnderecoEmpresa: data.enderecoEmpresa || null
    };

    console.log('🔹 authService.completeTenantSetup - Payload (PascalCase):', payload);
    
    const response = await apiClient.post<ApiResponse<LoginResponse>>(
      `${this.endpoint}/complete-tenant-setup`,
      payload
    );
    
    console.log('🔹 authService.completeTenantSetup - Response:', response.data);
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
   * Remove dados de autentica��o
 */
  clearAuthData(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('tenant');
    localStorage.removeItem('whiteLabel');
  }

  /**
   * Verifica se usu�rio est� autenticado
   */
  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  /**
   * Obt�m dados do usu�rio logado
   */
  getCurrentUser() {
    const userJson = localStorage.getItem('user');
    return userJson ? JSON.parse(userJson) : null;
  }

  /**
   * Obt�m dados do tenant
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

    // Atualizar t�tulo da p�gina
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
