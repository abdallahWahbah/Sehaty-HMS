using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Application.Dtos.MedicalRecordDto;
using Sehaty.Core.Entities.Business_Entities.Appointments;
using Sehaty.Core.Entities.Business_Entities.MedicalRecords;
using Sehaty.Core.Specifications.MedicalReord;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Dtos;
namespace Sehaty.APIs.Controllers
{

    public class MedicalRecordsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecordReadDto>>> GetAllMedicalRecord()
        {
            var spec = new MedicalRecordSpec();
            var medicalRecords = await unit.Repository<MedicalRecord>().GetAllWithSpecAsync(spec);
            if (medicalRecords is null) return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<List<MedicalRecordReadDto>>(medicalRecords));
        }



        [HttpGet("GetMedicalRecordForPatient/{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> GetMedicalRecordForPatient(int id)
        {
            var spec = new MedicalRecordSpec(m => m.Id == id);
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404));

            var userId = User.FindFirst("uid")?.Value;
            if (medicalRecord.Appointment.PatientId.ToString() != userId)
                return Unauthorized(new ApiResponse(401, "You are not allowed to view this record"));

            return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        }


        [HttpGet("GetMedicalRecordForDoctor/{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMedicalRecordForDoctor(int id)
        {
            var spec = new MedicalRecordSpec(m => m.Id == id);
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404));

            var userId = User.FindFirst("uid")?.Value;
            if (medicalRecord.Appointment.DoctorId.ToString() != userId)
                return Unauthorized(new ApiResponse(401, "You are not allowed to view this record"));

            return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        }


        [HttpGet("GetMedicalRecordForNurse/{id}")]
        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> GetMedicalRecordForNurse(int id)
        {
            var spec = new MedicalRecordSpec(m => m.Id == id);
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404));

            var today = DateTime.Now;
            if (medicalRecord.Appointment.AppointmentDateTime < today.AddDays(-7) || medicalRecord.Appointment.AppointmentDateTime > today.AddDays(7))
                return Unauthorized(new ApiResponse(401, "You are not allowed to view this record"));

            return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        }




        // Add Record

        [Authorize(Roles = "Doctor")]
        [HttpPost("AddByDoctor")]
        public async Task<IActionResult> AddMedicalRecordByDoctor([FromBody] MedicalRecordAddByDoctorDto model)
        {
            // Validate appointment existence
            if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));

            var appointment = await unit.Repository<Appointment>().GetByIdAsync(model.AppointmentId);
            if (appointment is null) return NotFound(new ApiResponse(404));

            // Prevent editing completed records
            if (appointment.Status == AppointmentStatus.Completed) return BadRequest(new ApiResponse(400, "Can't Modify completed Record"));


            // Create record
            var addMedicalRecord = mapper.Map<MedicalRecord>(model);
            await unit.Repository<MedicalRecord>().AddAsync(addMedicalRecord);

            // Finalize appointment if requested
            if (model.IsFinialize == true)
            {
                appointment.Status = AppointmentStatus.Completed;
                unit.Repository<Appointment>().Update(appointment);
            }
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? CreatedAtAction(nameof(GetMedicalRecordForDoctor),
                new { id = addMedicalRecord.Id }, mapper.Map<MedicalRecordReadDto>(addMedicalRecord))
                  : BadRequest(new ApiResponse(400));
        }

        //Add By Nurse
        [Authorize(Roles = "Nurse")]
        [HttpPost("AddByNurse")]
        public async Task<IActionResult> AddOrUpdateMedicalRecordByNurse([FromBody] MedicalRecordAddOrUpdateByNurseDto model)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));
            var spec = new MedicalRecordSpec(m => m.AppointmentId == model.AppointmentId);
            var record = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);
            if (record is null) return NotFound(new ApiResponse(404));
            if (record.Appointment?.Status == AppointmentStatus.Completed) return BadRequest(new ApiResponse(400, "Can't modify completed record"));

            // Update nurse-allowed fields only

            record.BpDiastolic = model.BpDiastolic;
            record.BpSystolic = model.BpSystolic;
            record.Temperature = model.Temperature;
            record.HeartRate = model.HeartRate;
            record.Weight = model.Weight;
            unit.Repository<MedicalRecord>().Update(record);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? CreatedAtAction(nameof(GetMedicalRecordForDoctor),
                new { id = record.Id }, mapper.Map<MedicalRecordReadDto>(record))
                  : BadRequest(new ApiResponse(400));
        }


        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateByDoctor/{id}")]
        public async Task<IActionResult> Update(int id, MedicalRecordUpdateDto model)
        {
            var spec = new MedicalRecordSpec(m => m.Id == id);
            var record = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);
            if (record == null) return NotFound(new ApiResponse(404));

            bool isSameDayUpdate = record.CreatedAt?.Date == DateTime.UtcNow.Date;

            if (isSameDayUpdate)
            {
                var auditEntries = new List<MedicalRecordAuditLog>();
                var props = typeof(MedicalRecordUpdateDto).GetProperties();

                foreach (var prop in props)
                {
                    var newValue = prop.GetValue(model)?.ToString();
                    var oldValue = record.GetType().GetProperty(prop.Name)?.GetValue(record)?.ToString();

                    if (newValue != oldValue)
                    {
                        auditEntries.Add(new MedicalRecordAuditLog
                        {
                            MedicalRecordId = record.Id,
                            FieldName = prop.Name,
                            OldValue = oldValue,
                            NewValue = newValue,
                            UpdatedByDoctorId = record.Appointment.DoctorId
                        });
                    }
                }

                foreach (var entry in auditEntries)
                    await unit.Repository<MedicalRecordAuditLog>().AddAsync(entry);
            }

            mapper.Map(model, record);
            unit.Repository<MedicalRecord>().Update(record);

            await unit.CommitAsync();
            return Ok(new ApiResponse(200, "Updated SuccessFully"));
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
            return RowAffected > 0 ? Ok(new ApiResponse(200, "Deleted Successfully")) : BadRequest(new ApiResponse(400));
        }

    }
}
