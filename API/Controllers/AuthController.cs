using API.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet("verify")]
        public async Task<ActionResult> Verify()
        {
            return Ok(new
            {
                isAuthenticated = true
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.email, Email = model.email };
            var result = await _userManager.CreateAsync(user, model.password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.email, model.password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user is null)
                {
                    return BadRequest("The email or password field are invalid, please verify the information.");
                }
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var JwtSecrets = new JwtSecrets();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSecrets.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtSecrets.Issuer,
                audience: JwtSecrets.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
