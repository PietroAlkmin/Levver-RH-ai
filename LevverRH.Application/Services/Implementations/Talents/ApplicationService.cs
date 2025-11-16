using AutoMapper;
using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces.Talents;

namespace LevverRH.Application.Services.Implementations.Talents
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IMapper _mapper;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            ICandidateRepository candidateRepository,
            IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _candidateRepository = candidateRepository;
            _mapper = mapper;
        }

        public async Task<ResultDTO<IEnumerable<ApplicationDTO>>> GetByJobIdAsync(Guid jobId, Guid tenantId)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(jobId);
                if (job == null || job.TenantId != tenantId)
                    return ResultDTO<IEnumerable<ApplicationDTO>>.FailureResult("Vaga não encontrada");

                var applications = await _applicationRepository.GetByJobIdAsync(jobId);
                var applicationsDto = _mapper.Map<IEnumerable<ApplicationDTO>>(applications);
                return ResultDTO<IEnumerable<ApplicationDTO>>.SuccessResult(applicationsDto);
            }
            catch (Exception ex)
            {
                return ResultDTO<IEnumerable<ApplicationDTO>>.FailureResult($"Erro ao buscar candidaturas: {ex.Message}");
            }
        }

        public async Task<ResultDTO<ApplicationDetailDTO>> GetByIdAsync(Guid id, Guid tenantId)
        {
            try
            {
                var application = await _applicationRepository.GetByIdAsync(id);
                
                if (application == null)
                    return ResultDTO<ApplicationDetailDTO>.FailureResult("Candidatura não encontrada");

                if (application.TenantId != tenantId)
                    return ResultDTO<ApplicationDetailDTO>.FailureResult("Acesso negado a esta candidatura");

                var applicationDto = _mapper.Map<ApplicationDetailDTO>(application);
                return ResultDTO<ApplicationDetailDTO>.SuccessResult(applicationDto);
            }
            catch (Exception ex)
            {
                return ResultDTO<ApplicationDetailDTO>.FailureResult($"Erro ao buscar candidatura: {ex.Message}");
            }
        }

        public async Task<ResultDTO<bool>> ChangeStatusAsync(Guid id, ApplicationStatus newStatus, Guid tenantId, Guid userId)
        {
            try
            {
                var application = await _applicationRepository.GetByIdAsync(id);
                
                if (application == null)
                    return ResultDTO<bool>.FailureResult("Candidatura não encontrada");

                if (application.TenantId != tenantId)
                    return ResultDTO<bool>.FailureResult("Acesso negado a esta candidatura");

                application.Status = newStatus;
                application.DataAtualizacaoStatus = DateTime.UtcNow;
                application.AvaliadoPor = userId;

                await _applicationRepository.UpdateAsync(application);
                
                return ResultDTO<bool>.SuccessResult(true, $"Status alterado para {newStatus}");
            }
            catch (Exception ex)
            {
                return ResultDTO<bool>.FailureResult($"Erro ao alterar status: {ex.Message}");
            }
        }

        public async Task<ResultDTO<bool>> ToggleFavoritoAsync(Guid id, Guid tenantId)
        {
            try
            {
                var application = await _applicationRepository.GetByIdAsync(id);
                
                if (application == null)
                    return ResultDTO<bool>.FailureResult("Candidatura não encontrada");

                if (application.TenantId != tenantId)
                    return ResultDTO<bool>.FailureResult("Acesso negado a esta candidatura");

                application.Favorito = !application.Favorito;
                await _applicationRepository.UpdateAsync(application);
                
                var message = application.Favorito ? "Adicionado aos favoritos" : "Removido dos favoritos";
                
                return ResultDTO<bool>.SuccessResult(true, message);
            }
            catch (Exception ex)
            {
                return ResultDTO<bool>.FailureResult($"Erro ao alterar favorito: {ex.Message}");
            }
        }
    }
}
