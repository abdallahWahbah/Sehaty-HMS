namespace Sehaty.Application.Services
{
    public class AppointmentService(IUnitOfWork unit,IMapper mapper,IPaymobService paymobEgy2Service,IPaymentService paymentService) : IAppointmentService
    {

        public async Task<Appointment> CreateAsync(AppointmentAddDto dto)
        {

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(dto.DoctorId)
                ?? throw new Exception("Doctor not found");


            var patient = await unit.Repository<Patient>().GetByIdAsync(dto.PatientId)
                ?? throw new Exception("Patient not found");


            if(dto.AppointmentDateTime < DateTime.Now)
                throw new Exception("Appointment date cannot be in the past");




            //var doctorSpec = new AppointmentSpecifications(ِA => A.DoctorId == dto.DoctorId &&
            //A.AppointmentDateTime.Date == dto.AppointmentDateTime.Date);

            var doctorAppointments = await unit.Repository<Appointment>().FindBy(A => A.DoctorId == dto.DoctorId &&
            A.AppointmentDateTime.Date == dto.AppointmentDateTime.Date).ToListAsync();


            if(doctorAppointments.Any(A =>
                dto.AppointmentDateTime < A.AppointmentDateTime.AddMinutes(A.DurationMinutes) &&
                dto.AppointmentDateTime.AddMinutes(30) > A.AppointmentDateTime))
                throw new Exception("Doctor Already Has An Overlapping Appointment");


            //var patientSpec = new AppointmentSpecifications(a => a.PatientId == dto.PatientId &&
            //                              a.AppointmentDateTime.Date == dto.AppointmentDateTime.Date);

            var patientAppointments = await unit.Repository<Appointment>().FindBy(a => a.PatientId == dto.PatientId &&
                                          a.AppointmentDateTime.Date == dto.AppointmentDateTime.Date).ToListAsync();

            if(patientAppointments.Any(a =>
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

            if(dto.AppointmentDateTime.Date < DateTime.Now.Date)
                throw new Exception("Appointment date cannot be in the past");

            var doctorAppointments = await unit.Repository<Appointment>()
                .FindBy(a => a.DoctorId == dto.DoctorId && a.AppointmentDateTime.Date == dto.AppointmentDateTime.Date)
                .ToListAsync();


            if(doctorAppointments.Any(a =>
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


        public async Task<Result<Appointment>> MarkAppointmentAsConfirmed(int billingId)
        {
            var specBilling = new BillingSpec(b => b.TransactionId == billingId.ToString());

            var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(specBilling);

            var spec = new AppointmentSpecifications(A => A.Id == billing.AppointmentId);
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

        public async Task<Result> CancelConfirmedAppointmentByPatient(int appointmentId)
        {
            var spec = new AppointmentSpecifications(appointmentId);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);

            if(appointment is null)
                return Result.Failure(ErrorType.NotFound,"Appointment Not Found");
            if(appointment.Status == AppointmentStatus.Canceled)
                return Result.Failure(ErrorType.BadRequest,"This appointment has already been canceled.");

            var requestTime = DateTime.UtcNow;
            if(appointment.AppointmentDateTime < requestTime)
                return Result.Failure(ErrorType.BadRequest,"You cannot cancel an appointment that has already passed.");

            var timeBeforeCancel = appointment.AppointmentDateTime - requestTime;
            var specBilling = new BillingSpec(B => B.AppointmentId == appointment.Id);
            var specSlot = new DoctorAppointmentSlotspec(B => B.AppointmentId == appointment.Id);
            var slot = await unit.Repository<DoctorAppointmentSlot>().GetByIdWithSpecAsync(specSlot);
            var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(specBilling);
            if(billing is null)
                return Result.Failure(ErrorType.BadRequest,"Billing not found");
            bool refundSuccess = false;
            if(timeBeforeCancel >= TimeSpan.FromHours(24))
            {
                refundSuccess = await paymentService.ProcessRefundAsync(billing.Id);
            }
            else
            {
                var refundAmount = billing.PaidAmount * 0.6m;

                refundSuccess = await paymentService.ProcessRefundAsync(billing.Id,refundAmount);

                if(!refundSuccess)
                    return Result.Failure(ErrorType.BadRequest,"Failed to process partial refund 60%!");
            }
            if(!refundSuccess)
                return Result.Failure(ErrorType.BadRequest,"Failed to process refund");

            slot.IsBooked = false;
            appointment.Status = AppointmentStatus.Canceled;
            unit.Repository<DoctorAppointmentSlot>().Update(slot);
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();

            if(rowsAffected <= 0)
                return Result.Failure(ErrorType.BadRequest,"Failed to cancel appointment");
            return Result.Success();
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
            appointment.CancellationReason = $"Canceled by doctor {String.Concat(doctor.FirstName," ",doctor.LastName)}";

            unit.Repository<Appointment>().Update(appointment);


            doctor.CancelledAppointmentsCount++;

            unit.Repository<Doctor>().Update(doctor);


            await unit.CommitAsync();
            return Result.Success();
        }
        public async Task<Result<ConfirmAppointmentResponseDto>> GetConfirmationLinkAsync(int appointmentId)
        {
            var spec = new AppointmentSpecifications(a => a.Id == appointmentId);

            var appointment = await unit.Repository<Appointment>()
                .GetByIdWithSpecAsync(spec);

            if(appointment == null)
                return Result<ConfirmAppointmentResponseDto>
                    .Failure(ErrorType.NotFound,"Appointment Not Found");

            var doctor = await unit.Repository<Doctor>()
                .GetByIdAsync(appointment.DoctorId);

            if(doctor == null)
                return Result<ConfirmAppointmentResponseDto>
                    .Failure(ErrorType.NotFound,"Doctor not found");

            int totalAmount = doctor.DetectionPrice;

            var paymentResult =
                await paymentService.GetPaymentLinkAsync(appointmentId,totalAmount);

            if(!paymentResult.IsSuccess)
                return Result<ConfirmAppointmentResponseDto>
                    .Failure(paymentResult.ErrorType,paymentResult.Error);

            var (link, billingId) = paymentResult.Data;

            if(string.IsNullOrEmpty(link))
                return Result<ConfirmAppointmentResponseDto>
                    .Failure(ErrorType.BadRequest,"Cann't Create Payment Link");

            var response = new ConfirmAppointmentResponseDto
            {
                Success = true,
                Payment_link = link,
                TotalAmount = totalAmount,
                Order_id = appointmentId,
                BillingId = billingId
            };

            return Result<ConfirmAppointmentResponseDto>.Success(response);
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
