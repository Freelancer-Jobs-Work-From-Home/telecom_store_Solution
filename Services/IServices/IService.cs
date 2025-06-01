using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IService<T> where T : class
    {
        void Add(T entity);
        T GetById(Guid id);
        List<T> GetAll();
        void Update(T entity);
        void Delete(Guid id);
    }
}
