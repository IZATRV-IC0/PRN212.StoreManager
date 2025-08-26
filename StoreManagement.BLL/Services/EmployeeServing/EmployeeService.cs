using StoreManagement.DAL.Entities;
using StoreManagement.DAL.Repositories.EmployeeRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services.EmployeeServing
{
    /// <summary>
    /// This file is not gonna be used in this project yet.
    /// Fuctions which commented out are bugs.
    /// </summary>
    public class EmployeeService
    {
        readonly EmployeeRepository _employeeRepository;
        public EmployeeService()
        {
            _employeeRepository = new();
        }
        // 1. QUẢN LÝ ORDERS
        public List<Order> GetMyOrders(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Employee ID must be greater than 0", nameof(employeeId));
            }
            return _employeeRepository.GetOrdersByEmployeeId(employeeId);
        }

        public void CreateOrder(Order order, int employeeId)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null");
            }

            if (employeeId <= 0)
            {
                throw new ArgumentException("Employee ID must be greater than 0", nameof(employeeId));
            }

            // Validate order details
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                throw new ArgumentException("Order must have at least one order detail");
            }

            // Set employee ID and order date
            order.EmployeeId = employeeId;
            if (order.OrderDate == null)
            {
                order.OrderDate = DateTime.Now;
            }

            _employeeRepository.AddOrder(order);
        }

        //public void UpdateMyOrder(Order order, int employeeId)
        //{
        //    if (order == null)
        //    {
        //        throw new ArgumentNullException(nameof(order), "Order cannot be null");
        //    }

        //    // Kiểm tra order có thuộc về employee này không
        //    if (!_employeeRepository.IsOrderBelongsToEmployee(order.OrderId, employeeId))
        //    {
        //        throw new UnauthorizedAccessException("Employee can only update their own orders");
        //    }

        //    _employeeRepository.UpdateOrder(order);
        //}

        public bool DeleteMyOrder(int orderId, int employeeId)
        {
            if (orderId <= 0 || employeeId <= 0)
            {
                return false;
            }
            return _employeeRepository.DeleteOrder(orderId, employeeId);
        }

        // 2. XEM PRODUCTS VÀ CATEGORIES
        public List<Product> GetAllProducts()
        {
            return _employeeRepository.GetAllProducts();
        }

        public List<Category> GetAllCategories()
        {
            return _employeeRepository.GetAllCategories();
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0", nameof(categoryId));
            }
            return _employeeRepository.GetProductsByCategory(categoryId);
        }

        public Product GetProductById(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            }
            return _employeeRepository.GetProductById(productId);
        }

        public List<Product> SearchProducts(string productName)
        {
            return _employeeRepository.SearchProductsByName(productName);
        }
    }
}
