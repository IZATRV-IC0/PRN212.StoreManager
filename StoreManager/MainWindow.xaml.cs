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
        Employee? _employee;
        Customer? _customer;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            pnlEmployeeMenu.Visibility = Visibility.Visible;
            pnlCustomerMenu.Visibility = Visibility.Collapsed;
            
            switch(_employee.RoleNum)
            {
                case 1:
                    // Admin role
                    lblRole.Content = "Admin Dashboard";
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
            lblUserInfo.Content = $"Welcome, {_employee.Name}";
        }
        public MainWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            lblRole.Content = "Customer Dashboard";
            lblUserInfo.Content = $"Welcome, {_customer.ContactName ?? _customer.CompanyName}";
            
            // Show customer menu
            pnlCustomerMenu.Visibility = Visibility.Visible;
            pnlEmployeeMenu.Visibility = Visibility.Collapsed;
            
            // Subscribe to cart changes
            ShoppingCartService.Instance.CartChanged += OnCartChanged;
            
            // Update initial cart count
            UpdateCartButton();
        }

        private void btn_Logout_Click(object sender, RoutedEventArgs e)
        {
            //Back to login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnViewProducts_Click(object sender, RoutedEventArgs e)
        {
            if (_customer != null)
            {
                ProductListWindow productListWindow = new ProductListWindow(_customer);
                productListWindow.ShowDialog();
            }
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_customer != null)
            {
                OrderWindow orderWindow = new OrderWindow(_customer);
                orderWindow.ShowDialog();
            }
        }

        private void btnOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            if (_customer != null)
            {
                OrderHistoryWindow orderHistoryWindow = new OrderHistoryWindow(_customer);
                orderHistoryWindow.ShowDialog();
            }
        }

        private void btnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_customer != null)
            {
                CustomerProfileWindow profileWindow = new CustomerProfileWindow(_customer);
                profileWindow.ShowDialog();
            }
        }

        private void OnCartChanged(object? sender, EventArgs e)
        {
            // Update cart button on UI thread
            Dispatcher.Invoke(UpdateCartButton);
        }

        private void UpdateCartButton()
        {
            if (_customer != null)
            {
                int cartCount = ShoppingCartService.Instance.GetTotalItems();
                btnPlaceOrder.Content = $"📦 Place Order ({cartCount})";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from cart changes to prevent memory leaks
            if (_customer != null)
            {
                ShoppingCartService.Instance.CartChanged -= OnCartChanged;
            }
            base.OnClosed(e);
        }
    }
}