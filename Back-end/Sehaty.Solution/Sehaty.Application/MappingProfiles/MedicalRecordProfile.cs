using AutoMapper;
using Sehaty.Application.Dtos.MedicalRecordDto;
using Sehaty.Core.Entites;
using Sehaty.Infrastructure.Dtos;

namespace Sehaty.Application.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordAddOrUpdateByDoctorDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordAddOrUpdateByNurseDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordReadDto>().ReverseMap();

        }
    }
}
