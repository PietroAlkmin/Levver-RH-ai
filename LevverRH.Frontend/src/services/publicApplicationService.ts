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
      formData.append('JobId', dto.jobId);
      formData.append('Nome', dto.nome);
      formData.append('Email', dto.email);
      formData.append('Telefone', dto.telefone);
      if (dto.linkedinUrl) {
        formData.append('LinkedinUrl', dto.linkedinUrl);
      }
      if (dto.criarConta) {
        formData.append('CriarConta', 'true');
        if (dto.senha) {
          formData.append('Senha', dto.senha);
        }
      }
      formData.append('Curriculo', curriculoFile);

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
    } catch (error) {
      console.error('Erro ao enviar candidatura:', error);
      throw error;
    }
  }
}

export const publicApplicationService = new PublicApplicationService();
