import apiClient from '../../../services/api';
import { 
  DashboardStatsDTO, 
  JobDTO, 
  ApplicationDTO,
  StartJobCreationDTO,
  JobChatMessageDTO,
  JobChatResponseDTO,
  ManualUpdateJobFieldDTO,
  CompleteJobCreationDTO,
  JobDetailDTO
} from '../types/talents.types';
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

  // ========== CRIAÇÃO DE VAGA COM IA ==========
  
  /**
   * Inicia criação de vaga assistida por IA
   * Cria uma vaga em rascunho e retorna a primeira pergunta da IA
   */
  startJobWithAI: async (dto: StartJobCreationDTO): Promise<JobChatResponseDTO> => {
    const response = await apiClient.post<ApiResponse<JobChatResponseDTO>>('/talents/jobs/ai/start', dto);
    return response.data.data!;
  },

  /**
   * Envia mensagem no chat de criação de vaga
   * A IA processa a mensagem e atualiza a vaga
   */
  sendChatMessage: async (dto: JobChatMessageDTO): Promise<JobChatResponseDTO> => {
    const response = await apiClient.post<ApiResponse<JobChatResponseDTO>>('/talents/jobs/ai/chat', dto);
    return response.data.data!;
  },

  /**
   * Atualiza campo manualmente e notifica IA sobre a mudança
   * A IA reconhece a alteração e pode ajustar suas próximas perguntas
   */
  manualUpdateField: async (dto: ManualUpdateJobFieldDTO): Promise<JobChatResponseDTO> => {
    const response = await apiClient.post<ApiResponse<JobChatResponseDTO>>('/talents/jobs/ai/manual-update', dto);
    return response.data.data!;
  },

  /**
   * Finaliza criação de vaga e opcionalmente publica
   */
  completeJobCreation: async (dto: CompleteJobCreationDTO): Promise<JobDetailDTO> => {
    const response = await apiClient.post<ApiResponse<JobDetailDTO>>('/talents/jobs/ai/complete', dto);
    return response.data.data!;
  },

  /**
   * Obtém detalhes completos de uma vaga
   */
  getJobDetail: async (jobId: string): Promise<JobDetailDTO> => {
    const response = await apiClient.get<ApiResponse<JobDetailDTO>>(`/talents/jobs/${jobId}/detail`);
    return response.data.data!;
  },
};
