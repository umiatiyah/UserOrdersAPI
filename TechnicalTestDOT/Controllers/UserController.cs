using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TechnicalTestDOT.Contracts;
using TechnicalTestDOT.Models;
using TechnicalTestDOT.Payloads.Request;
using TechnicalTestDOT.Payloads.Response;

namespace TechnicalTestDOT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<CommonResponse>> GetUsers()
        {
            var data = await _userRepository.GetUsers();
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else if (data.StatusCode == 204)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(data);
            }
        }
        [HttpGet("{username}")]
        public async Task<ActionResult<CommonResponse>> GetUsers(string username)
        {
            var data = await _userRepository.GetUser(username);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else if (data.StatusCode == 404)
            {
                return NotFound(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
        [HttpPost]
        public async Task<ActionResult<CommonResponse>> CreateUser(UserRequest user)
        {
            var data = await _userRepository.CreateUser(user);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CommonResponse>> UpdateUser(int id, UserRequest user)
        {
            var data = await _userRepository.UpdateUser(id, user);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else if (data.StatusCode == 404)
            {
                return NotFound(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
        [HttpDelete("{username}")]
        public async Task<ActionResult<CommonResponse>> DeleteUser(string username)
        {
            var data = await _userRepository.DeleteUser(username);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else if (data.StatusCode == 404)
            {
                return NotFound(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
    }
}
