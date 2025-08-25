using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    public class EmployeeManagementRepository
    {
        LucySalesDataContext _context;
        public EmployeeManagementRepository()
        {
            _context = new();
        }
        public List<Employee> GetAll()
        {
            return _context.Employees.ToList();
        }
        public Employee GetEmployeeByUserNameAndPassword(string userName, string password)
        {
            return _context.Employees.FirstOrDefault(e => e.UserName == userName && e.Password == password);
        }
        public List<Employee> SearchByName(string name)
        {
            List<Employee> employees = new List<Employee>();
            if (!string.IsNullOrEmpty(name))
            {
                employees = _context.Employees
                    .Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            return employees;
        }
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }
        public void Update(Employee employee)
        {
            var existingEmployee = _context.Employees.Find(employee.EmployeeId);
            if (existingEmployee != null)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.UserName = employee.UserName;
                existingEmployee.Password = employee.Password;
                existingEmployee.JobTitle = employee.JobTitle;
                existingEmployee.BirthDate = employee.BirthDate;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.Address = employee.Address;
                existingEmployee.RoleNum = employee.RoleNum;
                _context.SaveChanges();
            }
        }
        public bool Delete(int employeeId)
        {
            try
            {
                var employee = _context.Employees.Find(employeeId);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        // 1. QUẢN LÝ ORDERS (Employee tạo đơn hàng)
        public List<Order> GetOrdersByEmployeeId(int employeeId)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.EmployeeId == employeeId)
                .ToList();
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            var existingOrder = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == order.OrderId);

            if (existingOrder != null)
            {
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.RequiredDate = order.RequiredDate;
                existingOrder.ShippedDate = order.ShippedDate;
                existingOrder.Freight = order.Freight;
                existingOrder.ShipAddress = order.ShipAddress;

                // Update order details
                _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
                existingOrder.OrderDetails = order.OrderDetails;

                _context.SaveChanges();
            }
        }

        public bool DeleteOrder(int orderId, int employeeId)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.OrderId == orderId && o.EmployeeId == employeeId);

                if (order != null)
                {
                    _context.OrderDetails.RemoveRange(order.OrderDetails);
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return false;
        }

        // 2. XEM PRODUCTS VÀ CATEGORIES
        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => !p.Discontinued) // Chỉ lấy sản phẩm còn bán
                .ToList();
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && !p.Discontinued)
                .ToList();
        }

        public Product GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == productId);
        }

        public List<Product> SearchProductsByName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                return new List<Product>();

            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.ToLower().Contains(productName.ToLower()) &&
                           !p.Discontinued)
                .ToList();
        }

       
    }


}

