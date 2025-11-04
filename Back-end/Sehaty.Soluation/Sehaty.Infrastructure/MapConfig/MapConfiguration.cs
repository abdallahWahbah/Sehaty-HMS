using AutoMapper;
using Sehaty.Core.Entites;
using Sehaty.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.MapConfig
{
    public class MapConfiguration :Profile
    {
        public MapConfiguration()
        {
            CreateMap<MedicalRecord, MedicalRecordDto>().ReverseMap();
        }
    }
}
