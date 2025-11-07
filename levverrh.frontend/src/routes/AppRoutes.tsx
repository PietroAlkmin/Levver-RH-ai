import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { Login } from '../pages/Auth/Login';
import { RegisterTenant } from '../pages/Auth/RegisterTenant';
import { ProtectedRoute } from './ProtectedRoute';
import { useAuthStore } from '../stores/authStore';

// Lazy loading de páginas para performance
const Dashboard = React.lazy(() => import('../pages/Dashboard/Dashboard'));

/**
 * Configuração de rotas da aplicação
 */
export const AppRoutes: React.FC = () => {
  const { isAuthenticated } = useAuthStore();

  return (
    <BrowserRouter>
      {/* Toast notifications */}
      <Toaster
        position="top-right"
        toastOptions={{
    duration: 4000,
   style: {
   background: '#fff',
color: '#363636',
       boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
        },
          success: {
  iconTheme: {
   primary: '#10b981',
 secondary: '#fff',
       },
    },
          error: {
  iconTheme: {
       primary: '#ef4444',
     secondary: '#fff',
      },
     },
        }}
      />

   <React.Suspense fallback={<div>Carregando...</div>}>
  <Routes>
    {/* Rota raiz - redireciona para login ou dashboard */}
          <Route
            path="/"
   element={isAuthenticated ? <Navigate to="/dashboard" replace /> : <Navigate to="/login" replace />}
       />

          {/* Rotas públicas */}
          <Route path="/login" element={<Login />} />
          <Route path="/register-tenant" element={<RegisterTenant />} />
          {/* <Route path="/forgot-password" element={<ForgotPassword />} /> */}

      {/* Rotas protegidas */}
       <Route
 path="/dashboard"
            element={
              <ProtectedRoute>
           <Dashboard />
       </ProtectedRoute>
   }
  />

    {/* 404 */}
  <Route path="*" element={<div>404 - Página não encontrada</div>} />
        </Routes>
    </React.Suspense>
    </BrowserRouter>
  );
};
