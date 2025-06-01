using BussinessObject.Data;
using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ReviewDAO : BaseDAO<Review>
    {
        public ReviewDAO(AppDbContext context) : base(context) { }

        public IEnumerable<Review> GetReviewsByProductId(Guid productID)
        {
            return _context.Reviews.Where(r => r.ProductID == productID).OrderByDescending(r => r.CreatedAt).ToList();
        }

        public IEnumerable<Review> GetReviewsByUserId(Guid userID)
        {
            return _context.Reviews.Where(r => r.ProductID == userID).OrderByDescending(r => r.CreatedAt).ToList();
        }
    }
}
