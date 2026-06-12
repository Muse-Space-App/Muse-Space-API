using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public class GroupMappingProfile : Profile
{
    public GroupMappingProfile()
    {
        CreateMap<Group, GroupResponse>()
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Members.Count));

        CreateMap<GroupPost, GroupPostResponse>()
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.Author != null ? src.Author.Username : string.Empty))
            .ForMember(dest => dest.AuthorAvatarUrl, opt => opt.MapFrom(src => (src.Author != null && src.Author.UserProfile != null) ? src.Author.UserProfile.AvatarUrl : string.Empty));
    }
}
