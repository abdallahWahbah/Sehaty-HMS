using AutoMapper;
using Sehaty.Core.Entites;
using Sehaty.Infrastructure.Dtos;

namespace Sehaty.Application.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordDoctorDto>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordNurseDto>().ReverseMap();

        }
    }
}
