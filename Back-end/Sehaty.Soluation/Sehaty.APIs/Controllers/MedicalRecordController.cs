using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sehaty.Core.Entites;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sehaty.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper map;

        public MedicalRecordController(IUnitOfWork unitOfWork , IMapper map)
        {
            this.unitOfWork = unitOfWork;
            this.map = map;
        }
        [EndpointSummary("Get all medical records")]
        [EndpointDescription("Retrieves all medical records from the database and maps them to doctor DTOs")]
        [ProducesResponseType(typeof(List<MedicalRecordDoctorDto>), 200)]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<IActionResult> GetAllMedicalRecord()
        {
            var models = await unitOfWork.Repository<MedicalRecord>().GetAllAsync();
            if (models is null) return NotFound();
            var Data = map.Map<List<MedicalRecordDoctorDto>>(models);
            return Ok(Data);
        }

        //Get Specific
        [EndpointSummary("Get a specific medical record")]
        [EndpointDescription("Retrieves a medical record by its ID")]
        [ProducesResponseType(typeof(MedicalRecordDoctorDto), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var model = await unitOfWork.Repository<MedicalRecord>().GetByIdAsync(id);
            if (model is null) return NotFound();
            var Data = map.Map<MedicalRecordDoctorDto>(model);
            return Ok(Data);
               
        }


        // Add Record
        [EndpointSummary("Add or update a medical record by a doctor")]
        [EndpointDescription("Allows a doctor to create or update a medical record and finalize the appointment if needed.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[Authorize(Roles ="Doctor")]
        [HttpPost("AddByDoctor")]
        public async Task<IActionResult> AddMedicalRecordByDoctor([FromBody] MedicalRecordDoctorDto model)
        {
            // Validate appointment existence
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdAsync(model.AppointmentId);
            if (appointment is null) return NotFound();

            // Prevent editing completed records
            if (appointment.Status == AppointmentStatus.Completed) 
            {
                return BadRequest("Can't Modify completed Record");
            }
            if (!ModelState.IsValid) return BadRequest();

            // Create or update record
            if (model.Id == 0)
            {
                var Data = map.Map<MedicalRecord>(model);
                await unitOfWork.Repository<MedicalRecord>().Add(Data);
            }
            else 
            {
                var record = await unitOfWork.Repository<MedicalRecord>().GetByIdAsync(model.Id);
                if(record is null) return NotFound();
                var Data = map.Map<MedicalRecord>(model);
                unitOfWork.Repository<MedicalRecord>().Update(Data);

            }
            // Finalize appointment if requested
            if (model.IsFinialize == true)
            { 
                appointment.Status = AppointmentStatus.Completed;
                unitOfWork.Repository<Appointment>().Update(appointment);
            }
            var RowAffected = await unitOfWork.CommitAsync();
            return RowAffected > 0 ? Ok(model) : BadRequest();
           
        }


        //Add By Nurse
        [EndpointSummary("Add or update vital signs by nurse")]
        [EndpointDescription("Allows a nurse to record vital signs such as BP, temperature, and heart rate for a medical record.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[Authorize(Roles = "Nurse")]
        [HttpPost("AddByNurse")]
        public async Task<IActionResult> AddMedicalRecordByNurse([FromBody] MedicalRecordNurseDto model)
        {
           if(!ModelState.IsValid) return BadRequest();
            var record = await unitOfWork.Repository<MedicalRecord>().GetByIdAsync(model.Id);
            if(record is null) return NotFound();
            if (record.Appointment?.Status == AppointmentStatus.Completed) return BadRequest("Cann't modify completed record");

            // Update nurse-allowed fields only

            record.BpDiastolic = model.BpDiastolic;
            record.BpSystolic = model.BpSystolic;
            record.Temperature = model.Temperature; 
            record.HeartRate = model.HeartRate;
            record.Weight = model.Weight;
            unitOfWork.Repository<MedicalRecord>().Update(record);
            var RowAffected = await unitOfWork.CommitAsync();
            return RowAffected > 0 ? Ok(model) : BadRequest();
        }


        //Update Record
        [EndpointSummary("Update a medical record by ID")]
        [EndpointDescription("Allows a doctor to update an existing medical record using its ID.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
       // [Authorize(Roles ="Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int? id ,[FromBody] MedicalRecordDoctorDto model)
        {
            if(id is null) return BadRequest();
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return BadRequest();
            var Data = map.Map<MedicalRecord>(model);
            unitOfWork.Repository<MedicalRecord>().Update(Data);
           var RowAffected=  await unitOfWork.CommitAsync();
            if (RowAffected > 0)
            {
                return Ok(model);
            }
            else return BadRequest();
           
        }
 
    }
}
