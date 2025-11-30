// Talents Types - Espelham os DTOs do backend C#

// ========== ENUMS ==========
export enum JobStatus {
  Rascunho = 'Rascunho',
  Aberta = 'Aberta',
  Pausada = 'Pausada',
  Fechada = 'Fechada'
}

export enum ContractType {
  CLT = 'CLT',
  PJ = 'PJ',
  Estagio = 'Estagio',
  Temporario = 'Temporario'
}

export enum WorkModel {
  Presencial = 'Presencial',
  Remoto = 'Remoto',
  Hibrido = 'Hibrido'
}

export enum ApplicationStatus {
  Novo = 'Novo',
  Triagem = 'Triagem',
  Entrevista = 'Entrevista',
  Aprovado = 'Aprovado',
  Rejeitado = 'Rejeitado',
  Desistiu = 'Desistiu'
}

// ========== DTOs ==========
export interface DashboardStatsDTO {
  vagasAbertas: number;
  totalCandidaturas: number;
  candidaturasNovas: number;
  entrevistasAgendadas: number;
  taxaConversao: number;
}

export interface JobDTO {
  id: string;
  titulo: string;
  descricao: string;
  departamento?: string;
  localizacao?: string;
  cidade?: string;
  estado?: string;
  tipoContrato?: string;
  modeloTrabalho?: string;
  salarioMin?: number;
  salarioMax?: number;
  beneficios?: string;
  status: string;
  numeroVagas: number;
  dataCriacao: string;
  totalCandidaturas: number;
  iaCompletionPercentage?: number;
}

export interface JobDetailDTO {
  // Identificação
  id: string;
  tenantId: string;
  criadoPor: string;

  // Informações Básicas
  titulo: string;
  descricao: string;
  departamento?: string;
  numeroVagas: number;

  // Localização e Formato
  localizacao?: string;
  cidade?: string;
  estado?: string;
  tipoContrato?: string;
  modeloTrabalho?: string;

  // Requisitos
  anosExperienciaMinimo?: number;
  formacaoNecessaria?: string;
  conhecimentosObrigatorios?: string[];
  conhecimentosDesejaveis?: string[];
  competenciasImportantes?: string[];

  // Responsabilidades
  responsabilidades?: string;

  // Remuneração
  salarioMin?: number;
  salarioMax?: number;
  beneficios?: string;
  bonusComissao?: string;

  // Processo Seletivo
  etapasProcesso?: string[];
  tiposTesteEntrevista?: string[];
  previsaoInicio?: string;

  // Sobre a Vaga
  sobreTime?: string;
  diferenciais?: string;

  // Controle IA
  conversationId?: string;
  iaCompletionPercentage?: number;

  // Status e Auditoria
  status: string;
  dataCriacao: string;
  dataAtualizacao: string;
  dataFechamento?: string;

  // Contadores
  totalCandidaturas: number;
}

export interface CreateJobDTO {
  titulo: string;
  descricao: string;
  departamento?: string;
  localizacao?: string;
  cidade?: string;
  estado?: string;
  tipoContrato?: ContractType;
  modeloTrabalho?: WorkModel;
  salarioMin?: number;
  salarioMax?: number;
  beneficios?: string;
  numeroVagas?: number;
}

export interface ApplicationDTO {
  id: string;
  jobId: string;
  jobTitulo: string;
  candidateId: string;
  candidateNome: string;
  candidateEmail: string;
  status: string;
  dataInscricao: string;
  scoreGeral?: number;
  favorito: boolean;
}

// ========== DTOs de Criação com IA ==========

export interface StartJobCreationDTO {
  mensagemInicial?: string;
}

export interface JobChatMessageDTO {
  jobId: string;
  Mensagem: string; // Note: Backend espera com M maiúsculo
}

export interface JobChatResponseDTO {
  jobId: string;
  conversationId: string;
  mensagemIA: string;
  camposAtualizados: string[];
  completionPercentage: number;
  criacaoConcluida: boolean;
  jobAtualizado?: JobDetailDTO;
}

export interface ManualUpdateJobFieldDTO {
  jobId: string;
  fieldName: string;
  fieldValue?: string;
  userMessage?: string;
}

export interface CompleteJobCreationDTO {
  jobId: string;
  publicarImediatamente?: boolean;
}
