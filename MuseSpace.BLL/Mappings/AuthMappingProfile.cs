using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public sealed class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : (src.RoleId == 1 ? "Admin" : "Member")))
            .ForMember(dest => dest.LastLoginUtc, opt => opt.MapFrom(src => src.LastLoginUtc));

        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.UserProfile != null ? src.UserProfile.Bio : string.Empty))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserProfile != null ? src.UserProfile.AvatarUrl : string.Empty))
            .ForMember(dest => dest.BannerUrl, opt => opt.MapFrom(src => src.UserProfile != null ? src.UserProfile.BannerUrl : string.Empty))
            .ForMember(dest => dest.CreatorTier, opt => opt.MapFrom(src => src.UserProfile != null ? src.UserProfile.CreatorTier : string.Empty))
            .ForMember(dest => dest.IsAcceptingCommissions, opt => opt.MapFrom(src => src.UserProfile != null ? src.UserProfile.IsAcceptingCommissions : false))
            .ForMember(dest => dest.FollowerCount, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsFollowing, opt => opt.Ignore());
    }
}
