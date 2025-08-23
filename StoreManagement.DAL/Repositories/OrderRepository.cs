using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    /// <summary>
    /// This class is responsible for handling data operations related to Orders in the database.
    /// They are for managing whose orders to manage (is Customer ordered and Employee who handled the order?).
    /// Administrators do not need to add orders, as they are automatically generated when a customer places an order.
    /// </summary>
    public class OrderRepository
    {
        LucySalesDataContext _context;
        public OrderRepository()
        {
            _context = new LucySalesDataContext();
        }
        public List<Order> GetAll()
        {
            return _context.Orders.Include(o => o.Customer).Include(o => o.Employee).ToList();
        }
        public List<Order> SearchOrderByCustomer(string CustormerName, int CustomerId)
        {
            List<Order> orders = new List<Order>();
            //Name only
            if (!string.IsNullOrEmpty(CustormerName) && CustomerId == 0)
            {
                orders = _context.Orders.Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Where(o => o.Customer.CompanyName.ToLower().Contains(CustormerName.ToLower())).ToList();
            }
            //Id only
            else if (string.IsNullOrEmpty(CustormerName) && CustomerId != 0)
            {
                orders = _context.Orders.Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Where(o => o.CustomerId == CustomerId).ToList();
            }
            //Name and Id
            else if (!string.IsNullOrEmpty(CustormerName) && CustomerId != 0)
            {
                orders = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Where(o => o.Customer.CompanyName.ToLower().Contains(CustormerName.ToLower())
                    && o.CustomerId == CustomerId).ToList();
            }
            return orders;
        }
        public List<Order> SearchOrderByEmployee(string EmployeeName, int EmployeeId)
        {
            List<Order> orders = new List<Order>();
            //Name only (not UserName, but Name)
            if (!string.IsNullOrEmpty(EmployeeName) && EmployeeId == 0)
            {
                orders = _context.Orders.Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Where(o => o.Employee.Name.ToLower().Contains(EmployeeName.ToLower())).ToList();
            }
            //Id only
            else if (string.IsNullOrEmpty(EmployeeName) && EmployeeId != 0)
            {
                orders = _context.Orders.Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Where(o => o.EmployeeId == EmployeeId).ToList();
            }
            //Name and Id
            else if (!string.IsNullOrEmpty(EmployeeName) && EmployeeId != 0)
            {
                orders = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Where(o => o.Employee.Name.ToLower().Contains(EmployeeName.ToLower())
                    && o.EmployeeId == EmployeeId).ToList();
            }
            return orders;
        }
        public void Update(Order order)
        {
            var existingOrder = _context.Orders.Find(order.OrderId);
            if (existingOrder != null)
            {
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.EmployeeId = order.EmployeeId;
                existingOrder.OrderDate = order.OrderDate;
                _context.SaveChanges();
            }
        }
        public void Delete(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
