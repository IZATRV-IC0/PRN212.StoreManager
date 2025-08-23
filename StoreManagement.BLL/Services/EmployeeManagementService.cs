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
        public List<StoreManagement.DAL.Models.Employee> GetAllEmployees()
        {
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            if (employeeRepository == null)
            {
                throw new InvalidOperationException("Employee repository is not initialized or whether the database is not connected.");
            }
            return employeeRepository.GetAll();
        }
        public List<StoreManagement.DAL.Models.Employee> SearchEmployeesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Search name cannot be null or empty", nameof(name));
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            return employeeRepository.SearchByName(name);
        }
        public void AddEmployee(StoreManagement.DAL.Models.Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            var employeeRepository = new StoreManagement.DAL.Repositories.EmployeeManagementRepository();
            employeeRepository.Add(employee);
        }
        public void UpdateEmployee(StoreManagement.DAL.Models.Employee employee)
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
