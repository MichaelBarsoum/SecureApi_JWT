using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Secure_Api_Using_JWT.Models;
using Secure_Api_Using_JWT.Services;

namespace Secure_Api_Using_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated) return BadRequest(result.Message);
            return Ok(new
            {
                UserName = result.UserName,
                Email = result.Email,
                Token = result.Token,
                ExpiresOn = result.ExpirationTime
            });               
        }

        [HttpGet("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]TokenRequestModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.LoginAsync(model);
            if (!result.IsAuthenticated) return BadRequest(result.Message);
            return Ok(new
            {
                UserName = result.UserName,
                Email = result.Email,
                Token = result.Token,
                ExpiresOn = result.ExpirationTime
            });
        }
        [Authorize]
        [HttpPost("Role")]
        public async Task<IActionResult> AddRoleAsync(RoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result)) return BadRequest(result);
            return Ok(model);
        }
    }
}
