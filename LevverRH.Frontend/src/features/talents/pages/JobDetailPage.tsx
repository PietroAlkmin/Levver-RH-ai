import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Search, ChevronDown, ChevronUp, Edit, ArrowUpDown, Calendar, SortAsc, Eye, MapPin, Briefcase, Users, Share2, ArrowLeft } from 'lucide-react';
import { talentsService } from '../services/talentsService';
import { JobDetailDTO, ApplicationDTO } from '../types/talents.types';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import './JobDetailPage.css';

export const JobDetailPage: React.FC = () => {
  const { jobId } = useParams<{ jobId: string }>();
  const navigate = useNavigate();
  
  const [job, setJob] = useState<JobDetailDTO | null>(null);
  const [applications, setApplications] = useState<ApplicationDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [isJobExpanded, setIsJobExpanded] = useState(false);
  
  // Filtros
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState<'date' | 'name'>('date');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

  useEffect(() => {
    loadData();
  }, [jobId]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [jobData, applicationsData] = await Promise.all([
        talentsService.getJobDetail(jobId!),
        talentsService.getApplicationsByJob(jobId!)
      ]);
      setJob(jobData);
      setApplications(applicationsData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
      toast.error('Erro ao carregar dados da vaga');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = () => {
    // TODO: Navegar para tela de edição com IA
    toast('Funcionalidade em desenvolvimento', { icon: 'ℹ️' });
  };

  const handleViewDetails = (applicationId: string) => {
    // TODO: Navegar para detalhes do candidato
    toast('Funcionalidade em desenvolvimento', { icon: 'ℹ️' });
  };

  // Filtrar e ordenar candidatos
  const filteredApplications = applications
    .filter(app => {
      const searchLower = searchTerm.toLowerCase();
      return (
        app.candidateNome.toLowerCase().includes(searchLower) ||
        app.candidateEmail.toLowerCase().includes(searchLower)
      );
    })
    .sort((a, b) => {
      if (sortBy === 'date') {
        const dateA = new Date(a.dataInscricao).getTime();
        const dateB = new Date(b.dataInscricao).getTime();
        return sortOrder === 'asc' ? dateA - dateB : dateB - dateA;
      } else {
        const nameA = a.candidateNome.toLowerCase();
        const nameB = b.candidateNome.toLowerCase();
        return sortOrder === 'asc' ? nameA.localeCompare(nameB) : nameB.localeCompare(nameA);
      }
    });

  if (loading) {
    return (
      <MainLayout showHeader={false}>
        <div className="job-detail-container">
          <div className="job-detail-header">
            <h1>Lista de candidatos</h1>
          </div>
          <div className="loading-state">Carregando...</div>
        </div>
      </MainLayout>
    );
  }

  if (!job) {
    return (
      <MainLayout showHeader={false}>
        <div className="job-detail-container">
          <div className="job-detail-header">
            <h1>Lista de candidatos</h1>
          </div>
          <div className="error-state">Vaga não encontrada</div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout showHeader={false}>
      <div className="job-detail-container">
        {/* Header */}
        <div className="job-detail-header">
          <div className="header-title-group">
            <button className="btn-back" onClick={() => navigate('/talents/vagas')} title="Voltar para lista de vagas">
              <ArrowLeft size={20} />
            </button>
            <h1>Lista de candidatos</h1>
          </div>
          <button className="btn-edit-job" onClick={handleEdit}>
            <Edit size={18} />
            Editar vaga
          </button>
        </div>

        <div className="job-detail-content">
          {/* Job Info Card - Único e compacto */}
          <div className="job-detail-card">
            <div className="job-card-header">
              <div className="job-card-main">
                <div className="job-card-title-row">
                  <h2>{job.titulo}</h2>
                  <span className={`status-badge status-${job.status.toLowerCase()}`}>
                    {job.status}
                  </span>
                </div>
                <div className="job-card-info-row">
                  <span className="info-item">
                    {new Date(job.dataCriacao).toLocaleDateString('pt-BR')}
                  </span>
                  <span className="info-divider">•</span>
                  <span className="info-item">
                    <MapPin size={14} />
                    {job.cidade && job.estado ? `${job.cidade}, ${job.estado}` : job.localizacao || '-'}
                  </span>
                  <span className="info-divider">•</span>
                  <span className="info-item">
                    <Briefcase size={14} />
                    {job.tipoContrato || '-'}
                  </span>
                  <span className="info-divider">•</span>
                  <span className="info-item">
                    {job.modeloTrabalho || '-'}
                  </span>
                  <span className="info-divider">•</span>
                  <span className="info-item">
                    <Users size={14} />
                    {applications.length} candidaturas
                  </span>
                  <span className="info-divider">•</span>
                  <span className="info-item">
                    {job.numeroVagas} {job.numeroVagas === 1 ? 'vaga' : 'vagas'}
                  </span>
                </div>
              </div>
              <div className="job-card-actions">
                <button 
                  className="btn-copy-link"
                  onClick={(e) => {
                    e.stopPropagation();
                    navigator.clipboard.writeText(`${window.location.origin}/candidatura/${job.id}`);
                    toast.success('Link copiado!');
                  }}
                  title="Copiar link de candidatura"
                >
                  <Share2 size={16} />
                  Copiar link
                </button>
                <button 
                  className="btn-icon-action"
                  onClick={() => setIsJobExpanded(!isJobExpanded)}
                  title={isJobExpanded ? 'Recolher' : 'Ver tudo'}
                >
                  {isJobExpanded ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
                </button>
              </div>
            </div>

            {/* Conteúdo expandido */}
            {isJobExpanded && (
              <div className="job-card-expanded">
                {/* Seção: Descrição da vaga */}
                <div className="expanded-section">
                  <h3 className="section-title">Descrição</h3>
                  <div className="section-content">
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <p>{job.descricao || '-'}</p>
                      </div>
                      <div className="expanded-field"></div>
                      <div className="expanded-field"></div>
                    </div>
                  </div>
                </div>

                {/* Seção: Requisitos */}
                <div className="expanded-section">
                  <h3 className="section-title">Requisitos</h3>
                  <div className="section-content">
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Responsabilidades</label>
                        <p>{job.responsabilidades || '-'}</p>
                      </div>
                      <div className="expanded-field"></div>
                      <div className="expanded-field"></div>
                    </div>
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Experiência</label>
                        <span>{job.anosExperienciaMinimo ? `${job.anosExperienciaMinimo} ${job.anosExperienciaMinimo === 1 ? 'ano' : 'anos'}` : '-'}</span>
                      </div>
                      <div className="expanded-field">
                        <label>Formação</label>
                        <span>{job.formacaoNecessaria || '-'}</span>
                      </div>
                      <div className="expanded-field">
                        <label>Departamento</label>
                        <span>{job.departamento || '-'}</span>
                      </div>
                    </div>
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Conhecimentos obrigatórios</label>
                        {job.conhecimentosObrigatorios && job.conhecimentosObrigatorios.length > 0 ? (
                          <div className="tags-list">
                            {job.conhecimentosObrigatorios.map((item, idx) => (
                              <span key={idx} className="tag-mini">{item}</span>
                            ))}
                          </div>
                        ) : <span>-</span>}
                      </div>
                      <div className="expanded-field">
                        <label>Conhecimentos desejáveis</label>
                        {job.conhecimentosDesejaveis && job.conhecimentosDesejaveis.length > 0 ? (
                          <div className="tags-list">
                            {job.conhecimentosDesejaveis.map((item, idx) => (
                              <span key={idx} className="tag-mini">{item}</span>
                            ))}
                          </div>
                        ) : <span>-</span>}
                      </div>
                      <div className="expanded-field">
                        <label>Competências</label>
                        {job.competenciasImportantes && job.competenciasImportantes.length > 0 ? (
                          <div className="tags-list">
                            {job.competenciasImportantes.map((item, idx) => (
                              <span key={idx} className="tag-mini">{item}</span>
                            ))}
                          </div>
                        ) : <span>-</span>}
                      </div>
                    </div>
                  </div>
                </div>

                {/* Seção: Remuneração */}
                <div className="expanded-section">
                  <h3 className="section-title">Remuneração e Benefícios</h3>
                  <div className="section-content">
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Salário mínimo</label>
                        <span>{job.salarioMin ? `R$ ${job.salarioMin.toLocaleString('pt-BR')}` : '-'}</span>
                      </div>
                      <div className="expanded-field">
                        <label>Salário máximo</label>
                        <span>{job.salarioMax ? `R$ ${job.salarioMax.toLocaleString('pt-BR')}` : '-'}</span>
                      </div>
                      <div className="expanded-field">
                        <label>Bônus/Comissão</label>
                        <span>{job.bonusComissao || '-'}</span>
                      </div>
                    </div>
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Benefícios</label>
                        <p>{job.beneficios || '-'}</p>
                      </div>
                      <div className="expanded-field"></div>
                      <div className="expanded-field"></div>
                    </div>
                  </div>
                </div>

                {/* Seção: Processo Seletivo */}
                <div className="expanded-section">
                  <h3 className="section-title">Processo Seletivo</h3>
                  <div className="section-content">
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Previsão de início</label>
                        <span>{job.previsaoInicio ? new Date(job.previsaoInicio).toLocaleDateString('pt-BR') : '-'}</span>
                      </div>
                      <div className="expanded-field">
                        <label>Localização completa</label>
                        <span>{job.localizacao || '-'}</span>
                      </div>
                      <div className="expanded-field">
                        <label>Etapas do processo</label>
                        {job.etapasProcesso && job.etapasProcesso.length > 0 ? (
                          <div className="tags-list">
                            {job.etapasProcesso.map((item, idx) => (
                              <span key={idx} className="tag-mini">{item}</span>
                            ))}
                          </div>
                        ) : <span>-</span>}
                      </div>
                    </div>
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Tipos de teste</label>
                        {job.tiposTesteEntrevista && job.tiposTesteEntrevista.length > 0 ? (
                          <div className="tags-list">
                            {job.tiposTesteEntrevista.map((item, idx) => (
                              <span key={idx} className="tag-mini">{item}</span>
                            ))}
                          </div>
                        ) : <span>-</span>}
                      </div>
                      <div className="expanded-field"></div>
                      <div className="expanded-field"></div>
                    </div>
                  </div>
                </div>

                {/* Seção: Informações adicionais */}
                <div className="expanded-section">
                  <h3 className="section-title">Informações Adicionais</h3>
                  <div className="section-content">
                    <div className="expanded-field-row">
                      <div className="expanded-field">
                        <label>Sobre o time</label>
                        <p>{job.sobreTime || '-'}</p>
                      </div>
                      <div className="expanded-field">
                        <label>Diferenciais</label>
                        <p>{job.diferenciais || '-'}</p>
                      </div>
                      <div className="expanded-field"></div>
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Search and Filters */}
          <div className="filters-card">
            <div className="search-row">
              <Search size={20} />
              <input
                type="text"
                placeholder="Buscar por nome ou email..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="search-input"
              />
            </div>

            <div className="sort-row">
              <button
                className={`sort-button ${sortBy === 'date' ? 'active' : ''}`}
                onClick={() => setSortBy('date')}
              >
                <Calendar size={18} />
                Data de inscrição
              </button>
              <button
                className={`sort-button ${sortBy === 'name' ? 'active' : ''}`}
                onClick={() => setSortBy('name')}
              >
                <SortAsc size={18} />
                Ordem alfabética
              </button>
              <button
                className="sort-order-button"
                onClick={() => setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc')}
              >
                <ArrowUpDown size={18} />
                {sortOrder === 'asc' ? 'Crescente' : 'Decrescente'}
              </button>
            </div>
          </div>

          {/* Candidates Table */}
          {filteredApplications.length === 0 ? (
            <div className="empty-state">
              <p>Nenhum candidato encontrado</p>
            </div>
          ) : (
            <div className="candidates-table">
              <table>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Email</th>
                    <th>Data de inscrição</th>
                    <th>Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredApplications.map((app) => (
                    <tr key={app.id}>
                      <td className="candidate-name">{app.candidateNome}</td>
                      <td>{app.candidateEmail}</td>
                      <td>{new Date(app.dataInscricao).toLocaleDateString('pt-BR')}</td>
                      <td className="actions-cell">
                        <button
                          className="btn-icon btn-primary"
                          onClick={() => handleViewDetails(app.id)}
                          title="Ver detalhes"
                        >
                          <Eye size={18} />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </MainLayout>
  );
};
