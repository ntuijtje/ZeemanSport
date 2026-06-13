using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.Instructor;
using ZeemanSport.Core.Instructor;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService _instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpGet(Name = "GetInstructors")]
        public async Task<ActionResult<IReadOnlyCollection<InstructorResponse>>> Get()
        {
            IReadOnlyCollection<InstructorResponse> instructors = await _instructorService.GetAllAsync();

            return Ok(instructors);
        }

        [HttpGet("{id:int}", Name = "GetInstructor")]
        public async Task<ActionResult<InstructorResponse>> Get(int id)
        {
            InstructorResponse? instructor = await _instructorService.GetByIdAsync(id);

            if (instructor == null)
                return NotFound();

            return Ok(instructor);
        }

        [HttpPost(Name = "CreateInstructor")]
        public async Task<ActionResult<InstructorResponse>> Create(CreateInstructorRequest request)
        {
            InstructorResponse instructor = await _instructorService.CreateAsync(request);

            return CreatedAtAction(nameof(Get), new { id = instructor.Id }, instructor);
        }

        [HttpPut("{id:int}", Name = "UpdateInstructor")]
        public async Task<ActionResult<InstructorResponse>> Update(int id, UpdateInstructorRequest request)
        {
            InstructorResponse? instructor = await _instructorService.UpdateAsync(id, request);

            if (instructor == null)
                return NotFound();

            return Ok(instructor);
        }

        [HttpDelete("{id:int}", Name = "DeleteInstructor")]
        public async Task<ActionResult> Delete(int id)
        {
            bool deleted = await _instructorService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
