using AutoMapper;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Application.MappingHelpers;
using Sehaty.Core.Entites;

namespace Sehaty.Application.MappingProfiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, GetDoctorDto>()
                .ForMember(D => D.Department, O => O.MapFrom(S => S.Department.Name))
                .ForMember(D => D.User, O => O.MapFrom(S => S.User.UserName))
                .ForMember(D => D.ProfilePhotoUrl, O => O.MapFrom<DoctorProfileImageResolver>());
            CreateMap<DoctorAddUpdateDto, Doctor>()
                .ForMember(D => D.ProfilePhotoUrl, O => O.Ignore());
        }
    }
}
