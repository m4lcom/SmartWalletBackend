using SmartWallet.Application.Services;
using Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        private Guid? GetUserIdFromToken()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var id)) return id;
            return null;
        }

        private string? GetUserEmailFromToken()
        {
            return User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                   ?? User.FindFirst(ClaimTypes.Email)?.Value;
        }

        private bool IsAdmin()
        {
            if (User.IsInRole("Admin")) return true;
            var roleClaim = User.FindFirst("role")?.Value;
            return string.Equals(roleClaim, "Admin", StringComparison.OrdinalIgnoreCase);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userServices.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("/UserById/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
        {
            var tokenUserId = GetUserIdFromToken();

            if (!IsAdmin() && tokenUserId != userId)
            {
                return Forbid();
            }

            var user = await _userServices.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("/UserByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            var tokenEmail = GetUserEmailFromToken();

            if (!IsAdmin() && !string.Equals(tokenEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var user = await _userServices.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [AllowAnonymous] 
        [HttpPost("/RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserCreateRequest request)
        {
            var result = await _userServices.RegisterUser(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("/CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {

            var result = await _userServices.CreateUser(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDataRequest request)
        {
            var tokenUserId = GetUserIdFromToken();

            if (tokenUserId != id)
            {
                return Forbid();
            }

            var result = await _userServices.UpdateUser(id, request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("/ChangeActiveStatus/{id}")]
        public async Task<IActionResult> ChangeUserActiveStatus([FromRoute] Guid id)
        {

            var result = await _userServices.ChangeUserActiveStatus(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            var tokenUserId = GetUserIdFromToken();

            if (!IsAdmin() && tokenUserId != id)
            {
                return Forbid();
            }

            var result = await _userServices.DeleteUser(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}