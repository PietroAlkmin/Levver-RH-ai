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
import './RegisterTenant.css';

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
    <div className="auth-container">
      <div className="auth-card auth-card-wide">
        <div className="register-header">
          <div className="register-icon">
            <Building2 size={32} />
          </div>
          <h1 className="register-title">
            {isSsoMode ? 'Complete seu Cadastro' : 'Cadastrar Empresa'}
          </h1>
          <p className="register-subtitle">
            {isSsoMode
              ? 'Preencha os dados da sua empresa para finalizar'
              : 'Preencha os dados para começar a usar o Levver'}
          </p>
          {isSsoMode && user && (
            <div className="register-sso-badge">
              <User size={16} />
              <span className="register-sso-text">
                Logado como: <strong>{user.nome}</strong> ({user.email})
              </span>
            </div>
          )}
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="auth-form">
          <div className="register-section">
            <h2 className="register-section-title">
              <Building2 size={20} />
              Dados da Empresa
            </h2>
            <div className="auth-form">
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

              <div className="register-grid register-grid-2">
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

          {!isSsoMode && <div className="register-divider"></div>}

          {!isSsoMode && (
            <div className="register-section">
            <h2 className="register-section-title">
              <User size={20} />
              Dados do Administrador
            </h2>
            <div className="auth-form">
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

              <div className="register-grid register-grid-2">
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
                    className="register-toggle"
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

              <div className="register-password-hint">
                <p className="register-password-hint-title">
                  A senha deve conter:
                </p>
                <ul className="register-password-hint-list">
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

        {!isSsoMode && (
          <div className="auth-link-section">
            <p className="auth-link-text">
              Já tem uma conta?{' '}
              <Link to="/login" className="auth-link">
                Fazer login
              </Link>
            </p>
          </div>
        )}

        <div className="auth-footer">
          <p className="auth-footer-text">
            &copy; 2025 Levver. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  );
};
