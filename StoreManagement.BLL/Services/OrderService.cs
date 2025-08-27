﻿using StoreManagement.DAL.Repositories;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class OrderService
    {
        // Static event to notify when a new order is created
        public static event EventHandler<OrderCreatedEventArgs>? OrderCreated;
        OrderRepository _orderRepository;
        public OrderService()
        {
            _orderRepository = new OrderRepository();
        }
        public List<StoreManagement.DAL.Entities.Order> GetAllOrders()
        {
            if (_orderRepository == null)
            {
                throw new InvalidOperationException("Order repository is not initialized or whether the database is not connected.");
            }
            return _orderRepository.GetAll();
        }
        public List<StoreManagement.DAL.Entities.Order> SearchByOrderID(string orderID)
        {
            if (_orderRepository == null)
            {
                throw new InvalidOperationException("Order repository is not initialized or whether the database is not connected.");
            }
            return _orderRepository.SearchByOrderID(orderID);
        }
        public List<StoreManagement.DAL.Entities.Order> SearchOrderByCustomer(string customerName, int customerId)
        {
            if (customerId < 0 && string.IsNullOrEmpty(customerName))
            {
                throw new ArgumentException("Unidentified search criteria", nameof(customerId));
            }
            return _orderRepository.SearchOrderByCustomer(customerName, customerId);
        }
        public List<StoreManagement.DAL.Entities.Order> SearchOrderByEmployee(string employeeName, int employeeId)
        {
            if (employeeId < 0 && string.IsNullOrEmpty(employeeName))
            {
                throw new ArgumentException("Unidentified search criteria", nameof(employeeId));
            }
            return _orderRepository.SearchOrderByEmployee(employeeName, employeeId);
        }
        public List<StoreManagement.DAL.Entities.Order> SearchOrderByEmployeeAndCustomer(string employeeName, string customerName)
        {
            if (string.IsNullOrEmpty(customerName) && string.IsNullOrEmpty(employeeName))
            {
                throw new ArgumentException("Unidentified search criteria", nameof(customerName));
            }
            return _orderRepository.SearchOrderByEmployeeAndCustomer(employeeName, customerName);
        }
        public void UpdateOrder(StoreManagement.DAL.Entities.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null");
            }
            _orderRepository.Update(order);
        }
        public void DeleteOrder(int orderId)
        {
            if (orderId < 0)
            {
                throw new ArgumentException("Invalid order ID", nameof(orderId));
            }
            _orderRepository.Delete(orderId);
        }
        
        public List<StoreManagement.DAL.Entities.Order> GetOrdersByCustomerId(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Invalid customer ID", nameof(customerId));
            }
            return _orderRepository.GetOrdersByCustomerId(customerId);
        }
        
        public List<StoreManagement.DAL.Entities.Order> GetOrdersByEmployee(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Invalid Employee ID", nameof(employeeId));
            }
            return _orderRepository.GetOrdersByEmployeeId(employeeId);
        }
        
        public void AddOrder(StoreManagement.DAL.Entities.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null");
            }
            _orderRepository.Add(order);
            
            // Trigger the OrderCreated event
            OrderCreated?.Invoke(this, new OrderCreatedEventArgs { Order = order });
        }
    }
    
    // Event arguments class for OrderCreated event
    public class OrderCreatedEventArgs : EventArgs
    {
        public Order Order { get; set; } = null!;
    }
}
