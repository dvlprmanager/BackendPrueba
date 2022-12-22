using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PruebaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly string _user;
        private readonly string _pass;
        
        public TokenController(IConfiguration configuration) 
        {
            _configuration = configuration;
            _user = _configuration.GetValue<string>("CredencialesApi:user");
            _pass = _configuration.GetValue<string>("CredencialesApi:password");
        }

        [HttpGet]
        [Route("Token")]
        public async Task<IActionResult> Token(string user, string pass)
        {
            if (!(user == _user) || !(pass == _pass))
            {
                return BadRequest("credenciales invalidas");
            }

            string jwtToken = GenerateToken(user, pass);

            return Ok(new { token = jwtToken});
        }

        private string GenerateToken(string user, string pass)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Email, "test@password.com")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var securityToken = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: creds
                            );
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;

        }
    }
}
