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
    <div
      className="min-h-screen flex items-center justify-center p-4"
      style={{
        background: 'linear-gradient(135deg, #713BDB 0%, #CC12EF 100%)',
      }}
    >
      {/* Card Branco Centralizado */}
      <div className="w-full max-w-md bg-white rounded-2xl shadow-2xl p-8">
        {/* Logo Dinâmica (puxa do white label ou mostra Levver padrão) */}
        <DynamicLogo />

        {/* Formulário */}
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {/* Email */}
          <div>
            <label
              htmlFor="email"
              className="block text-xs font-medium text-gray-700 mb-1.5"
            >
              email:
            </label>
            <input
              {...register('email')}
              id="email"
              type="email"
              placeholder=""
              disabled={isLoading}
              className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all text-sm"
              style={{
                borderImage: errors.email
                  ? 'none'
                  : 'linear-gradient(135deg, #713BDB, #CC12EF) 1',
              }}
            />
            {errors.email && (
              <p className="text-xs text-red-500 mt-1">{errors.email.message}</p>
            )}
          </div>

          {/* Senha */}
          <div>
            <label
              htmlFor="password"
              className="block text-xs font-medium text-gray-700 mb-1.5"
            >
              senha:
            </label>
            <input
              {...register('password')}
              id="password"
              type="password"
              placeholder=""
              disabled={isLoading}
              className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all text-sm"
            />
            {errors.password && (
              <p className="text-xs text-red-500 mt-1">{errors.password.message}</p>
            )}
          </div>

          {/* Botão LOGIN */}
          <button
            type="submit"
            disabled={isLoading}
            className="w-full py-3 rounded-lg font-semibold text-sm transition-all hover:opacity-90 disabled:opacity-50"
            style={{
              border: '2px solid #713BDB',
              color: '#713BDB',
              backgroundColor: 'white',
            }}
          >
            {isLoading ? 'CARREGANDO...' : 'LOGIN'}
          </button>
        </form>

        {/* Divisor */}
        <div className="relative my-6">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-gray-200"></div>
          </div>
          <div className="relative flex justify-center text-xs">
            <span className="px-3 bg-white text-gray-500">ou</span>
          </div>
        </div>

        {/* Botão Microsoft SSO */}
        <AzureAdLoginButton />

        {/* Divisor */}
        <div className="relative my-6">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-gray-200"></div>
          </div>
        </div>

        {/* Link Cadastro */}
        <div className="text-center">
          <Link
            to="/register-tenant"
            className="text-sm font-medium hover:underline"
            style={{ color: '#713BDB' }}
          >
            Cadastre sua empresa
          </Link>
        </div>
      </div>
    </div>
  );
};
