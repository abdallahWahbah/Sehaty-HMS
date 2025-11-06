using AutoMapper;
using Sehaty.Application.Dtos.DepartmentDtos;
using Sehaty.Core.Entites;

namespace Sehaty.Application.MappingProfiles
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, GetDepartmentDto>()
                .ForMember(D => D.En_Name, O => O.MapFrom(S => S.Name))
                .ForMember(D => D.Ar_Name, O => O.MapFrom(S => S.NameLocal));
            CreateMap<DepartmentAddUpdateDto, Department>()
                .ForMember(D => D.Name, O => O.MapFrom(S => S.En_Name))
                .ForMember(D => D.NameLocal, O => O.MapFrom(S => S.Ar_Name));
        }
    }
}
