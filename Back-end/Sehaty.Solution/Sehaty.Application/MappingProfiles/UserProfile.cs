namespace Sehaty.Application.MappingProfiles
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            CreateMap<(ApplicationUser user, string role), AppUserDto>()
                .ForMember(U => U.Id, O => O.MapFrom(S => S.user.Id))
                .ForMember(U => U.PhoneNumber, O => O.MapFrom(S => S.user.PhoneNumber))
                .ForMember(U => U.Email, O => O.MapFrom(S => S.user.Email))
                .ForMember(U => U.UserName, O => O.MapFrom(S => S.user.UserName))
                .ForMember(U => U.FirstName, O => O.MapFrom(S => S.user.FirstName))
                .ForMember(U => U.LastNAme, O => O.MapFrom(S => S.user.LastName))
                .ForMember(U => U.Role, O => O.MapFrom(S => S.role));
        }
    }
}
