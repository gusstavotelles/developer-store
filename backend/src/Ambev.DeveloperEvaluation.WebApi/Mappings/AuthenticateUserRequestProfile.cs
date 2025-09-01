using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

/// <summary>
/// AutoMapper profile to map incoming WebApi authentication requests
/// to application layer commands.
/// </summary>
public class AuthenticateUserRequestProfile : Profile
{
    public AuthenticateUserRequestProfile()
    {
        CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
    }
}
