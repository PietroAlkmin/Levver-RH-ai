import { useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import authService from '../services/authService';
import { LoginRequest, RegisterTenantRequest, RegisterRequest } from '../types/auth.types';
import toast from 'react-hot-toast';

/**
 * Custom Hook para gerenciar autentica��o
 * Centraliza toda l�gica de login/logout/registro
 */
export const useAuth = () => {
  const navigate = useNavigate();
  const { setAuth, clearAuth, setLoading, user, tenant, whiteLabel, isAuthenticated, isLoading } =
    useAuthStore();

  /**
   * Login com email/senha
   */
  const login = useCallback(
    async (credentials: LoginRequest) => {
      try {
        setLoading(true);
        const response = await authService.login(credentials);

        console.log('🔐 useAuth.login - Response:', response);

        if (response.success && response.data) {
          console.log('✅ useAuth.login - Setando auth com token:', response.data.token.substring(0, 50) + '...');
          console.log('✅ useAuth.login - User:', response.data.user);
          console.log('✅ useAuth.login - Tenant:', response.data.tenant);
          
          setAuth(
            response.data.token,
            response.data.user,
            response.data.tenant,
            response.data.whiteLabel
          );
          authService.saveAuthData(response.data);
          
          toast.success(`Bem-vindo(a), ${response.data.user.nome}!`);
          
          console.log('🚀 useAuth.login - Navegando para /painel');
          navigate('/painel');
          return { success: true };
        } else {
          toast.error(response.message || 'Erro ao fazer login');
          return { success: false, message: response.message };
        }
      } catch (error: any) {
        console.error('❌ useAuth.login - Erro:', error);
        const errorMessage = error.response?.data?.message || 'Erro ao fazer login';
        toast.error(errorMessage);
        return { success: false, message: errorMessage };
      } finally {
        setLoading(false);
      }
    },
    [navigate, setAuth, setLoading]
  );

  /**
   * Registrar novo tenant (onboarding)
   */
  const registerTenant = useCallback(
    async (data: RegisterTenantRequest) => {
      try {
        setLoading(true);
        const response = await authService.registerTenant(data);

    if (response.success && response.data) {
    setAuth(
            response.data.token,
       response.data.user,
         response.data.tenant,
       response.data.whiteLabel
          );
     authService.saveAuthData(response.data);
          toast.success('Empresa cadastrada com sucesso!');
          navigate('/painel');
          return { success: true };
        } else {
toast.error(response.message || 'Erro ao cadastrar empresa');
      return { success: false, message: response.message };
        }
    } catch (error: any) {
  const errorMessage = error.response?.data?.message || 'Erro ao cadastrar empresa';
        toast.error(errorMessage);
  return { success: false, message: errorMessage };
      } finally {
      setLoading(false);
      }
 },
    [navigate, setAuth, setLoading]
  );

  /**
   * Registrar novo usu�rio
   */
  const registerUser = useCallback(
    async (data: RegisterRequest) => {
      try {
 setLoading(true);
        const response = await authService.registerUser(data);

    if (response.success && response.data) {
          setAuth(
            response.data.token,
        response.data.user,
            response.data.tenant,
  response.data.whiteLabel
     );
          authService.saveAuthData(response.data);
toast.success('Usuário cadastrado com sucesso!');
       navigate('/painel');
          return { success: true };
        } else {
          toast.error(response.message || 'Erro ao cadastrar usu�rio');
          return { success: false, message: response.message };
        }
      } catch (error: any) {
     const errorMessage = error.response?.data?.message || 'Erro ao cadastrar usu�rio';
        toast.error(errorMessage);
   return { success: false, message: errorMessage };
      } finally {
        setLoading(false);
      }
    },
    [navigate, setAuth, setLoading]
  );

  /**
   * Logout
   */
  const logout = useCallback(() => {
    clearAuth();
    authService.clearAuthData();
    toast.success('Logout realizado com sucesso');
    navigate('/login');
  }, [clearAuth, navigate]);

  /**
   * Verifica se usu�rio tem permiss�o
   */
  const hasRole = useCallback(
  (roles: string | string[]): boolean => {
      if (!user) return false;

      const allowedRoles = Array.isArray(roles) ? roles : [roles];
      return allowedRoles.includes(user.role);
    },
    [user]
  );

  return {
    // State
  user,
    tenant,
    whiteLabel,
    isAuthenticated,
    isLoading,

    // Actions
    login,
registerTenant,
    registerUser,
    logout,
    hasRole,
  };
};
