import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Share2 } from 'lucide-react';
import { JobDTO } from '../types/talents.types';
import { talentsService } from '../services/talentsService';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import './JobsList.css';

export const JobsList: React.FC = () => {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState<JobDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Filtros
  const [searchTerm, setSearchTerm] = useState('');
  const [locationFilter, setLocationFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [departmentFilter, setDepartmentFilter] = useState('');

  useEffect(() => {
    loadJobs();
  }, []);

  const loadJobs = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await talentsService.getAllJobs();
      setJobs(data);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar vagas');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNewJob = () => {
    navigate('/talents/jobs/new');
  };

  const handleViewJob = (jobId: string) => {
    navigate(`/talents/vagas/${jobId}`);
  };

  // Função para determinar se a vaga está completa (>= 80%)
  const isJobComplete = (job: JobDTO): boolean => {
    return (job.iaCompletionPercentage ?? 0) >= 80;
  };

  // Função para determinar o texto do botão
  const getJobButtonText = (job: JobDTO): string => {
    return isJobComplete(job) ? 'Ver vaga' : 'Finalizar a vaga';
  };

  // Função para lidar com clique no botão da vaga
  const handleJobAction = (job: JobDTO) => {
    if (isJobComplete(job)) {
      handleViewJob(job.id);
    } else {
      // Redireciona para continuar a criação com IA
      navigate(`/talents/jobs/new?jobId=${job.id}`);
    }
  };

  // Função para copiar link de candidatura
  const handleShareJob = (jobId: string, e: React.MouseEvent) => {
    e.stopPropagation(); // Prevenir click no card
    const candidaturaUrl = `${window.location.origin}/candidatura/${jobId}`;
    
    navigator.clipboard.writeText(candidaturaUrl)
      .then(() => {
        toast.success('Link de candidatura copiado!');
      })
      .catch(() => {
        toast.error('Erro ao copiar link');
      });
  };

  // Função para normalizar strings (remove acentos e caracteres especiais)
  const normalizeString = (str: string | undefined | null): string => {
    if (!str) return '';
    return str
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '') // Remove acentos
      .trim();
  };

  // Filtrar vagas - apenas mostrar vagas com progresso >= 80%
  const filteredJobs = jobs.filter(job => {
    // Primeiro verifica se a vaga tem pelo menos 80% de progresso
    const hasMinProgress = (job.iaCompletionPercentage ?? 0) >= 80;
    
    // Busca por título (normalizada, sem acentos)
    const matchesSearch = normalizeString(job.titulo).includes(normalizeString(searchTerm));
    
    // Localização (normalizada, busca em cidade, estado e localizacao)
    const normalizedFilter = normalizeString(locationFilter);
    const matchesLocation = !locationFilter || 
      normalizeString(job.localizacao).includes(normalizedFilter) ||
      normalizeString(job.cidade).includes(normalizedFilter) ||
      normalizeString(job.estado).includes(normalizedFilter) ||
      normalizeString(job.estado) === normalizedFilter;
    
    // Status (normalizado, comparação exata)
    const matchesStatus = !statusFilter || 
      normalizeString(job.status) === normalizeString(statusFilter);
    
    // Departamento (normalizado, comparação exata)
    const matchesDepartment = !departmentFilter || 
      normalizeString(job.departamento) === normalizeString(departmentFilter);
    
    return hasMinProgress && matchesSearch && matchesLocation && matchesStatus && matchesDepartment;
  });

  const getStatusBadgeClass = (status: string) => {
    switch (status.toLowerCase()) {
      case 'aberta': return 'status-badge status-aberta';
      case 'rascunho': return 'status-badge status-rascunho';
      case 'fechada': return 'status-badge status-fechada';
      default: return 'status-badge';
    }
  };

  const clearFilters = () => {
    setSearchTerm('');
    setLocationFilter('');
    setStatusFilter('');
    setDepartmentFilter('');
  };

  if (loading) {
    return (
      <MainLayout>
        <div className="jobs-list-container">
          <div className="loading-state">
            <div className="spinner"></div>
            <p>Carregando vagas...</p>
          </div>
        </div>
      </MainLayout>
    );
  }

  if (error) {
    return (
      <MainLayout>
        <div className="jobs-list-container">
          <div className="error-state">
            <p>{error}</p>
            <button onClick={loadJobs} className="btn-retry">Tentar novamente</button>
          </div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="jobs-list-container">
      {/* Header */}
      <div className="jobs-list-header">
        <h1>Vagas</h1>
        <button className="btn-create-job" onClick={handleCreateNewJob}>
          Criar Nova Vaga com IA
        </button>
      </div>

      {/* Filtros */}
      <div className="jobs-filters">
        <div className="filter-row">
          <div className="search-box">
            <input
              type="text"
              placeholder="Buscar vagas: React.js, Node.js"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="search-input"
            />
          </div>

          <div className="location-box">
            <input
              type="text"
              placeholder="Localização"
              value={locationFilter}
              onChange={(e) => setLocationFilter(e.target.value)}
              className="location-input"
            />
          </div>

          <button className="btn-clear-filters" onClick={clearFilters}>
            Limpar
          </button>
        </div>

        <div className="filter-row-secondary">
          <select 
            value={departmentFilter} 
            onChange={(e) => setDepartmentFilter(e.target.value)}
            className="filter-select"
          >
            <option value="">Departamento</option>
            <option value="tecnologia">Tecnologia</option>
            <option value="vendas">Vendas</option>
            <option value="marketing">Marketing</option>
            <option value="rh">RH</option>
            <option value="financeiro">Financeiro</option>
          </select>

          <select 
            value={statusFilter} 
            onChange={(e) => setStatusFilter(e.target.value)}
            className="filter-select"
          >
            <option value="">Status</option>
            <option value="Aberta">Aberta</option>
            <option value="Rascunho">Rascunho</option>
            <option value="Fechada">Fechada</option>
          </select>
        </div>
      </div>

      {/* Lista de Vagas */}
      <div className="jobs-count">
        {filteredJobs.length} {filteredJobs.length === 1 ? 'vaga encontrada' : 'vagas encontradas'}
      </div>

      {filteredJobs.length === 0 ? (
        <div className="empty-state">
          <div className="empty-illustration"></div>
          <h3>Nenhuma vaga encontrada</h3>
          <p>Crie sua primeira vaga com IA ou ajuste os filtros</p>
          <button className="btn-create-job" onClick={handleCreateNewJob}>
            Criar Nova Vaga com IA
          </button>
        </div>
      ) : (
        <div className="jobs-grid">
          {filteredJobs.map((job) => (
            <div 
              key={job.id} 
              className="job-card"
              onClick={() => isJobComplete(job) && handleViewJob(job.id)}
              style={{ cursor: isJobComplete(job) ? 'pointer' : 'default' }}
            >
              {/* Header do Card */}
              <div className="job-card-header">
                <div className="job-icon">
                  <div className="icon-placeholder" style={{ backgroundColor: getRandomColor(job.id) }}>
                    {job.titulo.charAt(0).toUpperCase()}
                  </div>
                </div>
                <div className="job-header-info">
                  <h3 className="job-title">{job.titulo}</h3>
                  <div className="job-meta">
                    <span className="job-department">{job.departamento || 'Não informado'}</span>
                    <span className="job-type">• {job.tipoContrato || 'N/A'}</span>
                  </div>
                </div>
              </div>

              {/* Informações */}
              <div className="job-card-body">
                <div className="job-info-row">
                  <span className="info-label">Localização:</span>
                  <span className="info-value">{job.localizacao || 'Remoto'}</span>
                </div>

                <div className="job-info-row">
                  <span className="info-label">Salário:</span>
                  <span className="info-value">
                    {job.salarioMin && job.salarioMax 
                      ? `R$ ${job.salarioMin.toLocaleString()} - R$ ${job.salarioMax.toLocaleString()}`
                      : 'A combinar'}
                  </span>
                </div>

                <div className="job-info-row">
                  <span className="info-label">Candidaturas:</span>
                  <span className="info-value">{job.totalCandidaturas}</span>
                </div>

                <div className="job-info-row">
                  <span className="info-label">Criada em:</span>
                  <span className="info-value">{new Date(job.dataCriacao).toLocaleDateString('pt-BR')}</span>
                </div>
              </div>

              {/* Status Badge */}
              <div className="job-card-status">
                <span className={getStatusBadgeClass(job.status)}>{job.status}</span>
              </div>

              {/* Ações */}
              <div className="job-card-actions">
                <button 
                  className="btn-view-job"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleJobAction(job);
                  }}
                >
                  {getJobButtonText(job)}
                </button>
                {isJobComplete(job) && (
                  <button
                    className="btn-share-job"
                    onClick={(e) => handleShareJob(job.id, e)}
                    title="Copiar link de candidatura"
                  >
                    <Share2 className="w-4 h-4" />
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
    </MainLayout>
  );
};

// Helper para gerar cores consistentes baseadas no ID
function getRandomColor(id: string): string {
  const colors = [
    '#667eea', // Roxo
    '#764ba2', // Roxo escuro
    '#f093fb', // Rosa
    '#4facfe', // Azul claro
    '#43e97b', // Verde
    '#fa709a', // Rosa claro
  ];
  
  const hash = id.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0);
  return colors[hash % colors.length];
}
