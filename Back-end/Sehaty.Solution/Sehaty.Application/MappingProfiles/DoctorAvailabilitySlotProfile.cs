
using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;

namespace Sehaty.Application.MappingProfiles
{
    public class DoctorAvailabilitySlotProfile : Profile
    {
        public DoctorAvailabilitySlotProfile()
        {
            CreateMap<CreateDoctorAvailabilityDto, DoctorAvailabilitySlot>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.AvailableFlag, opt => opt.MapFrom(src => true));


        }
    }
}
