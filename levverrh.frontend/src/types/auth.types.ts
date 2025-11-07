// Auth Types - Espelham os DTOs do backend C#
export interface LoginRequest {
  email: string;
password: string;
}

export interface RegisterRequest {
  tenantId: string;
  email: string;
  nome: string;
  password: string;
  role: UserRole;
}

export interface RegisterTenantRequest {
  nomeEmpresa: string;
  cnpj: string;
  emailEmpresa: string;
  telefoneEmpresa?: string;
  enderecoEmpresa?: string;
  emailAdmin: string;
  nomeAdmin: string;
  password: string;
  confirmPassword: string;
}

export interface AzureAdLoginRequest {
  azureToken: string;
}

export interface LoginResponse {
  token: string;
  user: UserInfo;
  tenant: TenantInfo;
  whiteLabel: WhiteLabelInfo | null;
}

export interface UserInfo {
  id: string;
  nome: string;
  email: string;
  role: string;
  authType: string;
  fotoUrl: string | null;
}

export interface TenantInfo {
  id: string;
  nome: string;
  email: string;
  status: string;
}

export interface WhiteLabelInfo {
  logoUrl: string | null;
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;
  backgroundColor: string;
  textColor: string;
  borderColor: string;
  systemName: string;
  faviconUrl: string | null;
dominioCustomizado: string | null;
}

// Enums (espelham os enums do backend)
export enum UserRole {
  Admin = 1,
  Recruiter = 2,
  Viewer = 3,
}

export enum AuthType {
  Local = 1,
  AzureAd = 2,
}
