using AutoMapper;
using StudyHub.Domain.Models;

namespace StudyHub.Application.MapperProfiles;
public class TutorFeedbackProfile : Profile
{
    public TutorFeedbackProfile()
    {
        CreateMap<Feedback, DataContext.Entities.Feedback>()
            .ForMember(x => x.Rate, src => src.MapFrom(dest => Convert.ToInt32(dest.Rate)))
            .ForMember(x => x.PartitionKey, src => src.MapFrom(dest => $"{dest.TutorName}_{dest.TutorSurname}_{dest.TutorMiddleName}"))
            .ForMember(x => x.RowKey, src => src.MapFrom(dest => $"{dest.TutorSurname}_{DateTime.UtcNow.Ticks}" ))
            .ForMember(x => x.Timestamp, src => src.MapFrom(dest => DateTimeOffset.Now));

        CreateMap<DataContext.Entities.Feedback, Feedback>()
            .ForMember(x => x.Rate, src => src.MapFrom(dest => (ushort)dest.Rate));
    }
}
