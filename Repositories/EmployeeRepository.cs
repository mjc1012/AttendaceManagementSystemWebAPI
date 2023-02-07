using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using System;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;
        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Employee> GetEmployees()
        {
            return _context.Employees.OrderBy(p => p.Id).ToList();
        }

        public ICollection<Employee> GetEmployeesByType(bool isAdmin)
        {
            return _context.Employees.Where(p => p.IsAdmin == isAdmin).ToList();
        }

        public Employee GetEmployee(int id)
        {
            return _context.Employees.Where(p => p.Id == id).FirstOrDefault();
        }

        public Employee GetEmployee(string firstname, string lastname)
        {
            return _context.Employees.Where(p => p.FirstName.Trim().ToUpper() == firstname.Trim().ToUpper() && p.LastName.Trim().ToUpper() == lastname.Trim().ToUpper()).FirstOrDefault();
        }

        public Employee GetEmployee(string firstname, string middlename, string lastname)
        {
            return _context.Employees.Where(p => p.FirstName.Trim().ToUpper() == firstname.Trim().ToUpper() && p.MiddleName.Trim().ToUpper() == middlename.Trim().ToUpper() && p.LastName.Trim().ToUpper() == lastname.Trim().ToUpper()).FirstOrDefault();
        }

        public Employee GetEmployee(string employeeId)
        {
            return _context.Employees.Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        }


        public bool EmployeeExists(int id)
        {
            return _context.Employees.Any(p => p.Id == id);
        }

        public bool EmployeeExists(string firstname, string middlename, string lastname)
        {
            return _context.Employees.Any(p => p.FirstName.Trim().ToUpper() == firstname.Trim().ToUpper() && p.MiddleName.Trim().ToUpper() == middlename.Trim().ToUpper() && p.LastName.Trim().ToUpper() == lastname.Trim().ToUpper());
        }

        public bool IsEmployeeAdmin(int id)
        {
            return _context.Employees.Where(p => p.Id == id).FirstOrDefault().IsAdmin;
        }

        public bool CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);

            return Save();
        }

        public bool UpdateEmployee(Employee employee)
        {
            _context.Update(employee);

            return Save();
        }

        public bool DeleteEmployee(Employee employee)
        {
            _context.Remove(employee);

            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
