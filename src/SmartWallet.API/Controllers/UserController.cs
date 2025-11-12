using SmartWallet.Application.Services;
using Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userServices.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userServices.GetUserById(userId);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userServices.GetUserByEmail(email);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest request)
        {
            var userWithWallet = await _userServices.RegisterUser(request);
            if (userWithWallet is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetUserById), new { userId = userWithWallet.Id }, userWithWallet);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAdminUser([FromBody] UserCreateRequest request)
        {
            var createdUser = await _userServices.CreateAdminUser(request);
            if (createdUser is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDataRequest request)
        {
            var updated = await _userServices.UpdateUser(id, request);
            if (updated is null)
                return BadRequest();

            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}/active")]
        public async Task<IActionResult> ChangeUserActiveStatus(Guid id)
        {
            var changed = await _userServices.ChangeUserActiveStatus(id);
            if (changed is null)
                return BadRequest();

            return Ok(changed);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userServices.DeleteUser(id);
            if (!result)
                return BadRequest();

            return Ok();
        }
    }
}