using BussinessObject.Data;
using BussinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CategoryDAO : BaseDAO<Category>
    {
        public CategoryDAO(AppDbContext context) : base(context) { }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public bool CanDelete(Guid id) => !_context.Products.Any(r => r.CategoryID == id);

        public Category GetByName(string categoryName)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
        }
    }
}
