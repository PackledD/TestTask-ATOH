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

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateDto user)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDto user)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}/password")]
        public async Task<ActionResult<User>> ChangeUserPassword([FromRoute] Guid id, [FromBody] string newPwd)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}/login")]
        public async Task<ActionResult<User>> ChangeUserLogin([FromRoute] Guid id, [FromBody] string newLogin)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<User>>> GetActiveList()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<ICollection<User>>> GetUserByLogin([FromRoute] string login)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{login}/auth")]
        public async Task<ActionResult<ICollection<User>>> AuthUser([FromRoute] string login, [FromBody] string password)
        {
            throw new NotImplementedException();
        }

        [HttpGet("elder")]
        public async Task<ActionResult<ICollection<User>>> GetElderThan([FromQuery] int age)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{login}")]
        public async Task<ActionResult<User>> DeleteUser([FromRoute] string login, [FromQuery] bool isSoft = true)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<User>> RestoreUser([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
