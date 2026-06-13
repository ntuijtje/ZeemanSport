using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.Session;
using ZeemanSport.Core.Session;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet(Name = "GetSchedule")]
        public async Task<ActionResult<IReadOnlyCollection<SessionResponse>>> GetSchedule([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            IReadOnlyCollection<SessionResponse> sessions = await _sessionService.GetScheduleAsync(from, to);

            return Ok(sessions);
        }

        [HttpGet("{id:int}", Name = "GetSession")]
        public async Task<ActionResult<SessionResponse>> Get(int id)
        {
            SessionResponse? session = await _sessionService.GetByIdAsync(id);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpGet("Instructor/{instructorId:int}", Name = "GetInstructorSessions")]
        public async Task<ActionResult<IReadOnlyCollection<SessionResponse>>> GetInstructorSessions(int instructorId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            IReadOnlyCollection<SessionResponse> sessions = await _sessionService.GetInstructorSessionsAsync(instructorId, from, to);

            return Ok(sessions);
        }

        [HttpPost(Name = "CreateSessions")]
        public async Task<ActionResult<IReadOnlyCollection<SessionResponse>>> Create(CreateSessionRequest request)
        {
            IReadOnlyCollection<SessionResponse> sessions = await _sessionService.CreateAsync(request);

            return Ok(sessions);
        }

        [HttpPut("{id:int}", Name = "UpdateSession")]
        public async Task<ActionResult<SessionResponse>> Update(int id, UpdateSessionRequest request)
        {
            SessionResponse? session = await _sessionService.UpdateAsync(id, request);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpDelete("{id:int}", Name = "DeleteSession")]
        public async Task<ActionResult> Delete(int id)
        {
            bool deleted = await _sessionService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
