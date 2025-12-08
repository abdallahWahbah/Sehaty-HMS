namespace Sehaty.Application.MappingProfiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, GetFeedbackDto>()
                .ForMember(F => F.AppointmentDateTime,
                O => O.MapFrom(S => S.Appointment.AppointmentDateTime));
            CreateMap<FeedbackAddUpdateDto, Feedback>();
        }
    }
}
