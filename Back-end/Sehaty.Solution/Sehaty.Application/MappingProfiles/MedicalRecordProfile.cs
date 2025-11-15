using AutoMapper;
using Sehaty.Application.Dtos.MedicalRecordDto;
using Sehaty.Core.Entities.Business_Entities.MedicalRecords;
using Sehaty.Infrastructure.Dtos;

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
