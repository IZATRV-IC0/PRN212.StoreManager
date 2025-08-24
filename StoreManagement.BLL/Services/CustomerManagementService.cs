using StoreManagement.DAL.Entities;
using StoreManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class CustomerManagementService
    {
        CustomerManagementRepository _customerManagementRepository;
        public CustomerManagementService()
        {
            _customerManagementRepository = new CustomerManagementRepository();
        }
        public List<Customer> GetAllCustomers()
        {
            _customerManagementRepository = new CustomerManagementRepository();
            if (_customerManagementRepository == null)
            {
                throw new InvalidOperationException("Customer repository is not initialized or whether the database is not connected.");
            }
            return _customerManagementRepository.GetAll();
        }
        public Customer Login(string userName, string password)
        {
            _customerManagementRepository = new CustomerManagementRepository();
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty");
            }
            var customer = _customerManagementRepository.GetCustomerByUserNameAndPassword(userName, password);
            return customer;
        }
        public List<Customer> SearchCustomersByName(string name)
        {
            _customerManagementRepository = new CustomerManagementRepository();
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Search name cannot be null or empty", nameof(name));
            }
            return _customerManagementRepository.SearchByName(name);
        }
        public void AddCustomer(Customer customer)
        {
            _customerManagementRepository = new CustomerManagementRepository();
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
            _customerManagementRepository.Add(customer);
        }
        public void UpdateCustomer(Customer customer)
        {
            _customerManagementRepository = new CustomerManagementRepository();
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
            _customerManagementRepository.Update(customer);
        }
        public bool DeleteCustomer(int customerId)
        {
            _customerManagementRepository = new CustomerManagementRepository();
            if (customerId < 0)
            {
                return false;
            }
            if (_customerManagementRepository.Delete(customerId)) 
            { 
                return true;
            }
            return false;
        }
    }
}
