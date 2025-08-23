using StoreManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    public class CategoryRepository
    {
        LucySalesDataContext _context;
        public CategoryRepository()
        {
            _context = new();
        }
        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }
        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        public void Update(Category category)
        {
            var c = _context.Categories.FirstOrDefault(ct => ct.CategoryId == category.CategoryId);
            if (c != null)
            {
                c.CategoryName = category.CategoryName;
                c.Description = category.Description;
                c.Picture = category.Picture;
                _context.SaveChanges();
            }
        }
        public void Delete(int categoryId)
        {
            var c = _context.Categories.FirstOrDefault(ct => ct.CategoryId == categoryId);
            if (c != null)
            {
                _context.Categories.Remove(c);
                _context.SaveChanges();
            }
        }
    }
}
