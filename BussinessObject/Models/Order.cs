using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Models
{
    public class Order
    {
        public Guid OrderID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; }
        public decimal TotalPrice { get; set; }
        public string DiscountCode { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
