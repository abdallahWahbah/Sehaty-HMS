using AutoMapper;
using Sehaty.Application.Dtos.NotificationsDTOs;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.MappingProfiles
{
    public class NotificationProfile:Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, AllNotificationsDto>()
                .ForMember(n => n.UserName, o => o.MapFrom(s => s.User.FirstName));

            CreateMap<CreateNotificationDto, Notification>()
                .ForMember(s=>s.Priority,o=>o.MapFrom(n=>n.Priority.ToString()));
        }
    }
}
