import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { FileUploadResume } from '../../components/FileUploadResume';
import { 
  publicApplicationService, 
  PublicJobDetailDTO,
  CreatePublicApplicationDTO 
} from '../../services/publicApplicationService';
import './JobApplication.css';

interface ApplicationFormData {
  nome: string;
  email: string;
  telefone: string;
  linkedinUrl?: string;
  criarConta: boolean;
  senha?: string;
  confirmarSenha?: string;
}

export const JobApplication: React.FC = () => {
  const { jobId } = useParams<{ jobId: string }>();

  const [job, setJob] = useState<PublicJobDetailDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [curriculoFile, setCurriculoFile] = useState<File | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<ApplicationFormData>({
    defaultValues: {
      criarConta: false,
    },
  });

  const watchCriarConta = watch('criarConta');

  useEffect(() => {
    loadJobDetails();
  }, [jobId]);

  const loadJobDetails = async () => {
    try {
      setLoading(true);
      const jobData = await publicApplicationService.getPublicJobDetail(jobId!);
      setJob(jobData);
    } catch (error) {
      console.error('Erro ao carregar detalhes da vaga:', error);
      toast.error('Erro ao carregar detalhes da vaga');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: ApplicationFormData) => {
    // Validar senha se criar conta
    if (data.criarConta) {
      if (!data.senha || data.senha.length < 6) {
        toast.error('A senha deve ter pelo menos 6 caracteres');
        return;
      }
      if (data.senha !== data.confirmarSenha) {
        toast.error('As senhas não coincidem');
        return;
      }
    }

    if (!curriculoFile) {
      toast.error('Por favor, envie seu currículo');
      return;
    }

    try {
      setSubmitting(true);

      const applicationData: CreatePublicApplicationDTO = {
        jobId: jobId!,
        nome: data.nome,
        email: data.email,
        telefone: data.telefone,
        linkedinUrl: data.linkedinUrl,
        criarConta: data.criarConta,
        senha: data.criarConta ? data.senha : undefined,
      };

      const response = await publicApplicationService.submitApplication(
        applicationData,
        curriculoFile
      );

      setSubmitted(true);
      toast.success('Candidatura enviada com sucesso!');

      // Se criou conta, fazer login automático
      if (response.contaCriada && response.accessToken) {
        localStorage.setItem('token', response.accessToken);
        toast.success('Conta criada! Redirecionando...');
        setTimeout(() => {
          window.location.href = '/painel';
        }, 3000);
      }
    } catch (error: any) {
      console.error('Erro ao enviar candidatura:', error);
      toast.error(error.message || 'Erro ao enviar candidatura');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="loading-container">
        <p>Carregando vaga...</p>
      </div>
    );
  }

  if (!job) {
    return (
      <div className="error-container">
        <p>Vaga não encontrada</p>
      </div>
    );
  }

  if (submitted) {
    return (
      <div className="success-container">
        <div className="success-card">
          <div className="success-icon">✓</div>
          <h2>Candidatura enviada com sucesso!</h2>
          <p>Recebemos sua candidatura para a vaga de <strong>{job?.titulo || 'esta posição'}</strong>{job?.nomeEmpresa ? ` na ${job.nomeEmpresa}` : ''}.</p>
          <p className="success-subtext">Nossa equipe irá analisar seu perfil e entraremos em contato em breve.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="public-job-form">
      <div className="public-job-container">
        {/* Header da vaga */}
        <div className="job-header-card">
          <div className="job-header-banner"></div>
          <div className="job-header-content">
            <div className="job-header-flex">
              {job.logoEmpresa ? (
                <img src={job.logoEmpresa} alt={job.nomeEmpresa || 'Empresa'} className="job-logo" />
              ) : (
                <div className="job-logo-placeholder">
                  <span>{job.nomeEmpresa?.charAt(0) || 'E'}</span>
                </div>
              )}
              <div className="job-header-info">
                <h1>{job.titulo || 'Vaga'}</h1>
                <p className="job-company">{job.nomeEmpresa || 'Empresa'}</p>
                <div className="job-tags">
                  {job.cidade && job.estado && (
                    <span className="tag tag-blue">📍 {job.cidade}, {job.estado}</span>
                  )}
                  {job.tipoContrato && (
                    <span className="tag tag-purple">⏰ {job.tipoContrato}</span>
                  )}
                  {job.modalidade && (
                    <span className="tag tag-pink">💼 {job.modalidade}</span>
                  )}
                </div>
              </div>
            </div>
            {(job.salarioMin || job.salarioMax) && (
              <div className="salary-box">
                <span className="salary-label">Salário:</span>
                <span className="salary-value">
                  {job.salarioMin && job.salarioMax
                    ? `R$ ${job.salarioMin.toLocaleString('pt-BR')} - R$ ${job.salarioMax.toLocaleString('pt-BR')}`
                    : job.salarioMin
                    ? `A partir de R$ ${job.salarioMin.toLocaleString('pt-BR')}`
                    : `Até R$ ${job.salarioMax?.toLocaleString('pt-BR')}`}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Formulário */}
        <div className="form-card">
          <h2>Candidate-se para esta vaga</h2>
          <p className="form-subtitle">Preencha o formulário abaixo e anexe seu currículo</p>

          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="nome">Nome completo *</label>
                <input
                  id="nome"
                  type="text"
                  {...register('nome', { required: 'Nome é obrigatório' })}
                  placeholder="Seu nome completo"
                />
                {errors.nome && <span className="error-message">{errors.nome.message}</span>}
              </div>

              <div className="form-group">
                <label htmlFor="email">E-mail *</label>
                <input
                  id="email"
                  type="email"
                  {...register('email', { 
                    required: 'E-mail é obrigatório',
                    pattern: {
                      value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                      message: 'E-mail inválido'
                    }
                  })}
                  placeholder="seu@email.com"
                />
                {errors.email && <span className="error-message">{errors.email.message}</span>}
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="telefone">Telefone *</label>
                <input
                  id="telefone"
                  type="tel"
                  {...register('telefone', { required: 'Telefone é obrigatório' })}
                  placeholder="(00) 00000-0000"
                />
                {errors.telefone && <span className="error-message">{errors.telefone.message}</span>}
              </div>

              <div className="form-group">
                <label htmlFor="linkedinUrl">LinkedIn (opcional)</label>
                <input
                  id="linkedinUrl"
                  type="url"
                  {...register('linkedinUrl')}
                  placeholder="linkedin.com/in/seu-perfil"
                />
              </div>
            </div>

            <div className="form-group">
              <label>Currículo *</label>
              <FileUploadResume
                onFileSelect={(file) => setCurriculoFile(file)}
              />
            </div>

            {/* Opção criar conta */}
            <div className="account-section">
              <div className="checkbox-group">
                <input
                  id="criarConta"
                  type="checkbox"
                  {...register('criarConta')}
                />
                <label htmlFor="criarConta" className="checkbox-label">
                  Desejo criar uma conta para acompanhar minhas candidaturas
                </label>
              </div>

              {watchCriarConta && (
                <div className="password-fields">
                  <div className="form-row">
                    <div className="form-group">
                      <label htmlFor="senha">Senha *</label>
                      <input
                        id="senha"
                        type="password"
                        {...register('senha')}
                        placeholder="Mínimo 6 caracteres"
                      />
                    </div>

                    <div className="form-group">
                      <label htmlFor="confirmarSenha">Confirmar senha *</label>
                      <input
                        id="confirmarSenha"
                        type="password"
                        {...register('confirmarSenha')}
                        placeholder="Digite a senha novamente"
                      />
                    </div>
                  </div>
                </div>
              )}
            </div>

            <button 
              type="submit" 
              className="submit-button"
              disabled={submitting}
            >
              {submitting ? 'Enviando...' : 'Enviar candidatura'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
