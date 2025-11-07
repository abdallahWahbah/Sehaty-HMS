using AutoMapper;
using Sehaty.Application.Dtos.AppointmentDTOs;
using Sehaty.Core.Entites;

namespace Sehaty.Application.MappingProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            //Read
            CreateMap<Appointment, AppointmentReadDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FirstName + " " + src.Doctor.LastName
                    : "Unknown Doctor"))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null
                    ? src.Patient.FirstName + " " + src.Patient.LastName
                    : "Unknown Patient"));



            // Create 
            CreateMap<AppointmentCreateDto, Appointment>()
                .ForMember(dest => dest.ScheduledDate,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(src.ScheduledDate)))
                .ForMember(dest => dest.ScheduledTime,
                    opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.ScheduledTime)))
                .ForMember(dest => dest.BookingDateTime,
                    opt => opt.MapFrom(_ => DateTime.Now))
                .AfterMap((src, dest) =>
                {
                    // Handle nullable or invalid status safely
                    if (!string.IsNullOrWhiteSpace(src.Status) &&
                        Enum.TryParse(src.Status, true, out AppointmentStatus parsed))
                        dest.Status = parsed;
                    else
                        dest.Status = AppointmentStatus.Pending;
                });

            // Update 
            CreateMap<AppointmentUpdateDto, Appointment>()
                .ForMember(dest => dest.ScheduledDate,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(src.ScheduledDate)))
                .ForMember(dest => dest.ScheduledTime,
                    opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.ScheduledTime)))
                .AfterMap((src, dest) =>
                {
                    if (!string.IsNullOrWhiteSpace(src.Status) &&
                        Enum.TryParse(src.Status, true, out AppointmentStatus parsed))
                        dest.Status = parsed;
                    // Keep existing status if DTO didn’t specify a new one
                });

            // Return
            CreateMap<Appointment, AppointmentCreateDto>()
                .ForMember(dest => dest.ScheduledDate,
                    opt => opt.MapFrom(src => src.ScheduledDate.HasValue
                        ? src.ScheduledDate.Value.ToDateTime(TimeOnly.MinValue)
                        : DateTime.MinValue))
                .ForMember(dest => dest.ScheduledTime,
                    opt => opt.MapFrom(src => src.ScheduledTime.HasValue
                        ? src.ScheduledTime.Value.ToTimeSpan()
                        : TimeSpan.Zero))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
