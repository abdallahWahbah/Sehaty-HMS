using AutoMapper;
using Sehaty.Application.Dtos.DoctorAvailabilitySlotDto;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.MappingProfiles
{
    public class DoctorAvailabilitySlotProfile :Profile
    {
        public DoctorAvailabilitySlotProfile()
        {
            CreateMap<DoctorAvailabilitySlot, DoctorAvailabilitySlotDto>()
                .AfterMap((src, dest) => { dest.DayOfWeek = src.DayOfWeek.ToString(); })
             .ReverseMap();
        }
    }
}
