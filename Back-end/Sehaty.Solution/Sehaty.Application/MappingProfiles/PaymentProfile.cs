namespace Sehaty.Application.MappingProfiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Billing, BillingReadDto>()
                .AfterMap((src, dest) =>
                {
                    dest.PatientName = src.Patient.FirstName + " " + src.Patient.LastName;
                    dest.DoctorName = src.Appointment.Doctor.FirstName + " " + src.Appointment.Doctor.LastName;
                    dest.AppointmentDateTime = src.Appointment.AppointmentDateTime;
                }).ReverseMap();

        }
    }
}
