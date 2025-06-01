using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Models
{
    public class Product
    {
        public Guid ProductID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryID { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
