using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Models
{
    public class Cart
    {
        public Guid CartID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
