import React from 'react';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useAuth } from '../../hooks/useAuth';
import { AzureAdLoginButton } from '../../components/auth/AzureAdLoginButton';
import { DynamicLogo } from '../../components/common/DynamicLogo';
import './Login.css';

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
    <div className="auth-container">
      <div className="auth-card">
        <DynamicLogo />

        <div style={{ marginTop: '2rem', marginBottom: '1.5rem' }}>
          <AzureAdLoginButton />
        </div>

        <div className="auth-divider">
          <div className="auth-divider-line"></div>
          <div className="auth-divider-text">
            <span>ou continue com email</span>
          </div>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="auth-form">
          <div>
            <label htmlFor="email" className="auth-label">
              Email
            </label>
            <input
              {...register('email')}
              id="email"
              type="email"
              placeholder="seu@email.com"
              disabled={isLoading}
              className="auth-input"
              style={{
                borderColor: errors.email ? 'var(--error-color)' : undefined,
              }}
            />
            {errors.email && (
              <p className="auth-error">{errors.email.message}</p>
            )}
          </div>

          <div>
            <label htmlFor="password" className="auth-label">
              Senha
            </label>
            <input
              {...register('password')}
              id="password"
              type="password"
              placeholder="••••••••"
              disabled={isLoading}
              className="auth-input"
              style={{
                borderColor: errors.password ? 'var(--error-color)' : undefined,
              }}
            />
            {errors.password && (
              <p className="auth-error">{errors.password.message}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="auth-button"
          >
            {isLoading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="auth-link-section">
          <p className="auth-link-text">
            Não tem uma conta?{' '}
            <Link to="/register-tenant" className="auth-link">
              Cadastre sua empresa
            </Link>
          </p>
        </div>

        <div className="auth-footer">
          <p className="auth-footer-text">
            © 2025 Levver. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </div>
  );
};
