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
        private enum ManageMode
        {
            Customer,
            Employee,
            Product,
            Category,
            Order
        }

        private ManageMode _currentMode;
        Employee _employee;
        CustomerManagementService _customerManagementService;
        EmployeeManagementService _employeeManagementService;
        ProductService _productService;
        CategoryService _categoryService;
        OrderService _orderService;
        OrderDetailsService _orderDetailsService;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            switch (_employee.RoleNum)
            {
                case 1:
                    // Admin role
                    lblRole.Content = "Admin Dashboard";
                    lbl_NameGreeting.Content = "Hello, " + _employee.Name;
                    break;
                case 3:
                    // Staff role
                    lblRole.Content = "Staff Dashboard";
                    lbl_NameGreeting.Content = "Hello, " + _employee.Name;
                    break;
                default:
                    lblRole.Content = "Unknown Role";
                    break;
            }
        }

        public MainWindow(Customer customer)
        {
            InitializeComponent();

            // Redirect to CustomerDashboardWindow
            CustomerDashboardWindow customerWindow = new CustomerDashboardWindow(customer);
            customerWindow.Show();
            this.Close();
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

        public void LoadEmployeeData()
        {
            _employeeManagementService = new EmployeeManagementService();
            dgEmployee.ItemsSource = _employeeManagementService.GetAllEmployees().ToList();
        }

        public void LoadProductData()
        {
            _productService = new ProductService();
            dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
        }

        public void LoadCategoryData()
        {
            _categoryService = new CategoryService();
            dgCategory.ItemsSource = _categoryService.GetAllCategories().ToList();
        }

        public void LoadOrderData()
        {
            _orderService = new OrderService();
            dgOrder.ItemsSource = _orderService.GetAllOrders().ToList();
        }

        public void LoadOrderDetailsData()
        {
            _orderDetailsService = new OrderDetailsService();
            dgOrderDetail.ItemsSource = _orderDetailsService.GetAllOrderDetails().ToList();
        }

        public void LoadCategoriesToSearch()
        {
            _categoryService = new CategoryService();
            var categoryNames = _categoryService.GetAllCategories().Select(c => c.CategoryName).ToList();
            categoryNames.Insert(0, "All Categories");
            cboxProductCategorySearch.ItemsSource = categoryNames;
            cboxProductCategorySearch.SelectedIndex = 0;
        }

        private void btn_Create_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == ManageMode.Customer)
            {
                ManageCustomerWindow manageCustomerWindow = new();
                manageCustomerWindow.lblEditor.Content = "Create New Customer";
                manageCustomerWindow.ShowDialog();
                dgCustomer.ItemsSource = null;
                dgCustomer.ItemsSource = _customerManagementService.GetAllCustomers().ToList();
            }
            else if (_currentMode == ManageMode.Employee)
            {
                ManageEmployeeWindow manageEmployeeWindow = new();
                manageEmployeeWindow.lblEditor.Content = "Create New Employee";
                manageEmployeeWindow.ShowDialog();
                dgEmployee.ItemsSource = null;
                dgEmployee.ItemsSource = _employeeManagementService.GetAllEmployees().ToList();
            }
            else if (_currentMode == ManageMode.Product)
            {
                ManageProductWindow manageProductWindow = new();
                manageProductWindow.lblEditor.Content = "Create New Product";
                manageProductWindow.ShowDialog();
                dgProduct.ItemsSource = null;
                dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
            }
            else if (_currentMode == ManageMode.Category)
            {
                ManageCategoryWindow manageCategoryWindow = new();
                manageCategoryWindow.lblEditor.Content = "Create New Category";
                manageCategoryWindow.ShowDialog();
                dgCategory.ItemsSource = null;
                dgCategory.ItemsSource = _categoryService.GetAllCategories().ToList();
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == ManageMode.Customer)
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
                }
                else
                {
                    MessageBox.Show("Please select a customer to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Employee)
            {
                Employee selectedEmployee = dgEmployee.SelectedItem as Employee;
                if (selectedEmployee != null)
                {
                    ManageEmployeeWindow updateWindow = new();
                    updateWindow.lblEditor.Content = "Update Employee";
                    updateWindow.employeeEdit = selectedEmployee;
                    updateWindow.ShowDialog();
                    dgEmployee.ItemsSource = null;
                    dgEmployee.ItemsSource = _employeeManagementService.GetAllEmployees().ToList();
                }
                else
                {
                    MessageBox.Show("Please select an employee to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Product)
            {
                Product selectedProduct = dgProduct.SelectedItem as Product;
                if (selectedProduct != null)
                {
                    ManageProductWindow updateWindow = new();
                    updateWindow.lblEditor.Content = "Update Product";
                    updateWindow.productEdit = selectedProduct;
                    updateWindow.ShowDialog();
                    dgProduct.ItemsSource = null;
                    dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
                }
                else
                {
                    MessageBox.Show("Please select a product to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Category)
            {
                Category selectedCategory = dgCategory.SelectedItem as Category;
                if (selectedCategory != null)
                {
                    ManageCategoryWindow updateWindow = new();
                    updateWindow.lblEditor.Content = "Update Category";
                    updateWindow.categoryEdit = selectedCategory;
                    updateWindow.ShowDialog();
                    dgCategory.ItemsSource = null;
                    dgCategory.ItemsSource = _categoryService.GetAllCategories().ToList();
                }
                else
                {
                    MessageBox.Show("Please select a category to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Order)
            {
                Order selectedOrder = dgOrder.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    ManageOrderWindow updateWindow = new ManageOrderWindow();
                    updateWindow.OrderEdit = selectedOrder;
                    updateWindow.ShowDialog();

                    LoadOrderData();
                    dgOrderDetail.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("Please select an order to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == ManageMode.Customer)
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
                    }
                }
                else
                {
                    MessageBox.Show("Please select a customer to delete", "Uhh...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Employee)
            {
                Employee employee = dgEmployee.SelectedItem as Employee;
                if (employee != null)
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete employee {employee.EmployeeId}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = _employeeManagementService.DeleteEmployee(employee.EmployeeId);
                        if (success)
                        {
                            MessageBox.Show("Employee deleted successfully.", "Deletion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                            dgEmployee.ItemsSource = null;
                            dgEmployee.ItemsSource = _employeeManagementService.GetAllEmployees().ToList();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete employee. Maybe there is still an order for this employee.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select an employee to delete", "Uhh...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Product)
            {
                Product product = dgProduct.SelectedItem as Product;
                if (product != null)
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete product {product.ProductId}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = _productService.DeleteProduct(product.ProductId);
                        if (success)
                        {
                            MessageBox.Show("Product deleted successfully.", "Deletion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                            dgProduct.ItemsSource = null;
                            dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete product. Maybe there is still an order for this product.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a product to delete", "Uhh...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Category)
            {
                Category category = dgCategory.SelectedItem as Category;
                if (category != null)
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete category {category.CategoryId}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = _categoryService.DeleteCategory(category.CategoryId);
                        if (success)
                        {
                            MessageBox.Show("Category deleted successfully.", "Deletion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                            dgCategory.ItemsSource = null;
                            dgCategory.ItemsSource = _categoryService.GetAllCategories().ToList();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete category. Maybe there is still a product for this category.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a category to delete", "Uhh...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btn_CustomerMenu_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Customer;
            dgCustomer.Visibility = Visibility.Visible;
            dgEmployee.Visibility = Visibility.Collapsed;
            dgProduct.Visibility = Visibility.Collapsed;
            dgCategory.Visibility = Visibility.Collapsed;
            dgOrder.Visibility = Visibility.Collapsed;
            dgOrderDetail.Visibility = Visibility.Collapsed;
            txtSearchValue.Visibility = Visibility.Visible;
            lblSearch.Content = "Name";
            lblSearch.Visibility = Visibility.Visible;
            cboxProductCategorySearch.Visibility = Visibility.Collapsed;
            btnSearch.Visibility = Visibility.Visible;
            btn_Create.Visibility = Visibility.Visible;
            btn_Delete.Visibility = Visibility.Visible;
            LoadCustomerData();
        }

        private void btn_EmployeeMenu_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Employee;
            dgCustomer.Visibility = Visibility.Collapsed;
            dgEmployee.Visibility = Visibility.Visible;
            dgProduct.Visibility = Visibility.Collapsed;
            dgCategory.Visibility = Visibility.Collapsed;
            dgOrder.Visibility = Visibility.Collapsed;
            dgOrderDetail.Visibility = Visibility.Collapsed;
            txtSearchValue.Visibility = Visibility.Visible;
            lblSearch.Content = "Name";
            lblSearch.Visibility = Visibility.Visible;
            cboxProductCategorySearch.Visibility = Visibility.Collapsed;
            btnSearch.Visibility = Visibility.Visible;
            btn_Create.Visibility = Visibility.Visible;
            btn_Delete.Visibility = Visibility.Visible;
            LoadEmployeeData();
        }

        private void btn_Product_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Product;
            dgCustomer.Visibility = Visibility.Collapsed;
            dgEmployee.Visibility = Visibility.Collapsed;
            dgProduct.Visibility = Visibility.Visible;
            dgCategory.Visibility = Visibility.Collapsed;
            dgOrderDetail.Visibility = Visibility.Collapsed;
            dgOrder.Visibility = Visibility.Collapsed;
            txtSearchValue.Visibility = Visibility.Visible;
            lblSearch.Content = "Name";
            lblSearch.Visibility = Visibility.Visible;
            cboxProductCategorySearch.Visibility = Visibility.Visible;
            btnSearch.Visibility = Visibility.Visible;
            btn_Create.Visibility = Visibility.Visible;
            btn_Delete.Visibility = Visibility.Visible;
            LoadProductData();
            LoadCategoriesToSearch();
        }

        private void btn_Category_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Category;
            dgCategory.Visibility = Visibility.Visible;
            dgCustomer.Visibility = Visibility.Collapsed;
            dgEmployee.Visibility = Visibility.Collapsed;
            dgProduct.Visibility = Visibility.Collapsed;
            dgOrderDetail.Visibility = Visibility.Collapsed;
            dgOrder.Visibility = Visibility.Collapsed;
            txtSearchValue.Visibility = Visibility.Collapsed;
            lblSearch.Visibility = Visibility.Collapsed;
            cboxProductCategorySearch.Visibility = Visibility.Collapsed;
            btnSearch.Visibility = Visibility.Collapsed;
            btn_Create.Visibility = Visibility.Visible;
            btn_Delete.Visibility = Visibility.Visible;
            LoadCategoryData();
        }

        private void btn_OrderManager_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Order;
            btn_Create.Visibility = Visibility.Collapsed;
            btn_Delete.Visibility = Visibility.Collapsed;
            dgOrder.Visibility = Visibility.Visible;
            dgOrderDetail.Visibility = Visibility.Visible;
            dgCategory.Visibility = Visibility.Collapsed;
            dgCustomer.Visibility = Visibility.Collapsed;
            dgEmployee.Visibility = Visibility.Collapsed;
            dgProduct.Visibility = Visibility.Collapsed;
            lblSearch.Content = "ID";
            txtSearchValue.Visibility = Visibility.Visible;
            lblSearch.Visibility = Visibility.Visible;
            cboxProductCategorySearch.Visibility = Visibility.Collapsed;
            btnSearch.Visibility = Visibility.Visible;
            LoadOrderData();
            dgOrderDetail.ItemsSource = null;
        }

        private void dgOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedOrder = dgOrder.SelectedItem as Order;
            if (selectedOrder != null)
            {
                _orderDetailsService = new OrderDetailsService();
                dgOrderDetail.ItemsSource = _orderDetailsService.GetOrderDetailsByOrderId(selectedOrder.OrderId);
            }
        }

        private void btn_Search_Clicked(object sender, RoutedEventArgs e)
        {
            switch (_currentMode)
            {
                case ManageMode.Product:
                    var searchName = txtSearchValue.Text;
                    var searchCategoryName = "";
                    if (cboxProductCategorySearch.SelectedIndex > 0)
                    {
                        searchCategoryName = cboxProductCategorySearch.SelectedValue as string;
                    }
                    _productService = new ProductService();
                    if (!string.IsNullOrEmpty(searchName) || !string.IsNullOrEmpty(searchCategoryName))
                    {
                        dgProduct.ItemsSource = _productService.SearchProductsByNameAndCategory(searchName, searchCategoryName).ToList();
                    }
                    else
                    {
                        dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
                    }
                    break;
                case ManageMode.Order:
                    var searchValue = txtSearchValue.Text;
                    _orderService = new OrderService();
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        dgOrder.ItemsSource = _orderService.SearchByOrderID(searchValue).ToList();
                    }
                    else
                    {
                        dgOrder.ItemsSource = _orderService.GetAllOrders().ToList();
                    }
                    break;
                case ManageMode.Customer:
                    searchName = txtSearchValue.Text;
                    _customerManagementService = new CustomerManagementService();
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        dgCustomer.ItemsSource = _customerManagementService.SearchCustomersByName(searchName).ToList();
                    }
                    else
                    {
                        dgCustomer.ItemsSource = _customerManagementService.GetAllCustomers().ToList();
                    }
                    break;
                case ManageMode.Employee:
                    searchName = txtSearchValue.Text;
                    _employeeManagementService = new EmployeeManagementService();
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        dgEmployee.ItemsSource = _employeeManagementService.SearchEmployeesByName(searchName).ToList();
                    }
                    else
                    {
                        dgEmployee.ItemsSource = _employeeManagementService.GetAllEmployees().ToList();
                    }
                    break;
            }
        }
    }
}