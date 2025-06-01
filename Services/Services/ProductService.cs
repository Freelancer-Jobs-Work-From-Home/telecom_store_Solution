using BussinessObject.Models;
using CsvHelper.Configuration;
using CsvHelper;
using DataAccess.DAO;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace Services.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDAO _ProductDAO;

        public ProductService(ProductDAO ProductDAO)
        {
            _ProductDAO = ProductDAO;
        }
        public void Add(Product entity)
        {
            _ProductDAO.Add(entity);
        }

        public void Delete(Guid id)
        {
            _ProductDAO?.Delete(id);
        }

        public List<Product> GetAll()
        {
            return _ProductDAO.GetAll();
        }

        public Product GetById(Guid id)
        {
            return _ProductDAO.GetById(id);
        }

        public void Update(Product entity)
        {
            _ProductDAO.Update(entity);
        }

        public Product GetByName(string name)
        {
            return _ProductDAO.GetByName(name);
        }

        public void AddRange(List<Product> products)
        {
           _ProductDAO.AddRange(products);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _ProductDAO.BeginTransaction();
        }

        public List<Product> GetProductsByCategory(Guid category)
        {
            return _ProductDAO.GetProductsByCategory(category);
        }

        public bool CanDelete(Guid productID)
        {
            return _ProductDAO.CanDelete(productID);
        }
    }
}
