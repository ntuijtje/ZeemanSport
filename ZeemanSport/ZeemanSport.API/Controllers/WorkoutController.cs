using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.Workout;
using ZeemanSport.Core.Workout;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet(Name = "GetWorkouts")]
        public async Task<ActionResult<IReadOnlyCollection<WorkoutResponse>>> Get()
        {
            IReadOnlyCollection<WorkoutResponse> workouts = await _workoutService.GetAllAsync();

            return Ok(workouts);
        }

        [HttpGet("{id:int}", Name = "GetWorkout")]
        public async Task<ActionResult<WorkoutResponse>> Get(int id)
        {
            WorkoutResponse? workout = await _workoutService.GetByIdAsync(id);

            if (workout == null)
                return NotFound();

            return Ok(workout);
        }

        [HttpPost(Name = "CreateWorkout")]
        public async Task<ActionResult<WorkoutResponse>> Create(CreateWorkoutRequest request)
        {
            WorkoutResponse workout = await _workoutService.CreateAsync(request);

            return CreatedAtAction(nameof(Get), new { id = workout.Id }, workout);
        }

        [HttpPut("{id:int}", Name = "UpdateWorkout")]
        public async Task<ActionResult<WorkoutResponse>> Update(int id, UpdateWorkoutRequest request)
        {
            WorkoutResponse? workout = await _workoutService.UpdateAsync(id, request);

            if (workout == null)
                return NotFound();

            return Ok(workout);
        }

        [HttpDelete("{id:int}", Name = "DeleteWorkout")]
        public async Task<ActionResult> Delete(int id)
        {
            bool deleted = await _workoutService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
