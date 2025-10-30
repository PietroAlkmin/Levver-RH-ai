using AutoMapper;
using LevverRH.Application.DTOs.Auth;
using LevverRH.Domain.Entities;

namespace LevverRH.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // User → UserInfoDTO
        CreateMap<User, UserInfoDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.AuthType, opt => opt.MapFrom(src => src.AuthType.ToString()));

        // Tenant → TenantInfoDTO
        CreateMap<Tenant, TenantInfoDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // WhiteLabel → WhiteLabelInfoDTO
        CreateMap<WhiteLabel, WhiteLabelInfoDTO>();
    }
}