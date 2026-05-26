using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.OrganizerUsername, opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.Username : string.Empty))
            .ForMember(dest => dest.OrganizerAvatarUrl, opt => opt.MapFrom(src => (src.Organizer != null && src.Organizer.UserProfile != null) ? src.Organizer.UserProfile.AvatarUrl : string.Empty));
    }
}
