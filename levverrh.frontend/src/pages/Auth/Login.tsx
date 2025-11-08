import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Mail, Lock, LogIn } from 'lucide-react';
import { useAuth } from '../../hooks/useAuth';
import { Button, Input, Card } from '../../components/common';
import { AzureAdLoginButton } from '../../components/auth/AzureAdLoginButton';

// Validação com Zod (espelha FluentValidation do backend)
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

/**
 * Página de Login
 * Segue padrões de UX de grandes corporações
 */
export const Login: React.FC = () => {
  const { login, isLoading } = useAuth();
  const [showPassword, setShowPassword] = useState(false);

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
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
  <Card className="w-full max-w-md">
        {/* Logo e Título */}
<div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-full mb-4">
 <LogIn className="text-white" size={32} />
 </div>
   <h1 className="text-3xl font-bold text-gray-900">LevverRH</h1>
          <p className="text-gray-600 mt-2">Faça login para continuar</p>
      </div>

    {/* Formulário */}
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Email */}
          <div>
 <Input
      {...register('email')}
     type="email"
       label="Email"
              placeholder="seu@email.com"
     error={errors.email?.message}
        disabled={isLoading}
  required
 />
          </div>

{/* Senha */}
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

    {/* Esqueci a senha */}
     <div className="text-right">
  <Link
              to="/forgot-password"
       className="text-sm text-blue-600 hover:text-blue-700 font-medium"
       >
        Esqueceu a senha?
            </Link>
          </div>

          {/* Botão de Login */}
   <Button
            type="submit"
            variant="primary"
            size="lg"
  fullWidth
            isLoading={isLoading}
    >
    <LogIn size={20} />
            Entrar
          </Button>
        </form>

        {/* Divisor */}
        <div className="relative my-6">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-gray-300"></div>
          </div>
          <div className="relative flex justify-center text-sm">
            <span className="px-2 bg-white text-gray-500">Ou</span>
          </div>
        </div>

        {/* Login com Microsoft */}
        <AzureAdLoginButton />

        {/* Divisor */}
        <div className="relative my-6">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-gray-300"></div>
          </div>
        </div>

        {/* Cadastro */}
<div className="text-center">
          <p className="text-gray-600">
  Não tem uma conta?{' '}
            <Link
              to="/register-tenant"
              className="text-blue-600 hover:text-blue-700 font-semibold"
            >
   Cadastre sua empresa
  </Link>
          </p>
   </div>

        {/* Footer */}
        <div className="mt-8 pt-6 border-t border-gray-200 text-center">
     <p className="text-xs text-gray-500">
       &copy; 2025 LevverRH. Todos os direitos reservados.
          </p>
        </div>
      </Card>
    </div>
  );
};
