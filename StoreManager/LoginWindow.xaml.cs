using StoreManagement.BLL.Services;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        EmployeeManagementService? _employeeService;
        CustomerManagementService? _customerService;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btn_Login_Employee_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                //Employee login
                _employeeService = new EmployeeManagementService();
                var employee = _employeeService.Login(username, password);
                if (employee != null)
                {
                    try
                    {
                        MainWindow dashboard = null;
                        string welcomeMessage = "";
                        
                        switch (employee.RoleNum)
                        {
                            case 1:
                                //Admin role
                                dashboard = new MainWindow(employee);
                                welcomeMessage = "Welcome, Admin!";
                                break;
                            case 2:
                                //Manager role
                                dashboard = new MainWindow(employee);
                                welcomeMessage = "Welcome, Manager!";
                                break;
                            case 3:
                                //Staff role
                                dashboard = new MainWindow(employee);
                                welcomeMessage = "Welcome, Staff!";
                                break;
                            default:
                                MessageBox.Show("Your role is not recognized. Please contact the administrator.", "Role Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                        }
                        
                        if (dashboard != null)
                        {
                            MessageBox.Show(welcomeMessage, "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                            dashboard.Show();
                            this.Close();
                        }
                    }
                    catch (Exception windowEx)
                    {
                        MessageBox.Show($"Error opening dashboard: {windowEx.Message}", "Window Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Database Connection Error: {ex.Message}\n\nPlease ensure:\n1. SQL Server is running\n2. Database exists\n3. Connection settings are correct", 
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void btn_Login_Customer_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                //Customer login
                _customerService = new CustomerManagementService();
                var customer = _customerService.Login(username, password);
                if (customer != null)
                {
                    try
                    {
                        CustomerDashboardWindow customerWindow = new CustomerDashboardWindow(customer);
                        customerWindow.Show();
                        this.Close();
                    }
                    catch (Exception windowEx)
                    {
                        MessageBox.Show($"Error opening customer dashboard: {windowEx.Message}", "Window Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Database Connection Error: {ex.Message}\n\nPlease ensure:\n1. SQL Server is running\n2. Database exists\n3. Connection settings are correct", 
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
