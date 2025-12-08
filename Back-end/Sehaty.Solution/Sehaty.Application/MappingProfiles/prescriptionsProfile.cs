namespace Sehaty.Application.MappingProfiles
{
    public class PrescriptionsProfile : Profile
    {
        public PrescriptionsProfile()
        {
            //mapping for get prescriptions dto
            CreateMap<Prescription, GetPrescriptionsDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.FirstName))
            .ForMember(d => d.PatiantName, o => o.MapFrom(s => s.Patient.FirstName))
            //.ForMember(d => d.MRN, o => o.MapFrom(s => s.Patient.MRN))
            .ForMember(d => d.MedicalRecordNotes, o => o.MapFrom(s => s.MedicalRecord.Notes))
            .ForMember(d => d.LicenseNumber, o => o.MapFrom(s => s.Doctor.LicenseNumber));



            //mapping for create prescription dto and reverse map
            CreateMap<CreatePrescriptionsDto, Prescription>();
            CreateMap<UpdatePrescriptionDto, Prescription>();

            CreateMap<MedicationDto, PrescriptionMedications>()
            .ForMember(dest => dest.Medication,
                       opt => opt.MapFrom(src => new Medication
                       {
                           Name = src.MedicationName
                       }
            )).ReverseMap();


            //mapping for doctor prescriptions dto
            CreateMap<Prescription, DoctorPrescriptionsDto>()
               .ForMember(dest => dest.PrescriptionId,
                          opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.PatientName,
                          opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
               .ForMember(dest => dest.DateIssued,
                          opt => opt.MapFrom(src => src.DateIssued))
               .ForMember(dest => dest.Status,
                          opt => opt.MapFrom(src => src.Status));


            //mapping for patient prescriptions dto
            CreateMap<Prescription, PatientPrescriptionsDto>()
                .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DoctorName,
                           opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
                .ForMember(dest => dest.DoctorNotes,
                           opt => opt.MapFrom(src => src.SpecialInstructions))
                .ForMember(dest => dest.Medications,
                           opt => opt.MapFrom(src => src.Medications));

            //maping for prescription history dto
            CreateMap<Prescription, PrescriptionHistoryDto>()
                .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateIssued, opt => opt.MapFrom(src => src.DateIssued))
                .ForMember(dest => dest.DoctorName,
                           opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
                .ForMember(dest => dest.MedicationNames, opt => opt.MapFrom(src => src.Medications.Select(m => m.Medication.Name).ToList()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        }
    }
}
