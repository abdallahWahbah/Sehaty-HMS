using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specifications.Prescription_Specs;
using Sehaty.Core.UnitOfWork.Contract;
using System.Security.Claims;

namespace Sehaty.APIs.Controllers
{

    public class PrescriptionsController(IUnitOfWork unit, IMapper map) : ApiBaseController
    {

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
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctorprescriptions")]
        public async Task<IActionResult> GetByDoctorId()
        {
            var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var spec = new PrescriptionSpecifications(P => P.DoctorId == doctorId);
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            var sortedprescriptions = prescriptions.OrderByDescending(p => p.DateIssued).ToList();
            if (sortedprescriptions.Any())
                return Ok(map.Map<IEnumerable<DoctorPrescriptionsDto>>(sortedprescriptions));
            return NotFound();
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctorprescriptions/{id}")]
        public async Task<IActionResult> GetPrescriptionDetails(int id)
        {
            var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var spec = new PrescriptionSpecifications(p => p.Id == id && p.DoctorId == doctorId);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription == null)
                return NotFound();
            return Ok(map.Map<GetPrescriptionsDto>(prescription));
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("patientprescriptions")]
        public async Task<IActionResult> GetByPatientId()
        {
            var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var spec = new PrescriptionSpecifications(P => P.PatientId == patientId);
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            var sortedprescriptions = prescriptions
                        .OrderByDescending(p => p.Status == PrescriptionStatus.Active)
                        .ThenByDescending(p => p.DateIssued)
                        .ToList();
            if (prescriptions != null)
                return Ok(map.Map<IEnumerable<PatientPrescriptionsDto>>(sortedprescriptions));
            return NotFound();
        }
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionsDto model)
        {
            if (ModelState.IsValid)
            {
                var prescription = map.Map<Prescription>(model);
                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                prescription.DoctorId = int.Parse(doctorId!);
                await unit.Repository<Prescription>().AddAsync(prescription);
                await unit.CommitAsync();
                return Ok(new { message = "Prescription created successfully", prescriptionId = prescription.Id });
            }
            return BadRequest(ModelState);
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
