using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specifications.Prescription_Specs;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class PrescriptionsController : ApiBaseController
    {
        private readonly IUnitOfWork unit;
        private readonly IMapper map;

        public PrescriptionsController(IUnitOfWork unit, IMapper map)
        {
            this.unit = unit;
            this.map = map;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spec = new PrescriptionSpecifications();
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            if (prescriptions != null)
                return Ok(map.Map<IEnumerable<GetPrescriptionsDto>>(prescriptions));
            return NotFound();
        }
        //get prescription by its id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = new PrescriptionSpecifications(id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription != null)
                return Ok(map.Map<GetPrescriptionsDto>(prescription));
            return NotFound();
        }
        [HttpGet("/GetByDoctorId/{id}")]
        public async Task<IActionResult> GetByDoctorId(int id)
        {
            var spec = new PrescriptionSpecifications(P => P.DoctorId == id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription != null)
                return Ok(map.Map<GetPrescriptionsDto>(prescription));
            return NotFound();
        }

        [HttpGet("/GetByPatientId/{id}")]
        public async Task<IActionResult> GetByPatientId(int id)
        {
            var spec = new PrescriptionSpecifications(P => P.PatientId == id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription != null)
                return Ok(map.Map<GetPrescriptionsDto>(prescription));
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionsDto model)
        {
            if (ModelState.IsValid)
            {
                var prescription = map.Map<Prescription>(model);
                await unit.Repository<Prescription>().AddAsync(prescription);
                await unit.CommitAsync();
                return Ok(prescription);
            }
            return BadRequest(model);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] CreatePrescriptionsDto model)
        {
            if (ModelState.IsValid)
            {
                var prescription = await unit.Repository<Prescription>().GetByIdAsync(id);
                if (prescription == null) return NotFound();
                map.Map(model, prescription);
                unit.Repository<Prescription>().Update(prescription);
                await unit.CommitAsync();
                return Ok();
            }
            return BadRequest(model);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await unit.Repository<Prescription>().GetByIdAsync(id);
            if (prescription == null) return NotFound();
            unit.Repository<Prescription>().Delete(prescription);
            await unit.CommitAsync();
            return NoContent();
        }
    }

}
