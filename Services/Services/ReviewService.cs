using BussinessObject.Models;
using DataAccess.DAO;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ReviewDAO _ReviewDAO;

        public ReviewService(ReviewDAO ReviewDAO)
        {
            _ReviewDAO = ReviewDAO;
        }
        public void Add(Review entity)
        {
            _ReviewDAO.Add(entity);
        }

        public void Delete(Guid id)
        {
            _ReviewDAO?.Delete(id);
        }

        public List<Review> GetAll()
        {
            return _ReviewDAO.GetAll();
        }

        public Review GetById(Guid id)
        {
            return _ReviewDAO.GetById(id);
        }

        public IEnumerable<Review> GetReviewsByProductId(Guid productID)
        {
            return _ReviewDAO.GetReviewsByProductId(productID);
        }

        public IEnumerable<Review> GetReviewsByUserId(Guid userID)
        {
            return _ReviewDAO.GetReviewsByUserId(userID);
        }

        public void Update(Review entity)
        {
            _ReviewDAO.Update(entity);
        }
    }
}
