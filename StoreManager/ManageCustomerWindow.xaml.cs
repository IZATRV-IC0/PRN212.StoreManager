using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
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

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for ManageCustomerWindow.xaml
    /// </summary>
    public partial class ManageCustomerWindow : Window
    {
        CustomerManagementService _customerManagementService;
        public Customer customerEdit { get; set; } = null;
        public ManageCustomerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (customerEdit != null)
            {
                FillCustomerForm(customerEdit);
                txtCustomerID.IsEnabled = false; // Disable editing of CustomerID when updating
            }
            else
            {
                lblCustomerID.Visibility = Visibility.Hidden;//hide CustomerID field and label when creating new customer
                txtCustomerID.Visibility = Visibility.Hidden;
            }
        }

        private void btn_CancelCustomer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtCompanyName.Text) || string.IsNullOrEmpty(txtContactName.Text) ||
                string.IsNullOrEmpty(txtContactTitle.Text) || string.IsNullOrEmpty(txtAddress.Text) ||
                string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        public void FillCustomerForm(Customer customer)
        {
            customerEdit = customer;
            txtCustomerID.Text = customer.CustomerId.ToString();
            txtCompanyName.Text = customer.CompanyName;
            txtContactName.Text = customer.ContactName;
            txtContactTitle.Text = customer.ContactTitle;
            txtAddress.Text = customer.Address;
            txtPhone.Text = customer.Phone;
        }

        private void btn_SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }
            if (customerEdit != null)
            {
                //Update existing customer
                customerEdit.CompanyName = txtCompanyName.Text.Trim();
                customerEdit.ContactName = txtContactName.Text.Trim();
                customerEdit.ContactTitle = txtContactTitle.Text.Trim();
                customerEdit.Address = txtAddress.Text.Trim();
                customerEdit.Phone = txtPhone.Text.Trim();
                _customerManagementService = new CustomerManagementService();
                _customerManagementService.UpdateCustomer(customerEdit);
                MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                //Create new customer
                Customer newCustomer = new Customer
                {
                    CompanyName = txtCompanyName.Text.Trim(),
                    ContactName = txtContactName.Text.Trim(),
                    ContactTitle = txtContactTitle.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                };
                _customerManagementService = new CustomerManagementService();
                _customerManagementService.AddCustomer(newCustomer);
                MessageBox.Show("Customer saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}
