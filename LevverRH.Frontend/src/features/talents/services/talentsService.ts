import apiClient from '../../../services/api';
import { DashboardStatsDTO, JobDTO, ApplicationDTO } from '../types/talents.types';
import { ApiResponse } from '../../../types/api.types';

/**
 * Serviço Talents
 * Comunica com os endpoints /api/talents/* do backend
 */
export const talentsService = {
  // ========== DASHBOARD ==========
  /**
   * Retorna as estatísticas do dashboard
   */
  getDashboardStats: async (): Promise<DashboardStatsDTO> => {
    const response = await apiClient.get<ApiResponse<DashboardStatsDTO>>('/talents/dashboard/stats');
    return response.data.data || {
      vagasAbertas: 0,
      totalCandidaturas: 0,
      candidaturasNovas: 0,
      entrevistasAgendadas: 0,
      taxaConversao: 0
    };
  },

  // ========== JOBS ==========
  /**
   * Retorna todas as vagas do tenant
   */
  getAllJobs: async (): Promise<JobDTO[]> => {
    const response = await apiClient.get<ApiResponse<JobDTO[]>>('/talents/jobs');
    return response.data.data || [];
  },

  /**
   * Retorna vagas por status
   */
  getJobsByStatus: async (status: string): Promise<JobDTO[]> => {
    const response = await apiClient.get<ApiResponse<JobDTO[]>>(`/talents/jobs/status/${status}`);
    return response.data.data || [];
  },

  // ========== APPLICATIONS ==========
  /**
   * Retorna candidaturas de uma vaga
   */
  getApplicationsByJob: async (jobId: string): Promise<ApplicationDTO[]> => {
    const response = await apiClient.get<ApiResponse<ApplicationDTO[]>>(`/talents/applications/job/${jobId}`);
    return response.data.data || [];
  },
};
