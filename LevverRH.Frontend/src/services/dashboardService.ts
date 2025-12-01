import apiClient from './api';

export interface DashboardStats {
  vagasAbertas: number;
  totalCandidaturas: number;
  candidaturasNovas: number;
  entrevistasAgendadas: number;
  taxaConversao: number;
}

export const dashboardService = {
  getStats: async (): Promise<DashboardStats> => {
    const response = await apiClient.get('/talents/dashboard/stats');
    return response.data.data;
  },
};
