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
    /// Interaction logic for CustomerProfileWindow.xaml
    /// </summary>
    public partial class CustomerProfileWindow : Window
    {
        private Customer _customer;
        private CustomerManagementService _customerService;

        public CustomerProfileWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _customerService = new CustomerManagementService();
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            txtCustomerId.Text = _customer.CustomerId.ToString();
            txtCompanyName.Text = _customer.CompanyName;
            txtContactName.Text = _customer.ContactName;
            txtContactTitle.Text = _customer.ContactTitle;
            txtAddress.Text = _customer.Address;
            txtPhone.Password = _customer.Phone;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
                {
                    MessageBox.Show("Company name is required.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContactName.Text))
                {
                    MessageBox.Show("Contact name (username) is required.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPhone.Password))
                {
                    MessageBox.Show("Phone (password) is required.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update customer object
                _customer.CompanyName = txtCompanyName.Text.Trim();
                _customer.ContactName = txtContactName.Text.Trim();
                _customer.ContactTitle = txtContactTitle.Text.Trim();
                _customer.Address = txtAddress.Text.Trim();
                _customer.Phone = txtPhone.Password.Trim();

                // Save to database
                _customerService.UpdateCustomer(_customer);

                MessageBox.Show("Profile updated successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
