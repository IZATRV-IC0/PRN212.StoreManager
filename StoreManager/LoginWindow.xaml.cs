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
            //Employee login
            _employeeService = new EmployeeManagementService();
            var employee = _employeeService.Login(username, password);
            if (employee != null)
            {
                switch (employee.RoleNum)
                {
                    case 1:
                        //Admin role
                        MainWindow adminDashboard = new MainWindow(employee);
                        MessageBox.Show("Welcome, Admin!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        adminDashboard.Show();
                        this.Close();
                        break;
                    case 3:
                        //Staff role
                        MainWindow staffDashboard = new MainWindow(employee);
                        MessageBox.Show("Welcome, Staff!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        staffDashboard.Show();
                        this.Close();
                        break;
                    default:
                        {
                            MessageBox.Show("Your role is not recognized. Please contact the administrator.", "Role Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }
                }
                return;
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //Customer login
            _customerService = new CustomerManagementService();
            var customer = _customerService.Login(username, password);
            if (customer != null)
            {
                MainWindow customerWindow = new MainWindow(customer);
                customerWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
