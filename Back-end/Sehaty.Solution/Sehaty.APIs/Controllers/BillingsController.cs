namespace Sehaty.APIs.Controllers
{
    public class BillingsController(IUnitOfWork unit, IMapper mapper, IBillingService billingService) : ApiBaseController
    {
        //GetAllData
        [HttpGet]
        public async Task<IActionResult> GetAllBilling()
        {
            var spec = new BillingSpec();
            var billingData = await unit.Repository<Billing>().GetAllWithSpecAsync(spec);
            if (billingData is null) return NotFound();
            return Ok(mapper.Map<List<BillingReadDto>>(billingData));
        }

        //GetByAppoinmentId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillingByAppointmentId(int? id)
        {
            var spec = new BillingSpec(b => b.AppointmentId == id);
            var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
            if (billing is null) return NotFound();
            return Ok(mapper.Map<BillingReadDto>(billing));
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilling(int? id)
        {
            if (id is null) return BadRequest(new ApiResponse(400));
            var billingData = await unit.Repository<Billing>().GetByIdAsync(id.Value);
            if (billingData is null) return NotFound(new ApiResponse(404));
            unit.Repository<Billing>().Delete(billingData);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? Ok(new ApiResponse(200, "Deleted successfully")) : BadRequest(new ApiResponse(400));
        }


        /// ////////////////////////////////////////////////////////

        [HttpPost("authorize")]
        public async Task<IActionResult> AuthorizeEscrow(BillingAddDto model)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));
            var spec = new BillingSpec(b => b.AppointmentId == model.AppointmentId);
            var existingBilling = await unit.Repository<Billing>().GetAllWithSpecAsync(spec);
            if (existingBilling.Any())
                return BadRequest(new ApiResponse(400, "Billing already exists for this appointment"));
            var specAppointment = new AppointmentSpecifications(a => a.Id == model.AppointmentId);
            var appointmentData = await unit.Repository<Appointment>().GetByIdWithSpecAsync(specAppointment);
            if (appointmentData is null) return NotFound(new ApiResponse(404, "Appointment not found"));
            if (appointmentData.Status != AppointmentStatus.Pending)
                return BadRequest(new ApiResponse(400, "Cannot authorize payment for this appointment Status not valid"));


            var (billing, redirectUrl) = await billingService.AuthorizeEscrowAsync(model);
            return Ok(new
            {
                BillingId = billing.Id,
                status = billing.Status,
                transactionId = billing.TransactionId,
                redirectUrl = redirectUrl,

            });
        }


    }
}

