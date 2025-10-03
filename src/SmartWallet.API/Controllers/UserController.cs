using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;

namespace SmartWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("/UserById/")]
        public IActionResult GetUserById([FromQuery] Guid userId)
        {
            var user = _userServices.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet]
        public IActionResult GetUserByEmail([FromQuery] string email)
        {
            var user = _userServices.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] SmartWallet.Contracts.Requests.UserCreateRequest request)
        {
            var result = _userServices.CreateUser(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateUser([FromQuery] string email, [FromBody] SmartWallet.Contracts.Requests.UserUpdateDataRequest request)
        {
            var result = _userServices.UpdateUser(email, request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] string email)
        {
            var result = _userServices.DeleteUser(email);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
