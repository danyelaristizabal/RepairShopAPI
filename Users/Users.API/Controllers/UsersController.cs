using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepairShop.Common.Helpers;
using Users.Contracts.Requests;
using Users.Domain.Exceptions;
using Users.Domain.Services;
using Users.Mapping;

namespace Users.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService user, ILogger<UsersController> logger)
        {
            _userService = user;
            _logger = logger;
        }

        [HttpPost("users")]
        public async Task<IActionResult> Create([FromBody] UserRequest request)
        {
            try
            {
                var user = request.ToUser();

                if (await _userService.CreateAsync(user))
                {
                    var userResponse = user.ToUserResponse();

                    return CreatedAtAction("Get", new { userResponse.Id }, userResponse);
                }

                return StatusCode(500, "An error occurred while creating the user.");
            }
            catch (UserAlreadyExistsException)
            {
                return Conflict("A user with this email already exists.");
            }
            catch (EmptyEmailException)
            {
                return BadRequest("Email cannot be empty.");
            }
        }

        [HttpGet("users/{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var user = await _userService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            var userResponse = user.ToUserResponse();
            return Ok(userResponse);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var usersResponse = users.ToCustomersResponse();
            return Ok(usersResponse);
        }

        [HttpPut("users/{id:guid}")]
        public async Task<IActionResult> Update([FromMultiSource] UpdateUserRequest request)
        {
            try
            {
                var existingCustomer = await _userService.GetAsync(request.Id);

                if (existingCustomer is null)
                {
                    return NotFound();
                }

                var user = request.ToUser();
                if (await _userService.UpdateAsync(user))
                {
                    var userResponse = user.ToUserResponse();
                    return Ok(userResponse);
                }
                else
                {
                    return StatusCode(500, "An error occurred while updating the user.");
                }
            }
            catch (EmptyEmailException)
            {
                return BadRequest("Email cannot be empty.");
            }
        }

        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
