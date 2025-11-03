using Microsoft.AspNetCore.Mvc;
using Sehaty.Core.Entites;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class DoctorsController : ApiBaseController
    {
        private readonly IUnitOfWork unit;

        public DoctorsController(IUnitOfWork unit)
        {
            this.unit = unit;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetAllDoctors()
        {
            var doctors = await unit.Repository<Doctor>().GetAllAsync();
            if (doctors is null)
                return NotFound();
            return Ok(doctors);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctorById(int id)
        {
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctor is null)
                return NotFound();
            return Ok(doctor);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctor is null)
                return NotFound();
            unit.Repository<Doctor>().Delete(doctor);
            await unit.CommitAsync();
            return Ok(doctor);
        }
    }
}
