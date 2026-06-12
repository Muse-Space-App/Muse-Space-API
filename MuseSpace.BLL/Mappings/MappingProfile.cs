using AutoMapper;
using MuseSpace.BLL.DTO;
using MuseSpace.Core.Entities;

namespace MuseSpace.BLL.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Comment, CommentResponse>();

        CreateMap<List<Comment>, IReadOnlyCollection<CommentResponse>>()
            .ConvertUsing((src, _, context) => context.Mapper.Map<List<CommentResponse>>(src).AsReadOnly());
            
        CreateMap<BaseEntity, SampleDto>()
            .ForMember(destination => destination.Name, options => options.MapFrom(source => source.CreatedBy));
    }
}
