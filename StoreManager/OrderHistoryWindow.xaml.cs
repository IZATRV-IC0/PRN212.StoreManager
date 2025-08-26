using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for OrderHistoryWindow.xaml
    /// </summary>
    public partial class OrderHistoryWindow : Window
    {
        private Customer _customer;
        private OrderService _orderService;
        private OrderDetailsService _orderDetailsService;
        private List<OrderViewModel> _orders = new List<OrderViewModel>();

        public OrderHistoryWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _orderService = new OrderService();
            _orderDetailsService = new OrderDetailsService();
            
            // Subscribe to OrderCreated event for real-time updates
            OrderService.OrderCreated += OnOrderCreated;
            this.Closing += OrderHistoryWindow_Closing;
            
            LoadOrderHistory();
        }

        private void LoadOrderHistory()
        {
            try
            {
                var orders = _orderService.GetOrdersByCustomerId(_customer.CustomerId);
                _orders = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Employee = o.Employee,
                    CustomerId = o.CustomerId,
                    EmployeeId = o.EmployeeId,
                    OrderDetails = o.OrderDetails.ToList(),
                    TotalItems = o.OrderDetails.Sum(od => od.Quantity),
                    TotalAmount = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount))
                }).ToList();

                dgOrders.ItemsSource = _orders;
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order history: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummary()
        {
            if (_orders != null)
            {
                lblTotalOrders.Text = _orders.Count.ToString();
                decimal totalSpent = _orders.Sum(o => o.TotalAmount);
                lblTotalSpent.Text = totalSpent.ToString("C");
            }
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgOrders.SelectedItem is OrderViewModel order)
            {
                LoadOrderDetails(order);
            }
        }

        private void LoadOrderDetails(OrderViewModel order)
        {
            try
            {
                var orderDetails = _orderDetailsService.GetOrderDetailsByOrderId(order.OrderId);
                
                var detailsViewModel = orderDetails.Select(od => new OrderDetailViewModel
                {
                    Product = od.Product,
                    ProductId = od.ProductId,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    Discount = od.Discount,
                    Subtotal = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)
                }).ToList();

                dgOrderDetails.ItemsSource = detailsViewModel;
                txtSelectedOrder.Text = $"Order #{order.OrderId} - {order.OrderDate:yyyy-MM-dd HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order details: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is OrderViewModel order)
            {
                dgOrders.SelectedItem = order;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrderHistory();
            dgOrderDetails.ItemsSource = null;
            txtSelectedOrder.Text = "Select an order to view details";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
        {
            // Check if the new order belongs to this customer
            if (e.Order.CustomerId == _customer.CustomerId)
            {
                // Use Dispatcher to update UI from a potentially different thread
                Dispatcher.Invoke(() =>
                {
                    LoadOrderHistory();
                    // Show a brief notification (optional)
                    MessageBox.Show("Your order history has been updated with the new order!", 
                        "Order Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        private void OrderHistoryWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unsubscribe from the event to prevent memory leaks
            OrderService.OrderCreated -= OnOrderCreated;
        }
    }

    // ViewModels
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public Employee Employee { get; set; } = null!;
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal Subtotal { get; set; }
    }
}
