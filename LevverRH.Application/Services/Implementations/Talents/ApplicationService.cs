using AutoMapper;
using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Public;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Entities;
using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces;
using LevverRH.Domain.Interfaces.Talents;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LevverRH.Application.Services.Implementations.Talents
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IStorageService _storageService;
        private readonly IPdfExtractor _pdfExtractor;
        private readonly ICandidateAnalyzer _candidateAnalyzer;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            ICandidateRepository candidateRepository,
            IUserRepository userRepository,
            ITenantRepository tenantRepository,
            IFileStorageService fileStorageService,
            IStorageService storageService,
            IPdfExtractor pdfExtractor,
            ICandidateAnalyzer candidateAnalyzer,
            IMapper mapper,
            IConfiguration configuration)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _candidateRepository = candidateRepository;
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _fileStorageService = fileStorageService;
            _storageService = storageService;
            _pdfExtractor = pdfExtractor;
            _candidateAnalyzer = candidateAnalyzer;
            _mapper = mapper;
            _configuration = configuration;
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

        public async Task<ResultDTO<PublicApplicationResponseDTO>> CreatePublicApplicationAsync(
            CreatePublicApplicationDTO dto,
            Stream? curriculoStream,
            string? curriculoFileName,
            string? curriculoContentType)
        {
            try
            {
                // 1. Validar se a vaga existe e está aberta
                var job = await _jobRepository.GetByIdAsync(dto.JobId);
                if (job == null)
                {
                    return ResultDTO<PublicApplicationResponseDTO>.FailureResult("Vaga não encontrada");
                }

                if (job.Status != JobStatus.Aberta)
                {
                    return ResultDTO<PublicApplicationResponseDTO>.FailureResult("Esta vaga não está mais recebendo candidaturas");
                }

                // 2. Validar e fazer upload do currículo
                string? curriculoUrl = null;
                Guid candidateTempId = Guid.NewGuid();
                
                if (curriculoStream != null && !string.IsNullOrEmpty(curriculoFileName))
                {
                    var validationError = _fileStorageService.ValidateFile(curriculoFileName, curriculoStream.Length);
                    if (validationError != null)
                    {
                        return ResultDTO<PublicApplicationResponseDTO>.FailureResult(validationError);
                    }

                    curriculoUrl = await _storageService.UploadCurriculoAsync(
                        job.TenantId,
                        candidateTempId,
                        curriculoStream,
                        curriculoFileName,
                        curriculoContentType ?? "application/pdf"
                    );
                }

                // 3. Verificar se candidato já existe (pelo email)
                var existingCandidate = await _candidateRepository.GetByEmailAsync(dto.Email, job.TenantId);
                
                Candidate candidate;
                if (existingCandidate != null)
                {
                    // Atualiza dados do candidato existente
                    candidate = existingCandidate;
                    candidate.Nome = dto.Nome;
                    candidate.Telefone = dto.Telefone;
                    candidate.LinkedinUrl = dto.LinkedinUrl;
                    
                    // Se enviou novo currículo
                    if (curriculoStream != null && !string.IsNullOrEmpty(curriculoFileName))
                    {
                        // Remove currículo antigo do blob se existir
                        if (!string.IsNullOrEmpty(candidate.CurriculoArquivoUrl))
                        {
                            await _storageService.DeleteFileAsync(candidate.CurriculoArquivoUrl);
                        }
                        
                        // Faz upload com ID correto do candidato
                        curriculoStream.Position = 0; // Reseta stream
                        curriculoUrl = await _storageService.UploadCurriculoAsync(
                            job.TenantId,
                            candidate.Id,
                            curriculoStream,
                            curriculoFileName,
                            curriculoContentType ?? "application/pdf"
                        );
                        
                        candidate.CurriculoArquivoUrl = curriculoUrl;
                    }
                    
                    candidate.DataAtualizacao = DateTime.UtcNow;
                    await _candidateRepository.UpdateAsync(candidate);
                }
                else
                {
                    // Cria novo candidato com ID real
                    candidate = new Candidate
                    {
                        Id = candidateTempId, // Usa o ID temporário que já foi usado no upload
                        TenantId = job.TenantId,
                        Nome = dto.Nome,
                        Email = dto.Email,
                        Telefone = dto.Telefone,
                        LinkedinUrl = dto.LinkedinUrl,
                        CurriculoArquivoUrl = curriculoUrl,
                        FonteOrigem = "Aplicação Pública",
                        DataCadastro = DateTime.UtcNow,
                        DataAtualizacao = DateTime.UtcNow
                    };
                    
                    await _candidateRepository.AddAsync(candidate);
                }

                // 4. Verificar se já não existe uma candidatura para esta vaga
                var existingApplication = await _applicationRepository.GetByJobAndCandidateAsync(dto.JobId, candidate.Id);
                if (existingApplication != null)
                {
                    return ResultDTO<PublicApplicationResponseDTO>.FailureResult("Você já se candidatou para esta vaga");
                }

                // 5. Criar a candidatura
                var application = new Domain.Entities.Talents.Application
                {
                    Id = Guid.NewGuid(),
                    TenantId = job.TenantId,
                    JobId = dto.JobId,
                    CandidateId = candidate.Id,
                    Status = ApplicationStatus.Novo,
                    DataInscricao = DateTime.UtcNow
                };

                await _applicationRepository.AddAsync(application);

                // 6. Criar conta de usuário se solicitado
                string? accessToken = null;
                bool contaCriada = false;

                if (dto.CriarConta && !string.IsNullOrEmpty(dto.Senha))
                {
                    // Verificar se já existe usuário com este email
                    var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
                    
                    if (existingUser == null)
                    {
                        // Buscar tenant para criar usuário
                        var tenant = await _tenantRepository.GetByIdAsync(job.TenantId);
                        if (tenant == null)
                        {
                            return ResultDTO<PublicApplicationResponseDTO>.FailureResult("Tenant não encontrado");
                        }

                        // Hash da senha usando BCrypt
                        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

                        // Criar novo usuário usando factory method
                        var newUser = User.CriarComSenha(
                            tenantId: job.TenantId,
                            email: dto.Email,
                            nome: dto.Nome,
                            passwordHash: passwordHash,
                            role: UserRole.Candidato,
                            tenant: tenant
                        );

                        newUser.RegistrarLogin();
                        await _userRepository.AddAsync(newUser);

                        // Gerar token JWT
                        accessToken = GenerateJwtToken(newUser);
                        contaCriada = true;
                    }
                }

                var response = new PublicApplicationResponseDTO
                {
                    Success = true,
                    Message = contaCriada 
                        ? "Candidatura enviada com sucesso! Sua conta foi criada e você já está logado." 
                        : "Candidatura enviada com sucesso! Entraremos em contato em breve.",
                    ApplicationId = application.Id,
                    ContaCriada = contaCriada,
                    AccessToken = accessToken
                };

                return ResultDTO<PublicApplicationResponseDTO>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao criar candidatura pública: {ex.Message}");
                return ResultDTO<PublicApplicationResponseDTO>.FailureResult($"Erro ao processar candidatura: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "LevverRH";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "LevverRH";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResultDTO<AnalyzeCandidateResponseDTO>> AnalyzeCandidateWithAIAsync(Guid applicationId, Guid tenantId)
        {
            try
            {
                // 1. Buscar a candidatura
                var application = await _applicationRepository.GetByIdAsync(applicationId);
                if (application == null || application.TenantId != tenantId)
                {
                    return ResultDTO<AnalyzeCandidateResponseDTO>.FailureResult("Candidatura não encontrada");
                }

                // 2. Buscar candidato e currículo
                var candidate = await _candidateRepository.GetByIdAsync(application.CandidateId);
                if (candidate == null)
                {
                    return ResultDTO<AnalyzeCandidateResponseDTO>.FailureResult("Candidato não encontrado");
                }

                if (string.IsNullOrEmpty(candidate.CurriculoArquivoUrl))
                {
                    return ResultDTO<AnalyzeCandidateResponseDTO>.FailureResult("Currículo não encontrado para este candidato");
                }

                // 3. Buscar informações da vaga
                var job = await _jobRepository.GetByIdAsync(application.JobId);
                if (job == null)
                {
                    return ResultDTO<AnalyzeCandidateResponseDTO>.FailureResult("Vaga não encontrada");
                }

                // 4. Baixar currículo do Azure Blob e extrair texto
                var storageService = _fileStorageService as FileStorageService;
                if (storageService == null)
                {
                    return ResultDTO<AnalyzeCandidateResponseDTO>.FailureResult("Serviço de storage indisponível");
                }

                byte[] pdfBytes = await storageService.DownloadFileAsync(candidate.CurriculoArquivoUrl);
                string resumeText = await _pdfExtractor.ExtractTextAsync(pdfBytes);

                // 5. Montar requisitos da vaga
                var jobRequirements = BuildJobRequirements(job);

                // 6. Analisar com IA
                var analysisResult = await _candidateAnalyzer.AnalyzeAsync(resumeText, jobRequirements);

                // 7. Salvar resultados na candidatura
                application.ScoreGeral = analysisResult.Score;
                application.JustificativaIA = analysisResult.Summary;
                application.DataCalculoScore = DateTime.UtcNow;
                
                await _applicationRepository.UpdateAsync(application);

                // 8. Retornar resposta
                var response = new AnalyzeCandidateResponseDTO
                {
                    ScoreGeral = analysisResult.Score,
                    Justificativa = analysisResult.Summary,
                    TokensUsed = analysisResult.TokensUsed,
                    EstimatedCost = analysisResult.EstimatedCost
                };

                return ResultDTO<AnalyzeCandidateResponseDTO>.SuccessResult(response, "Análise concluída com sucesso");
            }
            catch (Exception ex)
            {
                return ResultDTO<AnalyzeCandidateResponseDTO>.FailureResult($"Erro ao analisar candidato: {ex.Message}");
            }
        }

        private string BuildJobRequirements(Job job)
        {
            var requirements = new StringBuilder();
            requirements.AppendLine($"TÍTULO DA VAGA: {job.Titulo}");
            requirements.AppendLine($"DESCRIÇÃO: {job.Descricao}");
            
            if (!string.IsNullOrEmpty(job.Responsabilidades))
            {
                requirements.AppendLine($"RESPONSABILIDADES: {job.Responsabilidades}");
            }

            if (!string.IsNullOrEmpty(job.ConhecimentosObrigatorios))
            {
                requirements.AppendLine($"CONHECIMENTOS OBRIGATÓRIOS: {job.ConhecimentosObrigatorios}");
            }

            if (!string.IsNullOrEmpty(job.ConhecimentosDesejaveis))
            {
                requirements.AppendLine($"CONHECIMENTOS DESEJÁVEIS: {job.ConhecimentosDesejaveis}");
            }

            if (!string.IsNullOrEmpty(job.CompetenciasImportantes))
            {
                requirements.AppendLine($"COMPETÊNCIAS: {job.CompetenciasImportantes}");
            }

            if (!string.IsNullOrEmpty(job.FormacaoNecessaria))
            {
                requirements.AppendLine($"FORMAÇÃO: {job.FormacaoNecessaria}");
            }

            if (job.AnosExperienciaMinimo.HasValue)
            {
                requirements.AppendLine($"EXPERIÊNCIA MÍNIMA: {job.AnosExperienciaMinimo} anos");
            }
            
            if (!string.IsNullOrEmpty(job.Beneficios))
            {
                requirements.AppendLine($"BENEFÍCIOS: {job.Beneficios}");
            }

            if (job.SalarioMin.HasValue || job.SalarioMax.HasValue)
            {
                requirements.AppendLine($"FAIXA SALARIAL: R$ {job.SalarioMin ?? 0:N2} - R$ {job.SalarioMax ?? 0:N2}");
            }

            return requirements.ToString();
        }
    }
}
