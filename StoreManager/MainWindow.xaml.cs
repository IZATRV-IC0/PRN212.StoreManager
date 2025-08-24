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
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
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
    }
}