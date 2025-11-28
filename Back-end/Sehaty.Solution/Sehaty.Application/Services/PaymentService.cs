using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.Specifications.BillingSpec;
namespace Sehaty.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unit;
        private readonly IBillingService _paymobEgy2Service;
        private readonly PaymentSettings _paymentSettings;

        public PaymentService(IUnitOfWork _unit, IBillingService paymobEgy2Service, IOptions<PaymentSettings> paymentSettings)
        {
            unit = _unit;
            _paymobEgy2Service = paymobEgy2Service;
            _paymentSettings = paymentSettings.Value;
        }

        public async Task<(string link, int? billingId)> GetPaymentLinkAsync(int appointmentId, int totalAmount)
        {
            var spec = new AppointmentSpecifications(a => a.Id == appointmentId);
            var appointment = await unit.Repository<Appointment>()
                .GetByIdWithSpecAsync(spec);

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(appointment.DoctorId);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found");

            if (appointment is null || appointment.Status != AppointmentStatus.Pending)
            {
                throw new InvalidOperationException("The appointment is not valid for payment!");
            }

            if (!_paymentSettings.AcceptOnlinePayments)
                throw new InvalidOperationException("payment is not enabled");

            if (totalAmount <= 0)
                throw new ArgumentException("Amount must Be Larger Than Zero!", nameof(totalAmount));

            var billing = await CreatePendingBilling(appointment, totalAmount);

            if (_paymentSettings.PaymentProvider == (int)PaymentProvider.PaymobEgy2)
            {
                string link = await _paymobEgy2Service.GetPaymentLinkAsync(appointmentId, totalAmount);

                if (!string.IsNullOrEmpty(link))
                {
                    billing.Notes = $"Payment Link Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                    unit.Repository<Billing>().Update(billing);
                    await unit.CommitAsync();
                }

                return (link, billing.Id);
            }

            throw new NotSupportedException("Payment is Not Aviliable");
        }

        private async Task<Billing> CreatePendingBilling(Appointment appointment, int totalAmount)
        {
            var existingBillingSpec = new BillingSpec(b => b.AppointmentId == appointment.Id &&
                (b.Status == BillingStatus.Pending || b.Status == BillingStatus.Paid)
            );

            var existingBilling = await unit.Repository<Billing>().GetByIdWithSpecAsync(existingBillingSpec);

            if (existingBilling != null)
            {
                if (existingBilling.Status == BillingStatus.Paid)
                {
                    throw new InvalidOperationException("Billing is Paid");
                }

                return existingBilling;
            }

            var billing = new Billing
            {
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id,
                BillDate = DateTime.UtcNow,
                Subtotal = totalAmount,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = totalAmount,
                Status = BillingStatus.Pending,
                PaymentMethod = null,
                PaidAmount = 0,
                PaidAt = null,
                ItemsDetail = $"Appointment #{appointment.Id} Payment",
                TransactionId = appointment.Id.ToString(),
                CommissionApplied = null,
                NetAmount = totalAmount,
                Notes = "Awaiting Payment"
            };

            await unit.Repository<Billing>().AddAsync(billing);
            await unit.CommitAsync();

            Console.WriteLine($"Billing created with ID: {billing.Id} - Status: Pending");


            return billing;
        }

        public async Task<bool> ProcessRefundAsync(int billingId, decimal? partialAmount = null)
        {
            var billing = await unit.Repository<Billing>().GetByIdAsync(billingId);

            var appointmentSpec = new AppointmentSpecifications(a => a.Id == billing.AppointmentId);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(appointmentSpec);

            if (appointment == null)
                throw new InvalidOperationException("Appointment Not Found");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException(" Refund is not allowed for an Completed Appointment");
            if (billing == null)
                throw new InvalidOperationException(" Billing not found!");

            if (billing.Status != BillingStatus.Paid && billing.Status != BillingStatus.Partially)
                throw new InvalidOperationException(" Refund is not allowed for an unpaid billing!");

            if (string.IsNullOrEmpty(billing.TransactionId))
                throw new InvalidOperationException(" Transaction ID is missing!");

            decimal amountToRefund = partialAmount ?? billing.PaidAmount;

            decimal alreadyRefunded = billing.TotalAmount - billing.PaidAmount;
            decimal refundableAmount = billing.PaidAmount;

            if (amountToRefund <= 0 || amountToRefund > refundableAmount)
                throw new ArgumentException($" Invalid refund amount! Available refundable amount: {refundableAmount} EGP");

            bool refundSuccess = await _paymobEgy2Service.RefundPaymentAsync(
                billing.TransactionId,
                amountToRefund
            );

            if (!refundSuccess)
                return false;

            billing.PaidAmount -= amountToRefund;
            decimal totalRefunded = alreadyRefunded + amountToRefund;

            if (billing.PaidAmount <= 0)
            {
                billing.Status = BillingStatus.Refunded;
                billing.Notes += $"\n [Full Refund] {amountToRefund} EGP on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                billing.Notes += $"\n  Total refunded: {totalRefunded} EGP out of {billing.TotalAmount} EGP";
            }
            else
            {
                billing.Status = BillingStatus.Partially;
                billing.Notes += $"\n Partial Refund] {amountToRefund} EGP on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                billing.Notes += $"\n   Refunded so far: {totalRefunded} EGP | Remaining: {billing.PaidAmount} EGP";
            }

            unit.Repository<Billing>().Update(billing);
            await unit.CommitAsync();

            Console.WriteLine($"Billing #{billingId} - Refunded: {amountToRefund} EGP | Remaining: {billing.PaidAmount} EGP");

            return true;
        }

    }


}

