using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendAmatista.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using backendAmatista.Models;
using backend_amatista.Models;

namespace BackendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DbamatistaContext _dbamatistaContext;
        private readonly IConfiguration _config;
        public LoginController(DbamatistaContext dbamatistaContext, IConfiguration config)
        {
            _dbamatistaContext = dbamatistaContext;
            _config = config;

        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            var user = await Authenticate(userLogin);
            if (user != null)
            {
                // Generate token if the user is authenticated
                var token = GenerateToken(user);
                return Ok(new { token, userLogin });
            }
            return BadRequest("User or password incorrect");
        }

        private async Task<User> Authenticate(UserLogin userLogin)
        {
            var currentUser = await _dbamatistaContext.Users
                .FirstOrDefaultAsync(u => u.UserName == userLogin.UserName && u.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }
            return null!;
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // create the claims
            var claims = new[]
            {

                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)

            };


            // Create token

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(365),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}