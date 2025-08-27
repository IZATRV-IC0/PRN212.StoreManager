using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Windows;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for ManageEmployeeWindow.xaml
    /// </summary>
    public partial class ManageEmployeeWindow : Window
    {
        EmployeeManagementService _employeeManagementService;
        public Employee employeeEdit { get; set; } = null;

        public ManageEmployeeWindow()
        {
            InitializeComponent();
            ImplementWPFValidation();
        }
        private void ImplementWPFValidation()
        {
            //Date picker validation (age limit, default start date, no future date, default selected value)
            dpBirthDate.DisplayDateStart = new DateTime(1900, 1, 1);
            dpBirthDate.SelectedDate = DateTime.Today.AddYears(-16);
            dpBirthDate.DisplayDateEnd = DateTime.Today.AddYears(-16);
            dpHireDate.DisplayDateStart = new DateTime(1900, 1, 1);
            dpHireDate.SelectedDate = DateTime.Today;
            dpHireDate.DisplayDateEnd = DateTime.Today;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (employeeEdit != null)
            {
                FillEmployeeForm(employeeEdit);
                txtEmployeeID.IsEnabled = false; // disable editing ID
            }
            else
            {
                lblEmployeeID.Visibility = Visibility.Hidden;
                txtEmployeeID.Visibility = Visibility.Hidden;
            }
        }

        private void btn_CancelEmployee_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("Please fill in required fields (Name, Username).",
                                "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void FillEmployeeForm(Employee employee)
        {
            employeeEdit = employee;
            txtEmployeeID.Text = employee.EmployeeId.ToString();
            txtName.Text = employee.Name;
            txtUserName.Text = employee.UserName;
            txtPassword.Text = employee.Password;
            txtJobTitle.Text = employee.JobTitle;
            dpBirthDate.SelectedDate = employee.BirthDate;
            dpHireDate.SelectedDate = employee.HireDate;
            txtAddress.Text = employee.Address;
            txtRoleNum.Text = employee.RoleNum?.ToString();
        }

        private void btn_SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            _employeeManagementService = new EmployeeManagementService();

            if (employeeEdit != null)
            {
                // Update
                employeeEdit.Name = txtName.Text.Trim();
                employeeEdit.UserName = txtUserName.Text.Trim();
                employeeEdit.Password = txtPassword.Text.Trim();
                employeeEdit.JobTitle = txtJobTitle.Text.Trim();
                employeeEdit.BirthDate = dpBirthDate.SelectedDate;
                employeeEdit.HireDate = dpHireDate.SelectedDate;
                employeeEdit.Address = txtAddress.Text.Trim();
                employeeEdit.RoleNum = int.TryParse(txtRoleNum.Text, out int role) ? role : null;

                _employeeManagementService.UpdateEmployee(employeeEdit);
                MessageBox.Show("Employee updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Create
                Employee newEmployee = new Employee
                {
                    Name = txtName.Text.Trim(),
                    UserName = txtUserName.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    JobTitle = txtJobTitle.Text.Trim(),
                    BirthDate = dpBirthDate.SelectedDate,
                    HireDate = dpHireDate.SelectedDate,
                    Address = txtAddress.Text.Trim(),
                    RoleNum = int.TryParse(txtRoleNum.Text, out int role) ? role : null
                };

                _employeeManagementService.AddEmployee(newEmployee);
                MessageBox.Show("Employee saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Close();
        }
    }
}
