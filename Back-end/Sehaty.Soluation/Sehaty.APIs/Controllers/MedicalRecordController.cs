using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos;
using Sehaty.Core.Entites;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper map;

        public MedicalRecordController(IUnitOfWork unitOfWork, IMapper map)
        {
            this.unitOfWork = unitOfWork;
            this.map = map;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMedicalRecord()
        {
            var models = await unitOfWork.Repository<MedicalRecord>().GetAllAsync();
            if (models is null) return NotFound();
            var Data = map.Map<List<MedicalRecordDto>>(models);
            return Ok(Data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var model = await unitOfWork.Repository<MedicalRecord>().GetByIdAsync(id);
            if (model is null) return NotFound();
            var Data = map.Map<MedicalRecordDto>(model);
            return Ok(Data);

        }
        //[HttpPost]
        //public async Task<IActionResult> AddMedicalRecordByDoctor(MedicalRecordDto model)
        //{
        //    if (ModelState.IsValid) return BadRequest();
        //    var Data = map.Map<MedicalRecord>(model);
        //}
    }
}
