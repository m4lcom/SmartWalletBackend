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

        [HttpGet()]
        public IActionResult GetAllUsers()
        {
            var users = _userServices.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("/UserById/{userId}")]
        public IActionResult GetUserById([FromRoute] Guid userId)
        {
            var user = _userServices.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet("/UserByEmail/{email}")]
        public IActionResult GetUserByEmail([FromRoute] string email)
        {
            var user = _userServices.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody]  UserCreateRequest request)
        {
            var result = _userServices.CreateUser(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDataRequest request)
        {
            var result = _userServices.UpdateUser(id, request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("/ChangeActiveStatus/{id}")]
        public IActionResult ChangeUserActiveStatus([FromRoute] Guid id)
        {
            var result = _userServices.ChangeUserActiveStatus(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] Guid id)
        {
            var result = _userServices.DeleteUser(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
