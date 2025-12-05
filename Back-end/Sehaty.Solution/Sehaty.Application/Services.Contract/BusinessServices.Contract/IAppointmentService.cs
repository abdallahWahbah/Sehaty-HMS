namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAsync(AppointmentAddDto dto);
        Task<Appointment> CreateAsyncForReceptionist(AppointmentAddForAnonymousDto dto);
        Task<Result<Appointment>> MarkAppointmentAsConfirmed(int billingId);
        Task<Result<ConfirmAppointmentResponseDto>> GetConfirmationLinkAsync(int appointmentId);
        Task<Result> CancelConfirmedAppointmentByDoctor(int appointmentId);
        Task<Result> CancelConfirmedAppointmentByPatient(int appointmentId);
    }
}
