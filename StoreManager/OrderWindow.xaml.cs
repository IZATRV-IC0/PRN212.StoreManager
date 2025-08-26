using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private Customer _customer;
        private ProductService _productService;
        private OrderService _orderService;
        private EmployeeManagementService _employeeService;
        private ObservableCollection<OrderItemViewModel> _orderItems;
        private List<Product> _products = new List<Product>();

        public OrderWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _productService = new ProductService();
            _orderService = new OrderService();
            _employeeService = new EmployeeManagementService();
            _orderItems = new ObservableCollection<OrderItemViewModel>();
            dgOrderItems.ItemsSource = _orderItems;
            
            InitializeForm();
            // LoadProducts(); // Không cần load products nữa vì đã ẩn phần chọn sản phẩm
            LoadCartItems();
        }

        private void InitializeForm()
        {
            txtCustomerName.Text = _customer.ContactName ?? _customer.CompanyName;
            txtOrderDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            UpdateOrderSummary();
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productService.GetAllProducts()
                    .Where(p => !p.Discontinued && p.UnitsInStock > 0)
                    .ToList();
                cmbProducts.ItemsSource = _products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCartItems()
        {
            try
            {
                var cartItems = ShoppingCartService.Instance.GetCartItems();
                _orderItems.Clear();
                
                foreach (var cartItem in cartItems)
                {
                    _orderItems.Add(new OrderItemViewModel
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        CategoryName = cartItem.Product?.Category?.CategoryName ?? "Unknown",
                        UnitPrice = cartItem.UnitPrice,
                        Quantity = cartItem.Quantity
                    });
                }
                
                UpdateOrderSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cart items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProducts.SelectedItem is Product product)
            {
                txtQuantity.Text = "1";
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducts.SelectedItem is Product product)
            {
                if (int.TryParse(txtQuantity.Text, out int quantity) && quantity > 0)
                {
                    if (quantity > product.UnitsInStock)
                    {
                        MessageBox.Show($"Only {product.UnitsInStock} units available in stock.", 
                            "Insufficient Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Check if product already exists in order
                    var existingItem = _orderItems.FirstOrDefault(i => i.ProductId == product.ProductId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        _orderItems.Add(new OrderItemViewModel
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            CategoryName = product.Category?.CategoryName ?? "Unknown",
                            UnitPrice = product.UnitPrice ?? 0,
                            Quantity = quantity
                        });
                    }

                    UpdateOrderSummary();
                    cmbProducts.SelectedIndex = -1;
                    txtQuantity.Text = "1";
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantity.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a product.", "No Product Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is OrderItemViewModel item)
            {
                _orderItems.Remove(item);
                // Also remove from shopping cart
                ShoppingCartService.Instance.RemoveFromCartWithNotification(item.ProductId);
                UpdateOrderSummary();
            }
        }

        private void UpdateOrderSummary()
        {
            int totalItems = _orderItems.Sum(i => i.Quantity);
            decimal totalAmount = _orderItems.Sum(i => i.Subtotal);
            
            txtTotalItems.Text = totalItems.ToString();
            txtTotalAmount.Text = totalAmount.ToString("C");
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Please add at least one product to the order.", 
                    "Empty Order", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirmation dialog
            decimal totalAmount = _orderItems.Sum(i => i.Subtotal);
            int totalItems = _orderItems.Sum(i => i.Quantity);
            
            var result = MessageBox.Show(
                $"Do you want to confirm this order?\n\n" +
                $"Total Items: {totalItems}\n" +
                $"Total Amount: {totalAmount:C}\n\n" +
                $"Click 'Yes' to proceed with payment, 'No' to cancel.",
                "Confirm Order Payment", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // Get the first employee as default (in real app, this should be handled differently)
                var employees = _employeeService.GetAllEmployees();
                var defaultEmployee = employees.FirstOrDefault();
                
                if (defaultEmployee == null)
                {
                    MessageBox.Show("No employee available to process the order.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create order
                var order = new Order
                {
                    CustomerId = _customer.CustomerId,
                    EmployeeId = defaultEmployee.EmployeeId,
                    OrderDate = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                };

                // Add order details
                foreach (var item in _orderItems)
                {
                    if (item.Quantity > 0 && item.UnitPrice > 0)
                    {
                        var orderDetail = new OrderDetail
                        {
                            // OrderId will be set by repository after order is saved
                            ProductId = item.ProductId,
                            UnitPrice = item.UnitPrice,
                            Quantity = (short)item.Quantity,
                            Discount = 0
                        };
                        order.OrderDetails.Add(orderDetail);
                    }
                }

                // Validate that we have order details
                if (order.OrderDetails.Count == 0)
                {
                    MessageBox.Show("No valid items to order.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Save order
                _orderService.AddOrder(order);
                
                // Clear shopping cart after successful order
                ShoppingCartService.Instance.ClearCartWithNotification();
                
                MessageBox.Show("Order placed successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error placing order: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && dgOrderItems.SelectedItem is OrderItemViewModel item)
            {
                if (int.TryParse(textBox.Text, out int newQuantity) && newQuantity > 0)
                {
                    // Check stock availability
                    var product = _products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product != null && newQuantity > product.UnitsInStock)
                    {
                        MessageBox.Show($"Only {product.UnitsInStock} units available in stock.", 
                            "Insufficient Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                        textBox.Text = item.Quantity.ToString(); // Reset to original value
                        return;
                    }

                    item.Quantity = newQuantity;
                    // Update shopping cart as well
                    ShoppingCartService.Instance.UpdateQuantity(item.ProductId, newQuantity);
                    UpdateOrderSummary();
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantity (greater than 0).", 
                        "Invalid Quantity", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = item.Quantity.ToString(); // Reset to original value
                }
            }
        }
    }

    // ViewModel for order items
    public class OrderItemViewModel : INotifyPropertyChanged
    {
        private int _quantity;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        
        public int Quantity 
        { 
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }
        
        public decimal Subtotal => UnitPrice * Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
