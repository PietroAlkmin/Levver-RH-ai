import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { ErrorResponse } from '../types/api.types';

// Configura��o base do Axios
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

    console.log(`🌐 API Request - ${config.method?.toUpperCase()} ${config.url}`);
    console.log('🔑 API Request - Token:', token ? token.substring(0, 50) + '...' : 'NO TOKEN');

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
  response => {
    console.log(`✅ API Response - ${response.config.method?.toUpperCase()} ${response.config.url} - Status: ${response.status}`);
    return response;
  },
  (error: AxiosError<ErrorResponse>) => {
    console.log(`❌ API Error - ${error.config?.method?.toUpperCase()} ${error.config?.url} - Status: ${error.response?.status}`);
    console.log('❌ API Error - Response:', error.response?.data);

    // Unauthorized - Token expirado ou inválido
    if (error.response?.status === 401) {
      console.log('🚨 401 Unauthorized - Redirecionando para /login');
      console.log('🚨 401 - URL que causou:', error.config?.url);
      console.log('🚨 401 - Token atual:', localStorage.getItem('token')?.substring(0, 50) + '...');
      
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
