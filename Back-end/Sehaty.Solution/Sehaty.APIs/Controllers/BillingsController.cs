using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.BillngDto;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.BillingSpec;
using Sehaty.Core.UnitOfWork.Contract;

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
        public async Task<IActionResult> GetByAppointmentId(int? id)
        {
            var spec = new BillingSpec(b => b.AppointmentId == id);
            var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
            if (billing is null) return NotFound();
            return Ok(mapper.Map<BillingReadDto>(billing));
        }

        //AddData
        [HttpPost("AddBiliing")]
        public async Task<IActionResult> AddBilling([FromBody] BillingAddDto model)
        {
            if (ModelState.IsValid)
            {
                var billing = mapper.Map<Billing>(model);
                await unit.Repository<Billing>().AddAsync(billing);
                var rowAffected = await unit.CommitAsync();
                return rowAffected > 0 ? Ok(mapper.Map<BillingReadDto>(billing)) : BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("PayBilling")]
        public async Task<IActionResult> PayBilling([FromBody] BillingPaymentDto model)
        {
            if (ModelState.IsValid)
            {
                var spec = new BillingSpec(b => b.Id == model.BillingId);
                var billing = await unit.Repository<Billing>().GetByIdWithSpecAsync(spec);
                if (billing is null) return NotFound();
                if (billing.Status == BillingStatus.Paid) return BadRequest("Billing is Already Paid");
                billing.Status = BillingStatus.Paid;
                billing.PaidAmount = model.PaidAmount;
                billing.PaymentMethod = model.PaymentMethod;
                billing.TransactionId = model.TransactionId;
                billing.PaidAt = DateTime.Now;
                unit.Repository<Billing>().Update(billing);
                var rowAffected = await unit.CommitAsync();
                return rowAffected > 0 ? Ok(mapper.Map<BillingReadDto>(billing)) : BadRequest(ModelState);

            }
            return BadRequest(ModelState);
        }

        //Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBilling(int? id, [FromBody] BillingUpdateDto model)
        {
            if (id is null) return BadRequest();
            if (ModelState.IsValid)
            {
                var billingData = await unit.Repository<Billing>().GetByIdAsync(id.Value);
                if (billingData is null)
                    return NotFound();
                mapper.Map(model, billingData);
                unit.Repository<Billing>().Update(billingData);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilling(int? id)
        {
            if (id is null) return BadRequest();
            var billingData = await unit.Repository<Billing>().GetByIdAsync(id.Value);
            if (billingData is null) return NotFound();
            unit.Repository<Billing>().Delete(billingData);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? NoContent() : BadRequest(ModelState);
        }
    }
}

