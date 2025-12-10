using EcommerceWebAPI.Interfaces.IService;
using EcommerceWebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebAPI.Controllers
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
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(Register dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.RegisterAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(Login dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(result);
        }
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var result = await _authService.AssignRoleAsync(userId, role);
            return result.Success ? Ok(result) : NotFound(result);
        }
        [HttpPost("signout")]
        public async Task<IActionResult> SignOutAsync(string userId)
        {
            var result = await _authService.SignOut(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
