using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using Contracts.Requests;
using Microsoft.AspNetCore.Authorization;

namespace SmartWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userServices.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("/UserById/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
        {
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
            var user = await _userServices.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

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

        [HttpPost("/CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody]  UserCreateRequest request)
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
            var result = await _userServices.UpdateUser(id, request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

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
            var result = await _userServices.DeleteUser(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
