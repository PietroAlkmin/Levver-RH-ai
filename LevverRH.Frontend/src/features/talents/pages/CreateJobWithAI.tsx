import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { MainLayout } from '../../../components/layout/MainLayout/MainLayout';
import { ChatInterface, Message } from '../components/ChatInterface';
import { JobFormPreview } from '../components/JobFormPreview';
import { JobDetailDTO } from '../types/talents.types';
import { talentsService } from '../services/talentsService';
import { useAuthStore } from '../../../stores/authStore';
import './CreateJobWithAI.css';

/**
 * Página de Criação de Vaga com IA
 * Tela split: Chat à esquerda, Formulário à direita
 */
export const CreateJobWithAI: React.FC = () => {
  const navigate = useNavigate();
  const { token, user, tenant } = useAuthStore();
  
  // Tentar recuperar estado do localStorage
  const getInitialState = <T,>(key: string, defaultValue: T): T => {
    try {
      const stored = localStorage.getItem(`job-creation-${key}`);
      return stored ? JSON.parse(stored) : defaultValue;
    } catch {
      return defaultValue;
    }
  };
  
  const [jobId, setJobId] = useState<string>(() => getInitialState('jobId', ''));
  const [conversationId, setConversationId] = useState<string>(() => getInitialState('conversationId', ''));
  const [messages, setMessages] = useState<Message[]>(() => getInitialState('messages', []));
  const [jobData, setJobData] = useState<Partial<JobDetailDTO>>(() => getInitialState('jobData', {}));
  const [completionPercentage, setCompletionPercentage] = useState(() => getInitialState('completionPercentage', 0));
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [hasInitialized, setHasInitialized] = useState(() => getInitialState('hasInitialized', false));

  // Salvar estado no localStorage quando mudar
  useEffect(() => {
    if (jobId) localStorage.setItem('job-creation-jobId', JSON.stringify(jobId));
  }, [jobId]);

  useEffect(() => {
    if (conversationId) localStorage.setItem('job-creation-conversationId', JSON.stringify(conversationId));
  }, [conversationId]);

  useEffect(() => {
    if (messages.length > 0) localStorage.setItem('job-creation-messages', JSON.stringify(messages));
  }, [messages]);

  useEffect(() => {
    if (Object.keys(jobData).length > 0) localStorage.setItem('job-creation-jobData', JSON.stringify(jobData));
  }, [jobData]);

  useEffect(() => {
    localStorage.setItem('job-creation-completionPercentage', JSON.stringify(completionPercentage));
  }, [completionPercentage]);

  useEffect(() => {
    localStorage.setItem('job-creation-hasInitialized', JSON.stringify(hasInitialized));
  }, [hasInitialized]);

  // Inicializar conversa ao montar o componente (apenas uma vez)
  useEffect(() => {
    if (!hasInitialized) {
      initializeJobCreation();
    }
  }, [hasInitialized]);

  const initializeJobCreation = async () => {
    try {
      setIsLoading(true);
      setError('');
      
      // Debug: Verificar autenticação
      console.log('🔑 CreateJobWithAI - Token presente?', !!token);
      console.log('👤 CreateJobWithAI - User:', user);
      console.log('🏢 CreateJobWithAI - Tenant:', tenant);
      
      if (!token || !user || !tenant) {
        setError('Você precisa estar logado para criar uma vaga.');
        setIsLoading(false);
        setTimeout(() => navigate('/login'), 2000);
        return;
      }
      
      const response = await talentsService.startJobWithAI({
        // Não enviar mensagem inicial para usar a mensagem mocada padrão
      });

      console.log('✅ CreateJobWithAI - Response:', response);
      
      setJobId(response.jobId);
      setConversationId(response.conversationId);
      setHasInitialized(true); // Marcar como inicializado
      
      // Iniciar com 0% pois ainda não há dados preenchidos
      setCompletionPercentage(0);
      
      // Adicionar mensagem inicial da IA
      setMessages([{
        role: 'assistant',
        content: response.mensagemIA,
        timestamp: new Date()
      }]);

      // Atualizar jobData se disponível
      if (response.jobAtualizado) {
        setJobData(response.jobAtualizado);
      }
    } catch (err: unknown) {
      const error = err as { response?: { status?: number; data?: { message?: string } } };
      console.error('❌ Erro ao inicializar criação de vaga:', err);
      console.error('❌ Error response:', error.response);
      console.error('❌ Error response data:', error.response?.data);
      
      if (error.response?.status === 401) {
        setError('Erro de autenticação. Faça login novamente.');
      } else if (error.response?.data?.message) {
        setError(error.response.data.message);
      } else {
        setError('Erro ao conectar com o servidor. Verifique se o backend está rodando.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleSendMessage = async (messageText: string) => {
    if (!jobId || !conversationId) return;

    // Adicionar mensagem do usuário
    const userMessage: Message = {
      role: 'user',
      content: messageText,
      timestamp: new Date()
    };
    setMessages(prev => [...prev, userMessage]);
    setIsLoading(true);

    try {
      const response = await talentsService.sendChatMessage({
        jobId,
        Mensagem: messageText
      });

      // Adicionar resposta da IA
      const aiMessage: Message = {
        role: 'assistant',
        content: response.mensagemIA,
        timestamp: new Date()
      };
      setMessages(prev => [...prev, aiMessage]);

      if (response.jobAtualizado) {
        setJobData(response.jobAtualizado);
        setCompletionPercentage(calculateCompletionPercentage(response.jobAtualizado));
      }

      // Se concluído, mostrar opções
      if (response.criacaoConcluida) {
        handleJobCreationComplete();
      }
    } catch (err) {
      setError('Erro ao enviar mensagem');
      console.error('Erro ao enviar:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const calculateCompletionPercentage = (data: Partial<JobDetailDTO>): number => {
    const fields = [
      data.titulo, data.descricao, data.departamento, data.numeroVagas,
      data.tipoContrato, data.modeloTrabalho, data.cidade, data.estado,
      data.salarioMin, data.salarioMax
    ];
    const filled = fields.filter(f => f).length;
    return Math.round((filled / fields.length) * 100);
  };

  const handleFieldChange = async (fieldName: string, value: string | number | string[] | undefined) => {
    if (!jobId) return;

    const updatedData = { ...jobData, [fieldName]: value };
    setJobData(updatedData);
    setCompletionPercentage(calculateCompletionPercentage(updatedData));

    try {
      const valueStr = Array.isArray(value) ? value.join(', ') : String(value ?? '');
      await talentsService.manualUpdateField({
        jobId,
        fieldName,
        fieldValue: valueStr,
        userMessage: `Alterei o campo ${fieldName} manualmente`
      });
    } catch (err) {
      console.error('Erro ao atualizar campo:', err);
    }
  };

  const handleJobCreationComplete = () => {
    // Mostrar mensagem final
    const finalMessage: Message = {
      role: 'assistant',
      content: '✓ Vaga criada com sucesso! Você pode revisar os dados e publicar quando quiser.',
      timestamp: new Date()
    };
    setMessages(prev => [...prev, finalMessage]);
  };

  const clearLocalStorage = () => {
    localStorage.removeItem('job-creation-jobId');
    localStorage.removeItem('job-creation-conversationId');
    localStorage.removeItem('job-creation-messages');
    localStorage.removeItem('job-creation-jobData');
    localStorage.removeItem('job-creation-completionPercentage');
    localStorage.removeItem('job-creation-hasInitialized');
  };

  const validateRequiredFields = (): { isValid: boolean; missingFields: string[] } => {
    const missingFields: string[] = [];
    
    if (!jobData.titulo?.trim()) missingFields.push('Título da Vaga');
    if (!jobData.descricao?.trim()) missingFields.push('Descrição');
    
    return {
      isValid: missingFields.length === 0,
      missingFields
    };
  };

  const handlePublish = async () => {
    if (!jobId) return;

    // Validar campos obrigatórios
    const validation = validateRequiredFields();
    if (!validation.isValid) {
      const missingFieldsText = validation.missingFields.join(', ');
      setError(`Campos obrigatórios faltando: ${missingFieldsText}`);
      
      // Limpar erro após 5 segundos
      setTimeout(() => setError(''), 5000);
      return;
    }

    try {
      setIsLoading(true);
      await talentsService.completeJobCreation({
        jobId,
        publicarImediatamente: true
      });

      toast.success('Vaga publicada com sucesso!');
      clearLocalStorage(); // Limpar dados salvos
      navigate(`/talents/vagas/${jobId}`);
    } catch (err) {
      setError('Erro ao publicar vaga');
      console.error('Erro ao publicar:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSaveDraft = async () => {
    if (!jobId) return;

    try {
      setIsLoading(true);
      await talentsService.completeJobCreation({
        jobId,
        publicarImediatamente: false
      });

      clearLocalStorage(); // Limpar dados salvos
      navigate('/talents/jobs');
    } catch (err) {
      setError('Erro ao salvar rascunho');
      console.error('Erro ao salvar:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleStartNewJob = () => {
    if (window.confirm('Tem certeza que deseja descartar esta vaga e começar uma nova?')) {
      clearLocalStorage();
      window.location.reload();
    }
  };

  return (
    <MainLayout>
      <div className="create-job-container">
        {error && (
          <div className="create-job-error">
            {error}
            <button onClick={() => setError('')} className="create-job-error-close">×</button>
          </div>
        )}

        {/* Header com botão para nova vaga */}
        {hasInitialized && (
          <div className="create-job-header">
            <h1 className="create-job-title">Criar Nova Vaga com IA</h1>
            <button 
              onClick={handleStartNewJob}
              className="create-job-button-new"
            >
              Começar Nova Vaga
            </button>
          </div>
        )}

        <div className="create-job-split">
          {/* Chat à esquerda */}
          <div className="create-job-chat">
            <ChatInterface
              messages={messages}
              onSendMessage={handleSendMessage}
              completionPercentage={completionPercentage}
              isLoading={isLoading}
            />
          </div>

          {/* Formulário à direita */}
          <div className="create-job-form">
            <JobFormPreview
              jobData={jobData}
              onFieldChange={handleFieldChange}
              completionPercentage={completionPercentage}
              onSaveDraft={handleSaveDraft}
              onPublish={handlePublish}
              isLoading={isLoading}
            />
          </div>
        </div>
      </div>
    </MainLayout>
  );
};
