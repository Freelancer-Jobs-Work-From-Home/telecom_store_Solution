using BussinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICategoryService : IService<Category>
    {
        void AddRange(List<Category> newCategories);
        IDbContextTransaction BeginTransaction();
        bool CanDelete(Guid categoryId);
        Category GetByName(string categoryName);
    }
}
