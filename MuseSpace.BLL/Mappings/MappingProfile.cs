using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BaseEntity, SampleDto>()
            .ForMember(destination => destination.Name, options => options.MapFrom(source => source.CreatedBy));
    }
}
