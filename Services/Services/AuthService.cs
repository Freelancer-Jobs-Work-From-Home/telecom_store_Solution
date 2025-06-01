    using BussinessObject.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Authentication;
    using Services.IServices;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    namespace Services.Services
    {
        public class AuthService : IAuthService
        {
            private readonly IUserService _userService;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public AuthService(IUserService userService, IHttpContextAccessor httpContextAccessor)
            {
                _userService = userService;
                _httpContextAccessor = httpContextAccessor;
            }

        public async Task LoginAfterChangePassAsync(User user, bool isPersistent)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                        new Claim("FullName", user.FullName),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }


        public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
            {
                var user = _userService.GetByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // Kiểm tra mật khẩu đúng cách
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = rememberMe, // Lưu đăng nhập
                    ExpiresUtc = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1) // Thời hạn cookie
                };

                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return true;
            }

            public async Task LogoutAsync()
            {
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            public async Task<bool> RegisterAsync(User user)
            {
            if (_userService.GetByEmail(user.Email) != null)
                return false;
                user.Address = "";
                user.Avatar = "";
                user.DateOfBirth = DateTime.UtcNow;
                _userService.Add(user);
                return true;
            }

        }
    }
