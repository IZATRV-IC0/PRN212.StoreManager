using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for CreateNewOrderWindow.xaml
    /// </summary>
    public partial class CreateNewOrderWindow : Window
    {
        private Employee _currentEmployee;
        private CustomerManagementService _customerService;
        private ProductService _productService;
        private OrderService _orderService;
        private OrderDetailsService _orderDetailsService;
        
        private ObservableCollection<OrderItem> _orderItems;
        private Product _selectedProduct;

        public CreateNewOrderWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;
            
            _customerService = new CustomerManagementService();
            _productService = new ProductService();
            _orderService = new OrderService();
            _orderDetailsService = new OrderDetailsService();
            
            _orderItems = new ObservableCollection<OrderItem>();
            dgOrderItems.ItemsSource = _orderItems;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
            LoadProducts();
            UpdateOrderSummary();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                cmbCustomers.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts().Where(p => !p.Discontinued).ToList();
                cmbProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProducts.SelectedItem is Product product)
            {
                _selectedProduct = product;
                ShowProductInfo(product);
            }
            else
            {
                _selectedProduct = null;
                HideProductInfo();
            }
        }

        private void ShowProductInfo(Product product)
        {
            txtProductName.Text = product.ProductName;
            txtProductPrice.Text = $"Price: {product.UnitPrice:C}";
            txtProductCategory.Text = $"Category: {product.Category?.CategoryName ?? "N/A"}";
            pnlProductInfo.Visibility = Visibility.Visible;
        }

        private void HideProductInfo()
        {
            pnlProductInfo.Visibility = Visibility.Collapsed;
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product first.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if product already exists in order
            var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == _selectedProduct.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.Subtotal = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                var orderItem = new OrderItem
                {
                    ProductId = _selectedProduct.ProductId,
                    ProductName = _selectedProduct.ProductName,
                    UnitPrice = _selectedProduct.UnitPrice ?? 0,
                    Quantity = quantity,
                    Subtotal = (_selectedProduct.UnitPrice ?? 0) * quantity
                };
                _orderItems.Add(orderItem);
            }

            // Reset selection
            cmbProducts.SelectedIndex = -1;
            txtQuantity.Text = "1";
            HideProductInfo();
            
            UpdateOrderSummary();
        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is OrderItem item)
            {
                _orderItems.Remove(item);
                UpdateOrderSummary();
            }
        }

        private void UpdateOrderSummary()
        {
            txtTotalItems.Text = _orderItems.Sum(item => item.Quantity).ToString();
            txtTotalAmount.Text = _orderItems.Sum(item => item.Subtotal).ToString("C");
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateOrder())
                return;

            try
            {
                // Create new order
                var order = new Order
                {
                    CustomerId = (int)cmbCustomers.SelectedValue,
                    EmployeeId = _currentEmployee.EmployeeId,
                    OrderDate = DateTime.Now
                };

                // Add order to database
                _orderService.AddOrder(order);

                // Get the created order ID (assuming AddOrder updates the object with the new ID)
                // If not, you may need to get the last inserted order or modify AddOrder to return the ID
                var orderId = order.OrderId;
                if (orderId == 0)
                {
                    // If OrderId is still 0, we need to get the latest order for this customer/employee
                    var orders = _orderService.GetOrdersByEmployee(_currentEmployee.EmployeeId);
                    var latestOrder = orders.OrderByDescending(o => o.OrderDate).FirstOrDefault();
                    orderId = latestOrder?.OrderId ?? 0;
                }

                // Add order details
                foreach (var item in _orderItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        UnitPrice = item.UnitPrice,
                        Quantity = (short)item.Quantity,
                        Discount = 0
                    };
                    _orderDetailsService.CreateOrderDetail(orderDetail);
                }

                MessageBox.Show("Order created successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating order: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateOrder()
        {
            if (cmbCustomers.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Please add at least one product to the order.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Helper class for order items
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
