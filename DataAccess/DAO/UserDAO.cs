using BussinessObject.Data;
using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UserDAO : BaseDAO<User>
    {
        public UserDAO(AppDbContext context) : base(context) { }

        public bool CanDelete(Guid id)
        {
            if (_context.Reviews.Any(r => r.UserID == id)||
                _context.Carts.Any(c =>c.UserID == id) ||
                _context.Orders.Any(o => o.UserID == id) ||
                _context.Feedbacks.Any(f => f.UserID == id)
                )
            {
                return false;
            }
            return true;
        }

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
