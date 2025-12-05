namespace Sehaty.APIs.Controllers
{
    public class PaymentsController(IMapper mapper, IAppointmentService appointmentService, INotificationService notificationService, IPaymentService paymentService, IUnitOfWork unit) : ApiBaseController
    {


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

                int transactionId = model.obj.order.id;

                var billingSpec = new BillingSpec(
                    b => b.TransactionId == transactionId.ToString() && b.Status == BillingStatus.Pending
                );

                var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(billingSpec);

                if (billing == null)
                {
                    Console.WriteLine($" No pending billing found for Appointment #{transactionId}");
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

        private static PaymentMethod GetPaymentMethodFromCallback(PaymobCallbackPostModel model)
        {
            string method = model.obj?.data?.message?.ToLower();

            if (method?.Contains("wallet") == true)
                return PaymentMethod.MobileWallet;

            if (method?.Contains("card") == true || method?.Contains("credit") == true)
                return PaymentMethod.CreditCard;

            return PaymentMethod.CreditCard;
        }


        [HttpGet("Success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] int id, [FromQuery] bool success)//,[FromQuery] string order,[FromQuery] int? amount_cents   
        {
            if (success)
            {

                try
                {
                    var spec = new BillingSpec(B => B.TransactionId == id.ToString() && B.Status == BillingStatus.Paid);
                    var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
                    var specAppointment = new AppointmentSpecifications(a => a.Id == billing.AppointmentId);
                    var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(specAppointment);
                    if (appointment == null)
                        return NotFound(new ApiResponse(404, "Cannot Find Appointment"));

                    var result = await appointmentService.MarkAppointmentAsConfirmed(appointment.Id);
                    if (!result.IsSuccess)
                        return result.ToApiResponse();

                    var ConfirmedAppointment = result.Data;

                    string message = await notificationService.NotifyAppointmentConfirmation(ConfirmedAppointment)
                        ? "Appointment Confirmed - Check Your Email"
                        : "Appointment Confirmed";
                    string html = $@"
<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>تأكيد الحجز</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: rgb(251, 251, 251);
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }}
        .container {{
            background: white;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            text-align: center;
            max-width: 400px;
        }}
        .success-icon {{
            font-size: 60px;
            color: #4CAF50;
            margin-bottom: 20px;
        }}
        h1 {{
            color: #333;
            font-size: 24px;
            margin-bottom: 10px;
        }}
        p {{
            color: #666;
            font-size: 16px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='success-icon'>✓</div>
        <h1>{message}</h1>
        <p>Your appointment has been successfully confirmed!</p>
    </div>
</body>
</html>";

                    return Content(html, "text/html");

                }
                catch (Exception ex)
                {

                    return BadRequest(new ApiResponse(400, ex.Message));
                }


            }

            return BadRequest();
        }

        [HttpPost("Refund")]
        public async Task<IActionResult> RefundPayment([FromQuery] int billingId)
        {
            try
            {
                bool success = await paymentService.ProcessRefundAsync(billingId);

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

                bool success = await paymentService.ProcessRefundAsync(billingId, amount);

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

