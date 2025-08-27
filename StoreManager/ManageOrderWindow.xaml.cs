using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StoreManager
{
    public partial class ManageOrderWindow : Window
    {
        private OrderService _orderService;
        private OrderDetailsService _orderDetailsService;
        private EmployeeManagementService _employeeService;

        public Order OrderEdit { get; set; }
        public ManageOrderWindow()
        {
            InitializeComponent();
            _orderService = new OrderService();
            _orderDetailsService = new OrderDetailsService();
            _employeeService = new EmployeeManagementService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load danh sách employees vào ComboBox
            LoadEmployees();

            if (OrderEdit != null)
            {
                FillOrderForm(OrderEdit);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                cbEmployee.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillOrderForm(Order order)
        {
            txtOrderId.Text = order.OrderId.ToString();
            txtCustomer.Text = $"{order.Customer.CompanyName} ({order.Customer.ContactName})";
            dpOrderDate.SelectedDate = order.OrderDate;

            // Chọn employee tương ứng trong ComboBox
            if (order.Employee != null)
            {
                // Tìm employee trong danh sách
                var employees = cbEmployee.ItemsSource as System.Collections.IList;
                if (employees != null)
                {
                    foreach (var item in employees)
                    {
                        if (item is Employee emp && emp.EmployeeId == order.EmployeeId)
                        {
                            cbEmployee.SelectedItem = item;
                            break;
                        }
                    }
                }

                // Nếu không tìm thấy, hiển thị thông tin cũ
                if (cbEmployee.SelectedItem == null)
                {
                    // Có thể thêm một item tạm để hiển thị thông tin
                    cbEmployee.Text = order.Employee.Name;
                }
            }

            // Load order details
            // Note: Order details view removed from simplified UI
            // var orderDetails = _orderDetailsService.GetOrderDetailsByOrderId(order.OrderId);
            // dgOrderDetails.ItemsSource = orderDetails;
        }

        private void btn_CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate employee selection
                if (cbEmployee.SelectedItem == null)
                {
                    MessageBox.Show("Please select a valid employee from the list.", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedEmployee = cbEmployee.SelectedItem as Employee;
                if (selectedEmployee == null)
                {
                    MessageBox.Show("Invalid employee selection.", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update order information
                if (dpOrderDate.SelectedDate.HasValue)
                {
                    OrderEdit.OrderDate = dpOrderDate.SelectedDate.Value;
                }

                // Update employee ID
                OrderEdit.EmployeeId = selectedEmployee.EmployeeId;

                // Update order in database
                _orderService.UpdateOrder(OrderEdit);

                // Note: Order details update removed from simplified UI
                // Order details would be updated here if UI supported it

                MessageBox.Show("Order updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}