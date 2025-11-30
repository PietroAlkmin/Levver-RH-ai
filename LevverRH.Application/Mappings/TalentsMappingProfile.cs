using AutoMapper;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Domain.Entities.Talents;
using ApplicationEntity = LevverRH.Domain.Entities.Talents.Application;

namespace LevverRH.Application.Mappings;

public class TalentsMappingProfile : Profile
{
    public TalentsMappingProfile()
    {
        // Job mappings
        CreateMap<Job, JobDTO>()
            .ForMember(dest => dest.TipoContrato, opt => opt.MapFrom(src => src.TipoContrato.HasValue ? src.TipoContrato.Value.ToString() : null))
            .ForMember(dest => dest.ModeloTrabalho, opt => opt.MapFrom(src => src.ModeloTrabalho.HasValue ? src.ModeloTrabalho.Value.ToString() : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TotalCandidaturas, opt => opt.MapFrom(src => src.Applications != null ? src.Applications.Count : 0));

        CreateMap<CreateJobDTO, Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoPor, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataFechamento, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.ConversationId, opt => opt.Ignore())
            .ForMember(dest => dest.IaCompletionPercentage, opt => opt.Ignore())
            .ForMember(dest => dest.Tenant, opt => opt.Ignore())
            .ForMember(dest => dest.Criador, opt => opt.Ignore())
            .ForMember(dest => dest.Applications, opt => opt.Ignore());

        // Job completo para resposta detalhada
        CreateMap<Job, JobDetailDTO>()
            .ForMember(dest => dest.TipoContrato, opt => opt.MapFrom(src => src.TipoContrato.HasValue ? src.TipoContrato.Value.ToString() : null))
            .ForMember(dest => dest.ModeloTrabalho, opt => opt.MapFrom(src => src.ModeloTrabalho.HasValue ? src.ModeloTrabalho.Value.ToString() : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TotalCandidaturas, opt => opt.MapFrom(src => src.Applications != null ? src.Applications.Count : 0))
            // Ignorar campos JSON que serão convertidos manualmente por DeserializeJobJsonFields
            .ForMember(dest => dest.ConhecimentosObrigatorios, opt => opt.Ignore())
            .ForMember(dest => dest.ConhecimentosDesejaveis, opt => opt.Ignore())
            .ForMember(dest => dest.CompetenciasImportantes, opt => opt.Ignore())
            .ForMember(dest => dest.EtapasProcesso, opt => opt.Ignore())
            .ForMember(dest => dest.TiposTesteEntrevista, opt => opt.Ignore());

        // Candidate mappings
        CreateMap<Candidate, CandidateDTO>()
            .ForMember(dest => dest.NivelSenioridade, opt => opt.MapFrom(src => src.NivelSenioridade.HasValue ? src.NivelSenioridade.Value.ToString() : null));

        CreateMap<CreateCandidateDTO, Candidate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Tenant, opt => opt.Ignore())
            .ForMember(dest => dest.Applications, opt => opt.Ignore());

        // Application mappings
        CreateMap<ApplicationEntity, ApplicationDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.JobTitulo, opt => opt.MapFrom(src => src.Job != null ? src.Job.Titulo : string.Empty))
            .ForMember(dest => dest.CandidateNome, opt => opt.MapFrom(src => src.Candidate != null ? src.Candidate.Nome : string.Empty))
            .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate != null ? src.Candidate.Email : string.Empty));

        CreateMap<ApplicationEntity, ApplicationDetailDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Candidate, opt => opt.MapFrom(src => src.Candidate))
            .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.Job));
    }
}
