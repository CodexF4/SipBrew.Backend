using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SipBrew.Core;
using SipBrew.Core.DTO;
using SipBrew.Core.Models;
using SipBrew.Core.Services;

namespace SipBrew.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly JWTService _jwtService;

        public AuthController(AppDbContext ctx, JWTService jwtService) 
        {
            _ctx = ctx;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var admin = await _ctx.Admins.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (admin == null)
                return Unauthorized(new { message = "Invalid email or password" });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _jwtService.GenerateToken(admin);
            return Ok(new LoginResponseDTO{ Token = token });
        }

    }
}
