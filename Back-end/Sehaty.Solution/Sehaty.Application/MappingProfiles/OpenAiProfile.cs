namespace Sehaty.Application.MappingProfiles
{
    public class OpenAiProfile : Profile
    {
        public OpenAiProfile()
        {
            CreateMap<Prescription, PrescriptionAnalysisResponseDto>()
                .ForMember(P => P.PrescriptionId, O => O.MapFrom(S => S.Id))
                .ForMember(P => P.PatientName,
                O => O.MapFrom(S => String.Concat(S.Patient.FirstName, " ", S.Patient.LastName)))
                .ForMember(P => P.DoctorName,
                O => O.MapFrom(S => String.Concat(S.Doctor.FirstName, " ", S.Doctor.LastName)))
                .ForMember(P => P.GeneralInstructions, O => O.MapFrom(S => S.SpecialInstructions));
        }
    }
}
