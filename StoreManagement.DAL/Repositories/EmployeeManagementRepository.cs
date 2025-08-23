using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    public class EmployeeManagementRepository
    {
        LucySalesDataContext _context;
        public EmployeeManagementRepository()
        {
            _context = new();
        }
        public List<Employee> GetAll()
        {
            return _context.Employees.ToList();
        }
        public List<Employee> SearchByName(string name)
        {
            List<Employee> employees = new List<Employee>();
            if (!string.IsNullOrEmpty(name))
            {
                employees = _context.Employees
                    .Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            return employees;
        }
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }
        public void Update(Employee employee)
        {
            var existingEmployee = _context.Employees.Find(employee.EmployeeId);
            if (existingEmployee != null)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.UserName = employee.UserName;
                existingEmployee.Password = employee.Password;
                existingEmployee.JobTitle = employee.JobTitle;
                existingEmployee.BirthDate = employee.BirthDate;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.Address = employee.Address;
                _context.SaveChanges();
            }
        }
        public void Delete(int employeeId)
        {
            var employee = _context.Employees.Find(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }
    }
}
