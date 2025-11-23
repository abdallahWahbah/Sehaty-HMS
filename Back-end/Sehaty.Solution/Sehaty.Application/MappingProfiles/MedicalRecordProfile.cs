namespace Sehaty.Application.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordAddByDoctorDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordUpdateDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordAddOrUpdateByNurseDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordReadDto>()
                .ForMember(M => M.PatientName, opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}"));
        }
    }
}
