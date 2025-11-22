namespace Sehaty.Application.MappingProfiles
{
    public class DoctorAvailabilitySlotProfile : Profile
    {
        public DoctorAvailabilitySlotProfile()
        {
            CreateMap<DoctorAvailabilitySlot, DoctorAvailabilityAddOrUpdateDto>().ReverseMap();
            CreateMap<DoctorAvailabilitySlot, DoctorSlotsDto>().ReverseMap();
            CreateMap<DoctorAvailabilitySlot, DoctorAvailabilityReadDto>()
                .AfterMap((src, dest) => { dest.DayOfWeek = src.DayOfWeek.ToString(); })
             .ReverseMap();
        }
    }
}
