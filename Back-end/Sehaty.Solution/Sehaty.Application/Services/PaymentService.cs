namespace Sehaty.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unit;
        private readonly IPaymobService paymobEgy2Service;
        private readonly PaymentSettings paymentSettings;

        public PaymentService(IUnitOfWork unit,IPaymobService paymobEgy2Service,IOptions<PaymentSettings> paymentSettings)
        {
            this.unit = unit;
            this.paymobEgy2Service = paymobEgy2Service;
            this.paymentSettings = paymentSettings.Value;
        }
        public async Task<Result> CancelConfirmedAppointmentByDoctor(int appointmentId)
        {
            var appointment = await unit.Repository<Appointment>()
                .GetByIdAsync(appointmentId);

            if(appointment is null)
                return Result.Failure(ErrorType.NotFound,"Appointment not found");

            // تأكيد الحالة
            if(appointment.Status != AppointmentStatus.Confirmed)
                return Result.Failure(ErrorType.BadRequest,"Only confirmed appointments can be cancelled");

            var doctor = await unit.Repository<Doctor>()
                .GetByIdAsync(appointment.DoctorId);
            if(doctor is null)
                return Result.Failure(ErrorType.NotFound,"Doctor not found");


            var billing = await unit.Repository<Billing>()
                .GetByIdWithSpecAsync(
                    new BillingSpec(b => b.AppointmentId == appointmentId)
                );

            if(billing is null || billing.Status != BillingStatus.Paid)
                return Result.Failure(ErrorType.NotFound,"No paid billing found");

            // حساب الخصم
            decimal refundAmount = CalculateRefund(
                appointment.AppointmentDateTime,
                billing.PaidAmount
            );

            // تنفيذ Refund
            bool refundSuccess = await paymobEgy2Service.RefundPaymentAsync(
                billing.TransactionId,
                refundAmount
            );

            if(!refundSuccess)
                return Result.Failure(ErrorType.BadRequest,"Refund failed");



            billing.PaidAmount -= refundAmount;
            billing.Status = billing.PaidAmount == 0
                ? BillingStatus.Refunded
                : BillingStatus.Partially;
            billing.DiscountAmount += refundAmount;

            billing.Notes +=
                $"\nDoctor cancellation refund: {refundAmount} EGP at {DateTime.UtcNow}";

            unit.Repository<Billing>().Update(billing);


            appointment.Status = AppointmentStatus.Canceled;
            appointment.CancellationReason = "Canceled by doctor";

            unit.Repository<Appointment>().Update(appointment);


            doctor.CancelledAppointmentsCount++;

            unit.Repository<Doctor>().Update(doctor);


            await unit.CommitAsync();
            return Result.Success();
        }

        public async Task<Result<(string link, int? billingId)>> GetPaymentLinkAsync(int appointmentId,int totalAmount)
        {
            var specBilling = new BillingSpec(b => b.AppointmentId == appointmentId);
            var existeingBilling = await unit.Repository<Billing>().GetByIdWithSpecAsync(specBilling);
            if(existeingBilling != null)
            {
                if(existeingBilling.PaymentLink != null &&
                    existeingBilling.Status == BillingStatus.Pending)
                    return Result<(string link, int? billingId)>.Success((existeingBilling.PaymentLink, existeingBilling.Id));
            }

            var spec = new AppointmentSpecifications(a => a.Id == appointmentId);
            var appointment = await unit.Repository<Appointment>()
                .GetByIdWithSpecAsync(spec);
            if(appointment is null)
                return Result<(string link, int? billingId)>.Failure(ErrorType.NotFound,"Appointment not found");
            ;

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(appointment.DoctorId);

            if(doctor is null)
                return Result<(string link, int? billingId)>.Failure(ErrorType.NotFound,"Doctor not found");


            if(appointment?.Status != AppointmentStatus.Pending)
                return Result<(string link, int? billingId)>.Failure(ErrorType.BadRequest,"Only Pending Appointments Can Be Confirmed");

            if(!paymentSettings.AcceptOnlinePayments)
                return Result<(string link, int? billingId)>.Failure(ErrorType.BadRequest,"payment is not enabled");

            if(totalAmount <= 0)
                return Result<(string link, int? billingId)>.Failure(ErrorType.BadRequest,"Amount must Be Larger Than Zero!");

            Billing billing = new();
            if(paymentSettings.PaymentProvider == (int) PaymentProvider.PaymobEgy2)
            {
                var (link, orderId) = await paymobEgy2Service.GetPaymentLinkAsync(appointmentId,totalAmount);

                if(!string.IsNullOrEmpty(link))
                {
                    billing = await CreatePendingBilling(appointment,totalAmount,orderId);
                    billing.Notes = $"Payment Link Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                    billing.PaymentLink = link;
                    unit.Repository<Billing>().Update(billing);
                    await unit.CommitAsync();
                }

                return Result<(string link, int? billingId)>.Success((link, billing.Id));
            }

            throw new NotSupportedException("Payment is Not Aviliable");
        }

        private async Task<Billing> CreatePendingBilling(Appointment appointment,int totalAmount,int orderId)
        {
            var existingBillingSpec = new BillingSpec(b => b.AppointmentId == appointment.Id &&
                (b.Status == BillingStatus.Pending || b.Status == BillingStatus.Paid)
            );

            var existingBilling = await unit.Repository<Billing>().GetByIdWithSpecAsync(existingBillingSpec);

            if(existingBilling != null)
            {
                if(existingBilling.Status == BillingStatus.Paid)
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
                TransactionId = orderId.ToString(),
                CommissionApplied = null,
                NetAmount = totalAmount,
                Notes = "Awaiting Payment"
            };

            await unit.Repository<Billing>().AddAsync(billing);
            await unit.CommitAsync();

            Console.WriteLine($"Billing created with ID: {billing.Id} - Status: Pending");


            return billing;
        }

        public async Task<bool> ProcessRefundAsync(int billingId,decimal? partialAmount = null)
        {
            var billing = await unit.Repository<Billing>().GetByIdAsync(billingId);

            var appointmentSpec = new AppointmentSpecifications(a => a.Id == billing.AppointmentId);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(appointmentSpec)
                ?? throw new InvalidOperationException("Appointment Not Found");

            if(appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException(" Refund is not allowed for an Completed Appointment");
            if(billing == null)
                throw new InvalidOperationException(" Billing not found!");

            if(billing.Status != BillingStatus.Paid && billing.Status != BillingStatus.Partially)
                throw new InvalidOperationException(" Refund is not allowed for an unpaid billing!");

            if(string.IsNullOrEmpty(billing.TransactionId))
                throw new InvalidOperationException(" Transaction ID is missing!");

            decimal amountToRefund = partialAmount ?? billing.PaidAmount;

            decimal alreadyRefunded = billing.TotalAmount - billing.PaidAmount;
            decimal refundableAmount = billing.PaidAmount;

            if(amountToRefund <= 0 || amountToRefund > refundableAmount)
                throw new ArgumentException($" Invalid refund amount! Available refundable amount: {refundableAmount} EGP");

            bool refundSuccess = await paymobEgy2Service.RefundPaymentAsync(
                billing.TransactionId,
                amountToRefund
            );

            if(!refundSuccess)
                return false;

            billing.PaidAmount -= amountToRefund;
            decimal totalRefunded = alreadyRefunded + amountToRefund;

            if(billing.PaidAmount <= 0)
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
        private static decimal CalculateRefund(DateTime appointmentTime,decimal paidAmount)
        {
            var diff = appointmentTime - DateTime.UtcNow;

            if(diff.TotalHours < 2)
                return paidAmount * 0.5m;

            if(diff.TotalHours < 12)
                return paidAmount * 0.7m;

            return paidAmount * 0.9m;
        }


    }


}

