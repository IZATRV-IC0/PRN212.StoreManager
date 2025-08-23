using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    public class CustomerManagementRepository
    {
        LucySalesDataContext _context;
        public CustomerManagementRepository()
        {
            _context = new();
        }
        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }
        public List<Customer> SearchByName(string name)
        {
            List<Customer> customers = new List<Customer>();
            if (!string.IsNullOrEmpty(name))
            {
                customers = _context.Customers
                    .Where(c => c.ContactName.ToLower().Contains(name.ToLower())).ToList();
            }
            return customers;
        }
        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
        public void Update(Customer customer)
        {
            var existingCustomer = _context.Customers.Find(customer.CustomerId);
            if (existingCustomer != null)
            {
                existingCustomer.CompanyName = customer.CompanyName;
                existingCustomer.ContactName = customer.ContactName;
                existingCustomer.ContactTitle = customer.ContactTitle;
                existingCustomer.Address = customer.Address;
                existingCustomer.Phone = customer.Phone;
                _context.SaveChanges();
            }
        }
        public void Delete(int customerId)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }
    }
}
