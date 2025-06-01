using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IOrderDetailService : IService<OrderDetail>
    {
        void AddRange(List<OrderDetail> orderDetails);
    }
}
