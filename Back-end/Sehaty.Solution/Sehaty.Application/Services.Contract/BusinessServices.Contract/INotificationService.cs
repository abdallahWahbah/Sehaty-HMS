namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    using System.Threading.Tasks;

    public interface INotificationService
    {
        public Task<bool> CreateNotificationForAppointmentConfirmation(Appointment appointment);
        public Task<bool> CreateNotificationForAppointmentCancellation(Appointment appointment);
    }
}
