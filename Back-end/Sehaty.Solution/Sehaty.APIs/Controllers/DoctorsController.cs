namespace Sehaty.APIs.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class DoctorsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDoctorDto>>> GetAllDoctors()
        {
            var spec = new DoctorSpecifications(D => !D.IsDeleted);
            var doctors = await unit.Repository<Doctor>().GetAllWithSpecAsync(spec);
            if (doctors is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetDoctorDto>>(doctors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetDoctorDto>> GetDoctorById(int id)
        {
            var spec = new DoctorSpecifications(D => D.Id == id && !D.IsDeleted);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(spec);
            if (doctor is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetDoctorDto>(doctor));
        }

        [HttpGet("FindDeletedDoctor/{id}")]
        public async Task<ActionResult<GetDoctorDto>> FindDeletedDoctor(int id)
        {
            var spec = new DoctorSpecifications(D => D.Id == id && D.IsDeleted);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(spec);
            if (doctor is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetDoctorDto>(doctor));
        }

        [HttpPost]
        public async Task<ActionResult> AddDoctor([FromBody] DoctorAddUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await unit.Users.ExistsByIdAsync(dto.UserId);
            if (!userExists)
                return BadRequest(new ApiResponse(400, "Invalid User Id"));

            var departmentExists = await unit.Repository<Department>().AnyAsync(D => D.Id == dto.DepartmentId);
            if (!departmentExists)
                return BadRequest(new ApiResponse(400, "Invalid Department Id"));

            var userIsUsed = await unit.Repository<Doctor>().AnyAsync(D => D.UserId == dto.UserId);
            if (userIsUsed)
                return BadRequest(new ApiResponse(400, "User Id Is Already Used !!"));

            userIsUsed = await unit.Repository<Patient>().AnyAsync(P => P.UserId == dto.UserId);
            if (userIsUsed)
                return BadRequest(new ApiResponse(400, "User Id Is Already Used !!"));


            //var doctor = await doctorService.AddDoctorAsync(dto);
            var doctorToAdd = mapper.Map<Doctor>(dto);

            await unit.Repository<Doctor>().AddAsync(doctorToAdd);
            await unit.CommitAsync();

            return CreatedAtAction(nameof(GetDoctorById),
                new { id = doctorToAdd.Id },
                mapper.Map<GetDoctorDto>(doctorToAdd));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDoctor(int id, [FromBody] DoctorAddUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var doctor = await doctorService.UpdateDoctorAsync(id, dto);
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);

            if (doctor == null)
                return NotFound(new ApiResponse(404));
            mapper.Map(dto, doctor);
            unit.Repository<Doctor>().Update(doctor);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            //var success = await doctorService.DeleteDoctorAsync(id);
            //if (!success)
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new ApiResponse(404));
            doctor.IsDeleted = true;
            unit.Repository<Doctor>().Update(doctor);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpDelete("PermanentDelete/{id}")]
        public async Task<ActionResult> PermanentDelete(int id)
        {
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new ApiResponse(404));
            unit.Repository<Doctor>().Delete(doctor);
            await unit.CommitAsync();
            return NoContent();
        }
    }
}
