import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Building2, User } from 'lucide-react';
import { Button, Input, Card } from '../../components/common';
import authService from '../../services/authService';
import toast from 'react-hot-toast';
import { useAuthStore } from '../../stores/authStore';

// Validação com Zod
const registerTenantSchema = z.object({
  // Dados da Empresa
  nomeEmpresa: z
    .string()
    .min(2, 'Nome da empresa deve ter no mínimo 2 caracteres'),
  cnpj: z
    .string()
    .min(14, 'CNPJ inválido')
    .regex(/^[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}-?[0-9]{2}$/, 'CNPJ inválido'),
  emailEmpresa: z
    .string()
    .min(1, 'Email da empresa é obrigatório')
    .email('Email inválido'),
  telefoneEmpresa: z
    .string()
    .optional(),
  enderecoEmpresa: z
    .string()
    .optional(),
  
  // Dados do Administrador (opcionais em modo SSO)
  nomeAdmin: z
    .string()
    .min(2, 'Nome do administrador deve ter no mínimo 2 caracteres')
    .optional()
    .or(z.literal('')),
  emailAdmin: z
    .string()
    .email('Email inválido')
    .optional()
    .or(z.literal('')),
  password: z
    .string()
    .min(6, 'Senha deve ter no mínimo 6 caracteres')
    .regex(/[A-Z]/, 'Senha deve conter ao menos uma letra maiúscula')
    .regex(/[a-z]/, 'Senha deve conter ao menos uma letra minúscula')
    .regex(/[0-9]/, 'Senha deve conter ao menos um número')
    .optional()
    .or(z.literal('')),
  confirmPassword: z
    .string()
    .optional()
    .or(z.literal('')),
}).refine((data) => {
  // Validar senha apenas se foi preenchida
  if (data.password && data.password.length > 0) {
    return data.password === data.confirmPassword;
  }
  return true;
}, {
  message: 'As senhas não coincidem',
  path: ['confirmPassword'],
});

type RegisterTenantFormData = z.infer<typeof registerTenantSchema>;

/**
 * Página de Cadastro de Empresa (Tenant)
 * Onboarding completo: empresa + usuário admin
 * 
 * Modos de operação:
 * 1. SSO (usuário logado via Azure AD): apenas dados da empresa (sem senha)
 * 2. Normal (novo cadastro): empresa + admin com senha
 */
