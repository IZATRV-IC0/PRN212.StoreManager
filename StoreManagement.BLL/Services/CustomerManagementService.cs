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
        public List<StoreManagement.DAL.Models.Customer> GetAllCustomers()
        {
            if (_customerManagementRepository == null)
            {
                throw new InvalidOperationException("Customer repository is not initialized or whether the database is not connected.");
            }
            return _customerManagementRepository.GetAll();
        }
        public List<StoreManagement.DAL.Models.Customer> SearchCustomersByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Search name cannot be null or empty", nameof(name));
            }
            return _customerManagementRepository.SearchByName(name);
        }
        public void AddCustomer(StoreManagement.DAL.Models.Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
            _customerManagementRepository.Add(customer);
        }
        public void UpdateCustomer(StoreManagement.DAL.Models.Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
            _customerManagementRepository.Update(customer);
        }
        public void DeleteCustomer(int customerId)
        {
            if (customerId < 0)
            {
                throw new ArgumentException("Invalid customer ID", nameof(customerId));
            }
            _customerManagementRepository.Delete(customerId);
        }
    }
}
