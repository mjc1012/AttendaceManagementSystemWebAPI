using AttendaceManagementSystemWebAPI.Models;
using System;
using System.Security.Claims;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IEmployeeRepository
    {
        public Task<List<Employee>> GetEmployees();

        public void DetachEmployee(Employee employee);

        public Task<List<Employee>> GetEmployees(List<string> employeeIdNumbers);
        public Task<Employee> GetEmployee(int id);
        public List<Employee> GetEmployeesAbsentOnDate(DateTime date, int logTypeId);

        public Task<Employee> GetEmployee(string employeeIdNumber);

        public Task<Employee> GetEmployeeByEmail(string email);

        public Task<bool> EmployeeExists(string employeeIdNumber, string password);

        public Task<bool> EmployeeIdNumberExists(string employeeIdNumber);

        public Task<bool> EmailAddressExists(string emailAddress);

        public Task<Employee> CreateEmployee(Employee employee);

        public Task<Employee> UpdateEmployee(Employee employee);

        public Task<bool> DeleteEmployee(Employee employee);
        public Task<bool> DeleteEmployees(List<Employee> employee);
    }
}
