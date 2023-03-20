using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class AttendanceLogStateRepository : IAttendanceLogStateRepository
    {
        private readonly DataContext _context;
        public AttendanceLogStateRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AttendanceLogState> GetAttendanceLogState(int id)
        {
            try
            {
                return await _context.AttendanceLogStates.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AttendanceLogState> GetAttendanceLogState(string name)
        {
            try
            {
                return await _context.AttendanceLogStates.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
