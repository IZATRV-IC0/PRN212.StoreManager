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
            lblRole.Text = _employee.RoleNum == 1 ? "ADMIN" : "STAFF";
            lbl_NameGreeting.Text = $"Welcome, {_employee.Name}!";
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

        private void LoadOrderData() =>
            dgOrder.ItemsSource = _orderService.GetAllOrders().ToList();

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
            SetButtonVisibility(true, true, true);
            OrderDetailSection.Visibility = Visibility.Collapsed;
            ShowSearchControls(false, "Search by Name:");
            txtSearchValue.Clear();
            LoadCustomerData();
        }

        private void btn_EmployeeMenu_Click(object sender, RoutedEventArgs e)
        {
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
            SetButtonVisibility(true, true, true);
            OrderDetailSection.Visibility = Visibility.Collapsed;
            ShowSearchControls(true, "Search by Name:");
            txtSearchValue.Clear();
            LoadProductData();
            LoadCategoriesToSearch();
        }

        private void btn_Category_Click(object sender, RoutedEventArgs e)
        {
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
            SetButtonVisibility(false, false, true);
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

        private void SearchOrders()
        {
            var searchCustomerName = txtSearchValue.Text;
            var searchEmployeeName = txtEmployeeNameSearchValue.Text;
            if (!string.IsNullOrEmpty(searchCustomerName) || !string.IsNullOrEmpty(searchEmployeeName))
            {
                dgOrder.ItemsSource = _orderService.SearchOrderByEmployeeAndCustomer(searchEmployeeName, searchCustomerName).ToList();
            }
            else
            {
                dgOrder.ItemsSource = _orderService.GetAllOrders().ToList();
            }
        }

        #endregion

        #region CRUD Operations
        private void btn_Create_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMode == ManageMode.Customer)
            {
                ManageCustomerWindow manageCustomerWindow = new();
                manageCustomerWindow.lblEditor.Text = "Create New Customer";
                manageCustomerWindow.ShowDialog();
                dgCustomer.ItemsSource = null;
                dgCustomer.ItemsSource = _customerService.GetAllCustomers().ToList();
            }
            else if (_currentMode == ManageMode.Employee)
            {
                ManageEmployeeWindow manageEmployeeWindow = new();
                manageEmployeeWindow.lblEditor.Text = "Create New Employee";
                manageEmployeeWindow.ShowDialog();
                dgEmployee.ItemsSource = null;
                dgEmployee.ItemsSource = _employeeService.GetAllEmployees().ToList();
            }
            else if (_currentMode == ManageMode.Product)
            {
                ManageProductWindow manageProductWindow = new();
                manageProductWindow.lblEditor.Text = "Create New Product";
                manageProductWindow.ShowDialog();
                dgProduct.ItemsSource = null;
                dgProduct.ItemsSource = _productService.GetAllProducts().ToList();
            }
            else if (_currentMode == ManageMode.Category)
            {
                ManageCategoryWindow manageCategoryWindow = new();
                manageCategoryWindow.lblEditor.Text = "Create New Category";
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
            else if (_currentMode == ManageMode.Employee)
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
            else if (_currentMode == ManageMode.Product)
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
            else if (_currentMode == ManageMode.Category)
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
                    MessageBox.Show("Please select a product to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("Please select a category to delete", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        #endregion
    }
}