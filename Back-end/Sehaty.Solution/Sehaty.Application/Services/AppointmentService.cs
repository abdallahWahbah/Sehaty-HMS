namespace Sehaty.Application.Services
{
    public class AppointmentService(IUnitOfWork unit, IMapper mapper) : IAppointmentService
    {

        public async Task<Appointment> CreateAsync(AppointmentAddDto dto)
        {

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(dto.DoctorId)
                ?? throw new Exception("Doctor not found");


            var patient = await unit.Repository<Patient>().GetByIdAsync(dto.PatientId)
                ?? throw new Exception("Patient not found");


            if (dto.AppointmentDateTime < DateTime.Now)
                throw new Exception("Appointment date cannot be in the past");




            //var doctorSpec = new AppointmentSpecifications(ِA => A.DoctorId == dto.DoctorId &&
            //A.AppointmentDateTime.Date == dto.AppointmentDateTime.Date);

            var doctorAppointments = await unit.Repository<Appointment>().FindBy(A => A.DoctorId == dto.DoctorId &&
            A.AppointmentDateTime.Date == dto.AppointmentDateTime.Date).ToListAsync();


            if (doctorAppointments.Any(A =>
                dto.AppointmentDateTime < A.AppointmentDateTime.AddMinutes(A.DurationMinutes) &&
                dto.AppointmentDateTime.AddMinutes(30) > A.AppointmentDateTime))
                throw new Exception("Doctor Already Has An Overlapping Appointment");


            //var patientSpec = new AppointmentSpecifications(a => a.PatientId == dto.PatientId &&
            //                              a.AppointmentDateTime.Date == dto.AppointmentDateTime.Date);

            var patientAppointments = await unit.Repository<Appointment>().FindBy(a => a.PatientId == dto.PatientId &&
                                          a.AppointmentDateTime.Date == dto.AppointmentDateTime.Date).ToListAsync();

            if (patientAppointments.Any(a =>
                dto.AppointmentDateTime < a.AppointmentDateTime.AddMinutes(a.DurationMinutes) &&
                dto.AppointmentDateTime.AddMinutes(30) > a.AppointmentDateTime))
                throw new Exception("Cannot Book More Than 1 Appointment At This Time");


            var appointment = mapper.Map<Appointment>(dto);

            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();


            var finalSpec = new AppointmentSpecifications(a => a.Id == appointment.Id);
            appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(finalSpec);

            return appointment;
        }

        public async Task<Appointment> CreateAsyncForReceptionist(AppointmentAddForAnonymousDto dto)
        {

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(dto.DoctorId)
                ?? throw new Exception("Doctor not found");

            if (dto.AppointmentDateTime.Date < DateTime.Now.Date)
                throw new Exception("Appointment date cannot be in the past");

            var doctorAppointments = await unit.Repository<Appointment>()
                .FindBy(a => a.DoctorId == dto.DoctorId && a.AppointmentDateTime.Date == dto.AppointmentDateTime.Date)
                .ToListAsync();


            if (doctorAppointments.Any(a =>
                dto.AppointmentDateTime < a.AppointmentDateTime.AddMinutes(a.DurationMinutes) &&
                dto.AppointmentDateTime.AddMinutes(30) > a.AppointmentDateTime))
                throw new Exception("Doctor Already Has An Overlapping Appointment");

            var appointment = mapper.Map<Appointment>(dto);

            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();


            var finalSpec = new AppointmentSpecifications(a => a.Id == appointment.Id);
            appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(finalSpec);

            return appointment;
        }


        public async Task<Appointment> ConfirmAppointment(int billingId)
        {
            var specBilling = new BillingSpec(b => b.TransactionId == billingId.ToString());

            var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(specBilling);

            var spec = new AppointmentSpecifications(A => A.Id == billing.AppointmentId);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);
            if (appointment == null) return null;

            if (appointment.Status != AppointmentStatus.Pending) throw new Exception("Appointment cannot be confirmed");



            appointment.Status = AppointmentStatus.Confirmed;
            appointment.ConfirmationDateTime = DateTime.Now;
            var rowsAffected = await unit.CommitAsync();

            if (rowsAffected <= 0)
                throw new Exception("Failed to confirm appointment");
            return appointment;
        }

    }
}
