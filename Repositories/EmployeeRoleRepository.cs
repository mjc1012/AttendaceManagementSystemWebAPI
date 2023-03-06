using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class EmployeeRoleRepository : IEmployeeRoleRepository
    {
        private readonly DataContext _context;
        public EmployeeRoleRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<EmployeeRole> GetEmployeeRole(string name)
        {
            try
            {
                return await _context.EmployeeRoles.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
