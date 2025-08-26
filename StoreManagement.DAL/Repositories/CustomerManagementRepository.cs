﻿using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Entities;
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
        public Customer GetCustomerByUserNameAndPassword(string userName, string password)
        {
            return _context.Customers.FirstOrDefault(c => c.ContactName == userName && c.Phone == password);
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
        public bool Delete(int customerId)
        {
            try {
                var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
                 return true;
                }
            }
            catch (DbUpdateException ex)
            {
                // Handle the exception (e.g., log it, rethrow it, etc.)
                return false;
            }
            return false;
        }
    }
}
