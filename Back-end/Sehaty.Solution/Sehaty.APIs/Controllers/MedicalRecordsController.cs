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
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = new MedicalRecordSpec(m => m.Id == id);
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        }


        [HttpGet("GetMedicalRecordForPatient")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<MedicalRecordReadDto>> GetMedicalRecordForPatient()
        {
            var patientUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var patientId = unit.Repository<Patient>().FindBy(P => P.UserId == patientUserId).Select(P => P.Id).FirstOrDefault();

            var spec = new MedicalRecordSpec(M => M.PatientId == patientId);
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        }


        [HttpGet("GetMedicalRecordByPatientId/{patientId}")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<IActionResult> GetMedicalRecordByPatientId(int patientId)
        {
            var spec = new MedicalRecordSpec(m => m.PatientId == patientId);
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("AddMedicalRecord")]
        public async Task<IActionResult> AddMedicalRecord([FromBody] MedicalRecordAddByDoctorDto model)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));
            var patient = await unit.Repository<Patient>().GetByIdAsync(model.PatientId);
            if (patient is null) return NotFound(new ApiResponse(404, "Patient Not Found"));


            bool patientHasMedicalRecord = await unit.Repository<MedicalRecord>().AnyAsync(m => m.PatientId == model.PatientId);
            if (patientHasMedicalRecord)
                return BadRequest(new ApiResponse(400, "Patient already has a medical record"));

            var addMedicalRecord = mapper.Map<MedicalRecord>(model);
            await unit.Repository<MedicalRecord>().AddAsync(addMedicalRecord);

            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? CreatedAtAction(nameof(GetById),
                new { id = addMedicalRecord.Id }, mapper.Map<MedicalRecordReadDto>(addMedicalRecord))
                  : BadRequest(new ApiResponse(400));
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateByDoctor/{id}")]
        public async Task<IActionResult> Update(int id, MedicalRecordUpdateDto model)
        {
            var record = await unit.Repository<MedicalRecord>().GetByIdAsync(id);
            if (record == null) return NotFound(new ApiResponse(404, "Medical Record Not Found !!!"));

            bool isSameDayUpdate = record.CreatedAt?.Date == DateTime.UtcNow.Date;

            if (isSameDayUpdate)
            {
                var auditEntries = new List<MedicalRecordAuditLog>();
                var props = typeof(MedicalRecordUpdateDto).GetProperties();
                var doctorUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var doctorId = (await unit.Repository<Doctor>().GetFirstOrDefaultAsync(D => D.UserId == doctorUserId)).Id;

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
                            UpdatedByDoctorId = doctorId
                        });
                    }
                }

                foreach (var entry in auditEntries)
                    await unit.Repository<MedicalRecordAuditLog>().AddAsync(entry);
            }

            mapper.Map(model, record);
            unit.Repository<MedicalRecord>().Update(record);

            await unit.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdAsync(id);
            if (medicalRecord is null) return NotFound(new ApiResponse(404));
            unit.Repository<MedicalRecord>().Delete(medicalRecord);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? Ok(new ApiResponse(200, "Deleted Successfully")) : BadRequest(new ApiResponse(400));
        }

        //[HttpGet("GetMedicalRecordForNurse/{id}")]
        //[Authorize(Roles = "Nurse")]
        //public async Task<IActionResult> GetMedicalRecordForNurse(int id)
        //{
        //    var spec = new MedicalRecordSpec(m => m.Id == id);
        //    var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

        //    if (medicalRecord is null)
        //        return NotFound(new ApiResponse(404));

        //    var today = DateTime.Now;
        //    if (medicalRecord.Appointment.AppointmentDateTime < today.AddDays(-7) || medicalRecord.Appointment.AppointmentDateTime > today.AddDays(7))
        //        return Unauthorized(new ApiResponse(401, "You are not allowed to view this record"));

        //    return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));
        //}




        //// Add Record



        ////Add By Nurse
        //[Authorize(Roles = "Nurse")]
        //[HttpPost("AddByNurse")]
        //public async Task<IActionResult> AddOrUpdateMedicalRecordByNurse([FromBody] MedicalRecordAddOrUpdateByNurseDto model)
        //{
        //    if (!ModelState.IsValid) return BadRequest(new ApiResponse(400));
        //    var spec = new MedicalRecordSpec(m => m.AppointmentId == model.AppointmentId);
        //    var record = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);
        //    if (record is null) return NotFound(new ApiResponse(404));
        //    if (record.Appointment?.Status == AppointmentStatus.Completed) return BadRequest(new ApiResponse(400, "Can't modify completed record"));

        //    // Update nurse-allowed fields only

        //    record.BpDiastolic = model.BpDiastolic;
        //    record.BpSystolic = model.BpSystolic;
        //    record.Temperature = model.Temperature;
        //    record.HeartRate = model.HeartRate;
        //    record.Weight = model.Weight;
        //    unit.Repository<MedicalRecord>().Update(record);
        //    var RowAffected = await unit.CommitAsync();
        //    return RowAffected > 0 ? CreatedAtAction(nameof(GetMedicalRecordForDoctor),
        //        new { id = record.Id }, mapper.Map<MedicalRecordReadDto>(record))
        //          : BadRequest(new ApiResponse(400));
        //}


        //[Authorize(Roles = "Doctor")]
        //[HttpPut("UpdateByDoctor/{id}")]
        //public async Task<IActionResult> Update(int id, MedicalRecordUpdateDto model)
        //{
        //    var spec = new MedicalRecordSpec(m => m.Id == id);
        //    var record = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);
        //    if (record == null) return NotFound(new ApiResponse(404));

        //    bool isSameDayUpdate = record.CreatedAt?.Date == DateTime.UtcNow.Date;

        //    if (isSameDayUpdate)
        //    {
        //        var auditEntries = new List<MedicalRecordAuditLog>();
        //        var props = typeof(MedicalRecordUpdateDto).GetProperties();

        //        foreach (var prop in props)
        //        {
        //            var newValue = prop.GetValue(model)?.ToString();
        //            var oldValue = record.GetType().GetProperty(prop.Name)?.GetValue(record)?.ToString();

        //            if (newValue != oldValue)
        //            {
        //                auditEntries.Add(new MedicalRecordAuditLog
        //                {
        //                    MedicalRecordId = record.Id,
        //                    FieldName = prop.Name,
        //                    OldValue = oldValue,
        //                    NewValue = newValue,
        //                    UpdatedByDoctorId = record.Appointment.DoctorId
        //                });
        //            }
        //        }

        //        foreach (var entry in auditEntries)
        //            await unit.Repository<MedicalRecordAuditLog>().AddAsync(entry);
        //    }

        //    mapper.Map(model, record);
        //    unit.Repository<MedicalRecord>().Update(record);

        //    await unit.CommitAsync();
        //    return Ok(new ApiResponse(200, "Updated SuccessFully"));
        //}



        //[Authorize(Roles = "Doctor")]
        //[HttpGet("getByName/{FullName}")]
        //public async Task<IActionResult> GetByPatientName(string FullName)
        //{
        //    var spec = new MedicalRecordSpec(d =>
        //    (d.Appointment.Patient.FirstName + "" + d.Appointment.Patient.LastName).Contains(FullName));
        //    var medicalRecord = await unit.Repository<MedicalRecord>().GetByIdWithSpecAsync(spec);

        //    if (medicalRecord != null)
        //        return Ok(mapper.Map<MedicalRecordReadDto>(medicalRecord));

        //    return NotFound(new ApiResponse(404));
        //}

    }
}
