using StoreManagement.DAL.Entities;
using StoreManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class EmployeeManagementService
    {
        EmployeeManagementRepository _employeeManagementRepo;
        public EmployeeManagementService()
        {
            _employeeManagementRepo = new EmployeeManagementRepository();
        }
        public List<Employee> GetAllEmployees()
        {
            _employeeManagementRepo = new EmployeeManagementRepository();
            if (_employeeManagementRepo == null)
            {
                throw new InvalidOperationException("Employee repository is not initialized or whether the database is not connected.");
            }
            return _employeeManagementRepo.GetAll();
        }
        public Employee Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty");
            }
            _employeeManagementRepo = new EmployeeManagementRepository();
            var employee = _employeeManagementRepo.GetEmployeeByUserNameAndPassword(userName, password);
            return employee;
        }
        public List<Employee> SearchEmployeesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Search name cannot be null or empty", nameof(name));
            }
            _employeeManagementRepo = new EmployeeManagementRepository();
            return _employeeManagementRepo.SearchByName(name);
        }
        public void AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            _employeeManagementRepo = new EmployeeManagementRepository();
            _employeeManagementRepo.Add(employee);
        }
        public void UpdateEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            _employeeManagementRepo = new EmployeeManagementRepository();
            _employeeManagementRepo.Update(employee);
        }
        public bool DeleteEmployee(int employeeId)
        {
            _employeeManagementRepo = new EmployeeManagementRepository();
            if (employeeId < 0)
            {
                return false;
            }
            if(_employeeManagementRepo.Delete(employeeId))
            {
                return true;
            }
            return false;
        }
    }
}

