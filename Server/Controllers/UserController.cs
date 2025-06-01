using BussinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.User;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll().Select(user => new {
                user.UserID,
                user.FullName,
                user.Email,
                user.Address,
                user.Avatar,
                user.DateOfBirth,
                user.Role,
                CanDelete = _userService.CanDelete(user.UserID)
            });

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (_userService.GetByEmail(model.Email) != null)
                return Conflict(new { message = "Email đã tồn tại" });

            /*string avatarUrl = await SaveImageAsync(model.ImageFile);*/

            var user = new User
            {
                UserID = Guid.NewGuid(),
                FullName = model.FullName,
                Email = model.Email,
                Address = model.Address,
                Avatar = "",
                PasswordHash = model.Password,
                DateOfBirth = model.DateOfBirth,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow
            };

            _userService.Add(user);
            return Ok(new { message = "Tạo người dùng thành công", user });
        }


    }
}