export const RegisterTenant: React.FC = () => {
  const navigate = useNavigate();
  const { setAuth, user, tenant } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  // Detectar se é modo SSO (usuário já logado via Azure AD)
  const isSsoMode = user !== null && tenant?.status === 'PendenteSetup';

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterTenantFormData>({
    resolver: zodResolver(registerTenantSchema),
  });

  const onSubmit = async (data: RegisterTenantFormData) => {
    setIsLoading(true);
    
    console.log('📝 Submit do formulário - Modo SSO:', isSsoMode);
    console.log('📝 Dados do formulário:', data);
    
    try {
      let response;

      if (isSsoMode) {
        // Modo SSO: completar setup do tenant existente
        console.log('🔹 Chamando completeTenantSetup...');
        response = await authService.completeTenantSetup({
          nomeEmpresa: data.nomeEmpresa,
          cnpj: data.cnpj,
          emailEmpresa: data.emailEmpresa,
          telefoneEmpresa: data.telefoneEmpresa || undefined,
          enderecoEmpresa: data.enderecoEmpresa || undefined,
        });
        console.log('✅ Resposta completeTenantSetup:', response);
      } else {
        // Modo normal: registrar novo tenant com admin
        response = await authService.registerTenant({
          nomeEmpresa: data.nomeEmpresa,
          cnpj: data.cnpj,
          emailEmpresa: data.emailEmpresa,
          telefoneEmpresa: data.telefoneEmpresa || undefined,
          enderecoEmpresa: data.enderecoEmpresa || undefined,
          nomeAdmin: data.nomeAdmin || '',
          emailAdmin: data.emailAdmin || '',
          password: data.password || '',
          confirmPassword: data.confirmPassword || '',
        });
      }

      if (response.success && response.data) {
        // Salvar autenticação
        authService.saveAuthData(response.data);
        setAuth(
          response.data.token,
          response.data.user,
          response.data.tenant,
          response.data.whiteLabel
        );

        toast.success('Empresa cadastrada com sucesso! Bem-vindo ao LevverRH! 🎉');
        navigate('/painel');
      } else if (response.success && isSsoMode) {
        // Modo SSO: setup concluído, mas precisamos relogar para pegar tenant atualizado
        toast.success('Cadastro concluído! Redirecionando...');
        
        // Atualizar tenant no localStorage manualmente
        const currentTenant = authService.getCurrentTenant();
        if (currentTenant) {
          currentTenant.status = 'Ativo';
          currentTenant.nome = data.nomeEmpresa;
          currentTenant.cnpj = data.cnpj;
          currentTenant.email = data.emailEmpresa;
          currentTenant.telefone = data.telefoneEmpresa;
          currentTenant.endereco = data.enderecoEmpresa;
          localStorage.setItem('tenant', JSON.stringify(currentTenant));
          
          // Atualizar Zustand
          setAuth(
            localStorage.getItem('token') || '',
            authService.getCurrentUser(),
            currentTenant,
            null
          );
        }
        
        setTimeout(() => navigate('/painel'), 1000);
      } else {
        toast.error(response.message || 'Erro ao cadastrar empresa');
      }
    } catch (error: any) {
      console.error('❌ Erro ao cadastrar empresa:', error);
      console.error('❌ Detalhes do erro:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status,
      });
      toast.error(error.response?.data?.message || error.message || 'Erro ao cadastrar empresa');
    } finally {
      setIsLoading(false);
    }
  };

  // Formatar CNPJ enquanto digita
  const formatCnpj = (value: string) => {
    return value
      .replace(/\D/g, '')
      .replace(/^(\d{2})(\d)/, '$1.$2')
      .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
      .replace(/\.(\d{3})(\d)/, '.$1/$2')
      .replace(/(\d{4})(\d)/, '$1-$2')
      .slice(0, 18);
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4" style={{ background: 'radial-gradient(circle at top, rgba(113, 59, 219, 0.40) 0%, rgba(204, 18, 239, 0.15) 40%, #F9FAFB 70%)' }}>
      <div className="w-full max-w-2xl bg-white p-8" style={{ borderRadius: '8px', border: '1px solid #E5E7EB', boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)' }}>
        {/* Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 mb-4 rounded-full" style={{ background: 'linear-gradient(135deg, #713BDB 0%, #CC12EF 100%)' }}>
            <Building2 className="text-white" size={32} />
          </div>
          <h1 className="text-2xl font-bold" style={{ color: '#111827' }}>
            {isSsoMode ? 'Complete seu Cadastro' : 'Cadastrar Empresa'}
          </h1>
          <p className="text-sm mt-2" style={{ color: '#6B7280' }}>
            {isSsoMode
              ? 'Preencha os dados da sua empresa para finalizar'
              : 'Preencha os dados para começar a usar o Levver'}
          </p>
          {isSsoMode && user && (
            <div className="mt-4 inline-flex items-center gap-2 px-4 py-2" style={{ background: '#F9FAFB', borderRadius: '8px' }}>
              <User size={16} style={{ color: '#713BDB' }} />
              <span className="text-sm" style={{ color: '#111827' }}>
                Logado como: <strong>{user.nome}</strong> ({user.email})
              </span>
            </div>
          )}
        </div>

        {/* Formulário */}
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Seção: Dados da Empresa */}
          <div>
            <h2 className="text-lg font-semibold mb-4 flex items-center" style={{ color: '#111827' }}>
              <Building2 size={20} className="mr-2" style={{ color: '#713BDB' }} />
              Dados da Empresa
            </h2>
            <div className="space-y-4">
              <Input
                {...register('nomeEmpresa')}
                label="Nome da Empresa"
                placeholder="Ex: Levver Tecnologia LTDA"
                error={errors.nomeEmpresa?.message}
                disabled={isLoading}
                required
              />

              <Input
                {...register('cnpj')}
                label="CNPJ"
                placeholder="00.000.000/0000-00"
                error={errors.cnpj?.message}
                disabled={isLoading}
                required
                onChange={(e) => {
                  e.target.value = formatCnpj(e.target.value);
                }}
              />

              <Input
                {...register('emailEmpresa')}
                type="email"
                label="Email da Empresa"
                placeholder="contato@empresa.com.br"
                error={errors.emailEmpresa?.message}
                disabled={isLoading}
                required
              />

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                  {...register('telefoneEmpresa')}
                  label="Telefone (opcional)"
                  placeholder="(11) 99999-9999"
                  error={errors.telefoneEmpresa?.message}
                  disabled={isLoading}
                />

                <Input
                  {...register('enderecoEmpresa')}
                  label="Endereço (opcional)"
                  placeholder="Rua, Número, Cidade - UF"
                  error={errors.enderecoEmpresa?.message}
                  disabled={isLoading}
                />
              </div>
            </div>
          </div>

          {/* Divisor (apenas se não for SSO) */}
          {!isSsoMode && <div className="border-t border-gray-200 my-6"></div>}

          {/* Seção: Dados do Administrador (apenas se não for SSO) */}
          {!isSsoMode && (
            <div>
            <h2 className="text-lg font-semibold mb-4 flex items-center" style={{ color: '#111827' }}>
              <User size={20} className="mr-2" style={{ color: '#713BDB' }} />
              Dados do Administrador
            </h2>
            <div className="space-y-4">
              <Input
                {...register('nomeAdmin')}
                label="Nome Completo"
                placeholder="Ex: João Silva"
                error={errors.nomeAdmin?.message}
                disabled={isLoading}
                required
              />

              <Input
                {...register('emailAdmin')}
                type="email"
                label="Email do Administrador"
                placeholder="admin@empresa.com.br"
                error={errors.emailAdmin?.message}
                disabled={isLoading}
                required
              />

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <Input
                    {...register('password')}
                    type={showPassword ? 'text' : 'password'}
                    label="Senha"
                    placeholder="••••••••"
                    error={errors.password?.message}
                    disabled={isLoading}
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="text-sm hover:underline mt-1"
                    style={{ color: '#713BDB' }}
                  >
                    {showPassword ? 'Ocultar senha' : 'Mostrar senha'}
                  </button>
                </div>

                <Input
                  {...register('confirmPassword')}
                  type={showPassword ? 'text' : 'password'}
                  label="Confirmar Senha"
                  placeholder="••••••••"
                  error={errors.confirmPassword?.message}
                  disabled={isLoading}
                  required
                />
              </div>

              {/* Requisitos de Senha */}
              <div className="p-3" style={{ background: '#F9FAFB', border: '1px solid #E5E7EB', borderRadius: '8px' }}>
                <p className="text-xs font-medium mb-2" style={{ color: '#713BDB' }}>
                  A senha deve conter:
                </p>
                <ul className="text-xs space-y-1" style={{ color: '#6B7280' }}>
                  <li>✓ Mínimo de 6 caracteres</li>
                  <li>✓ Pelo menos uma letra maiúscula</li>
                  <li>✓ Pelo menos uma letra minúscula</li>
                  <li>✓ Pelo menos um número</li>
                </ul>
              </div>
            </div>
          </div>
          )}

          {/* Botão de Cadastro */}
          <Button
            type="submit"
            variant="primary"
            size="lg"
            fullWidth
            isLoading={isLoading}
            onClick={() => console.log('🔘 Botão clicado! Modo SSO:', isSsoMode)}
          >
            <Building2 size={20} />
            {isSsoMode ? 'Finalizar Cadastro' : 'Cadastrar Empresa'}
          </Button>
        </form>

        {/* Footer (apenas se não for SSO) */}
        {!isSsoMode && (
          <div className="mt-6 text-center">
            <p className="text-sm" style={{ color: '#6B7280' }}>
              Já tem uma conta?{' '}
              <Link
                to="/login"
                className="font-semibold hover:underline"
                style={{ color: '#713BDB' }}
              >
                Fazer login
              </Link>
            </p>
          </div>
        )}

        <div className="mt-8 pt-6 text-center" style={{ borderTop: '1px solid #E5E7EB' }}>
          <p className="text-xs" style={{ color: '#6B7280' }}>
            &copy; 2025 Levver. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  );
};
