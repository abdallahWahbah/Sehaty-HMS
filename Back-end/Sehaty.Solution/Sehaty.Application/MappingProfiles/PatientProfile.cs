namespace Sehaty.Application.MappingProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, GetPatientDto>()
                .ForMember(P => P.User, O => O.MapFrom(S => S.User.UserName));

            CreateMap<PatientAddDto, Patient>()
                .ForMember(D => D.RegistrationDate, O => O.Ignore())
                .ForMember(D => D.PatientId, O => O.Ignore());

            CreateMap<PatientUpdateByStaffDto, Patient>();
            CreateMap<PatientUpdateDto, Patient>();
        }
    }
}
