using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    /// <summary>
    /// This class is responsible for handling CRUD operations related to OrderDetails in the database.
    /// Its also similar to the OrderRepository class, which Administrators do not need to add Order.
    /// But its much more detailed than OrderRepository. 
    /// </summary>
    public class OrderDetailsRepository
    {
        LucySalesDataContext _context;
        public OrderDetailsRepository()
        {
            _context = new LucySalesDataContext();
        }
        public List<OrderDetail> GetAll()
        {
            return _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).ToList();
        }
        public List<OrderDetail> Search(int OrderId)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            if (OrderId != 0)
            {
                orderDetails = _context.OrderDetails.Include(o => o.Order)
                    .Include(o => o.Product)
                    .Where(o => o.OrderId == OrderId).ToList();
            }
            return orderDetails;
        }
        public void Update(OrderDetail orderDetail)
        {
            var od = _context.OrderDetails.FirstOrDefault(o => o.OrderId == orderDetail.OrderId && o.ProductId == orderDetail.ProductId);
            if (od != null)
            {
                od.UnitPrice = orderDetail.UnitPrice;
                od.Quantity = orderDetail.Quantity;
                od.Discount = orderDetail.Discount;
                _context.SaveChanges();
            }
        }
        public void Delete(int OrderId, int ProductId)
        {
            //Because of the foreign key constraint, we need to delete the Product ID first before deleting the Order.
            var od = _context.OrderDetails.FirstOrDefault(o => o.OrderId == OrderId && o.ProductId == ProductId);
            if (od != null)
            {
                _context.OrderDetails.Remove(od);
                _context.SaveChanges();
            }
        }
        public List<OrderDetail> GetOrderDetailsByEmployeeId(int employeeId)
        {
            return _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.Order.EmployeeId == employeeId)
                .ToList();
        }

    }
}
