using Microsoft.EntityFrameworkCore;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.DAL.Repositories
{
    public class EmployeeManagementRepository
    {
        readonly LucySalesDataContext _context;
        public EmployeeManagementRepository()
        {
            _context = new();
        }
        public List<StoreManagement.DAL.Entities.Employee> GetAll()
        {
            return _context.Employees.ToList();
        }
        public Employee GetEmployeeByUserNameAndPassword(string userName, string password)
        {
            return _context.Employees.FirstOrDefault(e => e.UserName == userName && e.Password == password);
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
                existingEmployee.RoleNum = employee.RoleNum;
                _context.SaveChanges();
            }
        }
        public bool Delete(int employeeId)
        {
            try
            {
                var employee = _context.Employees.Find(employeeId);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }       
    }
}

