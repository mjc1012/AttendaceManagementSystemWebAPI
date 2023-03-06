using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IEmployeeRoleRepository
    {
        public Task<EmployeeRole> GetEmployeeRole(string name);
    }
}
