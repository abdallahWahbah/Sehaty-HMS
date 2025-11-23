namespace Sehaty.Application.MappingProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            //Read
            CreateMap<Appointment, AppointmentReadDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FirstName + " " + src.Doctor.LastName
                    : "Unknown Doctor"))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null
                    ? src.Patient.FirstName + " " + src.Patient.LastName
                    : "Unknown Patient"));



            // Create 
            CreateMap<AppointmentAddDto, Appointment>()
                .ForMember(dest => dest.ScheduledDate,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(src.ScheduledDate)))
                .ForMember(dest => dest.ScheduledTime,
                    opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.ScheduledTime)))
                .ForMember(dest => dest.BookingDateTime,
                    opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.DurationMinutes,
                    opt => opt.MapFrom(src => src.DurationMinutes ?? 30));

        }


    }
}
