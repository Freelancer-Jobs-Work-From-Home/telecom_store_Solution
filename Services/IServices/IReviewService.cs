using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IReviewService : IService<Review>
    {
        IEnumerable<Review> GetReviewsByProductId(Guid productID);
        IEnumerable<Review> GetReviewsByUserId(Guid userID);
    }
}
