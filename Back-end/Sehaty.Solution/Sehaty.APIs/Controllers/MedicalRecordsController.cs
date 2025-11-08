using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Application.Dtos.MedicalRecordDto;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.MedicalReord;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Dtos;

namespace Sehaty.APIs.Controllers
{

    public class MedicalRecordsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {

        [EndpointSummary("Get all medical records")]
        [EndpointDescription("Retrieves all medical records from the database and maps them to doctor DTOs")]
        [ProducesResponseType(typeof(List<MedicalRecordReadDto>), 200)]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<IActionResult> GetAllMedicalRecord()
        {
            var spec = new MedicalRecordSpec();
            var medicalRecords = await unit.Repository<MedicalRecord>().GetAllWithSpecAsync(spec);
            if (medicalRecords is null) return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<List<MedicalRecordReadDto>>(medicalRecords));
        }

        //Get Specific
        [EndpointSummary("Get a specific medical record")]
        [EndpointDescription("Retrieves a medical record by its ID")]
        [ProducesResponseType(typeof(MedicalRecordReadDto), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var spec = new MedicalRecordSpec(m => m.Id == id);
            var medicalReord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);
            if (medicalReord is null) return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<MedicalRecordReadDto>(medicalReord));
        }


        // Add Record
        [EndpointSummary("Add or update a medical record by a doctor")]
        [EndpointDescription("Allows a doctor to create or update a medical record and finalize the appointment if needed.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Doctor")]
        [HttpPost("AddByDoctor")]
        public async Task<IActionResult> AddMedicalRecordByDoctor([FromBody] MedicalRecordAddOrUpdateByDoctorDto model)
        {
            // Validate appointment existence
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(model.AppointmentId);
            if (appointment is null) return NotFound(new ApiResponse(404));

            // Prevent editing completed records
            if (appointment.Status == AppointmentStatus.Completed) return BadRequest(new ApiResponse(400, "Can't Modify completed Record"));

            if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));

            // Create record
            var addMedicalRecord = mapper.Map<MedicalRecord>(model);
            await unit.Repository<MedicalRecord>().AddAsync(addMedicalRecord);
            await unit.CommitAsync();

            // Finalize appointment if requested
            if (model.IsFinialize == true)
            {
                appointment.Status = AppointmentStatus.Completed;
                unit.Repository<Appointment>().Update(appointment);
            }
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? CreatedAtAction(nameof(GetMedicalRecordById),
                new { id = addMedicalRecord.Id }, mapper.Map<MedicalRecordReadDto>(addMedicalRecord))
                  : BadRequest(new ApiResponse(400));
        }

        //Add By Nurse
        [EndpointSummary("Add or update vital signs by nurse")]
        [EndpointDescription("Allows a nurse to record vital signs such as BP, temperature, and heart rate for a medical record.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Nurse")]
        [HttpPost("AddByNurse")]
        public async Task<IActionResult> AddOrUpdateMedicalRecordByNurse([FromBody] MedicalRecordAddOrUpdateByNurseDto model)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));
            var record = await unit.Repository<MedicalRecord>().GetByIdAsync(model.AppointmentId);
            if (record is null) return NotFound(new ApiResponse(404));
            if (record.Appointment?.Status == AppointmentStatus.Completed) return BadRequest(new ApiResponse(400, "Cann't modify completed record"));

            // Update nurse-allowed fields only

            record.BpDiastolic = model.BpDiastolic;
            record.BpSystolic = model.BpSystolic;
            record.Temperature = model.Temperature;
            record.HeartRate = model.HeartRate;
            record.Weight = model.Weight;
            unit.Repository<MedicalRecord>().Update(record);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? CreatedAtAction(nameof(GetMedicalRecordById),
                new { id = record.Id }, mapper.Map<MedicalRecordReadDto>(record))
                  : BadRequest(new ApiResponse(400));
        }
        //Update Record
        [EndpointSummary("Update a medical record by ID")]
        [EndpointDescription("Allows a doctor to update an existing medical record using its ID.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecordByDoctor(int? id, [FromBody] MedicalRecordAddOrUpdateByDoctorDto model)
        {
            if (id is null) return BadRequest(new ApiResponse(400));

            if (ModelState.IsValid)
            {
                var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdAsync(id.Value);
                if (medicalRecord is null) return NotFound(new ApiResponse(404));

                mapper.Map(model, medicalRecord);
                unit.Repository<MedicalRecord>().Update(medicalRecord);
                await unit.CommitAsync();
                return Ok(new ApiResponse(200, "Updated SucessFully"));
            }
            return BadRequest(new ApiResponse(400));

        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("getByName/{FullName}")]
        public async Task<IActionResult> GetByPatientName(string FullName)
        {
            var spec = new MedicalRecordSpec(d =>
            (d.Appointment.Patient.FirstName + "" + d.Appointment.Patient.LastName).Contains(FullName));
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord != null)
                return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));

            return NotFound(new ApiResponse(404));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int? id)
        {
            if (id is null) return BadRequest(new ApiResponse(400));
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdAsync(id.Value);
            if (medicalRecord is null) return NotFound(new ApiResponse(404));
            unit.Repository<MedicalRecord>().Delete(medicalRecord);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? Ok(new ApiResponse(200, "Updated SucessFully")) : BadRequest(new ApiResponse(400));
        }

    }
}
