using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.DepartmentDtos;
using Sehaty.Core.Entites;
using Sehaty.Core.Specefications;
using Sehaty.Core.Specifications.DepartmentSpec;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{
    public class DepartmentsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDepartmentDto>>> GetAllDepartments()
        {
            var spec = new DepartmentSpecifications();
            var departments = await unit.Repository<Department>().GetAllWithSpecAsync(spec);
            if (departments is null) return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetDepartmentDto>>(departments));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetDepartmentDto>> GetDepartmentById(int id)
        {
            var spec = new DepartmentSpecifications(id);
            var department = await unit.Repository<Department>().GetByIdWithSpecAsync(spec);
            if (department is null) return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetDepartmentDto>(department));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDepartment(int id, [FromBody] DepartmentAddUpdateDto departmentDto)
        {
            var departmentToEdit = await unit.Repository<Department>().GetByIdAsync(id);
            if (departmentToEdit is null) return NotFound(new ApiResponse(404));
            if (ModelState.IsValid)
            {
                mapper.Map(departmentDto, departmentToEdit);
                unit.Repository<Department>().Update(departmentToEdit);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);

        }

        [HttpPost]
        public async Task<ActionResult> AddDepartment([FromBody] DepartmentAddUpdateDto departmentDto)
        {
            if (ModelState.IsValid)
            {
                var departmentToAdd = mapper.Map<Department>(departmentDto);
                await unit.Repository<Department>().AddAsync(departmentToAdd);
                await unit.CommitAsync();
                return CreatedAtAction(nameof(GetDepartmentById), new { id = departmentToAdd.Id }, mapper.Map<GetDepartmentDto>(departmentToAdd));
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var departmentToDelete = await unit.Repository<Department>().GetByIdAsync(id);
            if (departmentToDelete is null) return NotFound(new ApiResponse(404));
            unit.Repository<Department>().Delete(departmentToDelete);
            await unit.CommitAsync();
            return NoContent();
        }

    }
}
