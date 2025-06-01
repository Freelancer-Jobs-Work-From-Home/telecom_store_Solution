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
    public class CartService : ICartService
    {
        private readonly CartDAO _cartDAO;

        public CartService(CartDAO cartDAO)
        {
            _cartDAO = cartDAO;
        }
        public void Add(Cart entity)
        {
            _cartDAO.Add(entity);
        }

        public void Delete(Guid id)
        {
            _cartDAO?.Delete(id);
        }

        public List<Cart> GetAll()
        {
            return _cartDAO.GetAll();
        }

        public Cart GetById(Guid id)
        {
            return _cartDAO.GetById(id);
        }

        public void Update(Cart entity)
        {
            _cartDAO.Update(entity);    
        }
    }
}
