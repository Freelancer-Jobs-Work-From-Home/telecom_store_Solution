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
    public class ProductDAO : BaseDAO<Product>
    {
        public ProductDAO(AppDbContext context) : base(context) { }

        public void AddRange(List<Product> products)
        {
            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public bool CanDelete(Guid id)
        {
            if (_context.Reviews.Any(r => r.ProductID == id) ||
                _context.Carts.Any(c => c.ProductID == id) ||
                _context.OrderDetails.Any(o => o.ProductID == id) 
                )
            {
                return false;
            }
            return true;
        }

        public Product GetByName(string name)
        {
            return _context.Products.FirstOrDefault(p => p.Name == name);
        }

        public List<Product> GetProductsByCategory(Guid category)
        {
            return _context.Products.Where(p => p.CategoryID == category).ToList();
        }
    }
}
