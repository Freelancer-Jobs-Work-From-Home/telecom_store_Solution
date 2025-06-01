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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly OrderDetailDAO _OrderDetailDAO;

        public OrderDetailService(OrderDetailDAO OrderDetailDAO)
        {
            _OrderDetailDAO = OrderDetailDAO;
        }
        public void Add(OrderDetail entity)
        {
            _OrderDetailDAO.Add(entity);
        }

        public void Delete(Guid id)
        {
            _OrderDetailDAO?.Delete(id);
        }

        public List<OrderDetail> GetAll()
        {
            return _OrderDetailDAO.GetAll();
        }

        public OrderDetail GetById(Guid id)
        {
            return _OrderDetailDAO.GetById(id);
        }

        public void Update(OrderDetail entity)
        {
            _OrderDetailDAO.Update(entity);
        }

        public void AddRange(List<OrderDetail> orderDetails)
        {
            _OrderDetailDAO.AddRange(orderDetails);
        }
    }
}
