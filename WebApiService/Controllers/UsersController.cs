using Mapster;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApiService.Dto;
using WebApiService.Services;

namespace WebApiService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private UsersService _service;
        public UsersController(UsersService service) : base()
        {
            _service = service;
        }

        private string GetCurrentUser()
        {
            return "111";
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateDto user)
        {
            var creator = GetCurrentUser();
            var res = await _service.AddUserAsync(user.Adapt<User>(), creator);
            if (res is null)
            {
                return BadRequest();
            }
            return Created();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDto user)
        {
            var updater = GetCurrentUser();
            var res = await _service.UpdateUserDataAsync(id, updater, name: user.Name, gender: user.Gender, birthday: user.Birthday);
            if (res is null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{id}/password")]
        public async Task<ActionResult<User>> ChangeUserPassword([FromRoute] Guid id, [FromBody] string newPwd)
        {
            var updater = GetCurrentUser();
            var res = await _service.UpdatePasswordAsync(id, newPwd, updater);
            if (res is null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{id}/login")]
        public async Task<ActionResult<User>> ChangeUserLogin([FromRoute] Guid id, [FromBody] string newLogin)
        {
            var updater = GetCurrentUser();
            var res = await _service.UpdateLoginAsync(id, newLogin, updater);
            if (res is null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<User>>> GetActiveList()
        {
            return Ok(await _service.GetActiveUsersAsync());
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<ICollection<User>>> GetUserByLogin([FromRoute] string login)
        {
            var res = await _service.GetUserAsync(login);
            if (res is null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost("{login}/auth")]
        public async Task<ActionResult<ICollection<User>>> AuthUser([FromRoute] string login, [FromBody] string password)
        {
            var res = await _service.GetUserWithPasswordAsync(login, password);
            if (res is null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpGet("elder")]
        public async Task<ActionResult<ICollection<User>>> GetElderThan([FromQuery] int age)
        {
            return Ok(await _service.GetElderUsersAsync(age));
        }

        [HttpDelete("{login}")]
        public async Task<ActionResult<User>> DeleteUser([FromRoute] string login, [FromQuery] bool isSoft = true)
        {
            User? res;
            if (isSoft)
            {
                var deleter = GetCurrentUser();
                res = await _service.DeleteUserSoftAsync(login, deleter);
            }
            else
            {
                res = await _service.DeleteUserHardAsync(login);
            }
            if (res is null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost("{login}")]
        public async Task<ActionResult<User>> RestoreUser([FromRoute] string login)
        {
            var res = await _service.RestoreUserAsync(login);
            if (res is null)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
