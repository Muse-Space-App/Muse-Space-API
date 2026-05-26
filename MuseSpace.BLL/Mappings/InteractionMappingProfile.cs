using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public class InteractionMappingProfile : Profile
{
    public InteractionMappingProfile()
    {
        CreateMap<Comment, CommentResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
            .ForMember(dest => dest.UserProfileImageUrl, opt => opt.MapFrom(src => (src.User != null && src.User.UserProfile != null) ? src.User.UserProfile.AvatarUrl : string.Empty));
    }
}
