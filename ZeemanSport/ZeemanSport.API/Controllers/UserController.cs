using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.User;
using ZeemanSport.Core.User;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> Get()
        {
            IReadOnlyCollection<UserResponse> users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpPost("Login", Name = "LoginUser")]
        public async Task<ActionResult<UserResponse>> Login(LoginUserRequest request)
        {
            UserResponse? user = await _userService.LoginAsync(request);

            if (user == null)
                return Unauthorized();

            return Ok(user);
        }

        [HttpPost("Register", Name = "RegisterUser")]
        public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest request)
        {
            UserResponse user = await _userService.RegisterAsync(request);

            return CreatedAtAction(nameof(Get),
                new
                {
                    id = user.Id
                }, user);
        }

        [HttpGet("{id:int}", Name = "Get")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            UserResponse user = await _userService.GetAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id:int}", Name = "Update")]
        public async Task<ActionResult<UserResponse>> Update(int id, UpdateUserRequest request)
        {
            UserResponse user = await _userService.UpdateAsync(id, request);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
