using AutoMapper;
using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces.Talents;
using System.Text.Json;

namespace LevverRH.Application.Services.Implementations.Talents;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobAIService _jobAIService;
    private readonly IMapper _mapper;

    public JobService(IJobRepository jobRepository, IJobAIService jobAIService, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _jobAIService = jobAIService;
        _mapper = mapper;
    }

    #region CRUD Básico

    public async Task<ResultDTO<IEnumerable<JobDTO>>> GetAllAsync(Guid tenantId)
    {
        try
        {
            var jobs = await _jobRepository.GetByTenantIdAsync(tenantId);
            var jobsDto = _mapper.Map<IEnumerable<JobDTO>>(jobs);
            return ResultDTO<IEnumerable<JobDTO>>.SuccessResult(jobsDto);
        }
        catch (Exception ex)
        {
            return ResultDTO<IEnumerable<JobDTO>>.FailureResult($"Erro ao buscar vagas: {ex.Message}");
        }
    }

    public async Task<ResultDTO<JobDTO>> GetByIdAsync(Guid id, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(id, tenantId);
            
            if (job == null)
                return ResultDTO<JobDTO>.FailureResult("Vaga não encontrada");

            var jobDto = _mapper.Map<JobDTO>(job);
            return ResultDTO<JobDTO>.SuccessResult(jobDto);
        }
        catch (Exception ex)
        {
            return ResultDTO<JobDTO>.FailureResult($"Erro ao buscar vaga: {ex.Message}");
        }
    }

    public async Task<ResultDTO<JobDetailDTO>> GetDetailByIdAsync(Guid id, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(id, tenantId);
            
            if (job == null)
                return ResultDTO<JobDetailDTO>.FailureResult("Vaga não encontrada");

            var jobDto = _mapper.Map<JobDetailDTO>(job);
            
            // Deserializar campos JSON para listas
            jobDto.ConhecimentosObrigatorios = DeserializeJsonList(job.ConhecimentosObrigatorios);
            jobDto.ConhecimentosDesejaveis = DeserializeJsonList(job.ConhecimentosDesejaveis);
            jobDto.CompetenciasImportantes = DeserializeJsonList(job.CompetenciasImportantes);
            jobDto.EtapasProcesso = DeserializeJsonList(job.EtapasProcesso);
            jobDto.TiposTesteEntrevista = DeserializeJsonList(job.TiposTesteEntrevista);

            return ResultDTO<JobDetailDTO>.SuccessResult(jobDto);
        }
        catch (Exception ex)
        {
            return ResultDTO<JobDetailDTO>.FailureResult($"Erro ao buscar vaga: {ex.Message}");
        }
    }

    public async Task<ResultDTO<JobDTO>> CreateAsync(CreateJobDTO createDto, Guid tenantId, Guid userId)
    {
        try
        {
            var job = _mapper.Map<Job>(createDto);
            job.Id = Guid.NewGuid();
            job.TenantId = tenantId;
            job.CriadoPor = userId;
            job.DataCriacao = DateTime.UtcNow;
            job.DataAtualizacao = DateTime.UtcNow;
            job.Status = JobStatus.Rascunho;

            var createdJob = await _jobRepository.AddAsync(job);
            var jobDto = _mapper.Map<JobDTO>(createdJob);
            
            return ResultDTO<JobDTO>.SuccessResult(jobDto, "Vaga criada com sucesso");
        }
        catch (Exception ex)
        {
            return ResultDTO<JobDTO>.FailureResult($"Erro ao criar vaga: {ex.Message}");
        }
    }

    public async Task<ResultDTO<JobDTO>> UpdateAsync(Guid id, UpdateJobDTO updateDto, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(id, tenantId);
            
            if (job == null)
                return ResultDTO<JobDTO>.FailureResult("Vaga não encontrada");

            // Atualizar campos básicos
            if (!string.IsNullOrEmpty(updateDto.Titulo))
                job.Titulo = updateDto.Titulo;
            
            if (!string.IsNullOrEmpty(updateDto.Descricao))
                job.Descricao = updateDto.Descricao;

            if (updateDto.Departamento != null)
                job.Departamento = updateDto.Departamento;

            if (updateDto.Localizacao != null)
                job.Localizacao = updateDto.Localizacao;

            if (updateDto.Cidade != null)
                job.Cidade = updateDto.Cidade;

            if (updateDto.Estado != null)
                job.Estado = updateDto.Estado;

            if (updateDto.TipoContrato.HasValue)
                job.TipoContrato = updateDto.TipoContrato.Value;

            if (updateDto.ModeloTrabalho.HasValue)
                job.ModeloTrabalho = updateDto.ModeloTrabalho.Value;

            if (updateDto.SalarioMin.HasValue)
                job.SalarioMin = updateDto.SalarioMin.Value;

            if (updateDto.SalarioMax.HasValue)
                job.SalarioMax = updateDto.SalarioMax.Value;

            if (updateDto.Beneficios != null)
                job.Beneficios = updateDto.Beneficios;

            if (updateDto.BonusComissao != null)
                job.BonusComissao = updateDto.BonusComissao;

            // Atualizar campos de requisitos
            if (updateDto.AnosExperienciaMinimo.HasValue)
                job.AnosExperienciaMinimo = updateDto.AnosExperienciaMinimo.Value;

            if (updateDto.FormacaoNecessaria != null)
                job.FormacaoNecessaria = updateDto.FormacaoNecessaria;

            if (updateDto.ConhecimentosObrigatorios != null)
                job.ConhecimentosObrigatorios = SerializeToJson(updateDto.ConhecimentosObrigatorios);

            if (updateDto.ConhecimentosDesejaveis != null)
                job.ConhecimentosDesejaveis = SerializeToJson(updateDto.ConhecimentosDesejaveis);

            if (updateDto.CompetenciasImportantes != null)
                job.CompetenciasImportantes = SerializeToJson(updateDto.CompetenciasImportantes);

            if (updateDto.Responsabilidades != null)
                job.Responsabilidades = updateDto.Responsabilidades;

            // Atualizar campos de processo seletivo
            if (updateDto.EtapasProcesso != null)
                job.EtapasProcesso = SerializeToJson(updateDto.EtapasProcesso);

            if (updateDto.TiposTesteEntrevista != null)
                job.TiposTesteEntrevista = SerializeToJson(updateDto.TiposTesteEntrevista);

            if (updateDto.PrevisaoInicio.HasValue)
                job.PrevisaoInicio = updateDto.PrevisaoInicio.Value;

            // Atualizar campos sobre a vaga
            if (updateDto.SobreTime != null)
                job.SobreTime = updateDto.SobreTime;

            if (updateDto.Diferenciais != null)
                job.Diferenciais = updateDto.Diferenciais;

            if (updateDto.NumeroVagas.HasValue)
                job.NumeroVagas = updateDto.NumeroVagas.Value;

            job.DataAtualizacao = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);
            var jobDto = _mapper.Map<JobDTO>(job);
            
            return ResultDTO<JobDTO>.SuccessResult(jobDto, "Vaga atualizada com sucesso");
        }
        catch (Exception ex)
        {
            return ResultDTO<JobDTO>.FailureResult($"Erro ao atualizar vaga: {ex.Message}");
        }
    }

    public async Task<ResultDTO<bool>> DeleteAsync(Guid id, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(id, tenantId);
            
            if (job == null)
                return ResultDTO<bool>.FailureResult("Vaga não encontrada");

            await _jobRepository.DeleteAsync(id);
            return ResultDTO<bool>.SuccessResult(true, "Vaga excluída com sucesso");
        }
        catch (Exception ex)
        {
            return ResultDTO<bool>.FailureResult($"Erro ao excluir vaga: {ex.Message}");
        }
    }

    #endregion

    #region Filtros

    public async Task<ResultDTO<IEnumerable<JobDTO>>> GetByStatusAsync(Guid tenantId, JobStatus status)
    {
        try
        {
            var jobs = await _jobRepository.GetByStatusAsync(tenantId, status);
            var jobsDto = _mapper.Map<IEnumerable<JobDTO>>(jobs);
            return ResultDTO<IEnumerable<JobDTO>>.SuccessResult(jobsDto);
        }
        catch (Exception ex)
        {
            return ResultDTO<IEnumerable<JobDTO>>.FailureResult($"Erro ao buscar vagas por status: {ex.Message}");
        }
    }

    public async Task<ResultDTO<IEnumerable<JobDTO>>> GetDraftsByUserAsync(Guid userId, Guid tenantId)
    {
        try
        {
            var jobs = await _jobRepository.GetDraftsByUserAsync(userId);
            // Filtrar apenas do tenant correto
            var filteredJobs = jobs.Where(j => j.TenantId == tenantId);
            var jobsDto = _mapper.Map<IEnumerable<JobDTO>>(filteredJobs);
            return ResultDTO<IEnumerable<JobDTO>>.SuccessResult(jobsDto);
        }
        catch (Exception ex)
        {
            return ResultDTO<IEnumerable<JobDTO>>.FailureResult($"Erro ao buscar rascunhos: {ex.Message}");
        }
    }

    public async Task<ResultDTO<bool>> ChangeStatusAsync(Guid id, JobStatus newStatus, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(id, tenantId);
            
            if (job == null)
                return ResultDTO<bool>.FailureResult("Vaga não encontrada");

            job.Status = newStatus;
            job.DataAtualizacao = DateTime.UtcNow;

            if (newStatus == JobStatus.Fechada)
                job.DataFechamento = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);
            
            return ResultDTO<bool>.SuccessResult(true, $"Status da vaga alterado para {newStatus}");
        }
        catch (Exception ex)
        {
            return ResultDTO<bool>.FailureResult($"Erro ao alterar status da vaga: {ex.Message}");
        }
    }

    #endregion

    #region Criação com IA

    public async Task<ResultDTO<JobChatResponseDTO>> StartJobCreationWithAIAsync(StartJobCreationDTO dto, Guid tenantId, Guid userId)
    {
        try
        {
            // Criar vaga em rascunho
            var job = new Job
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CriadoPor = userId,
                Titulo = string.Empty,
                Descricao = string.Empty,
                Status = JobStatus.Rascunho,
                ConversationId = Guid.NewGuid(),
                IaCompletionPercentage = 0,
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            await _jobRepository.AddAsync(job);

            // Obter primeira pergunta da IA
            var firstQuestion = await _jobAIService.GetFirstQuestionAsync(dto.MensagemInicial);

            var response = new JobChatResponseDTO
            {
                JobId = job.Id,
                ConversationId = job.ConversationId!.Value,
                MensagemIA = firstQuestion,
                CamposAtualizados = new List<string>(),
                CompletionPercentage = 0,
                CriacaoConcluida = false,
                JobAtualizado = _mapper.Map<JobDetailDTO>(job)
            };

            return ResultDTO<JobChatResponseDTO>.SuccessResult(response, "Criação de vaga iniciada");
        }
        catch (Exception ex)
        {
            return ResultDTO<JobChatResponseDTO>.FailureResult($"Erro ao iniciar criação de vaga: {ex.Message}");
        }
    }

    public async Task<ResultDTO<JobChatResponseDTO>> ProcessAIChatMessageAsync(JobChatMessageDTO dto, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(dto.JobId, tenantId);
            
            if (job == null)
                return ResultDTO<JobChatResponseDTO>.FailureResult("Vaga não encontrada");

            if (job.Status != JobStatus.Rascunho)
                return ResultDTO<JobChatResponseDTO>.FailureResult("Esta vaga já foi finalizada");

            // TODO: Recuperar histórico da conversa do banco (TALENTS.chat_messages)
            var conversationHistory = new List<ChatMessageItem>();

            // Processar resposta com IA
            var aiResult = await _jobAIService.ProcessUserResponseAsync(job, conversationHistory, dto.Mensagem);

            // Atualizar campos extraídos
            foreach (var field in aiResult.ExtractedFields)
            {
                UpdateJobField(job, field.Key, field.Value);
            }

            job.IaCompletionPercentage = aiResult.CompletionPercentage;
            job.DataAtualizacao = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);

            // TODO: Salvar mensagens no histórico (TALENTS.chat_messages)

            var jobDetail = _mapper.Map<JobDetailDTO>(job);
            DeserializeJobJsonFields(job, jobDetail);

            var response = new JobChatResponseDTO
            {
                JobId = job.Id,
                ConversationId = job.ConversationId!.Value,
                MensagemIA = aiResult.AIResponse,
                CamposAtualizados = aiResult.UpdatedFieldNames,
                CompletionPercentage = aiResult.CompletionPercentage,
                CriacaoConcluida = aiResult.IsComplete,
                JobAtualizado = jobDetail
            };

            return ResultDTO<JobChatResponseDTO>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            return ResultDTO<JobChatResponseDTO>.FailureResult($"Erro ao processar mensagem: {ex.Message}");
        }
    }

    public async Task<ResultDTO<JobDetailDTO>> CompleteJobCreationAsync(CompleteJobCreationDTO dto, Guid tenantId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAndTenantAsync(dto.JobId, tenantId);
            
            if (job == null)
                return ResultDTO<JobDetailDTO>.FailureResult("Vaga não encontrada");

            if (job.Status != JobStatus.Rascunho)
                return ResultDTO<JobDetailDTO>.FailureResult("Esta vaga já foi finalizada");

            // Validar campos obrigatórios
            if (string.IsNullOrWhiteSpace(job.Titulo))
                return ResultDTO<JobDetailDTO>.FailureResult("Título é obrigatório para publicar a vaga");

            if (string.IsNullOrWhiteSpace(job.Descricao))
                return ResultDTO<JobDetailDTO>.FailureResult("Descrição é obrigatória para publicar a vaga");

            // Atualizar status
            job.Status = dto.PublicarImediatamente ? JobStatus.Aberta : JobStatus.Rascunho;
            job.IaCompletionPercentage = 100;
            job.DataAtualizacao = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);

            var jobDetail = _mapper.Map<JobDetailDTO>(job);
            DeserializeJobJsonFields(job, jobDetail);

            var message = dto.PublicarImediatamente 
                ? "Vaga publicada com sucesso!" 
                : "Vaga salva como rascunho";

            return ResultDTO<JobDetailDTO>.SuccessResult(jobDetail, message);
        }
        catch (Exception ex)
        {
            return ResultDTO<JobDetailDTO>.FailureResult($"Erro ao finalizar vaga: {ex.Message}");
        }
    }

    #endregion

    #region Helpers

    private void UpdateJobField(Job job, string fieldName, object? value)
    {
        switch (fieldName.ToLower())
        {
            case "titulo":
                job.Titulo = value?.ToString() ?? string.Empty;
                break;
            case "descricao":
                job.Descricao = value?.ToString() ?? string.Empty;
                break;
            case "departamento":
                job.Departamento = value?.ToString();
                break;
            case "numerovagas":
                if (int.TryParse(value?.ToString(), out var numVagas))
                    job.NumeroVagas = numVagas;
                break;
            case "localizacao":
                job.Localizacao = value?.ToString();
                break;
            case "cidade":
                job.Cidade = value?.ToString();
                break;
            case "estado":
                job.Estado = value?.ToString();
                break;
            case "tipocontrato":
                if (Enum.TryParse<ContractType>(value?.ToString(), true, out var tipoContrato))
                    job.TipoContrato = tipoContrato;
                break;
            case "modelotrabalho":
                if (Enum.TryParse<WorkModel>(value?.ToString(), true, out var modeloTrabalho))
                    job.ModeloTrabalho = modeloTrabalho;
                break;
            case "anosExperienciaminimo":
                if (int.TryParse(value?.ToString(), out var anosExp))
                    job.AnosExperienciaMinimo = anosExp;
                break;
            case "formacaonecessaria":
                job.FormacaoNecessaria = value?.ToString();
                break;
            case "conhecimentosobrigatorios":
                job.ConhecimentosObrigatorios = SerializeToJson(value);
                break;
            case "conhecimentosdesejaveis":
                job.ConhecimentosDesejaveis = SerializeToJson(value);
                break;
            case "competenciasimportantes":
                job.CompetenciasImportantes = SerializeToJson(value);
                break;
            case "responsabilidades":
                job.Responsabilidades = value?.ToString();
                break;
            case "salariomin":
                if (decimal.TryParse(value?.ToString(), out var salarioMin))
                    job.SalarioMin = salarioMin;
                break;
            case "salariomax":
                if (decimal.TryParse(value?.ToString(), out var salarioMax))
                    job.SalarioMax = salarioMax;
                break;
            case "beneficios":
                job.Beneficios = value?.ToString();
                break;
            case "bonuscomissao":
                job.BonusComissao = value?.ToString();
                break;
            case "etapasprocesso":
                job.EtapasProcesso = SerializeToJson(value);
                break;
            case "tipostesteentrevista":
                job.TiposTesteEntrevista = SerializeToJson(value);
                break;
            case "previsaoinicio":
                if (DateTime.TryParse(value?.ToString(), out var previsaoInicio))
                    job.PrevisaoInicio = previsaoInicio;
                break;
            case "sobretime":
                job.SobreTime = value?.ToString();
                break;
            case "diferenciais":
                job.Diferenciais = value?.ToString();
                break;
        }
    }

    private static string? SerializeToJson(object? value)
    {
        if (value == null) return null;
        if (value is string str) return str;
        return JsonSerializer.Serialize(value);
    }

    private static List<string>? DeserializeJsonList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch
        {
            return null;
        }
    }

    private void DeserializeJobJsonFields(Job job, JobDetailDTO dto)
    {
        dto.ConhecimentosObrigatorios = DeserializeJsonList(job.ConhecimentosObrigatorios);
        dto.ConhecimentosDesejaveis = DeserializeJsonList(job.ConhecimentosDesejaveis);
        dto.CompetenciasImportantes = DeserializeJsonList(job.CompetenciasImportantes);
        dto.EtapasProcesso = DeserializeJsonList(job.EtapasProcesso);
        dto.TiposTesteEntrevista = DeserializeJsonList(job.TiposTesteEntrevista);
    }

    #endregion
}
