import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { ErrorResponse } from '../types/api.types';

// Configuração base do Axios
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5113/api';

const apiClient: AxiosInstance = axios.create({
  baseURL: API_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor - Adiciona token JWT
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('token');

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response Interceptor - Trata erros globalmente
apiClient.interceptors.response.use(
  response => response,
  (error: AxiosError<ErrorResponse>) => {
    // Unauthorized - Token expirado ou inválido
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }

    // Forbidden - Sem permissão
    if (error.response?.status === 403) {
      console.error('Acesso negado');
    }

    // Server Error
  if (error.response?.status && error.response.status >= 500) {
      console.error('Erro no servidor. Tente novamente mais tarde.');
}

    return Promise.reject(error);
  }
);

export default apiClient;
