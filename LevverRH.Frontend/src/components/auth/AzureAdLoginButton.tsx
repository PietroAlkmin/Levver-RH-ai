import React, { useState } from 'react';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig, loginRequest } from '../../config/msalConfig';
import { Button } from '../common';
import authService from '../../services/authService';
import { useAuthStore } from '../../stores/authStore';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';

/**
 * Componente de Login com Microsoft / Azure AD
 * Usa MSAL (Microsoft Authentication Library) para SSO
 */

const msalInstance = new PublicClientApplication(msalConfig);

export const AzureAdLoginButton: React.FC = () => {
  const [isLoading, setIsLoading] = useState(false);
  const { setAuth } = useAuthStore();
  const navigate = useNavigate();

  const handleAzureAdLogin = async () => {
    setIsLoading(true);

    try {
      // 1️⃣ Inicializar MSAL
      await msalInstance.initialize();

      // 2️⃣ Abrir popup de login da Microsoft
      const loginResponse = await msalInstance.loginPopup(loginRequest);

      if (!loginResponse.idToken) {
        throw new Error('Token não recebido do Azure AD');
      }

      console.log('✅ Login Azure AD bem-sucedido:', {
        account: loginResponse.account?.username,
        name: loginResponse.account?.name,
      });

      console.log('🔹 Enviando token para backend...', {
        token: loginResponse.idToken.substring(0, 50) + '...',
      });

      // 3️⃣ Enviar token para o backend validar
      const response = await authService.loginWithAzureAd({
        azureToken: loginResponse.idToken,
      });

      if (response.success && response.data) {
        // 4️⃣ Salvar dados no Zustand + localStorage
        authService.saveAuthData(response.data); // ✅ Salvar no formato do authService
        setAuth(
          response.data.token,
          response.data.user,
          response.data.tenant,
          response.data.whiteLabel
        );

        toast.success(`Bem-vindo, ${response.data.user.nome}! 🎉`);

        // 5️⃣ Redirecionar baseado no status do tenant
        if (response.data.tenant.status === 'PendenteSetup') {
          // Tenant precisa completar cadastro
          toast('Complete o cadastro da sua empresa para continuar.', {
            icon: 'ℹ️',
          });
          navigate('/register-tenant');
        } else {
          // Tenant já está ativo, vai pro painel
          navigate('/painel');
        }
      } else {
        throw new Error(response.message || 'Erro ao autenticar com Azure AD');
      }
    } catch (error: any) {
      console.error('❌ Erro no login Azure AD:', error);
      console.error('❌ Detalhes do erro:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status,
      });

      // Mensagens de erro mais amigáveis
      let errorMessage = 'Erro ao fazer login com Microsoft';

      if (error.errorCode === 'popup_window_error') {
        errorMessage = 'Popup bloqueado. Permita popups para este site.';
      } else if (error.errorCode === 'user_cancelled') {
        errorMessage = 'Login cancelado.';
      } else if (error.message) {
        errorMessage = error.message;
      }

      toast.error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <button
      type="button"
      onClick={handleAzureAdLogin}
      disabled={isLoading}
      className="w-full flex items-center justify-center gap-2 py-2.5 font-semibold text-sm transition-all hover:opacity-90 disabled:opacity-50"
      style={{
        border: '1px solid #E5E7EB',
        borderRadius: '8px',
        background: 'white',
        color: '#111827'
      }}
    >
      {isLoading ? (
        <>
          <div className="w-5 h-5 border-2 rounded-full animate-spin" style={{ borderColor: '#E5E7EB', borderTopColor: '#713BDB' }} />
          <span>Conectando...</span>
        </>
      ) : (
        <>
          <svg className="w-5 h-5" viewBox="0 0 23 23" fill="none">
            {/* Logo da Microsoft */}
            <path fill="#f25022" d="M0 0h11v11H0z" />
            <path fill="#00a4ef" d="M12 0h11v11H12z" />
            <path fill="#7fba00" d="M0 12h11v11H0z" />
            <path fill="#ffb900" d="M12 12h11v11H12z" />
          </svg>
          <span>Entrar com Microsoft</span>
        </>
      )}
    </button>
  );
};
