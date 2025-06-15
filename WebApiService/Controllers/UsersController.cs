using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiService.Dto;
using WebApiService.Services;

namespace WebApiService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private UsersService _service;
        private string _jwtKey;
        public UsersController(UsersService service) : base()
        {
            _service = service;
            _jwtKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<string>("JwtKey");
        }

        private string GetCurrentUser()
        {
            return User?.Identity?.Name ?? string.Empty;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
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
        [Authorize]
        public async Task<ActionResult<User>> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDto user)
        {
            if (!await _AllowToUpdate(id))
            {
                return Forbid();
            }
            var updater = GetCurrentUser();
            var res = await _service.UpdateUserDataAsync(id, updater, name: user.Name, gender: user.Gender, birthday: user.Birthday);
            if (res is null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{id}/password")]
        [Authorize]
        public async Task<ActionResult<User>> ChangeUserPassword([FromRoute] Guid id, [FromBody] string newPwd)
        {
            if (!await _AllowToUpdate(id))
            {
                return Forbid();
            }
            var updater = GetCurrentUser();
            var res = await _service.UpdatePasswordAsync(id, newPwd, updater);
            if (res is null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{id}/login")]
        [Authorize]
        public async Task<ActionResult<User>> ChangeUserLogin([FromRoute] Guid id, [FromBody] string newLogin)
        {
            if (!await _AllowToUpdate(id))
            {
                return Forbid();
            }
            var updater = GetCurrentUser();
            var res = await _service.UpdateLoginAsync(id, newLogin, updater);
            if (res is null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ICollection<User>>> GetActiveList()
        {
            return Ok(await _service.GetActiveUsersAsync());
        }

        [HttpGet("{login}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<User>> GetUserByLogin([FromRoute] string login)
        {
            var res = await _service.GetUserAsync(login);
            if (res is null)
            {
                return NotFound();
            }
            return Ok(new { res.Name, res.Gender, res.Birthday, isActive = !res.RevokedOn.HasValue });
        }

        [HttpPost("{login}/auth")]
        public async Task<ActionResult<User>> AuthUser([FromRoute] string login, [FromBody] string password)
        {
            var res = await _service.GetUserWithPasswordAsync(login, password);
            if (res is null)
            {
                return NotFound();
            }
            var token = _GenerateJwtToken(res);
            _SetCookie(token);
            return Ok(res);
        }

        [HttpGet("elder")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ICollection<User>>> GetElderThan([FromQuery] int age)
        {
            return Ok(await _service.GetElderUsersAsync(age));
        }

        [HttpDelete("{login}")]
        [Authorize(Policy = "AdminOnly")]
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
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<User>> RestoreUser([FromRoute] string login)
        {
            var res = await _service.RestoreUserAsync(login);
            if (res is null)
            {
                return NotFound();
            }
            return Ok();
        }

        private string _GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtKey)
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
                    new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "Default")
                };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Issuer = "UsersService",
                Audience = "UsersService"
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void _SetCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddYears(1)
            };
            Response.Cookies.Append("jwtToken", token, cookieOptions);
        }

        private async Task<bool> _AllowToUpdate(Guid guid)
        {
            
            return User.IsInRole("Admin") ||
                   (User.HasClaim(ClaimTypes.NameIdentifier, guid.ToString()) &&
                   await _service.IsActive(guid));
        }
    }
}
