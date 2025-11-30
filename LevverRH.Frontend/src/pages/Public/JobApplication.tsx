import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { 
  Briefcase, 
  MapPin, 
  DollarSign, 
  Clock, 
  Building2,
  Loader2,
  CheckCircle2,
  AlertCircle
} from 'lucide-react';
import { FileUploadResume } from '../../components/FileUploadResume';
import { 
  publicApplicationService, 
  PublicJobDetailDTO,
  CreatePublicApplicationDTO 
} from '../../services/publicApplicationService';
import '../../index.css';

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
  const navigate = useNavigate();

  const [job, setJob] = useState<PublicJobDetailDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [curriculoFile, setCurriculoFile] = useState<File | null>(null);
  const [fileError, setFileError] = useState<string>('');
  const [wantCreateAccount, setWantCreateAccount] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<ApplicationFormData>({
    defaultValues: {
      criarConta: false
    }
  });

  const watchCriarConta = watch('criarConta');

  useEffect(() => {
    if (!jobId) {
      navigate('/');
      return;
    }

    loadJobDetails();
  }, [jobId]);

  const loadJobDetails = async () => {
    try {
      setLoading(true);
      const jobData = await publicApplicationService.getPublicJobDetail(jobId!);
      setJob(jobData);
    } catch (error) {
      console.error('Erro ao carregar vaga:', error);
      toast.error('Vaga não encontrada ou não está disponível');
      setTimeout(() => navigate('/'), 3000);
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: ApplicationFormData) => {
    if (!curriculoFile) {
      setFileError('Por favor, anexe seu currículo');
      return;
    }

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

    setFileError('');
    setSubmitting(true);

    try {
      const dto: CreatePublicApplicationDTO = {
        jobId: jobId!,
        nome: data.nome,
        email: data.email,
        telefone: data.telefone,
        linkedinUrl: data.linkedinUrl,
        criarConta: data.criarConta,
        senha: data.criarConta ? data.senha : undefined,
      };

      const response = await publicApplicationService.submitApplication(dto, curriculoFile);

      if (response.success) {
        setSubmitted(true);
        
        // Se criou conta, salvar token e redirecionar
        if (response.contaCriada && response.accessToken) {
          localStorage.setItem('token', response.accessToken);
          toast.success(response.message || 'Conta criada e candidatura enviada!');
          
          // Redirecionar após 3 segundos
          setTimeout(() => {
            window.location.href = '/painel';
          }, 3000);
        } else {
          toast.success(response.message || 'Candidatura enviada com sucesso!');
        }
      } else {
        toast.error(response.message || 'Erro ao enviar candidatura');
      }
    } catch (error: any) {
      console.error('Erro ao enviar candidatura:', error);
      
      const errorMessage = error?.response?.data?.message || 
                          error?.response?.data?.errors?.[0] ||
                          'Erro ao enviar candidatura. Tente novamente.';
      
      toast.error(errorMessage);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-12 h-12 text-blue-500 animate-spin mx-auto mb-4" />
          <p className="text-gray-600">Carregando vaga...</p>
        </div>
      </div>
    );
  }

  if (!job) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <AlertCircle className="w-12 h-12 text-red-500 mx-auto mb-4" />
          <p className="text-gray-600">Vaga não encontrada</p>
        </div>
      </div>
    );
  }

  if (submitted) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center px-4">
        <div className="max-w-md w-full bg-white rounded-2xl shadow-2xl p-10 text-center transform transition-all">
          <div className="bg-green-100 rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6">
            <CheckCircle2 className="w-12 h-12 text-green-600" />
          </div>
          <h2 className="text-3xl font-bold text-gray-900 mb-3">
            Candidatura enviada com sucesso!
          </h2>
          <p className="text-gray-600 text-lg mb-4">
            Recebemos sua candidatura para a vaga de <strong className="text-blue-600">{job.titulo}</strong> na {job.nomeEmpresa}.
          </p>
          <div className="bg-blue-50 rounded-lg p-4 mb-6">
            <p className="text-sm text-gray-700">
              Nossa equipe irá analisar seu perfil e entraremos em contato em breve através do e-mail cadastrado.
            </p>
          </div>
          <p className="text-sm text-gray-500">
            Boa sorte! 🚀
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-12 px-4">
      <div className="max-w-5xl mx-auto">
        {/* Header da empresa */}
        <div className="bg-white rounded-2xl shadow-xl overflow-hidden mb-8">
          {/* Banner superior com gradiente */}
          <div className="h-2 bg-gradient-to-r from-blue-500 via-indigo-500 to-purple-500"></div>
          
          <div className="p-8">
            <div className="flex items-start space-x-6 mb-8">
              {job.logoEmpresa ? (
                <div className="flex-shrink-0">
                  <img 
                    src={job.logoEmpresa} 
                    alt={job.nomeEmpresa}
                    className="w-24 h-24 object-contain rounded-xl border-2 border-gray-100 p-2 bg-white shadow-md"
                  />
                </div>
              ) : (
                <div className="flex-shrink-0 w-24 h-24 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl flex items-center justify-center shadow-md">
                  <Building2 className="w-12 h-12 text-white" />
                </div>
              )}
              <div className="flex-1">
                <h1 className="text-4xl font-bold text-gray-900 mb-3 leading-tight">{job.titulo}</h1>
                <div className="flex items-center text-lg text-gray-600 mb-4">
                  <Building2 className="w-5 h-5 mr-2 text-blue-500" />
                  <span className="font-medium">{job.nomeEmpresa}</span>
                </div>
                <div className="flex flex-wrap gap-3">
                  {job.cidade && job.estado && (
                    <span className="inline-flex items-center px-4 py-2 rounded-full bg-blue-50 text-blue-700 text-sm font-medium">
                      <MapPin className="w-4 h-4 mr-2" />
                      {job.cidade}, {job.estado}
                    </span>
                  )}
                  {job.tipoContrato && (
                    <span className="inline-flex items-center px-4 py-2 rounded-full bg-purple-50 text-purple-700 text-sm font-medium">
                      <Clock className="w-4 h-4 mr-2" />
                      {job.tipoContrato}
                    </span>
                  )}
                  {job.modalidade && (
                    <span className="inline-flex items-center px-4 py-2 rounded-full bg-green-50 text-green-700 text-sm font-medium">
                      <Briefcase className="w-4 h-4 mr-2" />
                      {job.modalidade}
                    </span>
                  )}
                </div>
              </div>
            </div>

            {/* Salário destacado */}
            {(job.salarioMin || job.salarioMax) && (
              <div className="bg-gradient-to-r from-green-50 to-emerald-50 rounded-xl p-6 mb-8 border border-green-100">
                <div className="flex items-center">
                  <DollarSign className="w-8 h-8 text-green-600 mr-3" />
                  <div>
                    <p className="text-sm text-gray-600 font-medium mb-1">Faixa salarial</p>
                    <p className="text-2xl font-bold text-gray-900">
                      {job.salarioMin && job.salarioMax
                        ? `R$ ${job.salarioMin.toLocaleString()} - R$ ${job.salarioMax.toLocaleString()}`
                        : job.salarioMin
                        ? `A partir de R$ ${job.salarioMin.toLocaleString()}`
                        : `Até R$ ${job.salarioMax?.toLocaleString()}`
                      }
                    </p>
                  </div>
                </div>
              </div>
            )}

            {/* Descrição */}
            {job.descricao && (
              <div className="mb-8">
                <h3 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                  <div className="w-1 h-6 bg-blue-500 rounded-full mr-3"></div>
                  Sobre a vaga
                </h3>
                <p className="text-gray-700 leading-relaxed whitespace-pre-wrap">{job.descricao}</p>
              </div>
            )}

            {/* Requisitos */}
            {job.requisitos && (
              <div className="mb-8">
                <h3 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                  <div className="w-1 h-6 bg-indigo-500 rounded-full mr-3"></div>
                  Requisitos
                </h3>
                <p className="text-gray-700 leading-relaxed whitespace-pre-wrap">{job.requisitos}</p>
              </div>
            )}

            {/* Benefícios */}
            {job.beneficios && (
              <div>
                <h3 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                  <div className="w-1 h-6 bg-purple-500 rounded-full mr-3"></div>
                  Benefícios
                </h3>
                <p className="text-gray-700 leading-relaxed whitespace-pre-wrap">{job.beneficios}</p>
              </div>
            )}
          </div>
        </div>

        {/* Formulário de candidatura */}
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <div className="border-b border-gray-200 pb-6 mb-8">
            <h2 className="text-3xl font-bold text-gray-900 mb-2">Candidate-se para esta vaga</h2>
            <p className="text-gray-600">Preencha o formulário abaixo e anexe seu currículo</p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {/* Nome */}
            <div>
              <label htmlFor="nome" className="block text-sm font-semibold text-gray-700 mb-2">
                Nome completo *
              </label>
              <input
                id="nome"
                type="text"
                {...register('nome', { required: 'Nome é obrigatório' })}
                className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-gray-900"
                placeholder="Seu nome completo"
              />
              {errors.nome && (
                <p className="mt-2 text-sm text-red-600 flex items-center">
                  <AlertCircle className="w-4 h-4 mr-1" />
                  {errors.nome.message}
                </p>
              )}
            </div>

            {/* Email */}
            <div>
              <label htmlFor="email" className="block text-sm font-semibold text-gray-700 mb-2">
                E-mail *
              </label>
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
                className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-gray-900"
                placeholder="seu@email.com"
              />
              {errors.email && (
                <p className="mt-2 text-sm text-red-600 flex items-center">
                  <AlertCircle className="w-4 h-4 mr-1" />
                  {errors.email.message}
                </p>
              )}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Telefone */}
              <div>
                <label htmlFor="telefone" className="block text-sm font-semibold text-gray-700 mb-2">
                  Telefone *
                </label>
                <input
                  id="telefone"
                  type="tel"
                  {...register('telefone', { required: 'Telefone é obrigatório' })}
                  className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-gray-900"
                  placeholder="(00) 00000-0000"
                />
                {errors.telefone && (
                  <p className="mt-2 text-sm text-red-600 flex items-center">
                    <AlertCircle className="w-4 h-4 mr-1" />
                    {errors.telefone.message}
                  </p>
                )}
              </div>

              {/* LinkedIn (opcional) */}
              <div>
                <label htmlFor="linkedinUrl" className="block text-sm font-semibold text-gray-700 mb-2">
                  LinkedIn <span className="text-gray-400 font-normal">(opcional)</span>
                </label>
                <input
                  id="linkedinUrl"
                  type="url"
                  {...register('linkedinUrl')}
                  className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all text-gray-900"
                  placeholder="linkedin.com/in/seu-perfil"
                />
              </div>
            </div>

            {/* Upload de currículo */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Currículo *
              </label>
              <FileUploadResume 
                onFileSelect={setCurriculoFile}
                error={fileError}
              />
            </div>

            {/* Botão de envio */}
            <button
              type="submit"
              disabled={submitting}
              className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-4 rounded-xl font-bold text-lg hover:from-blue-700 hover:to-indigo-700 transition-all shadow-lg hover:shadow-xl disabled:from-gray-400 disabled:to-gray-400 disabled:cursor-not-allowed flex items-center justify-center transform hover:scale-[1.02] active:scale-[0.98]"
            >
              {submitting ? (
                <>
                  <Loader2 className="w-6 h-6 mr-2 animate-spin" />
                  Enviando candidatura...
                </>
              ) : (
                <>
                  <CheckCircle2 className="w-6 h-6 mr-2" />
                  Enviar candidatura
                </>
              )}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
