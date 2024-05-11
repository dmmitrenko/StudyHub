using AutoMapper;
using StudyHub.Domain.Models;

namespace StudyHub.Application.MapperProfiles;
public class ReminderProfile : Profile
{
    public ReminderProfile()
    {
        CreateMap<Reminder, DataContext.Entities.Reminder>()
            .ForMember(x => x.ChatId, src => src.MapFrom(dest => dest.ChatId))
            .ForMember(x => x.SendTime, src => src.MapFrom(dest => dest.SendTime))
            .ForMember(x => x.Text, src => src.MapFrom(dest => dest.Text))
            .ForMember(x => x.PartitionKey, src => src.MapFrom(dest => dest.ChatId.ToString()))
            .ForMember(x => x.RowKey, src => src.MapFrom(dest => dest.SendTime.Ticks.ToString()));

        CreateMap<DataContext.Entities.Reminder, Reminder>();
    }
}
