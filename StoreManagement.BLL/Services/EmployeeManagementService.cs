using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class EmployeeManagementService
    {
        CustomerManagementService _customerManagementService;
        public EmployeeManagementService()
        {
            _customerManagementService = new CustomerManagementService();
        }
        public List<StoreManagement.DAL.Entities.Employee> GetAllEmployees()
        {
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            if (employeeRepository == null)
            {
                throw new InvalidOperationException("Employee repository is not initialized or whether the database is not connected.");
            }
            return employeeRepository.GetAll();
        }
        public Employee Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            var employee = employeeRepository.GetEmployeeByUserNameAndPassword(userName, password);
            return employee;
        }
        public List<StoreManagement.DAL.Entities.Employee> SearchEmployeesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Search name cannot be null or empty", nameof(name));
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            return employeeRepository.SearchByName(name);
        }
        public void AddEmployee(StoreManagement.DAL.Entities.Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Add(employee);
        }
        public void UpdateEmployee(StoreManagement.DAL.Entities.Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Update(employee);
        }
        public void DeleteEmployee(int employeeId)
        {
            if (employeeId < 0)
            {
                throw new ArgumentException("Invalid employee ID", nameof(employeeId));
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Delete(employeeId);
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

        public void UpdateMyOrder(Order order, int employeeId)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null");
            }

            // Kiểm tra order có thuộc về employee này không
            if (!_employeeRepository.IsOrderBelongsToEmployee(order.OrderId, employeeId))
            {
                throw new UnauthorizedAccessException("Employee can only update their own orders");
            }

            _employeeRepository.UpdateOrder(order);
        }

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

