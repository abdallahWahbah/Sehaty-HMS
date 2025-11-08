using AutoMapper;
using Sehaty.Application.Dtos;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Application.Dtos.MedicationDTOs;
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
            .ForMember(d => d.LicenseNumber, o => o.MapFrom(s => s.Doctor.LicenseNumber));




            CreateMap<CreatePrescriptionsDto, Prescription>();
            CreateMap<MedicationDto, PrescriptionMedications>()
            .ForMember(dest => dest.Medication,
                       opt => opt.MapFrom(src => new Medication
                       {
                           Name = src.MedicationName
                       }
            ));


            CreateMap<Prescription, DoctorPrescriptionsDto>()
               .ForMember(dest => dest.PrescriptionId,
                          opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.PatientName,
                          opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
               .ForMember(dest => dest.DateIssued,
                          opt => opt.MapFrom(src => src.DateIssued))
               .ForMember(dest => dest.Status,
                          opt => opt.MapFrom(src => src.Status));


            CreateMap<PrescriptionMedications, MedicationDto>()
                .ForMember(dest => dest.MedicationName,
                            opt => opt.MapFrom(src => src.Medication.Name))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

        }
    }
}
