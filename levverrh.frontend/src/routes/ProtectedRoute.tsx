import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { Loading } from '../components/common';
import './ProtectedRoute.css';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
}

/**
 * Componente para proteger rotas que exigem autentica��o
 */
export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ 
  children, 
  requiredRoles 
}) => {
  const { isAuthenticated, user, isLoading, token, _hasHydrated } = useAuthStore();

  // DEBUG: Log do estado de autenticação
  console.log('🔒 ProtectedRoute - _hasHydrated:', _hasHydrated);
  console.log('🔒 ProtectedRoute - isAuthenticated:', isAuthenticated);
  console.log('🔒 ProtectedRoute - isLoading:', isLoading);
  console.log('🔒 ProtectedRoute - token:', token ? 'exists' : 'null');
  console.log('🔒 ProtectedRoute - user:', user);

  // Aguardar hidratação do Zustand
  if (!_hasHydrated) {
    console.log('⏳ ProtectedRoute - Aguardando hidratação do Zustand...');
    return <Loading fullScreen text="Carregando..." />;
  }

  if (isLoading) {
    return <Loading fullScreen text="Verificando autenticação..." />;
  }

  if (!isAuthenticated || !token) {
    console.log('❌ ProtectedRoute - Redirecionando para /login');
    return <Navigate to="/login" replace />;
  }

  // Verificar roles se especificado
  if (requiredRoles && requiredRoles.length > 0 && user) {
    const hasRequiredRole = requiredRoles.includes(user.role);
    
  if (!hasRequiredRole) {
      return (
        <div className="error-page">
          <div className="error-content">
            <h1 className="error-code">403</h1>
            <p className="error-title">Acesso negado</p>
            <p className="error-description">Você não tem permissão para acessar esta página.</p>
          </div>
        </div>
      );
    }
  }

  return <>{children}</>;
};
