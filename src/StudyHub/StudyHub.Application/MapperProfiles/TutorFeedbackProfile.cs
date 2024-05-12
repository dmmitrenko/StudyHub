using AutoMapper;
using StudyHub.Domain.Models;
using System.Net;

namespace StudyHub.Application.MapperProfiles;
public class TutorFeedbackProfile : Profile
{
    public TutorFeedbackProfile()
    {
        CreateMap<Feedback, DataContext.Entities.Feedback>()
            .ForMember(x => x.Rate, src => src.MapFrom(dest => Convert.ToInt32(dest.Rate)))
            .ForMember(x => x.PartitionKey, src => src.MapFrom(dest => $"{dest.TutorSurname}_{dest.TutorName}_{dest.TutorMiddleName}"))
            .ForMember(x => x.RowKey, src => src.MapFrom(dest => $"{dest.TutorSurname}_{DateTime.UtcNow.Ticks}" ))
            .ForMember(x => x.Timestamp, src => src.MapFrom(dest => DateTimeOffset.Now))
            .ForMember(x => x.TutorMiddleName, src => src.MapFrom(dest => WebUtility.HtmlEncode(dest.TutorMiddleName)))
            .ForMember(x => x.TutorName, src => src.MapFrom(dest => WebUtility.HtmlEncode(dest.TutorName)))
            .ForMember(x => x.TutorSurname, src => src.MapFrom(dest => WebUtility.HtmlEncode(dest.TutorSurname)))
            .ForMember(x => x.Text, src => src.MapFrom(dest => WebUtility.HtmlEncode(dest.Text)));

        CreateMap<DataContext.Entities.Feedback, Feedback>()
            .ForMember(x => x.Rate, src => src.MapFrom(dest => (ushort)dest.Rate));
    }
}
