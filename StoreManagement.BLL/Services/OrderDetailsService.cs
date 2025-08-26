using StoreManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class OrderDetailsService
    {
        OrderDetailsRepository _orderDetailsRepository;
        public OrderDetailsService()
        {
            _orderDetailsRepository = new OrderDetailsRepository();
        }
        public List<StoreManagement.DAL.Entities.OrderDetail> GetAllOrderDetails()
        {
            if (_orderDetailsRepository == null)
            {
                throw new InvalidOperationException("OrderDetails repository is not initialized or whether the database is not connected.");
            }
            return _orderDetailsRepository.GetAll();
        }
        public List<StoreManagement.DAL.Entities.OrderDetail> SearchOrderDetailsByOrderId(int orderId)
        {
            if (orderId < 0)
            {
                throw new ArgumentException("Search Order ID cannot be negative", nameof(orderId));
            }
            return _orderDetailsRepository.Search(orderId);
        }
        public void UpdateOrderDetail(StoreManagement.DAL.Entities.OrderDetail orderDetail)
        {
            if (orderDetail == null)
            {
                throw new ArgumentNullException(nameof(orderDetail), "OrderDetail cannot be null");
            }
            _orderDetailsRepository.Update(orderDetail);
        }
        public void DeleteOrderDetail(int orderId, int productId)
        {
            if (orderId < 0 || productId < 0)
            {
                throw new ArgumentException("Invalid Order ID or Product ID", nameof(orderId));
            }
            _orderDetailsRepository.Delete(orderId, productId);
        }
        public List<StoreManagement.DAL.Entities.OrderDetail> GetOrderDetailsByEmployee(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Invalid Employee ID", nameof(employeeId));
            }
            return _orderDetailsRepository.GetOrderDetailsByEmployeeId(employeeId);
        }

    }
}
