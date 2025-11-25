import React from 'react';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useAuth } from '../../hooks/useAuth';
import { AzureAdLoginButton } from '../../components/auth/AzureAdLoginButton';
import { DynamicLogo } from '../../components/common/DynamicLogo';

// Validação com Zod
const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'Email é obrigatório')
    .email('Email inválido'),
  password: z
    .string()
    .min(6, 'Senha deve ter no mínimo 6 caracteres'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export const Login: React.FC = () => {
  const { login, isLoading } = useAuth();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    await login(data);
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4" style={{ background: 'radial-gradient(circle at top, rgba(113, 59, 219, 0.40) 0%, rgba(204, 18, 239, 0.15) 40%, #F9FAFB 70%)' }}>
      {/* Card Centralizado */}
      <div className="w-full max-w-md bg-white p-8" style={{ borderRadius: '8px', border: '1px solid #E5E7EB', boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)' }}>
        {/* Logo Dinâmica */}
        <DynamicLogo />

        {/* Botão Microsoft SSO - Destaque */}
        <div className="mb-6">
          <AzureAdLoginButton />
        </div>

        {/* Divisor */}
        <div className="relative my-6">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full" style={{ borderTop: '1px solid #E5E7EB' }}></div>
          </div>
          <div className="relative flex justify-center text-xs">
            <span className="px-3 bg-white" style={{ color: '#6B7280' }}>ou continue com email</span>
          </div>
        </div>

        {/* Formulário de Login */}
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Email */}
          <div>
            <label
              htmlFor="email"
              className="block text-sm font-medium mb-1.5"
              style={{ color: '#111827' }}
            >
              Email
            </label>
            <input
              {...register('email')}
              id="email"
              type="email"
              placeholder="seu@email.com"
              disabled={isLoading}
              className="w-full px-4 py-2.5 text-sm transition-all focus:outline-none"
              style={{
                border: '1px solid #E5E7EB',
                borderRadius: '8px',
                color: '#111827'
              }}
              onFocus={(e) => e.target.style.border = '1px solid #713BDB'}
              onBlur={(e) => e.target.style.border = '1px solid #E5E7EB'}
            />
            {errors.email && (
              <p className="text-xs mt-1.5" style={{ color: '#E84358' }}>{errors.email.message}</p>
            )}
          </div>

          {/* Senha */}
          <div>
            <label
              htmlFor="password"
              className="block text-sm font-medium mb-1.5"
              style={{ color: '#111827' }}
            >
              Senha
            </label>
            <input
              {...register('password')}
              id="password"
              type="password"
              placeholder="••••••••"
              disabled={isLoading}
              className="w-full px-4 py-2.5 text-sm transition-all focus:outline-none"
              style={{
                border: '1px solid #E5E7EB',
                borderRadius: '8px',
                color: '#111827'
              }}
              onFocus={(e) => e.target.style.border = '1px solid #713BDB'}
              onBlur={(e) => e.target.style.border = '1px solid #E5E7EB'}
            />
            {errors.password && (
              <p className="text-xs mt-1.5" style={{ color: '#E84358' }}>{errors.password.message}</p>
            )}
          </div>

          {/* Botão Login */}
          <button
            type="submit"
            disabled={isLoading}
            className="w-full py-2.5 font-semibold text-sm text-white transition-all hover:opacity-90 disabled:opacity-50 mt-6"
            style={{
              background: 'linear-gradient(135deg, #713BDB 0%, #CC12EF 100%)',
              borderRadius: '8px',
              border: 'none'
            }}
          >
            {isLoading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        {/* Link Cadastro */}
        <div className="mt-6 text-center">
          <p className="text-sm" style={{ color: '#6B7280' }}>
            Não tem uma conta?{' '}
            <Link
              to="/register-tenant"
              className="font-semibold hover:underline"
              style={{ color: '#713BDB' }}
            >
              Cadastre sua empresa
            </Link>
          </p>
        </div>

        {/* Footer */}
        <div className="mt-8 pt-6 text-center" style={{ borderTop: '1px solid #E5E7EB' }}>
          <p className="text-xs" style={{ color: '#6B7280' }}>
            © 2025 Levver. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  );
};
