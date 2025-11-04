using AutoMapper;
using Sehaty.Application.Dtos;
using Sehaty.Core.Entites;

namespace Sehaty.Application.MappingProfiles
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordDto>().ReverseMap();
        }
    }
}
