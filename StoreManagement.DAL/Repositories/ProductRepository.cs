using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    public class ProductRepository
    {
        LucySalesDataContext _context;
        public ProductRepository()
        {
            _context = new();
        }
        public List<Product> GetAll()
        {
            return _context.Products.Include(o => o.Category).ToList();
        }
        public List<Product> SearchProductByNameAndCategory(string name, string categoryName)
        {
            List<Product> products = new List<Product>();
            //Name only
            if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(categoryName))
            {
                products = _context.Products
                    .Include(o => o.Category)
                    .Where(p => p.ProductName.Contains(name))
                    .ToList();
            }
            //Category only
            else if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(categoryName))
            {
                products = _context.Products
                    .Include(o => o.Category)
                    .Where(p => p.Category != null && p.Category.CategoryName.ToLower().Contains(categoryName.ToLower()))
                    .ToList();
            }
            //Both
            else if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(categoryName))
            {
                products = _context.Products
                    .Include(o => o.Category)
                    .Where(p => p.ProductName.ToLower().Contains(name.ToLower()) && p.Category != null && p.Category.CategoryName.ToLower().Contains(categoryName.ToLower()))
                    .ToList();
            }
            return products;
        }
        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            var p = _context.Products.FirstOrDefault(pr => pr.ProductId == product.ProductId);
            if (p != null)
            {
                p.ProductName = product.ProductName;
                p.CategoryId = product.CategoryId;
                p.QuantityPerUnit = product.QuantityPerUnit;
                p.UnitPrice = product.UnitPrice;
                p.UnitsInStock = product.UnitsInStock;
                p.UnitsOnOrder = product.UnitsOnOrder;
                p.ReorderLevel = product.ReorderLevel;
                p.Discontinued = product.Discontinued;
                _context.SaveChanges();
            }
        }
        public bool Delete(int productId)
        {
            try
            {
                var p = _context.Products.FirstOrDefault(pr => pr.ProductId == productId);
                if (p != null)
                {
                    _context.Products.Remove(p);
                    _context.SaveChanges();
                    return true;
                }
            } catch (DbUpdateException ex)
            {
                return false;
            }
            return false;
        }
    }
}
