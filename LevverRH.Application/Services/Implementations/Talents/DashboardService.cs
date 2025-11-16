using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces.Talents;

namespace LevverRH.Application.Services.Implementations.Talents
{
    public class DashboardService : IDashboardService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IApplicationRepository _applicationRepository;

        public DashboardService(IJobRepository jobRepository, IApplicationRepository applicationRepository)
        {
            _jobRepository = jobRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<ResultDTO<DashboardStatsDTO>> GetStatsAsync(Guid tenantId)
        {
            try
            {
                // Buscar vagas abertas
                var vagasAbertas = await _jobRepository.CountActiveJobsByTenantAsync(tenantId);

                // Buscar todas as candidaturas do tenant
                var todasCandidaturas = await _applicationRepository.GetByTenantIdAsync(tenantId);
                
                var totalCandidaturas = todasCandidaturas.Count();
                var candidaturasNovas = todasCandidaturas.Count(a => a.Status == ApplicationStatus.Novo);
                var entrevistasAgendadas = todasCandidaturas.Count(a => a.Status == ApplicationStatus.Entrevista);

                // Calcular taxa de conversão (aprovados / total de candidaturas)
                var aprovados = todasCandidaturas.Count(a => a.Status == ApplicationStatus.Aprovado);
                var taxaConversao = totalCandidaturas > 0 
                    ? Math.Round((decimal)aprovados / totalCandidaturas * 100, 2) 
                    : 0;

                var stats = new DashboardStatsDTO
                {
                    VagasAbertas = vagasAbertas,
                    TotalCandidaturas = totalCandidaturas,
                    CandidaturasNovas = candidaturasNovas,
                    EntrevistasAgendadas = entrevistasAgendadas,
                    TaxaConversao = taxaConversao
                };

                return ResultDTO<DashboardStatsDTO>.SuccessResult(stats);
            }
            catch (Exception ex)
            {
                return ResultDTO<DashboardStatsDTO>.FailureResult($"Erro ao buscar estatísticas: {ex.Message}");
            }
        }
    }
}
