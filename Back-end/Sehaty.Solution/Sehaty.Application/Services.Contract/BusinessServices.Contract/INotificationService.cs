namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    using System.Threading.Tasks;

    public interface INotificationService
    {
        public Task<bool> NotifyAppointmentConfirmation(Appointment appointment);
        public Task<bool> NotifyAppointmentCancellation(Appointment appointment);
        public Task<bool> NotifyAppointmentUpdated(Appointment appointment);
        public Task<bool> NotifyPrescriptionComplation(Prescription prescription);
    }
}
