using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Employee _employee;
        Customer _custormer;
        CustomerManagementService _customerManagementService;
        public MainWindow()
        {
            InitializeComponent();
            LoadCustomerData();
        }
        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            LoadCustomerData();
            switch (_employee.RoleNum)
            {
                case 1:
                    // Admin role
                    lblRole.Content = "Admin Dashboard";
                    lbl_NameGreeting.Content = "Hello, " + _employee.Name;
                    break;
                case 2:
                    // Manager role
                    lblRole.Content = "Manager Dashboard";
                    break;
                case 3:
                    // Staff role
                    lblRole.Content = "Staff Dashboard";
                    break;
                default:
                    lblRole.Content = "Unknown Role";
                    break;
            }
        }
        public MainWindow(Customer customer)
        {
            InitializeComponent();
            _custormer = customer;
            lblRole.Content = "Welcome Customer!";
        }

        private void btn_Logout_Click(object sender, RoutedEventArgs e)
        {
            //Back to login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        public void LoadCustomerData()
        {
            _customerManagementService = new CustomerManagementService();
            dgCustomer.ItemsSource = _customerManagementService.GetAllCustomers().ToList();
        }

        private void btn_CreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            ManageCustomerWindow manageCustomerWindow = new();
            manageCustomerWindow.lblEditor.Content = "Create New Customer";
            manageCustomerWindow.ShowDialog();
            dgCustomer.ItemsSource = null;
            dgCustomer.ItemsSource = _customerManagementService.GetAllCustomers().ToList();
        }

        private void btn_UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer selectedCustomer = dgCustomer.SelectedItem as Customer;
            if (selectedCustomer != null)
            {
                ManageCustomerWindow updateWindow = new();
                updateWindow.lblEditor.Content = "Update Customer";
                updateWindow.customerEdit = selectedCustomer;
                updateWindow.ShowDialog();
                dgCustomer.ItemsSource = null;
                dgCustomer.ItemsSource = _customerManagementService.GetAllCustomers().ToList();
            } else
            {
                MessageBox.Show("Please select a customer to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = dgCustomer.SelectedItem as Customer;
            if (customer != null)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete customer {customer.CustomerId}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bool success = _customerManagementService.DeleteCustomer(customer.CustomerId);
                    if (success)
                    {
                        MessageBox.Show("Customer deleted successfully.", "Deletion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        dgCustomer.ItemsSource = null;
                        dgCustomer.ItemsSource = _customerManagementService.GetAllCustomers().ToList();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete customer. Maybe there is still an order for this customer.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } else
                {
                    MessageBox.Show("Please select a customer to delete", "Uhh...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}