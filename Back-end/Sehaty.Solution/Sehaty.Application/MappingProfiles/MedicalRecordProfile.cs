namespace Sehaty.Application.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordAddByDoctorDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordUpdateDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordAddOrUpdateByNurseDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordReadDto>().AfterMap((src, dest) => { dest.RecordType = src.RecordType.ToString(); })
             .ReverseMap();
        }
    }
}
