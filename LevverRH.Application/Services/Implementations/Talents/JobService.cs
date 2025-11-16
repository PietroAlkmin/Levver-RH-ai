using AutoMapper;
using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces.Talents;

namespace LevverRH.Application.Services.Implementations.Talents
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        public JobService(IJobRepository jobRepository, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

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
                var job = await _jobRepository.GetByIdAsync(id);
                
                if (job == null)
                    return ResultDTO<JobDTO>.FailureResult("Vaga não encontrada");

                if (job.TenantId != tenantId)
                    return ResultDTO<JobDTO>.FailureResult("Acesso negado a esta vaga");

                var jobDto = _mapper.Map<JobDTO>(job);
                return ResultDTO<JobDTO>.SuccessResult(jobDto);
            }
            catch (Exception ex)
            {
                return ResultDTO<JobDTO>.FailureResult($"Erro ao buscar vaga: {ex.Message}");
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
                var job = await _jobRepository.GetByIdAsync(id);
                
                if (job == null)
                    return ResultDTO<JobDTO>.FailureResult("Vaga não encontrada");

                if (job.TenantId != tenantId)
                    return ResultDTO<JobDTO>.FailureResult("Acesso negado a esta vaga");

                // Atualizar apenas os campos fornecidos
                if (!string.IsNullOrEmpty(updateDto.Titulo))
                    job.Titulo = updateDto.Titulo;
                
                if (!string.IsNullOrEmpty(updateDto.Descricao))
                    job.Descricao = updateDto.Descricao;

                if (updateDto.Departamento != null)
                    job.Departamento = updateDto.Departamento;

                if (updateDto.Localizacao != null)
                    job.Localizacao = updateDto.Localizacao;

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
                var job = await _jobRepository.GetByIdAsync(id);
                
                if (job == null)
                    return ResultDTO<bool>.FailureResult("Vaga não encontrada");

                if (job.TenantId != tenantId)
                    return ResultDTO<bool>.FailureResult("Acesso negado a esta vaga");

                await _jobRepository.DeleteAsync(id);
                return ResultDTO<bool>.SuccessResult(true, "Vaga excluída com sucesso");
            }
            catch (Exception ex)
            {
                return ResultDTO<bool>.FailureResult($"Erro ao excluir vaga: {ex.Message}");
            }
        }

        public async Task<ResultDTO<bool>> ChangeStatusAsync(Guid id, JobStatus newStatus, Guid tenantId)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(id);
                
                if (job == null)
                    return ResultDTO<bool>.FailureResult("Vaga não encontrada");

                if (job.TenantId != tenantId)
                    return ResultDTO<bool>.FailureResult("Acesso negado a esta vaga");

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
    }
}
