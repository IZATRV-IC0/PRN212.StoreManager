using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class EmployeeManagementService
    {
        CustomerManagementService _customerManagementService;
        public EmployeeManagementService()
        {
            _customerManagementService = new CustomerManagementService();
        }
        public List<StoreManagement.DAL.Entities.Employee> GetAllEmployees()
        {
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            if (employeeRepository == null)
            {
                throw new InvalidOperationException("Employee repository is not initialized or whether the database is not connected.");
            }
            return employeeRepository.GetAll();
        }
        public Employee Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            var employee = employeeRepository.GetEmployeeByUserNameAndPassword(userName, password);
            return employee;
        }
        public List<StoreManagement.DAL.Entities.Employee> SearchEmployeesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Search name cannot be null or empty", nameof(name));
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            return employeeRepository.SearchByName(name);
        }
        public void AddEmployee(StoreManagement.DAL.Entities.Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Add(employee);
        }
        public void UpdateEmployee(StoreManagement.DAL.Entities.Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Update(employee);
        }
        public void DeleteEmployee(int employeeId)
        {
            if (employeeId < 0)
            {
                throw new ArgumentException("Invalid employee ID", nameof(employeeId));
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Delete(employeeId);
        }
    }
}
