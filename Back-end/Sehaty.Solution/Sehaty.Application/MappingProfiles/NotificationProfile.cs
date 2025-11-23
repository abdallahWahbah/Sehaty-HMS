namespace Sehaty.Application.MappingProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, AllNotificationsDto>()
                .ForMember(n => n.UserName, o => o.MapFrom(s => s.User.FirstName));

            CreateMap<CreateNotificationDto, Notification>();
            //.ForMember(S => S.NotificationType,
            //O => O.MapFrom(N => N.NotificationType));
            //.ForMember(s=>s.Priority,o=>o.MapFrom(n=>n.Priority.ToString()));
        }
    }
}
