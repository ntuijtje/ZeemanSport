using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.Location;
using ZeemanSport.Core.Location;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet(Name = "GetLocations")]
        public async Task<ActionResult<IReadOnlyCollection<LocationResponse>>> Get()
        {
            IReadOnlyCollection<LocationResponse> locations = await _locationService.GetAllAsync();

            return Ok(locations);
        }

        [HttpGet("{id:int}", Name = "GetLocation")]
        public async Task<ActionResult<LocationResponse>> Get(int id)
        {
            LocationResponse? location = await _locationService.GetByIdAsync(id);

            if (location == null)
                return NotFound();

            return Ok(location);
        }

        [HttpPost(Name = "CreateLocation")]
        public async Task<ActionResult<LocationResponse>> Create(CreateLocationRequest request)
        {
            LocationResponse location = await _locationService.CreateAsync(request);

            return CreatedAtAction(nameof(Get), new { id = location.Id }, location);
        }

        [HttpPut("{id:int}", Name = "UpdateLocation")]
        public async Task<ActionResult<LocationResponse>> Update(int id, UpdateLocationRequest request)
        {
            LocationResponse? location = await _locationService.UpdateAsync(id, request);

            if (location == null)
                return NotFound();

            return Ok(location);
        }

        [HttpDelete("{id:int}", Name = "DeleteLocation")]
        public async Task<ActionResult> Delete(int id)
        {
            bool deleted = await _locationService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
