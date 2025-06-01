using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Models
{
    public class Category
    {
        public Guid CategoryID { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
