namespace Sehaty.APIs.Controllers
{

    public class PatientsController(IUnitOfWork unit, IMapper mapper, IPatientService patientService) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPatientDto>>> GetAllPatients()
        {
            var spec = new PatientSpecifications(P => !P.IsDeleted);
            var patients = await unit.Repository<Patient>().GetAllWithSpecAsync(spec);
            if (patients is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetPatientDto>>(patients));
        }

        [HttpGet("Search")]
        //[Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<IEnumerable<GetPatientDto>>> SearchForPatient([FromQuery] PatientSpecsParams specParam)
        {
            var spec = new PatientSearchSpecification(specParam);
            var patients = await unit.Repository<Patient>().GetAllWithSpecAsync(spec);
            if (patients is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetPatientDto>>(patients));
        }

        [HttpGet("UpdateStatus/{id}")]
        //[Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<IEnumerable<GetPatientDto>>> UpdatePatientStatus(int id, PatientStatus status)
        {
            var patient = await unit.Repository<Patient>().GetByIdAsync(id);
            if (patient is null)
                return NotFound(new ApiResponse(404));
            patient.Status = status;
            unit.Repository<Patient>().Update(patient);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<GetPatientDto>> GetPatientById(int id)
        {
            //var spec = new PatientSpecifications(P => P.Id == id && !P.IsDeleted);
            //var patient = await unit.Repository<Patient>().GetByIdWithSpecAsync(spec);
            //if (patient is null)
            //    return NotFound(new ApiResponse(404));
            //return Ok(mapper.Map<GetPatientDto>(patient));
            return await FindPatient(id);
        }

        [HttpGet("FindDeletedPatient/{id}")]
        //[Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<GetPatientDto>> FindDeletedPatient(int id)
        {
            //var spec = new PatientSpecifications(P => P.Id == id && P.IsDeleted);
            //var patient = await unit.Repository<Patient>().GetByIdWithSpecAsync(spec);
            //if (patient is null)
            //    return NotFound(new ApiResponse(404));
            //return Ok(mapper.Map<GetPatientDto>(patient));
            return await FindPatient(id, isDeleted: true);
        }

        [HttpPost]
        //[Authorize(Roles = "Patient,Receptionist")]
        public async Task<ActionResult> AddPatient([FromBody] PatientAddDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var patientToAdd = await patientService.AddPatientAsync(dto);
            return CreatedAtAction(nameof(GetPatientById), new { id = patientToAdd.Id }, mapper.Map<GetPatientDto>(patientToAdd));

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult> UpdatePatientByStuff(int id, [FromBody] PatientUpdateByStaffDto dto)
        {
            if (ModelState.IsValid)
            {
                var patientToEdit = await unit.Repository<Patient>().GetByIdAsync(id);
                if (patientToEdit is null)
                    return NotFound(new ApiResponse(404));
                if (patientToEdit.IsDeleted)
                    return BadRequest(new ApiResponse(400, "Cannot update a deleted patient."));
                mapper.Map(dto, patientToEdit);
                unit.Repository<Patient>().Update(patientToEdit);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpPut("EditProfile/{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> EditPatientProfile(int id, [FromBody] PatientUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var patientToEdit = await unit.Repository<Patient>().GetByIdAsync(id);
                if (patientToEdit is null)
                    return NotFound(new ApiResponse(404));
                mapper.Map(dto, patientToEdit);
                unit.Repository<Patient>().Update(patientToEdit);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var patientToDelete = await unit.Repository<Patient>().GetByIdAsync(id);
            if (patientToDelete is null)
                return NotFound(new ApiResponse(404));
            patientToDelete.IsDeleted = true;
            unit.Repository<Patient>().Update(patientToDelete);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpDelete("PermanentDelete/{id}")]
        //[Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult> PermanentDelete(int id)
        {
            var patientToDelete = await unit.Repository<Patient>().GetByIdAsync(id);
            if (patientToDelete is null)
                return NotFound(new ApiResponse(404));
            unit.Repository<Patient>().Delete(patientToDelete);
            await unit.CommitAsync();
            return NoContent();
        }
        async Task<ActionResult<GetPatientDto>> FindPatient(int id, bool isDeleted = false)
        {
            var spec = new PatientSpecifications(P => P.Id == id && P.IsDeleted == isDeleted);
            var patient = await unit.Repository<Patient>().GetByIdWithSpecAsync(spec);
            if (patient is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetPatientDto>(patient));
        }
    }
}
