using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using Contracts.Requests;

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
        public IActionResult CreateUser([FromBody]  UserCreateRequest request)
        {
            var result = _userServices.CreateUser(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateUser([FromQuery] Guid id, [FromBody] UserUpdateDataRequest request)
        {
            var result = _userServices.UpdateUser(id, request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("/ChangeActiveStatus/")]
        public IActionResult ChangeUserActiveStatus([FromQuery] Guid id)
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
