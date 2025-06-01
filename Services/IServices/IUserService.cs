using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IUserService : IService<User> 
    {
        bool CanDelete(Guid id);
        Task<bool> ChangePassword(User user, string currentPassword, string newPassword);
        User GetByEmail(string email);
        bool VerifyPassword(string enteredPassword, string storedHash);
    }
}
