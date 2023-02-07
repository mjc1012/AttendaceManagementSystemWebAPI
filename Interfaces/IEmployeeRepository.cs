using AttendaceManagementSystemWebAPI.Models;
using System;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IEmployeeRepository
    {
        ICollection<Employee> GetEmployees();

        ICollection<Employee> GetEmployeesByType(bool isAdmin);

        Employee GetEmployee(int id);

        Employee GetEmployee(string firstname, string lastname);

        Employee GetEmployee(string firstname, string middlename, string lastname);

        Employee GetEmployee(string employeeId);


        bool EmployeeExists(int id);

        bool EmployeeExists(string firstname, string middlename, string lastname);

        bool IsEmployeeAdmin(int id);

        bool CreateEmployee(Employee employee);

        bool UpdateEmployee(Employee employee);

        bool DeleteEmployee(Employee employee);

        bool Save();
    }
}
