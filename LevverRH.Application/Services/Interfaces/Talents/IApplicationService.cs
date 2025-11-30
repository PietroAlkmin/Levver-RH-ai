using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Public;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Application.Services.Interfaces.Talents
{
    public interface IApplicationService
    {
        Task<ResultDTO<IEnumerable<ApplicationDTO>>> GetByJobIdAsync(Guid jobId, Guid tenantId);
        Task<ResultDTO<ApplicationDetailDTO>> GetByIdAsync(Guid id, Guid tenantId);
        Task<ResultDTO<bool>> ChangeStatusAsync(Guid id, ApplicationStatus newStatus, Guid tenantId, Guid userId);
        Task<ResultDTO<bool>> ToggleFavoritoAsync(Guid id, Guid tenantId);
        
        /// <summary>
        /// Cria uma candidatura pública (sem autenticação)
        /// Usado quando candidatos se aplicam através do formulário público
        /// </summary>
        Task<ResultDTO<PublicApplicationResponseDTO>> CreatePublicApplicationAsync(
            CreatePublicApplicationDTO dto, 
            Stream? curriculoStream, 
            string? curriculoFileName, 
            string? curriculoContentType);
    }
}
