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
  
  // Dados do Administrador
  nomeAdmin: z
    .string()
    .min(2, 'Nome do administrador deve ter no mínimo 2 caracteres'),
  emailAdmin: z
    .string()
    .min(1, 'Email do administrador é obrigatório')
    .email('Email inválido'),
  password: z
    .string()
    .min(6, 'Senha deve ter no mínimo 6 caracteres')
    .regex(/[A-Z]/, 'Senha deve conter ao menos uma letra maiúscula')
    .regex(/[a-z]/, 'Senha deve conter ao menos uma letra minúscula')
    .regex(/[0-9]/, 'Senha deve conter ao menos um número'),
  confirmPassword: z
    .string()
    .min(1, 'Confirmação de senha é obrigatória'),
}).refine((data) => data.password === data.confirmPassword, {
  message: 'As senhas não coincidem',
  path: ['confirmPassword'],
});

type RegisterTenantFormData = z.infer<typeof registerTenantSchema>;

/**
 * Página de Cadastro de Empresa (Tenant)
 * Onboarding completo: empresa + usuário admin
 */
export const RegisterTenant: React.FC = () => {
  const navigate = useNavigate();
  const { setAuth } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterTenantFormData>({
    resolver: zodResolver(registerTenantSchema),
  });

  const onSubmit = async (data: RegisterTenantFormData) => {
    setIsLoading(true);
    try {
      const response = await authService.registerTenant({
        nomeEmpresa: data.nomeEmpresa,
        cnpj: data.cnpj,
        emailEmpresa: data.emailEmpresa,
        telefoneEmpresa: data.telefoneEmpresa || undefined,
        enderecoEmpresa: data.enderecoEmpresa || undefined,
        nomeAdmin: data.nomeAdmin,
        emailAdmin: data.emailAdmin,
        password: data.password,
        confirmPassword: data.confirmPassword,
      });

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
        navigate('/dashboard');
      } else {
        toast.error(response.message || 'Erro ao cadastrar empresa');
      }
    } catch (error: any) {
      console.error('Erro ao cadastrar empresa:', error);
      toast.error(error.response?.data?.message || 'Erro ao cadastrar empresa');
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
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <Card className="w-full max-w-2xl">
        {/* Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-full mb-4">
            <Building2 className="text-white" size={32} />
          </div>
          <h1 className="text-3xl font-bold text-gray-900">Cadastrar Empresa</h1>
          <p className="text-gray-600 mt-2">
            Preencha os dados para começar a usar o LevverRH
          </p>
        </div>

        {/* Formulário */}
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Seção: Dados da Empresa */}
          <div>
            <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
              <Building2 size={20} className="mr-2 text-blue-600" />
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

          {/* Divisor */}
          <div className="border-t border-gray-200 my-6"></div>

          {/* Seção: Dados do Administrador */}
          <div>
            <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
              <User size={20} className="mr-2 text-blue-600" />
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
                    className="text-sm text-blue-600 hover:text-blue-700 mt-1"
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
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                <p className="text-xs text-blue-800 font-medium mb-2">
                  A senha deve conter:
                </p>
                <ul className="text-xs text-blue-700 space-y-1">
                  <li>✓ Mínimo de 6 caracteres</li>
                  <li>✓ Pelo menos uma letra maiúscula</li>
                  <li>✓ Pelo menos uma letra minúscula</li>
                  <li>✓ Pelo menos um número</li>
                </ul>
              </div>
            </div>
          </div>

          {/* Botão de Cadastro */}
          <Button
            type="submit"
            variant="primary"
            size="lg"
            fullWidth
            isLoading={isLoading}
          >
            <Building2 size={20} />
            Cadastrar Empresa
          </Button>
        </form>

        {/* Footer */}
        <div className="mt-6 text-center">
          <p className="text-gray-600">
            Já tem uma conta?{' '}
            <Link
              to="/login"
              className="text-blue-600 hover:text-blue-700 font-semibold"
            >
              Fazer login
            </Link>
          </p>
        </div>

        <div className="mt-8 pt-6 border-t border-gray-200 text-center">
          <p className="text-xs text-gray-500">
            &copy; 2025 LevverRH. Todos os direitos reservados.
          </p>
        </div>
      </Card>
    </div>
  );
};
