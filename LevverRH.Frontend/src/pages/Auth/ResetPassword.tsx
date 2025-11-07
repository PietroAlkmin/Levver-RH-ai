import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Lock, Mail, KeyRound } from 'lucide-react';
import { Button, Input, Card } from '../../components/common';
import apiClient from '../../services/api';
import toast from 'react-hot-toast';

// Validação
const resetPasswordSchema = z.object({
  email: z.string().min(1, 'Email é obrigatório').email('Email inválido'),
  newPassword: z.string().min(6, 'Senha deve ter no mínimo 6 caracteres'),
  confirmPassword: z.string().min(6, 'Confirmação é obrigatória'),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: 'As senhas não coincidem',
  path: ['confirmPassword'],
});

type ResetPasswordFormData = z.infer<typeof resetPasswordSchema>;

/**
 * Página de Redefinir Senha (Versão Simples para Testes)
 * Apenas email + nova senha, sem token
 */
export const ResetPassword: React.FC = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = React.useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ResetPasswordFormData>({
    resolver: zodResolver(resetPasswordSchema),
  });

  const onSubmit = async (data: ResetPasswordFormData) => {
    setIsLoading(true);

    try {
      const response = await apiClient.post('/auth/reset-password', {
        email: data.email,
        newPassword: data.newPassword,
      });

      if (response.data.success) {
        toast.success('Senha redefinida com sucesso!');
        setTimeout(() => navigate('/login'), 2000);
      } else {
        toast.error(response.data.message || 'Erro ao redefinir senha');
      }
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || 'Erro ao redefinir senha';
      toast.error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <Card className="w-full max-w-md">
        {/* Logo e Título */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-full mb-4">
            <KeyRound className="text-white" size={32} />
          </div>
          <h1 className="text-3xl font-bold text-gray-900">Redefinir Senha</h1>
          <p className="text-gray-600 mt-2">Digite seu email e a nova senha</p>
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

          {/* Nova Senha */}
          <div>
            <Input
              {...register('newPassword')}
              type="password"
              label="Nova Senha"
              placeholder="••••••••"
              error={errors.newPassword?.message}
              disabled={isLoading}
              required
            />
          </div>

          {/* Confirmar Senha */}
          <div>
            <Input
              {...register('confirmPassword')}
              type="password"
              label="Confirmar Nova Senha"
              placeholder="••••••••"
              error={errors.confirmPassword?.message}
              disabled={isLoading}
              required
            />
          </div>

          {/* Botão */}
          <Button
            type="submit"
            variant="primary"
            size="lg"
            fullWidth
            isLoading={isLoading}
          >
            <Lock size={20} />
            Redefinir Senha
          </Button>
        </form>

        {/* Voltar para Login */}
        <div className="text-center mt-6">
          <Link
            to="/login"
            className="text-sm text-blue-600 hover:text-blue-700 font-medium"
          >
            ← Voltar para o login
          </Link>
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
