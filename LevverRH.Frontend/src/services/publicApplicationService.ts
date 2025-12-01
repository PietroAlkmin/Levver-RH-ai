import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5113/api';

// Cliente axios sem autenticação para rotas públicas
const publicClient = axios.create({
  baseURL: API_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

export interface PublicJobDetailDTO {
  id: string;
  titulo: string;
  descricao: string;
  requisitos?: string;
  beneficios?: string;
  cidade?: string;
  estado?: string;
  salarioMin?: number;
  salarioMax?: number;
  tipoContrato?: string;
  modalidade?: string;
  nomeEmpresa: string;
  logoEmpresa?: string;
}

export interface CreatePublicApplicationDTO {
  jobId: string;
  nome: string;
  email: string;
  telefone: string;
  linkedinUrl?: string;
  criarConta?: boolean;
  senha?: string;
}

export interface PublicApplicationResponseDTO {
  success: boolean;
  message: string;
  applicationId?: string;
  contaCriada?: boolean;
  accessToken?: string;
}

/**
 * Serviço para candidaturas públicas (sem autenticação)
 */
class PublicApplicationService {
  /**
   * Busca detalhes públicos de uma vaga
   */
  async getPublicJobDetail(jobId: string): Promise<PublicJobDetailDTO> {
    try {
      const response = await publicClient.get<PublicJobDetailDTO>(`/public/jobs/${jobId}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao buscar detalhes da vaga:', error);
      throw error;
    }
  }

  /**
   * Envia candidatura pública com currículo
   */
  async submitApplication(
    dto: CreatePublicApplicationDTO,
    curriculoFile: File
  ): Promise<PublicApplicationResponseDTO> {
    try {
      const formData = new FormData();
      formData.append('jobId', dto.jobId);
      formData.append('nome', dto.nome);
      formData.append('email', dto.email);
      formData.append('telefone', dto.telefone);
      
      if (dto.linkedinUrl) {
        formData.append('linkedinUrl', dto.linkedinUrl);
      }
      
      formData.append('criarConta', dto.criarConta ? 'true' : 'false');
      
      if (dto.criarConta && dto.senha) {
        formData.append('senha', dto.senha);
      }
      
      formData.append('curriculo', curriculoFile);

      const response = await publicClient.post<PublicApplicationResponseDTO>(
        '/public/applications',
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        }
      );

      return response.data;
    } catch (error: any) {
      // Captura mensagem específica do backend
      const errorMessage = error.response?.data?.message || error.response?.data?.Message || 'Erro ao enviar candidatura';
      throw new Error(errorMessage);
    }
  }
}

export const publicApplicationService = new PublicApplicationService();
