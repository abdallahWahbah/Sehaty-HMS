namespace Sehaty.APIs.Controllers
{
    public class BillingsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
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

        //AddData
        [HttpPost("AddBiliing")]
        public async Task<IActionResult> AddBilling(BillingAddDto model)
        {
            if (ModelState.IsValid)
            {
                var billing = mapper.Map<Billing>(model);
                await unit.Repository<Billing>().AddAsync(billing);
                var rowAffected = await unit.CommitAsync();
                var spec = new BillingSpec(b => b.Id == billing.Id);
                var billing2 = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
                var billingReadDto = mapper.Map<BillingReadDto>(billing2);

                return rowAffected > 0 ? Ok(billingReadDto) : BadRequest(new ApiResponse(400));
            }
            return BadRequest(new ApiResponse(400));
        }

        [HttpPost("PayBilling")]
        public async Task<IActionResult> PayBilling([FromBody] BillingPaymentDto model)
        {
            if (ModelState.IsValid)
            {
                var spec = new BillingSpec(b => b.Id == model.BillingId);
                var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
                if (billing is null) return NotFound();
                if (billing.Status == BillingStatus.Paid) return BadRequest(new ApiResponse(400, "Billing is Already Paid"));
                billing.Status = BillingStatus.Paid;
                billing.PaidAmount = model.PaidAmount;
                billing.PaymentMethod = model.PaymentMethod;
                billing.TransactionId = model.TransactionId;
                billing.PaidAt = DateTime.Now;
                unit.Repository<Billing>().Update(billing);
                var rowAffected = await unit.CommitAsync();
                return rowAffected > 0 ? Ok(mapper.Map<BillingReadDto>(billing)) : BadRequest(new ApiResponse(400));

            }
            return BadRequest(new ApiResponse(400));
        }

        //Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBilling(int? id, [FromBody] BillingUpdateDto model)
        {
            if (id is null) return BadRequest(new ApiResponse(400));
            if (ModelState.IsValid)
            {
                var billingData = await unit.Repository<Billing>().GetByIdAsync(id.Value);
                if (billingData is null)
                    return NotFound(new ApiResponse(404));
                mapper.Map(model, billingData);
                unit.Repository<Billing>().Update(billingData);
                await unit.CommitAsync();
                return Ok(new ApiResponse(200, "Updated successfully"));
            }
            return BadRequest(new ApiResponse(400));
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
    }
}

