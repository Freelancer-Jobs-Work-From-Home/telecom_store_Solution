using BussinessObject.Data;
using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDAO : BaseDAO<Order>
    {
        public OrderDAO(AppDbContext context) : base(context) { }

        public int CountOrdersByStatus(string status)
        {
            return _context.Orders.Count(o => o.Status == status);
        }

        public List<decimal> GetEarningsByMonth()
        {
            var earningsByMonth = Enumerable.Range(1, 12)
                .Select(month => _context.Orders
                    .Where(o => o.CreatedAt.Month == month
                                && o.CreatedAt.Year == DateTime.Now.Year
                                && o.Status == "Completed") // Chỉ lấy đơn hàng đã hoàn thành
                    .Sum(o => (decimal?)o.TotalPrice) ?? 0) // Xử lý nếu không có dữ liệu
                .ToList();

            return earningsByMonth;
        }

        public decimal GetMonthlyEarnings()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return _context.Orders
                .Where(o => o.Status == "Completed" && o.CreatedAt.Month == currentMonth && o.CreatedAt.Year == currentYear)
                .Sum(o => o.TotalPrice);
        }

        public decimal GetYearlyEarnings()
        {
            var currentYear = DateTime.Now.Year;

            return _context.Orders
                .Where(o => o.Status == "Completed" && o.CreatedAt.Year == currentYear)
                .Sum(o => o.TotalPrice);

        }
    }
}
