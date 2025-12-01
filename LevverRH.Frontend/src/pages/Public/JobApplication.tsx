import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { Check, X } from 'lucide-react';
import { DynamicLogo } from '../../components/common/DynamicLogo';
import { FileUploadResume } from '../../components/FileUploadResume';
import { 
  publicApplicationService, 
  PublicJobDetailDTO,
  CreatePublicApplicationDTO 
} from '../../services/publicApplicationService';
import './JobApplication.css';

interface FormData {
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

  const { register, handleSubmit, formState: { errors }, watch } = useForm<FormData>({
    defaultValues: { criarConta: false }
  });

  const watchCriarConta = watch('criarConta');
  const watchSenha = watch('senha');
  const watchConfirmarSenha = watch('confirmarSenha');

  const senhaValida = watchSenha && watchSenha.length >= 6;
  const senhasIguais = watchSenha && watchConfirmarSenha && watchSenha === watchConfirmarSenha;

  useEffect(() => {
    loadJob();
  }, [jobId]);

  const loadJob = async () => {
    try {
      setLoading(true);
      const data = await publicApplicationService.getPublicJobDetail(jobId!);
      setJob(data);
    } catch (error) {
      toast.error('Erro ao carregar vaga');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: FormData) => {
    if (data.criarConta) {
      if (!data.senha || data.senha.length < 6) {
        toast.error('A senha deve ter pelo menos 6 caracteres');
        return;
      }
      if (!data.confirmarSenha) {
        toast.error('Confirme sua senha');
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

      const response = await publicApplicationService.submitApplication(applicationData, curriculoFile);
      setSubmitted(true);
      toast.success('Candidatura enviada com sucesso!');

      if (response.contaCriada && response.accessToken) {
        localStorage.setItem('token', response.accessToken);
        toast.success('Conta criada! Redirecionando...');
        setTimeout(() => window.location.href = '/painel', 3000);
      }
    } catch (error: any) {
      const errorMessage = error.message || 'Erro ao enviar candidatura';
      
      // Mensagens específicas para erros conhecidos
      if (errorMessage.toLowerCase().includes('já se candidatou')) {
        toast.warning('⚠️ Você já enviou uma candidatura para esta vaga anteriormente.');
      } else if (errorMessage.toLowerCase().includes('não está mais recebendo')) {
        toast.error('Esta vaga não está mais recebendo candidaturas.');
      } else {
        toast.error(errorMessage);
      }
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <div className="app-loading">Carregando...</div>;
  if (!job) return <div className="app-loading">Vaga não encontrada</div>;

  if (submitted) {
    return (
      <div className="app-container">
        <div className="app-content">
          <div className="app-success">
            <div className="app-logo">
              <DynamicLogo />
            </div>
            <h1>Candidatura enviada!</h1>
            <p>Recebemos sua candidatura para <strong>{job.titulo}</strong>{job.nomeEmpresa && <> na {job.nomeEmpresa}</>}.</p>
            <p>Nossa equipe irá analisar seu perfil e entraremos em contato em breve.</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="app-container">
      <div className="app-content">
        <div className="app-card">
          <div className="app-logo">
            <DynamicLogo />
          </div>
          <h1>{job.titulo}</h1>
          {job.nomeEmpresa && <p className="company">{job.nomeEmpresa}</p>}
        </div>

        <div className="app-card">
          <h2>Candidate-se</h2>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="form-row">
              <div className="form-group">
                <label>Nome *</label>
                <input {...register('nome', { required: 'Campo obrigatório' })} placeholder="Seu nome completo" />
                {errors.nome && <span className="error">{errors.nome.message}</span>}
              </div>
              <div className="form-group">
                <label>E-mail *</label>
                <input {...register('email', { required: 'Campo obrigatório' })} placeholder="seu@email.com" />
                {errors.email && <span className="error">{errors.email.message}</span>}
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>Telefone *</label>
                <input {...register('telefone', { required: 'Campo obrigatório' })} placeholder="(00) 00000-0000" />
                {errors.telefone && <span className="error">{errors.telefone.message}</span>}
              </div>
              <div className="form-group">
                <label>LinkedIn</label>
                <input {...register('linkedinUrl')} placeholder="linkedin.com/in/seu-perfil" />
              </div>
            </div>

            <div className="form-group">
              <label>Currículo *</label>
              <FileUploadResume onFileSelect={(file) => setCurriculoFile(file)} />
            </div>

            <div className="checkbox-section">
              <label>
                <input type="checkbox" {...register('criarConta')} />
                Criar conta para acompanhar candidaturas
              </label>

              {watchCriarConta && (
                <div className="password-fields">
                  <div className="form-row">
                    <div className="form-group">
                      <label>Senha *</label>
                      <div className="password-input-wrapper">
                        <input type="password" {...register('senha')} placeholder="Mínimo 6 caracteres" />
                        {watchSenha && (
                          <span className={`password-indicator ${senhaValida ? 'valid' : 'invalid'}`}>
                            {senhaValida ? <Check size={16} /> : <X size={16} />}
                          </span>
                        )}
                      </div>
                      {watchSenha && !senhaValida && (
                        <span className="error">Mínimo 6 caracteres</span>
                      )}
                    </div>
                    <div className="form-group">
                      <label>Confirmar senha *</label>
                      <div className="password-input-wrapper">
                        <input type="password" {...register('confirmarSenha')} placeholder="Digite novamente" />
                        {watchConfirmarSenha && (
                          <span className={`password-indicator ${senhasIguais ? 'valid' : 'invalid'}`}>
                            {senhasIguais ? <Check size={16} /> : <X size={16} />}
                          </span>
                        )}
                      </div>
                      {watchConfirmarSenha && !senhasIguais && (
                        <span className="error">As senhas não coincidem</span>
                      )}
                    </div>
                  </div>
                </div>
              )}
            </div>

            <button type="submit" className="btn-submit" disabled={submitting}>
              {submitting ? 'Enviando...' : 'Enviar candidatura'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
