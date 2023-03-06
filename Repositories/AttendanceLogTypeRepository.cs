using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class AttendanceLogTypeRepository : IAttendanceLogTypeRepository
    {
        private readonly DataContext _context;
        public AttendanceLogTypeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AttendanceLogType> GetAttendanceLogType(string name)
        {
            try
            {
                return await _context.AttendanceLogTypes.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AttendanceLogType> GetAttendanceLogType(int id)
        {
            try
            {
                return await _context.AttendanceLogTypes.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetAttendanceLogType(DateTime date, Employee employee)
        {
            try
            {
                int count = _context.AttendanceLogs.Where(p => p.Employee == employee && p.TimeLog.Year == date.Year && p.TimeLog.Month == date.Month && p.TimeLog.Day == date.Day).Count();
                if (count == 1)
                {
                    return 2;
                }
                else if( count == 0)
                {
                    return 1;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
