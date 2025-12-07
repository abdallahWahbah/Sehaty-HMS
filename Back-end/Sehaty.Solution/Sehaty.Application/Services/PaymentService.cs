using Microsoft.AspNetCore.Mvc;

namespace Sehaty.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unit;
        private readonly IPaymobService paymobEgy2Service;
        private readonly INotificationService notificationService;
        private readonly PaymentSettings paymentSettings;

        public PaymentService(IUnitOfWork unit,IPaymobService paymobEgy2Service,IOptions<PaymentSettings> paymentSettings,INotificationService notificationService)
        {
            this.unit = unit;
            this.paymobEgy2Service = paymobEgy2Service;
            this.notificationService = notificationService;
            this.paymentSettings = paymentSettings.Value;
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

        public async Task<Result<ContentResult>> CheckSuccessAsync(int id,bool success)//,[FromQuery] string order,[FromQuery] int? amount_cents   
        {
            if(!success)
                return Result<ContentResult>.Failure(ErrorType.BadRequest,"Payment Failed");


            var spec = new BillingSpec(B => B.TransactionId == id.ToString() && B.Status == BillingStatus.Paid);
            var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
            if(billing == null)
            {
                return Result<ContentResult>.Failure(
                    ErrorType.NotFound,
                    "Payment processing not completed yet, please refresh page in a few seconds"
                );
            }
            string html = "<html><body><h1>✅ تم تأكيد الحجز بنجاح</h1></body></html>";

            return Result<ContentResult>.Success(new ContentResult() { ContentType = "text/html",Content = html });
        }

        public async Task<Result<Appointment>> MarkAppointmentAsConfirmedAsync(int appointmentId)
        {

            var spec = new AppointmentSpecifications(A => A.Id == appointmentId);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);
            if(appointment == null)
                return Result<Appointment>.Failure(ErrorType.NotFound,"Appointment Not Found");

            if(appointment.Status != AppointmentStatus.Pending)
                return Result<Appointment>.Failure(ErrorType.BadRequest,"Appointment cannot be confirmed");

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.ConfirmationDateTime = DateTime.Now;
            var rowsAffected = await unit.CommitAsync();

            if(rowsAffected <= 0)
                return Result<Appointment>.Failure(ErrorType.BadRequest,"Failed to confirm appointment");
            return Result<Appointment>.Success(appointment);
        }


        public async Task<Result<string>> CallbackAsync(PaymobCallbackPostModel model)
        {
            try
            {
                if(model?.obj == null)
                    return Result<string>.Success("Invalid data");

                var orderId = model.obj.order.id.ToString();

                var billing = await unit.Repository<Billing>()
                    .GetByIdWithSpecAsync(new BillingSpec(b =>
                        b.TransactionId == orderId &&
                        b.Status == BillingStatus.Pending));

                if(billing == null)
                    return Result<string>.Success("Billing not found");

                if(!model.obj.success)
                {
                    billing.Status = BillingStatus.Canceled;
                    billing.Notes = "Payment failed";

                    unit.Repository<Billing>().Update(billing);
                    await unit.CommitAsync();

                    return Result<string>.Success("Payment failed");
                }

                billing.Status = BillingStatus.Paid;
                billing.PaidAmount = model.obj.amount_cents / 100;
                billing.PaidAt = DateTime.UtcNow;
                billing.TransactionId = orderId;
                billing.PaymentMethod = GetPaymentMethodFromCallback(model);

                unit.Repository<Billing>().Update(billing);
                await unit.CommitAsync();

                await MarkAppointmentAsConfirmedAsync(billing.AppointmentId);

                var appointment = await unit.Repository<Appointment>().GetByIdAsync(billing.AppointmentId);
                await notificationService.NotifyAppointmentConfirmation(appointment);

                return Result<string>.Success("Billing & Appointment confirmed");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return Result<string>.Failure(ErrorType.BadRequest,"Callback processing error");
            }
        }


        private static PaymentMethod GetPaymentMethodFromCallback(PaymobCallbackPostModel model)
        {
            string method = model.obj?.data?.message?.ToLower();

            if(method?.Contains("wallet") == true)
                return PaymentMethod.MobileWallet;

            if(method?.Contains("card") == true || method?.Contains("credit") == true)
                return PaymentMethod.CreditCard;

            return PaymentMethod.CreditCard;
        }
    }


}

