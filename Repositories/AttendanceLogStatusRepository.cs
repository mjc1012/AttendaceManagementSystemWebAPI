using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class AttendanceLogStatusRepository : IAttendanceLogStatusRepository
    {
        private readonly DataContext _context;
        public AttendanceLogStatusRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AttendanceLogStatus> GetAttendanceLogStatus(int id)
        {
            try
            {
                return await _context.AttendanceLogStatuses.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AttendanceLogStatus> GetAttendanceLogStatus(string name)
        {
            try
            {
                return await _context.AttendanceLogStatuses.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
