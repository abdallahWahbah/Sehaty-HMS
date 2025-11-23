using Sehaty.Core.Specifications.Appointment_Specs;

namespace Sehaty.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork unit;
        private readonly IMapper mapper;

        public AppointmentService(IUnitOfWork unit, IMapper mapper)
        {
            this.unit = unit;
            this.mapper = mapper;
        }

        public async Task<Appointment> CreateAsync(AppointmentAddDto dto)
        {

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(dto.DoctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");


            var patient = await unit.Repository<Patient>().GetByIdAsync(dto.PatientId);
            if (patient == null)
                throw new Exception("Patient not found");


            if (dto.AppointmentDateTime < DateTime.Now)
                throw new Exception("Appointment date cannot be in the past");


            var doctorSpec = new AppointmentSpecifications(a =>
                a.DoctorId == dto.DoctorId && a.AppointmentDateTime == dto.AppointmentDateTime);

            var doctorAppointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(doctorSpec);
            if (doctorAppointments.Count() > 0)
                throw new Exception("Doctor already has an appointment at this time");


            var patientSpec = new AppointmentSpecifications(a =>
                a.PatientId == dto.PatientId && a.AppointmentDateTime == dto.AppointmentDateTime);

            var patientAppointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(patientSpec);
            if (patientAppointments.Count() > 0)
                throw new Exception("Cannot book more than 1 appointment at this time");


            var appointment = mapper.Map<Appointment>(dto);

            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();


            var finalSpec = new AppointmentSpecifications(a => a.Id == appointment.Id);
            appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(finalSpec);

            return appointment;
        }
    }
}
