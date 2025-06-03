using BussinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IProductService : IService<Product>
    {
        /*void ImportFromCsv(string filePath);

        void ExportToCsv(string filePath);*/

        Product GetByName(string name); 
        void AddRange(List<Product> products); 
        IDbContextTransaction BeginTransaction();
        List<Product> GetProductsByCategory(Guid category);
        bool CanDelete(Guid productID);
        void Add(Product product);
    }
}
