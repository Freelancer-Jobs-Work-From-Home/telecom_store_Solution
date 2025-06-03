using BussinessObject.Models;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _environment;

        public UserController(IUserService userService, IWebHostEnvironment environment)
        {
            _userService = userService;
            _environment = environment;
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

        [HttpGet("{id}")]
        public IActionResult GetDetail(Guid id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.UserID,
                user.FullName,
                user.Email,
                user.Address,
                user.Avatar,
                user.DateOfBirth,
                user.Role
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_userService.GetByEmail(model.Email) != null)
                return Conflict(new { message = "Email đã tồn tại" });

            string avatarRelativePath = "";

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var projectRoot = Directory.GetCurrentDirectory();
                var relativeFolder = Path.Combine("Upload", "images", "users");
                var fullSavePath = Path.Combine(projectRoot, relativeFolder);

                if (!Directory.Exists(fullSavePath))
                    Directory.CreateDirectory(fullSavePath);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ImageFile.FileName)}";
                var fullFilePath = Path.Combine(fullSavePath, uniqueFileName);

                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                avatarRelativePath = Path.Combine(relativeFolder, uniqueFileName).Replace("\\", "/");
            }

            var user = new User
            {
                UserID = Guid.NewGuid(),
                FullName = model.FullName,
                Email = model.Email,
                Address = model.Address,
                Avatar = avatarRelativePath,
                PasswordHash = model.Password,
                DateOfBirth = model.DateOfBirth,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow
            };

            _userService.Add(user);

            return Ok(new { message = "Tạo người dùng thành công", user });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UserViewModel model)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Role = model.Role;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }
            // Nếu rỗng => không đổi mật khẩu

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var projectRoot = Directory.GetCurrentDirectory();
                var relativeFolder = Path.Combine("Upload", "images", "users");
                var fullSavePath = Path.Combine(projectRoot, relativeFolder);

                if (!Directory.Exists(fullSavePath))
                    Directory.CreateDirectory(fullSavePath);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ImageFile.FileName)}";
                var fullFilePath = Path.Combine(fullSavePath, uniqueFileName);

                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                user.Avatar = Path.Combine(relativeFolder, uniqueFileName).Replace("\\", "/");
            }

            _userService.Update(user);

            return Ok(new { message = "Cập nhật người dùng thành công", user });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            if (!_userService.CanDelete(id))
                return BadRequest(new { message = "Không thể xoá người dùng này" });

            _userService.Delete(user.UserID);

            return Ok(new { message = "Xoá người dùng thành công" });
        }

        [HttpGet("Search")]
        public IActionResult Search([FromQuery] string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống" });
            }

            var users = _userService.GetAll()
                .Where(u =>
                    (!string.IsNullOrEmpty(u.FullName) && u.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.Role) && u.Role.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                )
                .Select(user => new
                {
                    user.UserID,
                    user.FullName,
                    user.Email,
                    user.Address,
                    user.Avatar,
                    user.DateOfBirth,
                    user.Role,
                    CanDelete = _userService.CanDelete(user.UserID)
                })
                .ToList();

            return Ok(users);
        }

    }
}
