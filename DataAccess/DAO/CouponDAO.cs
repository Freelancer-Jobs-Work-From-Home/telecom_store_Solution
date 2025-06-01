using BussinessObject.Data;
using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CouponDAO : BaseDAO<Coupon>
    {
        public CouponDAO(AppDbContext context) : base(context) { }

        public Coupon GetByCode(string code)
        {
            return _context.Coupons.FirstOrDefault(c => c.Code == code);
        }
    }
}
