using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Public;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Application.Services.Interfaces.Talents;

public interface IJobService
{
    // ========== CRUD Básico ==========
    Task<ResultDTO<IEnumerable<JobDTO>>> GetAllAsync(Guid tenantId);
    Task<ResultDTO<JobDTO>> GetByIdAsync(Guid id, Guid tenantId);
    Task<ResultDTO<JobDetailDTO>> GetDetailByIdAsync(Guid id, Guid tenantId);
    Task<ResultDTO<JobDTO>> CreateAsync(CreateJobDTO dto, Guid tenantId, Guid userId);
    Task<ResultDTO<JobDTO>> UpdateAsync(Guid id, UpdateJobDTO dto, Guid tenantId);
    Task<ResultDTO<bool>> DeleteAsync(Guid id, Guid tenantId);
    
    // ========== Endpoints Públicos ==========
    /// <summary>
    /// Obtém detalhes públicos de uma vaga (sem autenticação)
    /// </summary>
    Task<ResultDTO<PublicJobDetailDTO>> GetPublicJobDetailAsync(Guid jobId);
    
    // ========== Filtros ==========
    Task<ResultDTO<IEnumerable<JobDTO>>> GetByStatusAsync(Guid tenantId, JobStatus status);
    Task<ResultDTO<IEnumerable<JobDTO>>> GetDraftsByUserAsync(Guid userId, Guid tenantId);
    Task<ResultDTO<bool>> ChangeStatusAsync(Guid id, JobStatus newStatus, Guid tenantId);

    // ========== Criação com IA ==========
    
    /// <summary>
    /// Inicia criação de vaga assistida por IA
    /// Cria um Job em rascunho e retorna a primeira pergunta da IA
    /// </summary>
    Task<ResultDTO<JobChatResponseDTO>> StartJobCreationWithAIAsync(StartJobCreationDTO dto, Guid tenantId, Guid userId);

    /// <summary>
    /// Processa mensagem do usuário no chat de criação de vaga
    /// A IA extrai dados, atualiza a vaga e retorna a próxima pergunta
    /// </summary>
    Task<ResultDTO<JobChatResponseDTO>> ProcessAIChatMessageAsync(JobChatMessageDTO dto, Guid tenantId);

    /// <summary>
    /// Atualiza campo manualmente e notifica IA sobre a mudança no contexto da conversa
    /// A IA reconhece a alteração e pode ajustar suas próximas perguntas
    /// </summary>
    Task<ResultDTO<JobChatResponseDTO>> ManualUpdateWithAIContextAsync(ManualUpdateJobFieldDTO dto, Guid tenantId);

    /// <summary>
    /// Finaliza criação de vaga e opcionalmente publica
    /// </summary>
    Task<ResultDTO<JobDetailDTO>> CompleteJobCreationAsync(CompleteJobCreationDTO dto, Guid tenantId);
}
