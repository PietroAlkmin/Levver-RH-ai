// Talents Types - Espelham os DTOs do backend C#

// ========== ENUMS ==========
export enum JobStatus {
  Rascunho = 'Rascunho',
  Aberta = 'Aberta',
  Pausada = 'Pausada',
  Fechada = 'Fechada'
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
  tipoContrato?: string;
  modeloTrabalho?: string;
  salarioMin?: number;
  salarioMax?: number;
  beneficios?: string;
  status: string;
  numeroVagas: number;
  dataCriacao: string;
  totalCandidaturas: number;
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
