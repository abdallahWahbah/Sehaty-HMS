namespace Sehaty.APIs.Controllers
{
    public class PaymentsController(IPaymentService paymentService,IUnitOfWork unit) : ApiBaseController//IMapper mapper,IAppointmentService appointmentService,INotificationService notificationService,
    {

        [HttpPost("callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymobCallbackPostModel model)
        {
            var result = await paymentService.CallbackAsync(model);

            return Ok(result.Data);
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


        [HttpGet("Success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] int id,[FromQuery] bool success)//,[FromQuery] string order,[FromQuery] int? amount_cents   
        {
            var result = await paymentService.CheckSuccessAsync(id,success);
            if(!result.IsSuccess)
                return BadRequest(new ApiResponse(400,result.Error));
            return result.Data;

        }

        [HttpPost("Refund")]
        public async Task<IActionResult> RefundPayment([FromQuery] int billingId)
        {
            try
            {
                bool success = await paymentService.ProcessRefundAsync(billingId);

                if(success)
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
                    return StatusCode(500,new
                    {
                        success = false,
                        error = " Failed to recover the amount from Paymob"
                    });
                }
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { success = false,error = ex.Message });
            }
            catch(Exception ex)
            {
                Console.WriteLine($"❌ Refund Error: {ex.Message}");
                return StatusCode(500,new { success = false,error = ex.Message });
            }
        }

        [HttpPost("PartialRefund")]
        public async Task<IActionResult> PartialRefund([FromQuery] int billingId,[FromQuery] decimal amount)
        {
            try
            {
                if(amount <= 0)
                    return BadRequest(new { error = "The amount must be greater than zero!" });

                bool success = await paymentService.ProcessRefundAsync(billingId,amount);

                if(success)
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
                    return StatusCode(500,new
                    {
                        success = false,
                        error = "Failed to recover the amount from Paymob"
                    });
                }
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { success = false,error = ex.Message });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { success = false,error = ex.Message });
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Partial Refund Error: {ex.Message}");
                return StatusCode(500,new { success = false,error = ex.Message });
            }
        }

        [HttpGet("GetBillingDetails")]
        public async Task<IActionResult> GetBillingDetails([FromQuery] int billingId)
        {
            try
            {
                var billing = await unit.Repository<Billing>().GetByIdAsync(billingId);

                if(billing == null)
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
            catch(Exception ex)
            {
                return StatusCode(500,new { error = ex.Message });
            }
        }
    }


}

