using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.DoctorAvailabilitySlotDto;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specifications.DoctorAvailabilitySlotSpec;
using Sehaty.Core.Specifications.Prescription_Specs;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAvailabilitySlotController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper map;

        public DoctorAvailabilitySlotController(IUnitOfWork unitOfWork, IMapper map)
        {
            this.unitOfWork = unitOfWork;
            this.map = map;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var models = await unitOfWork.Repository<DoctorAvailabilitySlot>().GetAllAsync();
            var Data = map.Map<List<DoctorAvailabilitySlotDto>>(models);
            return Ok(Data);
        }
        //Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int? id) 
        {
            if (id is null) return BadRequest();
            var model = await unitOfWork.Repository<DoctorAvailabilitySlot>().GetByIdAsync(id.Value);
            if (model is null) return NotFound();
           return Ok(model);
        }

        [HttpGet("getByName{FullName}")]
        public async Task<IActionResult> GetByName(string FullName)
        {
            var spec = new DoctorAvailabilitySlotSpec(d=>
            (d.Doctor.FirstName + ""+ d.Doctor.LastName).Contains(FullName));
            var slot = await unitOfWork.Repository<DoctorAvailabilitySlot>().GetByIdWithSpecAsync(spec);

            if (slot != null)
                return Ok(map.Map<DoctorAvailabilitySlotDto>(slot));

            return NotFound();
        }

        // PostData
        [HttpPost]
        public async Task<IActionResult> AddData(DoctorAvailabilitySlotDto model) 
        {
            if (!ModelState.IsValid) return BadRequest();
            if (model.Id == 0)
            {
                var Data = map.Map<DoctorAvailabilitySlot>(model);
                await unitOfWork.Repository<DoctorAvailabilitySlot>().AddAsync(Data);
            }
            else
            {
                var record = await unitOfWork.Repository<DoctorAvailabilitySlot>().GetByIdAsync(model.Id);
                if (record is null) return NotFound();
                var Data = map.Map<DoctorAvailabilitySlot>(model);
                unitOfWork.Repository<DoctorAvailabilitySlot>().Update(Data);

            }
             var RowAffected = await unitOfWork.CommitAsync();
            return RowAffected>0 ? Ok(model) : BadRequest();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateData(int? id,[FromBody]DoctorAvailabilitySlotDto model)
        {
            if (id is null) return BadRequest();
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return BadRequest();
            var Data = map.Map<DoctorAvailabilitySlot>(model);
            unitOfWork.Repository<DoctorAvailabilitySlot>().Update(Data);
            var RowAffected = await unitOfWork.CommitAsync();
            return RowAffected > 0 ? Ok(model) : BadRequest();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteData(int? id)
        {
            if (id is null) return BadRequest();
            var model = await unitOfWork.Repository<DoctorAvailabilitySlot>().GetByIdAsync(id.Value);
            if(model is null) return NotFound();
             unitOfWork.Repository<DoctorAvailabilitySlot>().Delete(model);
            var RowAffected = await unitOfWork.CommitAsync();
            return RowAffected > 0 ? Ok(model) : BadRequest();
        }
       

    }
}
