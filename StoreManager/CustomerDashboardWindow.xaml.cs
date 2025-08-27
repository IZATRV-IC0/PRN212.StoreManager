using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System.Windows;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for CustomerDashboardWindow.xaml
    /// </summary>
    public partial class CustomerDashboardWindow : Window
    {
        private Customer _customer;

        public CustomerDashboardWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;

            
           

            // Subscribe to cart changes
            ShoppingCartService.Instance.CartChanged += OnCartChanged;

            // Update initial cart count
            UpdateCartButton();
        }

        private void btn_Logout_Click(object sender, RoutedEventArgs e)
        {
            // Back to login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        // Customer menu handlers
        private void btnViewProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductListWindow productListWindow = new ProductListWindow(_customer);
            productListWindow.ShowDialog();
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow(_customer);
            orderWindow.ShowDialog();
        }

        private void btnOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            OrderHistoryWindow orderHistoryWindow = new OrderHistoryWindow(_customer);
            orderHistoryWindow.ShowDialog();
        }

        private void btnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            CustomerProfileWindow profileWindow = new CustomerProfileWindow(_customer);
            profileWindow.ShowDialog();
        }

        private void OnCartChanged(object? sender, EventArgs e)
        {
            // Update cart button on UI thread
            Dispatcher.Invoke(UpdateCartButton);
        }

        private void UpdateCartButton()
        {
            int cartCount = ShoppingCartService.Instance.GetTotalItems();
            btnPlaceOrder.Content = $"📦 Place Order ({cartCount})";
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from cart changes to prevent memory leaks
            ShoppingCartService.Instance.CartChanged -= OnCartChanged;
            base.OnClosed(e);
        }
    }
}