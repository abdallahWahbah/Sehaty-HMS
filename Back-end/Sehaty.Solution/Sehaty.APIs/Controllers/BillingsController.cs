using AutoMapper;
using Sehaty.Application.Services;

namespace Sehaty.APIs.Controllers
{
    public class BillingsController(IEmailSender emailSender, IWebHostEnvironment env, IMapper mapper, IPaymentService _paymentService, IUnitOfWork unit) : ApiBaseController
    {

        [HttpGet("GetLink")]
        public async Task<IActionResult> GetPaymentLink([FromQuery] int appointmentId)
        {
            try
            {
                var spec = new AppointmentSpecifications(a => a.Id == appointmentId);
                var appointment = await unit.Repository<Appointment>()
                    .GetByIdWithSpecAsync(spec);

                if (appointment == null)
                    return NotFound(new { error = "Appointment not found" });

                var doctor = await unit.Repository<Doctor>().GetByIdAsync(appointment.DoctorId);

                if (doctor == null)
                    return NotFound(new { error = "Doctor not found" });

                int totalAmount = (int)doctor.Price;

                var (link, billingId) = await _paymentService.GetPaymentLinkAsync(appointmentId, totalAmount);
                if (string.IsNullOrEmpty(appointmentId.ToString()))
                    return BadRequest(new { error = "AppointmentId Is Required" });

                if (string.IsNullOrEmpty(link))
                    return StatusCode(500, new { error = "Cann't Create PaymentLink" });

                return Ok(new
                {
                    success = true,
                    payment_link = link,
                    totalAmount,
                    order_id = appointmentId,
                    billingId = billingId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("Callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymobCallbackPostModel model)
        {
            try
            {
                Console.WriteLine(" Callback received from Paymob");

                if (model?.obj == null)
                {
                    Console.WriteLine(" Invalid callback data");
                    return Ok(new { message = "Invalid data" });
                }

                string merchantOrderId = model.obj.order?.merchant_order_id ?? "";

                if (!int.TryParse(merchantOrderId, out int appointmentId))
                {
                    Console.WriteLine(" Invalid appointment ID");
                    return Ok(new { message = "Invalid order ID" });
                }

                var billingSpec = new BillingSpec(
                    b => b.AppointmentId == appointmentId && b.Status == BillingStatus.Pending
                );

                var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(billingSpec);

                if (billing == null)
                {
                    Console.WriteLine($" No pending billing found for Appointment #{appointmentId}");
                    return Ok(new { message = "Billing not found" });
                }

                if (model.obj.success)
                {
                    billing.Status = BillingStatus.Paid;
                    billing.PaidAmount = model.obj.amount_cents / 100;
                    billing.PaidAt = DateTime.UtcNow;
                    billing.TransactionId = model.obj.id.ToString();
                    billing.PaymentMethod = GetPaymentMethodFromCallback(model);
                    billing.Notes = $"Paid via Paymob - Transaction #{model.obj.id} - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

                    Console.WriteLine($" Billing #{billing.Id} marked as PAID");
                }
                else
                {
                    billing.Status = BillingStatus.Canceled;
                    billing.Notes = $"Payment Failed - {model.obj.data?.message ?? "Unknown error"} - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

                    Console.WriteLine($" Billing #{billing.Id} marked as CANCELED");
                }

                unit.Repository<Billing>().Update(billing);
                await unit.CommitAsync();

                Console.WriteLine($" Billing updated successfully");

                return Ok(new { message = "Callback processed successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Callback Error: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");
                return Ok(new { message = "Error processed" });
            }
        }

        private PaymentMethod GetPaymentMethodFromCallback(PaymobCallbackPostModel model)
        {
            string method = model.obj?.data?.message?.ToLower();

            if (method?.Contains("wallet") == true)
                return PaymentMethod.MobileWallet;

            if (method?.Contains("card") == true || method?.Contains("credit") == true)
                return PaymentMethod.CreditCard;

            return PaymentMethod.CreditCard;
        }


        [HttpGet("Success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] int merchant_order_id, [FromQuery] bool success, [FromQuery] string order, [FromQuery] int? amount_cents)
        {
            if (success)
            {
                #region MyRegion
                //    return Ok(new
                //    {
                //        message = "Payment completed successfully!",
                //        order_id = order,
                //        amount = amount_cents / 100m,
                //        html = @"
                //    <html>
                //    <head><title>Payment Success</title></head>
                //    <body style='text-align:center; padding:50px; font-family:Arial'>
                //        <h1 style='color:green'> Payment completed successfully!</h1>
                //        <p>رقم الطلب: " + order + @"</p>
                //        <p>المبلغ: " + (amount_cents / 100m) + @" Egp</p>

                //    </body>
                //    </html>
                //"
                //    }); 
                #endregion

                var spec = new AppointmentSpecifications(a => a.Id == merchant_order_id);
                var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);
                if (appointment == null) return NotFound(new ApiResponse(404));
                if (appointment.Status != AppointmentStatus.Pending) return BadRequest(new ApiResponse(400, "Appointment cannot be confirmed"));



                appointment.Status = AppointmentStatus.Confirmed;
                var rowsAffected = await unit.CommitAsync();

                if (rowsAffected <= 0)
                    return BadRequest(new ApiResponse(400, "Failed to confirm appointment"));
                var patient = await unit.Repository<Patient>().GetByIdAsync(appointment.PatientId);
                if (patient != null)
                {
                    string message = $"تم تأكيد موعدك مع الطبيب {appointment.Doctor.FirstName} {appointment.Doctor.LastName} بتاريخ {appointment.AppointmentDateTime}";

                    var notificationDto = new CreateNotificationDto
                    {
                        UserId = appointment.PatientId,
                        Title = "Appointment Confirmed",
                        Message = message,
                        Priority = NotificationPriority.High,
                        RelatedEntityType = "Appointment",
                        RelatedEntityId = appointment.Id,
                        SentViaEmail = false,
                        SentViaSMS = true,
                        NotificationType = NotificationType.Appointment,
                        IsRead = false
                    };
                    var notification = mapper.Map<Notification>(notificationDto);
                    await unit.Repository<Notification>().AddAsync(notification);
                    await unit.CommitAsync();
                    if (!string.IsNullOrEmpty(patient.User.Email))
                    {
                        var filepath = $"{env.WebRootPath}/templates/ConfirmEmail.html";
                        StreamReader reader = new StreamReader(filepath);
                        var body = reader.ReadToEnd();
                        reader.Close();
                        body = body.Replace("[header]", message)
                            .Replace("[body]", "تم تأكيد موعدك بنجاح. نتمنى لك دوام الصحة.")
                            .Replace("[imageUrl]", "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763918337/icon-positive-vote-3_xfc5be.png\r\n");
                        await emailSender.SendEmailAsync(patient.User.Email, "Sehaty", body);
                        notificationDto.SentViaEmail = true;
                    }
                    //if (!string.IsNullOrEmpty(patient.User.PhoneNumber))
                    //{
                    //    smsSender.SendSmsAsync(patient.User.PhoneNumber, message);
                    //    notificationDto.SentViaSMS = true;
                    //}
                    await unit.CommitAsync();
                }

                return Ok(new ApiResponse(200, "Appointment confirmed successfully"));
            }

            return Ok(new
            {
                message = "Payment failed",
                html = @"
            <html>
            <head><title>Payment Failed</title></head>
            <body style='text-align:center; padding:50px; font-family:Arial'>
                <h1 style='color:red'>Payment failed</h1>
                <p>Tey Again</p>
            </body>
            </html>
        "
            });
        }

        [HttpPost("Refund")]
        public async Task<IActionResult> RefundPayment([FromQuery] int billingId)
        {
            try
            {
                bool success = await _paymentService.ProcessRefundAsync(billingId);

                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "The amount has been successfully refunded",
                        billing_id = billingId
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        error = " Failed to recover the amount from Paymob"
                    });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Refund Error: {ex.Message}");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("PartialRefund")]
        public async Task<IActionResult> PartialRefund([FromQuery] int billingId, [FromQuery] decimal amount)
        {
            try
            {
                if (amount <= 0)
                    return BadRequest(new { error = "The amount must be greater than zero!" });

                bool success = await _paymentService.ProcessRefundAsync(billingId, amount);

                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"{amount} pounds has been successfully refunded",
                        billing_id = billingId,
                        refunded_amount = amount
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        error = "Failed to recover the amount from Paymob"
                    });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Partial Refund Error: {ex.Message}");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("GetBillingDetails")]
        public async Task<IActionResult> GetBillingDetails([FromQuery] int billingId)
        {
            try
            {
                var billing = await unit.Repository<Billing>().GetByIdAsync(billingId);

                if (billing == null)
                    return NotFound(new { error = " Billing Not Found" });

                bool canRefund = billing.Status == BillingStatus.Paid &&
                                !string.IsNullOrEmpty(billing.TransactionId);

                return Ok(new
                {
                    success = true,
                    billing = new
                    {
                        billing.Id,
                        billing.AppointmentId,
                        billing.PatientId,
                        billing.Status,
                        billing.TotalAmount,
                        billing.PaidAmount,
                        billing.TransactionId,
                        billing.PaymentMethod,
                        billing.PaidAt,
                        billing.BillDate,
                        billing.Notes,
                        canRefund,
                        refundableAmount = canRefund ? billing.PaidAmount : 0,
                        statusText = billing.Status switch
                        {
                            BillingStatus.Pending => "Pending",
                            BillingStatus.Paid => "Paid",
                            BillingStatus.Partially => "Partially",
                            BillingStatus.Refunded => "Refunded",
                            BillingStatus.Canceled => "Canceled",
                            _ => "UnKnown"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }


}

