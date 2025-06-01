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
    public class OrderService : IOrderService
    {
        private readonly OrderDAO _OrderDAO;

        public OrderService(OrderDAO OrderDAO)
        {
            _OrderDAO = OrderDAO;
        }
        public void Add(Order entity)
        {
            _OrderDAO.Add(entity);
        }

        public int CountOrdersByStatus(string v)
        {
            return _OrderDAO.CountOrdersByStatus(v);
        }

        public void Delete(Guid id)
        {
            _OrderDAO?.Delete(id);
        }

        public List<Order> GetAll()
        {
            return _OrderDAO.GetAll();
        }

        public Order GetById(Guid id)
        {
            return _OrderDAO.GetById(id);
        }

        public List<decimal> GetEarningsByMonth()
        {
            return _OrderDAO.GetEarningsByMonth();
        }

        public decimal GetMonthlyEarnings()
        {
            return _OrderDAO.GetMonthlyEarnings();
        }

        public decimal GetYearlyEarnings()
        {
            return _OrderDAO.GetYearlyEarnings();
        }

        public void Update(Order entity)
        {
            _OrderDAO.Update(entity);
        }
    }
}
