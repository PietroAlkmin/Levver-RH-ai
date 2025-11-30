using AutoMapper;
using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Interfaces.Talents;

namespace LevverRH.Application.Services.Implementations.Talents
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IMapper _mapper;

        public CandidateService(ICandidateRepository candidateRepository, IMapper mapper)
        {
            _candidateRepository = candidateRepository;
            _mapper = mapper;
        }

        public async Task<ResultDTO<IEnumerable<CandidateDTO>>> GetAllAsync(Guid tenantId)
        {
            try
            {
                var candidates = await _candidateRepository.GetByTenantIdAsync(tenantId);
                var candidatesDto = _mapper.Map<IEnumerable<CandidateDTO>>(candidates);
                return ResultDTO<IEnumerable<CandidateDTO>>.SuccessResult(candidatesDto);
            }
            catch (Exception ex)
            {
                return ResultDTO<IEnumerable<CandidateDTO>>.FailureResult($"Erro ao buscar candidatos: {ex.Message}");
            }
        }

        public async Task<ResultDTO<CandidateDTO>> GetByIdAsync(Guid id, Guid tenantId)
        {
            try
            {
                var candidate = await _candidateRepository.GetByIdAsync(id);
                
                if (candidate == null)
                    return ResultDTO<CandidateDTO>.FailureResult("Candidato não encontrado");

                if (candidate.TenantId != tenantId)
                    return ResultDTO<CandidateDTO>.FailureResult("Acesso negado a este candidato");

                var candidateDto = _mapper.Map<CandidateDTO>(candidate);
                return ResultDTO<CandidateDTO>.SuccessResult(candidateDto);
            }
            catch (Exception ex)
            {
                return ResultDTO<CandidateDTO>.FailureResult($"Erro ao buscar candidato: {ex.Message}");
            }
        }

        public async Task<ResultDTO<CandidateDTO>> CreateAsync(CreateCandidateDTO createDto, Guid tenantId)
        {
            try
            {
                // Verificar se já existe candidato com o mesmo email
                var existingCandidate = await _candidateRepository.GetByEmailAsync(createDto.Email, tenantId);
                if (existingCandidate != null)
                    return ResultDTO<CandidateDTO>.FailureResult("Já existe um candidato cadastrado com este e-mail");

                var candidate = _mapper.Map<Candidate>(createDto);
                candidate.Id = Guid.NewGuid();
                candidate.TenantId = tenantId;
                candidate.DataCadastro = DateTime.UtcNow;
                candidate.DataAtualizacao = DateTime.UtcNow;

                var createdCandidate = await _candidateRepository.AddAsync(candidate);
                var candidateDto = _mapper.Map<CandidateDTO>(createdCandidate);
                
                return ResultDTO<CandidateDTO>.SuccessResult(candidateDto, "Candidato cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return ResultDTO<CandidateDTO>.FailureResult($"Erro ao cadastrar candidato: {ex.Message}");
            }
        }

        public async Task<ResultDTO<IEnumerable<CandidateDTO>>> SearchAsync(Guid tenantId, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllAsync(tenantId);

                var candidates = await _candidateRepository.SearchAsync(tenantId, searchTerm);
                var candidatesDto = _mapper.Map<IEnumerable<CandidateDTO>>(candidates);
                return ResultDTO<IEnumerable<CandidateDTO>>.SuccessResult(candidatesDto);
            }
            catch (Exception ex)
            {
                return ResultDTO<IEnumerable<CandidateDTO>>.FailureResult($"Erro ao buscar candidatos: {ex.Message}");
            }
        }
    }
}
