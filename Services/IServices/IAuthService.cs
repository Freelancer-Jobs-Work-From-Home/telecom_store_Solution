using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IAuthService
    {
        Task LoginAfterChangePassAsync(User user, bool isPersistent);
        Task<bool> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<bool> RegisterAsync(User user);
    }
}
