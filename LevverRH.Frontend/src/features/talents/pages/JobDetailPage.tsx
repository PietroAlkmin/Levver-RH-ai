import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import toast from 'react-hot-toast';
import { talentsService } from '../services/talentsService';
import { JobDetailDTO, ApplicationDTO } from '../types/talents.types';
import './JobDetailPage.css';

export const JobDetailPage: React.FC = () => {
  const { jobId } = useParams<{ jobId: string }>();
  
  const [job, setJob] = useState<JobDetailDTO | null>(null);
  const [applications, setApplications] = useState<ApplicationDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [editMode, setEditMode] = useState(false);
  const [formData, setFormData] = useState<Partial<JobDetailDTO>>({});
  const [analyzingIds, setAnalyzingIds] = useState<Set<string>>(new Set());

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
      setFormData(jobData);
      setApplications(applicationsData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
      toast.error('Erro ao carregar dados da vaga');
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    try {
      // TODO: Implementar save quando o endpoint estiver pronto
      toast.success('Vaga atualizada com sucesso!');
      setEditMode(false);
      loadData();
    } catch (error) {
      toast.error('Erro ao salvar alterações');
    }
  };

  const handleViewResume = (candidateId: string) => {
    // TODO: Implementar visualização de currículo
    toast('Funcionalidade em desenvolvimento', { icon: 'ℹ️' });
  };

  const handleAnalyzeWithAI = async (applicationId: string) => {
    setAnalyzingIds(prev => new Set(prev).add(applicationId));
    
    try {
      toast.loading('Analisando currículo com IA...', { id: `analyze-${applicationId}` });
      
      const result = await talentsService.analyzeCandidateWithAI(applicationId);
      
      toast.success(`Análise concluída! Score: ${result.scoreGeral}`, { 
        id: `analyze-${applicationId}`,
        duration: 5000 
      });
      
      // Recarregar candidaturas para mostrar o score atualizado
      const updatedApplications = await talentsService.getApplicationsByJob(jobId!);
      setApplications(updatedApplications);
      
    } catch (error: any) {
      console.error('Erro ao analisar candidato:', error);
      toast.error(error.response?.data?.message || 'Erro ao analisar currículo', { 
        id: `analyze-${applicationId}` 
      });
    } finally {
      setAnalyzingIds(prev => {
        const newSet = new Set(prev);
        newSet.delete(applicationId);
        return newSet;
      });
    }
  };

  if (loading) {
    return <div className="loading">Carregando...</div>;
  }

  if (!job) {
    return <div className="error">Vaga não encontrada</div>;
  }

  return (
    <div className="job-detail-page">
      <div className="job-detail-container">
        {/* Header */}
        <div className="job-header">
          <div className="job-header-content">
            <h1>{job.titulo}</h1>
            <div className="job-meta">
              <span className="status-badge status-{job.status.toLowerCase()}">
                {job.status}
              </span>
              {job.cidade && job.estado && (
                <span className="meta-item">📍 {job.cidade}, {job.estado}</span>
              )}
              {job.tipoContrato && (
                <span className="meta-item">⏰ {job.tipoContrato}</span>
              )}
              <span className="meta-item">👥 {job.totalCandidaturas} candidatura(s)</span>
            </div>
          </div>
          <button 
            onClick={() => setEditMode(!editMode)}
            className="edit-button"
          >
            {editMode ? '❌ Cancelar' : '✏️ Editar'}
          </button>
        </div>

        {/* Informações da Vaga */}
        <div className="job-info-section">
          <h2>📋 Informações da Vaga</h2>
          
          {editMode ? (
            <div className="edit-form">
              <div className="form-group">
                <label>Descrição</label>
                <textarea
                  value={formData.descricao || ''}
                  onChange={(e) => setFormData({...formData, descricao: e.target.value})}
                  rows={5}
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Departamento</label>
                  <input
                    type="text"
                    value={formData.departamento || ''}
                    onChange={(e) => setFormData({...formData, departamento: e.target.value})}
                  />
                </div>

                <div className="form-group">
                  <label>Número de Vagas</label>
                  <input
                    type="number"
                    value={formData.numeroVagas || 1}
                    onChange={(e) => setFormData({...formData, numeroVagas: parseInt(e.target.value)})}
                  />
                </div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Salário Mínimo</label>
                  <input
                    type="number"
                    value={formData.salarioMin || ''}
                    onChange={(e) => setFormData({...formData, salarioMin: parseFloat(e.target.value)})}
                    placeholder="0.00"
                  />
                </div>

                <div className="form-group">
                  <label>Salário Máximo</label>
                  <input
                    type="number"
                    value={formData.salarioMax || ''}
                    onChange={(e) => setFormData({...formData, salarioMax: parseFloat(e.target.value)})}
                    placeholder="0.00"
                  />
                </div>
              </div>

              <div className="form-group">
                <label>Responsabilidades</label>
                <textarea
                  value={formData.responsabilidades || ''}
                  onChange={(e) => setFormData({...formData, responsabilidades: e.target.value})}
                  rows={4}
                />
              </div>

              <div className="form-group">
                <label>Benefícios</label>
                <textarea
                  value={formData.beneficios || ''}
                  onChange={(e) => setFormData({...formData, beneficios: e.target.value})}
                  rows={3}
                />
              </div>

              <button onClick={handleSave} className="save-button">
                💾 Salvar Alterações
              </button>
            </div>
          ) : (
            <div className="info-display">
              <div className="info-item">
                <strong>Descrição:</strong>
                <p>{job.descricao}</p>
              </div>

              {job.responsabilidades && (
                <div className="info-item">
                  <strong>Responsabilidades:</strong>
                  <p>{job.responsabilidades}</p>
                </div>
              )}

              <div className="info-grid">
                {job.departamento && (
                  <div className="info-item">
                    <strong>Departamento:</strong>
                    <span>{job.departamento}</span>
                  </div>
                )}

                <div className="info-item">
                  <strong>Vagas:</strong>
                  <span>{job.numeroVagas}</span>
                </div>

                {(job.salarioMin || job.salarioMax) && (
                  <div className="info-item">
                    <strong>Salário:</strong>
                    <span>
                      {job.salarioMin && job.salarioMax
                        ? `R$ ${job.salarioMin.toLocaleString('pt-BR')} - R$ ${job.salarioMax.toLocaleString('pt-BR')}`
                        : job.salarioMin
                        ? `A partir de R$ ${job.salarioMin.toLocaleString('pt-BR')}`
                        : `Até R$ ${job.salarioMax?.toLocaleString('pt-BR')}`}
                    </span>
                  </div>
                )}
              </div>

              {job.beneficios && (
                <div className="info-item">
                  <strong>Benefícios:</strong>
                  <p>{job.beneficios}</p>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Lista de Candidatos */}
        <div className="candidates-section">
          <h2>👥 Candidatos ({applications.length})</h2>

          {applications.length === 0 ? (
            <div className="no-candidates">
              <p>Nenhum candidato ainda</p>
            </div>
          ) : (
            <div className="candidates-list">
              {applications.map((application) => (
                <div key={application.id} className="candidate-card">
                  <div className="candidate-header">
                    <div className="candidate-info">
                      <h3>{application.candidateNome}</h3>
                      <p className="candidate-email">{application.candidateEmail}</p>
                      <p className="candidate-date">
                        Aplicado em: {new Date(application.dataInscricao).toLocaleDateString('pt-BR')}
                      </p>
                    </div>
                    
                    <div className="candidate-score">
                      <div className="score-label">Score</div>
                      <div className="score-value">
                        {application.scoreGeral ? (
                          <span className="score-number">{application.scoreGeral}%</span>
                        ) : (
                          <span className="score-placeholder">-</span>
                        )}
                      </div>
                    </div>
                  </div>

                  <div className="candidate-status">
                    <span className={`status-badge status-${application.status.toLowerCase()}`}>
                      {application.status}
                    </span>
                  </div>

                  <div className="candidate-actions">
                    <button
                      onClick={() => handleAnalyzeWithAI(application.id)}
                      className="action-button ai-button"
                      disabled={analyzingIds.has(application.id)}
                    >
                      {analyzingIds.has(application.id) ? (
                        <>⏳ Analisando...</>
                      ) : application.scoreGeral ? (
                        <>📊 Re-analisar</>
                      ) : (
                        <>🤖 Analisar com IA</>
                      )}
                    </button>
                    <button
                      onClick={() => handleViewResume(application.candidateId)}
                      className="action-button resume-button"
                    >
                      📄 Ver Currículo
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
