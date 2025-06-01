using BussinessObject.Models;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryService(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public void Add(Category entity)
        {
            _categoryDAO.Add(entity);
        }

        public void AddRange(List<Category> newCategories)
        {
            _categoryDAO.AddRange(newCategories);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _categoryDAO.BeginTransaction();
        }

        public bool CanDelete(Guid categoryId)
        {
            return _categoryDAO.CanDelete(categoryId);
        }

        public void Delete(Guid id)
        {
            _categoryDAO?.Delete(id);
        }

        public List<Category> GetAll()
        {
           return _categoryDAO.GetAll();
        }

        public Category GetById(Guid id)
        {
            return _categoryDAO.GetById(id);
        }

        public Category GetByName(string categoryName)
        {
            return _categoryDAO.GetByName(categoryName);
        }

        public void Update(Category entity)
        {
           _categoryDAO.Update(entity);
        }
    }
}
