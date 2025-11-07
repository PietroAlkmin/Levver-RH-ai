import React from 'react';
import { useAuth } from '../../hooks/useAuth';
import { Button } from '../../components/common';
import { CheckCircle, LogOut, User, Building2 } from 'lucide-react';

/**
 * Dashboard simples - apenas mostra que o login foi bem-sucedido
 */
const Dashboard: React.FC = () => {
  const { user, tenant, logout } = useAuth();

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-50 to-blue-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full">
        {/* Ícone de sucesso */}
        <div className="flex justify-center mb-6">
          <div className="bg-green-100 rounded-full p-4">
         <CheckCircle className="text-green-600" size={64} />
       </div>
        </div>

        {/* Título */}
 <h1 className="text-3xl font-bold text-center text-gray-900 mb-2">
          Login realizado com sucesso!
        </h1>
   <p className="text-center text-gray-600 mb-8">
  Você está autenticado no sistema
   </p>

   {/* Informações do usuário */}
        <div className="space-y-4 mb-8">
 <div className="bg-blue-50 p-4 rounded-lg">
<div className="flex items-center gap-3 mb-2">
       <User className="text-blue-600" size={20} />
              <span className="font-semibold text-gray-700">Usuário</span>
  </div>
            <p className="text-gray-900 font-medium">{user?.nome}</p>
        <p className="text-gray-600 text-sm">{user?.email}</p>
            <p className="text-blue-600 text-sm mt-1">Role: {user?.role}</p>
          </div>

        <div className="bg-purple-50 p-4 rounded-lg">
       <div className="flex items-center gap-3 mb-2">
       <Building2 className="text-purple-600" size={20} />
      <span className="font-semibold text-gray-700">Empresa</span>
            </div>
     <p className="text-gray-900 font-medium">{tenant?.nome}</p>
      <p className="text-gray-600 text-sm">{tenant?.email}</p>
       <p className="text-purple-600 text-sm mt-1">Status: {tenant?.status}</p>
    </div>
  </div>

        {/* Botão de Logout */}
        <Button
          onClick={logout}
          variant="danger"
          size="lg"
          fullWidth
 >
  <LogOut size={20} />
  Sair
        </Button>

        {/* Info */}
        <div className="mt-6 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
          <p className="text-sm text-yellow-800">
            <strong>?? Parabéns!</strong> Seu sistema de autenticação está funcionando perfeitamente!
          </p>
   </div>
      </div>
    </div>
  );
};

export default Dashboard;
