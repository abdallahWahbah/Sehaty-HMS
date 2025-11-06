using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.FeedbackDtos;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.FeedbackSpec;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class FeedbacksController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetFeedbackDto>>> GetAllFeedbacks()
        {
            var spec = new FeedbackSpecification();
            var feedbacks = await unit.Repository<Feedback>().GetAllWithSpecAsync(spec);
            if (feedbacks is null) return NotFound();
            return Ok(mapper.Map<IEnumerable<GetFeedbackDto>>(feedbacks));
        }
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<GetFeedbackDto>> GetFeedbackById(int id)
        {
            var spec = new FeedbackSpecification(id);
            var feedback = await unit.Repository<Feedback>().GetByIdWithSpecAsync(spec);
            if (feedback is null) return NotFound();
            return Ok(mapper.Map<GetFeedbackDto>(feedback));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFeedback(int id, [FromBody] FeedbackAddUpdateDto feedbackDto)
        {
            var feedbackToEdit = await unit.Repository<Feedback>().GetByIdAsync(id);
            if (feedbackToEdit is null) return NotFound();
            if (ModelState.IsValid)
            {
                mapper.Map(feedbackDto, feedbackToEdit);
                unit.Repository<Feedback>().Update(feedbackToEdit);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);

        }

        [HttpPost]
        public async Task<ActionResult> AddFeedback([FromBody] FeedbackAddUpdateDto feedbackDto)
        {
            if (ModelState.IsValid)
            {
                var feedbackToAdd = mapper.Map<Feedback>(feedbackDto);
                await unit.Repository<Feedback>().AddAsync(feedbackToAdd);
                await unit.CommitAsync();
                return CreatedAtAction(nameof(GetFeedbackById), new { id = feedbackToAdd.Id }, feedbackToAdd);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            var feedbackToDelete = await unit.Repository<Feedback>().GetByIdAsync(id);
            if (feedbackToDelete is null) return NotFound();
            unit.Repository<Feedback>().Delete(feedbackToDelete);
            await unit.CommitAsync();
            return NoContent();
        }

    }
}
