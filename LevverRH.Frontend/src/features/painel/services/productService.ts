import apiClient from '../../../services/api';
import { Product, TenantProduct } from '../types/product.types';

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export const productService = {
  /**
   * Retorna todos os produtos disponíveis no catálogo
   */
  getAllProducts: async (): Promise<Product[]> => {
    const response = await apiClient.get<ApiResponse<Product[]>>('/products');
    return response.data.data || [];
  },

  /**
   * Retorna os produtos que o tenant logado tem acesso
   */
  getMyProducts: async (): Promise<TenantProduct[]> => {
    const response = await apiClient.get<ApiResponse<TenantProduct[]>>('/products/my-products');
    return response.data.data || [];
  },

  /**
   * Verifica se o tenant tem acesso a um produto específico
   */
  hasAccessToProduct: async (productId: string): Promise<boolean> => {
    const response = await apiClient.get<ApiResponse<boolean>>(`/products/has-access/${productId}`);
    return response.data.data || false;
  },
};
