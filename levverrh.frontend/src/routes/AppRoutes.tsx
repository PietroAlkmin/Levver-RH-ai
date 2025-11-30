import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { Login } from '../pages/Auth/Login';
import { RegisterTenant } from '../pages/Auth/RegisterTenant';
import { JobApplication } from '../pages/Public/JobApplication';
import { ProtectedRoute } from './ProtectedRoute';
import { useAuthStore } from '../stores/authStore';

// Lazy loading de páginas para performance
const PainelDashboard = React.lazy(() => import('../features/painel/pages/PainelDashboard').then(module => ({ default: module.PainelDashboard })));
const TalentsDashboard = React.lazy(() => import('../features/talents/pages/TalentsDashboard').then(module => ({ default: module.default })));
const CreateJobWithAI = React.lazy(() => import('../features/talents/pages/CreateJobWithAI').then(module => ({ default: module.CreateJobWithAI })));
const JobsList = React.lazy(() => import('../features/talents/pages/JobsList').then(module => ({ default: module.JobsList })));
const JobDetailPage = React.lazy(() => import('../features/talents/pages/JobDetailPage').then(module => ({ default: module.JobDetailPage })));

/**
 * Configuração de rotas da aplicação
 */
export const AppRoutes: React.FC = () => {
  const { isAuthenticated, _hasHydrated } = useAuthStore();

  console.log('🛣️ AppRoutes - _hasHydrated:', _hasHydrated);
  console.log('🛣️ AppRoutes - isAuthenticated:', isAuthenticated);

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
    {/* Rota raiz - redireciona para login ou painel */}
          <Route
            path="/"
            element={
              !_hasHydrated ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                  Carregando...
                </div>
              ) : isAuthenticated ? (
                <Navigate to="/painel" replace />
              ) : (
                <Navigate to="/login" replace />
              )
            }
          />

          {/* Rotas públicas */}
          <Route path="/login" element={<Login />} />
          <Route path="/register-tenant" element={<RegisterTenant />} />
          <Route path="/candidatura/:jobId" element={<JobApplication />} />
          {/* <Route path="/forgot-password" element={<ForgotPassword />} /> */}

      {/* Rotas protegidas */}
       <Route
 path="/painel"
            element={
              <ProtectedRoute>
           <PainelDashboard />
       </ProtectedRoute>
   }
  />

      {/* Levver Talents */}
      <Route
        path="/talents"
        element={
          <ProtectedRoute>
            <TalentsDashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="/talents/jobs/new"
        element={
          <ProtectedRoute>
            <CreateJobWithAI />
          </ProtectedRoute>
        }
      />

      <Route
        path="/talents/vagas"
        element={
          <ProtectedRoute>
            <JobsList />
          </ProtectedRoute>
        }
      />

      <Route
        path="/talents/vagas/:jobId"
        element={
          <ProtectedRoute>
            <JobDetailPage />
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
