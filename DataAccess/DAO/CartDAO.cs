using BussinessObject.Data;
using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CartDAO : BaseDAO<Cart>
    {
        public CartDAO(AppDbContext context) : base(context) { }
    }
}
