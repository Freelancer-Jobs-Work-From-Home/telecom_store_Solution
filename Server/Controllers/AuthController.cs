using BussinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.AuthVM;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
            if (!success)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

            var user = _userService.GetByEmail(model.Email);
            return Ok(new
            {
                message = "Đăng nhập thành công",
                user = new
                {
                    user.UserID,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserID = Guid.NewGuid(),
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.Password,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };

            var success = await _authService.RegisterAsync(user);
            if (!success)
                return Conflict(new { message = "Email đã tồn tại." });

            return Ok(new { message = "Đăng ký thành công" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Đăng xuất thành công" });
        }

    }
}
