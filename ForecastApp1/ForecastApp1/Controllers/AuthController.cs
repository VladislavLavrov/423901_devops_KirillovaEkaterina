using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Security.Claims;

namespace ForecastApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Func<SqliteConnection> _connectionFactory;

        public AuthController(Func<SqliteConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            using var conn = _connectionFactory();
            conn.Open();
            var user = await conn.QuerySingleOrDefaultAsync<dynamic>(
                "SELECT * FROM users WHERE username = @u", new { u = username });

            if (user == null) return Unauthorized("Пользователь не найден");

            if (user.password_hash != password) return Unauthorized("Неверный пароль");

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, (string)user.username),
            new Claim(ClaimTypes.Role, (string)user.role)
        };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return Ok(new { username = user.username, role = user.role });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Ok("Вы вышли");
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();
            return Ok(new
            {
                username = User.Identity?.Name,
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
