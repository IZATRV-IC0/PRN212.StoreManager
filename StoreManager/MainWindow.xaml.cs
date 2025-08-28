using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StoreManager
{
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
        private Employee _employee;
        private Customer _customer;

        private CustomerManagementService _customerService;
        private EmployeeManagementService _employeeService;
        private ProductService _productService;
        private CategoryService _categoryService;
        private OrderService _orderService;
        private OrderDetailsService _orderDetailsService;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            _customerService = new CustomerManagementService();
            _employeeService = new EmployeeManagementService();
            _productService = new ProductService();
            _categoryService = new CategoryService();
            _orderService = new OrderService();
            _orderDetailsService = new OrderDetailsService();

            // Display employee information
            lblRole.Text = GetRoleName(_employee.RoleNum ?? 0);
            lbl_NameGreeting.Text = $"Welcome, {_employee.Name}!";
            
            // Configure UI based on role
            ConfigureUIForRole();
        }

        public MainWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            lblRole.Text = "CUSTOMER";
            lbl_NameGreeting.Text = "Welcome Customer!";
        }

        private void btn_Logout_Click(object sender, RoutedEventArgs e)
        {            
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            new LoginWindow().Show();
            this.Close();
        }


        #region Load Data Methods
        private void LoadCustomerData() =>
            dgCustomer.ItemsSource = _customerService.GetAllCustomers().ToList();

        private void LoadEmployeeData() =>
            dgEmployee.ItemsSource = _employeeService.GetAllEmployees().ToList();

        private void LoadProductData() =>
            dgProduct.ItemsSource = _productService.GetAllProducts().ToList();

        private void LoadCategoryData() =>
            dgCategory.ItemsSource = _categoryService.GetAllCategories().ToList();

        private void LoadOrderData() => LoadOrderDataForStaff();

        private void LoadCategoriesToSearch()
        {
            var categoryNames = _categoryService.GetAllCategories().Select(c => c.CategoryName).ToList();
            categoryNames.Insert(0, "All Categories");
            cboxProductCategorySearch.ItemsSource = categoryNames;
            cboxProductCategorySearch.SelectedIndex = 0;
        }

        #endregion

        #region Search Controls Management
        private void ShowSearchControls(bool showCategory = false, string searchLabel = "Search by Name:")
        {
            if (_currentMode == ManageMode.Order)
            {
                pnlEmployeeNameSearchForOrder.Visibility = Visibility.Visible;
            }
            else
            {
                pnlEmployeeNameSearchForOrder.Visibility = Visibility.Collapsed;
            }
            SearchSection.Visibility = Visibility.Visible;
            lblSearch.Visibility = Visibility.Visible;
            txtSearchValue.Visibility = Visibility.Visible;
            btnSearch.Visibility = Visibility.Visible;

            lblSearch.Text = searchLabel;

            cboxProductCategorySearch.Visibility = showCategory ? Visibility.Visible : Visibility.Collapsed;
        }

        private void HideSearchControls()
        {
            SearchSection.Visibility = Visibility.Collapsed;
            lblSearch.Visibility = Visibility.Collapsed;
            txtSearchValue.Visibility = Visibility.Collapsed;
            btnSearch.Visibility = Visibility.Collapsed;
            cboxProductCategorySearch.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Sidebar Button Handlers
        private void btn_CustomerMenu_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Customer;
            ShowOnlyDataGrid(dgCustomer);
            
            // Configure buttons based on role
            if (_employee != null)
            {
                switch (_employee.RoleNum ?? 0)
                {
                    case 1: // Admin - full access
                        SetButtonVisibility(true, true, true);
                        break;
                        
                    case 2: // Staff - can create, update, delete customers
                        SetButtonVisibility(true, true, true);
                        break;
                        
                    case 3: // Junior Staff - can only view and create customers
                        SetButtonVisibility(true, false, false);
                        break;
                        
                    default:
                        SetButtonVisibility(false, false, false);
                        break;
                }
            }
            
            OrderDetailSection.Visibility = Visibility.Collapsed;
            ShowSearchControls(false, "Search by Name:");
            txtSearchValue.Clear();
            LoadCustomerData();
        }

        private void btn_EmployeeMenu_Click(object sender, RoutedEventArgs e)
        {
            // Only Admin can access Employee Management
            if (_employee != null && (_employee.RoleNum ?? 0) != 1)
            {
                MessageBox.Show("You don't have permission to access Employee Management.", "Access Denied", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            _currentMode = ManageMode.Employee;
            ShowOnlyDataGrid(dgEmployee);
            SetButtonVisibility(true, true, true);
            OrderDetailSection.Visibility = Visibility.Collapsed;
            ShowSearchControls(false, "Search by Name:");
            txtSearchValue.Clear();
            LoadEmployeeData();
        }

        private void btn_Product_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Product;
            ShowOnlyDataGrid(dgProduct);
            
            // Configure buttons based on role
            if (_employee != null)
            {
                switch (_employee.RoleNum ?? 0)
                {
                    case 1: // Admin - full access
                        SetButtonVisibility(true, true, true);
                        break;
                        
                    case 2: // Staff - can only view products (read-only)
                        SetButtonVisibility(false, false, false);
                        break;
                        
                    case 3: // Junior Staff - no access (should not reach here)
                        SetButtonVisibility(false, false, false);
                        break;
                        
                    default:
                        SetButtonVisibility(false, false, false);
                        break;
                }
            }
            
            OrderDetailSection.Visibility = Visibility.Collapsed;
            ShowSearchControls(true, "Search by Name:");
            txtSearchValue.Clear();
            LoadProductData();
            LoadCategoriesToSearch();
        }

        private void btn_Category_Click(object sender, RoutedEventArgs e)
        {
            // Only Admin can access Category Management
            if (_employee != null && (_employee.RoleNum ?? 0) != 1)
            {
                MessageBox.Show("You don't have permission to access Category Management.", "Access Denied", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            _currentMode = ManageMode.Category;
            ShowOnlyDataGrid(dgCategory);
            SetButtonVisibility(true, true, true);
            OrderDetailSection.Visibility = Visibility.Collapsed;
            HideSearchControls();
            LoadCategoryData();
        }

        private void btn_OrderManager_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = ManageMode.Order;
            ShowOnlyDataGrid(dgOrder);
            
            // Configure buttons based on role
            if (_employee != null)
            {
                switch (_employee.RoleNum ?? 0)
                {
                    case 1: // Admin
                        SetButtonVisibility(false, false, true); // Default admin behavior
                        break;
                        
                    case 2: // Staff - can create and update orders
                        SetButtonVisibility(true, false, true);
                        break;
                        
                    case 3: // Junior Staff - can only view orders
                        SetButtonVisibility(false, false, false);
                        break;
                        
                    default:
                        SetButtonVisibility(false, false, false);
                        break;
                }
            }
            
            OrderDetailSection.Visibility = Visibility.Visible;
            ShowSearchControls(false, "& Customer Name:");
            txtSearchValue.Clear();
            LoadOrderData();
            dgOrderDetail.ItemsSource = null;
        }
        #endregion

        private void ShowOnlyDataGrid(DataGrid visibleGrid)
        {
            // Hide all DataGrids
            dgCustomer.Visibility = Visibility.Collapsed;
            dgEmployee.Visibility = Visibility.Collapsed;
            dgProduct.Visibility = Visibility.Collapsed;
            dgCategory.Visibility = Visibility.Collapsed;
            dgOrder.Visibility = Visibility.Collapsed;

            // Show the selected DataGrid
            visibleGrid.Visibility = Visibility.Visible;
        }

        private void SetButtonVisibility(bool createVisible, bool deleteVisible, bool updateVisible)
        {
            btn_Create.Visibility = createVisible ? Visibility.Visible : Visibility.Collapsed;
            btn_Delete.Visibility = deleteVisible ? Visibility.Visible : Visibility.Collapsed;
            btn_Update.Visibility = updateVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void dgOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgOrder.SelectedItem is Order selectedOrder)
            {
                dgOrderDetail.ItemsSource = _orderDetailsService.GetOrderDetailsByOrderId(selectedOrder.OrderId);
            }
        }

        #region Search Functionality
        private void btn_Search_Clicked(object sender, RoutedEventArgs e)
        {
            switch (_currentMode)
            {
                case ManageMode.Customer:
                    SearchCustomers();
                    break;
                case ManageMode.Employee:
                    SearchEmployees();
                    break;
                case ManageMode.Product:
                    SearchProducts();
                    break;
                case ManageMode.Order:
                    SearchOrders();
                    break;
            }
        }

        private void SearchCustomers()
        {
            var searchName = txtSearchValue.Text;
            if (!string.IsNullOrEmpty(searchName))
            {
                dgCustomer.ItemsSource = _customerService.SearchCustomersByName(searchName).ToList();
            }
            else
            {
                dgCustomer.ItemsSource = _customerService.GetAllCustomers().ToList();
            }
        }

        private void SearchEmployees()
        {
            var searchName = txtSearchValue.Text;
            if (!string.IsNullOrEmpty(searchName))
            {
                dgEmployee.ItemsSource = _employeeService.SearchEmployeesByName(searchName).ToList();
            }
            else
            {
                dgEmployee.ItemsSource = _employeeService.GetAllEmployees().ToList();
            }
        }

        private void SearchProducts()
        {
            var searchName = txtSearchValue.Text;
            var searchCategoryName = "";
            if (cboxProductCategorySearch.SelectedIndex > 0)
            {
                searchCategoryName = cboxProductCategorySearch.SelectedValue as string;
            }

            if (!string.IsNullOrEmpty(searchName) || !string.IsNullOrEmpty(searchCategoryName))
            {
                dgProduct.ItemsSource = _productService.SearchProductsByNameAndCategory(searchName, searchCategoryName).ToList();
            }
            else
            {
                dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
            }
        }

        private void SearchOrders() => SearchOrdersForStaff();

        #endregion

        #region CRUD Operations
        private void btn_Create_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == ManageMode.Customer)
            {
                // Check permissions for customer creation
                if (_employee != null && ((_employee.RoleNum ?? 0) == 1 || (_employee.RoleNum ?? 0) == 2 || (_employee.RoleNum ?? 0) == 3))
                {
                    ManageCustomerWindow manageCustomerWindow = new();
                    manageCustomerWindow.lblEditor.Text = "Create New Customer";
                    manageCustomerWindow.ShowDialog();
                    dgCustomer.ItemsSource = null;
                    dgCustomer.ItemsSource = _customerService.GetAllCustomers().ToList();
                }
                else
                {
                    MessageBox.Show("You don't have permission to create customers.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Employee)
            {
                // Only Admin can create employees
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    ManageEmployeeWindow manageEmployeeWindow = new();
                    manageEmployeeWindow.lblEditor.Text = "Create New Employee";
                    manageEmployeeWindow.ShowDialog();
                    dgEmployee.ItemsSource = null;
                    dgEmployee.ItemsSource = _employeeService.GetAllEmployees().ToList();
                }
                else
                {
                    MessageBox.Show("You don't have permission to create employees.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Product)
            {
                // Only Admin can create products
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    ManageProductWindow manageProductWindow = new();
                    manageProductWindow.lblEditor.Text = "Create New Product";
                    manageProductWindow.ShowDialog();
                    dgProduct.ItemsSource = null;
                    dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
                }
                else
                {
                    MessageBox.Show("You don't have permission to create products.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Category)
            {
                // Only Admin can create categories
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    ManageCategoryWindow manageCategoryWindow = new();
                    manageCategoryWindow.lblEditor.Text = "Create New Category";
                    manageCategoryWindow.ShowDialog();
                    dgCategory.ItemsSource = null;
                    dgCategory.ItemsSource = _categoryService.GetAllCategories().ToList();
                }
                else
                {
                    MessageBox.Show("You don't have permission to create categories.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Order)
            {
                // Only Staff (RoleNum = 2) can create new orders
                if (_employee != null && (_employee.RoleNum ?? 0) == 2)
                {
                    CreateNewOrderWindow createOrderWindow = new CreateNewOrderWindow(_employee);
                    createOrderWindow.ShowDialog();
                    LoadOrderData();
                    dgOrderDetail.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("You don't have permission to create orders.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == ManageMode.Customer)
            {
                // Check permissions for customer update
                if (_employee != null && ((_employee.RoleNum ?? 0) == 1 || (_employee.RoleNum ?? 0) == 2))
                {
                    Customer selectedCustomer = dgCustomer.SelectedItem as Customer;
                    if (selectedCustomer != null)
                    {
                        ManageCustomerWindow updateWindow = new();
                        updateWindow.lblEditor.Text = "Update Customer";
                        updateWindow.customerEdit = selectedCustomer;
                        updateWindow.ShowDialog();
                        dgCustomer.ItemsSource = null;
                        dgCustomer.ItemsSource = _customerService.GetAllCustomers().ToList();
                    }
                    else
                    {
                        MessageBox.Show("Please select a customer to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("You don't have permission to update customers.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Employee)
            {
                // Only Admin can update employees
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    Employee selectedEmployee = dgEmployee.SelectedItem as Employee;
                    if (selectedEmployee != null)
                    {
                        ManageEmployeeWindow updateWindow = new();
                        updateWindow.lblEditor.Text = "Update Employee";
                        updateWindow.employeeEdit = selectedEmployee;
                        updateWindow.ShowDialog();
                        dgEmployee.ItemsSource = null;
                        dgEmployee.ItemsSource = _employeeService.GetAllEmployees().ToList();
                    }
                    else
                    {
                        MessageBox.Show("Please select an employee to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("You don't have permission to update employees.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Product)
            {
                // Only Admin can update products
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    Product selectedProduct = dgProduct.SelectedItem as Product;
                    if (selectedProduct != null)
                    {
                        ManageProductWindow updateWindow = new();
                        updateWindow.lblEditor.Text = "Update Product";
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
                else
                {
                    MessageBox.Show("You don't have permission to update products.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Category)
            {
                // Only Admin can update categories
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    Category selectedCategory = dgCategory.SelectedItem as Category;
                    if (selectedCategory != null)
                    {
                        ManageCategoryWindow updateWindow = new();
                        updateWindow.lblEditor.Text = "Update Category";
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
                else
                {
                    MessageBox.Show("You don't have permission to update categories.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Order)
            {
                // Check permissions for order update
                if (_employee != null && ((_employee.RoleNum ?? 0) == 1 || (_employee.RoleNum ?? 0) == 2))
                {
                    Order selectedOrder = dgOrder.SelectedItem as Order;
                    if (selectedOrder != null)
                    {
                        // Staff can only update their own orders
                        if ((_employee.RoleNum ?? 0) == 2 && selectedOrder.EmployeeId != _employee.EmployeeId)
                        {
                            MessageBox.Show("You can only update orders created by yourself.", "Access Denied", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        
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
                else
                {
                    MessageBox.Show("You don't have permission to update orders.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        bool success = _customerService.DeleteCustomer(customer.CustomerId);
                        if (success)
                        {
                            MessageBox.Show("Customer deleted successfully.", "Deletion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                            dgCustomer.ItemsSource = null;
                            dgCustomer.ItemsSource = _customerService.GetAllCustomers().ToList();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete customer. Maybe there is still an order for this customer.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a customer to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Employee)
            {
                // Only Admin can delete employees
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
                {
                    Employee employee = dgEmployee.SelectedItem as Employee;
                    if (employee != null)
                    {
                        MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete employee {employee.EmployeeId}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            bool success = _employeeService.DeleteEmployee(employee.EmployeeId);
                            if (success)
                            {
                                MessageBox.Show("Employee deleted successfully.", "Deletion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                dgEmployee.ItemsSource = null;
                                dgEmployee.ItemsSource = _employeeService.GetAllEmployees().ToList();
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete employee. Maybe there is still an order for this employee.", "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select an employee to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("You don't have permission to delete employees.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Product)
            {
                // Only Admin can delete products
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
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
                        MessageBox.Show("Please select a product to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("You don't have permission to delete products.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentMode == ManageMode.Category)
            {
                // Only Admin can delete categories
                if (_employee != null && (_employee.RoleNum ?? 0) == 1)
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
                        MessageBox.Show("Please select a category to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("You don't have permission to delete categories.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        #endregion

        #region Role-based UI Configuration
        private string GetRoleName(int roleNum)
        {
            return roleNum switch
            {
                1 => "ADMIN",
                2 => "STAFF",
                3 => "JUNIOR STAFF",
                _ => "UNKNOWN"
            };
        }

        private void ConfigureUIForRole()
        {
            if (_employee != null)
            {
                switch (_employee.RoleNum ?? 0)
                {
                    case 1: // Admin - Full access
                        // No restrictions
                        break;
                        
                    case 2: // Staff - Limited access
                        ConfigureStaffPermissions();
                        break;
                        
                    case 3: // Junior Staff - Very limited access
                        ConfigureJuniorStaffPermissions();
                        break;
                        
                    default:
                        // Unknown role - minimal access
                        ConfigureJuniorStaffPermissions();
                        break;
                }
            }
        }

        private void ConfigureStaffPermissions()
        {
            // Hide restricted sidebar buttons for Staff
            btn_EmployeeMenu.Visibility = Visibility.Collapsed;
            btn_Category.Visibility = Visibility.Collapsed;
            
            // Set default view to customers for staff
            btn_CustomerMenu_Click(null, null);
        }

        private void ConfigureJuniorStaffPermissions()
        {
            // Hide most sidebar buttons for Junior Staff
            btn_EmployeeMenu.Visibility = Visibility.Collapsed;
            btn_Category.Visibility = Visibility.Collapsed;
            btn_Product.Visibility = Visibility.Collapsed; // Junior staff can't even view products
            
            // Set default view to customers (only allowed function)
            btn_CustomerMenu_Click(null, null);
        }

        private void LoadOrderDataForStaff()
        {
            switch (_employee.RoleNum ?? 0)
            {
                case 1: // Admin - see all orders
                    dgOrder.ItemsSource = _orderService.GetAllOrders().ToList();
                    break;
                    
                case 2: // Staff - only see their own orders
                case 3: // Junior Staff - only see their own orders
                    dgOrder.ItemsSource = _orderService.GetOrdersByEmployee(_employee.EmployeeId).ToList();
                    break;
                    
                default:
                    // Unknown role - no orders
                    dgOrder.ItemsSource = new List<Order>();
                    break;
            }
        }

        private void SearchOrdersForStaff()
        {
            var searchCustomerName = txtSearchValue.Text;
            var searchEmployeeName = txtEmployeeNameSearchValue.Text;
            
            switch (_employee.RoleNum ?? 0)
            {
                case 1: // Admin - search all orders
                    if (!string.IsNullOrEmpty(searchCustomerName) || !string.IsNullOrEmpty(searchEmployeeName))
                    {
                        dgOrder.ItemsSource = _orderService.SearchOrderByEmployeeAndCustomer(searchEmployeeName, searchCustomerName).ToList();
                    }
                    else
                    {
                        dgOrder.ItemsSource = _orderService.GetAllOrders().ToList();
                    }
                    break;
                    
                case 2: // Staff - only search in their own orders
                case 3: // Junior Staff - only search in their own orders
                    if (!string.IsNullOrEmpty(searchCustomerName))
                    {
                        var staffOrders = _orderService.GetOrdersByEmployee(_employee.EmployeeId);
                        dgOrder.ItemsSource = staffOrders.Where(o => 
                            o.Customer != null && 
                            o.Customer.ContactName.ToLower().Contains(searchCustomerName.ToLower())
                        ).ToList();
                    }
                    else
                    {
                        LoadOrderDataForStaff();
                    }
                    break;
                    
                default:
                    // Unknown role - no search results
                    dgOrder.ItemsSource = new List<Order>();
                    break;
            }
        }
        #endregion
    }
}