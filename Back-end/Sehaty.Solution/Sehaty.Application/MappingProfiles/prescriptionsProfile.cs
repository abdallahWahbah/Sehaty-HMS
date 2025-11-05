using AutoMapper;
using Sehaty.Application.Dtos;
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
    public class PrescriptionsProfile : Profile
    {
        public PrescriptionsProfile()
        {
            CreateMap<Prescription, GetPrescriptionsDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.FirstName))
            .ForMember(d => d.PatiantName, o => o.MapFrom(s => s.Patient.FirstName))
            .ForMember(d => d.MRN, o => o.MapFrom(s => s.Patient.MRN))
            .ForMember(d => d.MedicalRecordNotes, o => o.MapFrom(s => s.MedicalRecord.Notes))
            .ForMember(d=>d.LicenseNumber,o=>o.MapFrom(s=>s.Doctor.LicenseNumber));



            CreateMap<CreatePrescriptionsDto, Prescription>();
          
        }
    }
}
