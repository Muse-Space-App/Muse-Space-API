using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public class ArtworkMappingProfile : Profile
{
    public ArtworkMappingProfile()
    {
        CreateMap<Tag, TagResponse>();
        CreateMap<Tag, PopularTagResponse>();

        CreateMap<Artwork, ArtworkResponse>()
            .ForMember(dest => dest.CreatorUsername, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Username : string.Empty))
            .ForMember(dest => dest.CreatorProfileImageUrl, opt => opt.MapFrom(src => (src.Creator != null && src.Creator.UserProfile != null) ? src.Creator.UserProfile.AvatarUrl : string.Empty))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ArtworkTags.Select(at => at.Tag)));
    }
}
