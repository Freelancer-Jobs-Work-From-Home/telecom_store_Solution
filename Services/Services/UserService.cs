using BussinessObject.Models;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserDAO _userDAO;

        public UserService(UserDAO userDAO)
        {
            _userDAO = userDAO;
        }
        

        public void Delete(Guid id)
        {
            _userDAO.Delete(id);
        }

        public List<User> GetAll()
        {
            return _userDAO.GetAll();
        }

        public User GetById(Guid id)
        {
            return _userDAO.GetById(id);
        }

        public void Update(User entity)
        {
            _userDAO.Update(entity);
        }

        public void Add(User entity)
        {
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);
            _userDAO.Add(entity);
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        public User GetByEmail(string email)
        {
            return _userDAO.GetByEmail(email);
        }

        public async Task<bool> ChangePassword(User user, string currentPassword, string newPassword)
        {
            
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false; 
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _userDAO.Update(user);
            return true;
        }

        public bool CanDelete(Guid id)
        {
            return _userDAO.CanDelete(id);
        }
    }
}
