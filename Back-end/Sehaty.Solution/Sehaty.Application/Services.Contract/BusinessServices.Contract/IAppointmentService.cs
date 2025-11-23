namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAsync(AppointmentAddDto dto);
    }
}
