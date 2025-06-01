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
    public class CouponService : ICouponService
    {

        private readonly CouponDAO _couponDAO;

        public CouponService(CouponDAO couponDAO)
        {
            _couponDAO = couponDAO;
        }

        public void Add(Coupon entity)
        {
            _couponDAO.Add(entity);
        }

        public void Delete(Guid id)
        {
            _couponDAO?.Delete(id);
        }

        public List<Coupon> GetAll()
        {
            return _couponDAO.GetAll();
        }

        public Coupon GetById(Guid id)
        {
            return (_couponDAO.GetById(id));
        }

        public void Update(Coupon entity)
        {
            _couponDAO.Update(entity);
        }

        public Coupon GetByCode(string code)
        {
            return (_couponDAO.GetByCode(code));
        }
    }
}
