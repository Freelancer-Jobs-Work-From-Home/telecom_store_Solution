using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IOrderService : IService<Order>
    {
        int CountOrdersByStatus(string v);
        List<decimal> GetEarningsByMonth();
        decimal GetMonthlyEarnings();
        decimal GetYearlyEarnings();
    }
}
