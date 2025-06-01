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
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackDAO _FeedbackDAO;

        public FeedbackService(FeedbackDAO FeedbackDAO)
        {
            _FeedbackDAO = FeedbackDAO;
        }
        public void Add(Feedback entity)
        {
            _FeedbackDAO.Add(entity);
        }

        public void Delete(Guid id)
        {
            _FeedbackDAO?.Delete(id);
        }

        public List<Feedback> GetAll()
        {
            return _FeedbackDAO.GetAll();
        }

        public Feedback GetById(Guid id)
        {
            return _FeedbackDAO.GetById(id);
        }

        public void Update(Feedback entity)
        {
            _FeedbackDAO.Update(entity);
        }
    }
}
