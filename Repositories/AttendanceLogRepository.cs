using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class AttendanceLogRepository : IAttendanceLogRepository
    {
        private readonly DataContext _context;
        public AttendanceLogRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<List<AttendanceLog>> GetAttendanceLogs()
        {
            try
            {
                return await _context.AttendanceLogs.OrderByDescending(p => p.TimeLog).ThenBy(p => p.Employee.LastName).ThenBy(p => p.Employee.MiddleName).ThenBy(p => p.Employee.FirstName).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.AttendanceLogState).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<List<AttendanceLog>> GetAttendanceLogs(List<int> ids)
        {
            try
            {
                return await _context.AttendanceLogs.Where(p => ids.Contains(p.Id)).OrderByDescending(p => p.TimeLog).ThenBy(p => p.Employee.LastName).ThenBy(p => p.Employee.MiddleName).ThenBy(p => p.Employee.FirstName).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.AttendanceLogState).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

            public async Task<List<AttendanceLog>> GetAttendanceLogs(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.AttendanceLogs.Where(p => p.TimeLog >= startDate && p.TimeLog <= endDate).OrderByDescending(p => p.TimeLog).ThenBy(p => p.Employee.LastName).ThenBy(p => p.Employee.MiddleName).ThenBy(p => p.Employee.FirstName).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogState).Include(p => p.AttendanceLogStatus).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AttendanceLog>> GetAttendanceLogs(DateTime startDate, DateTime endDate, int attendaceLogTypeId)
        {
            try
            {
                return await _context.AttendanceLogs.Where(p => p.TimeLog >= startDate && p.TimeLog <= endDate && p.AttendanceLogTypeId == attendaceLogTypeId).OrderByDescending(p => p.TimeLog).ThenBy(p => p.Employee.LastName).ThenBy(p => p.Employee.MiddleName).ThenBy(p => p.Employee.FirstName).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.AttendanceLogState).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AttendanceLog>> GetAttendanceLogs(int employeeId)
        {
            try
            {
                return await _context.AttendanceLogs.Where(p => p.Employee.Id == employeeId).OrderByDescending(p => p.TimeLog).ThenBy(p => p.Employee.LastName).ThenBy(p => p.Employee.MiddleName).ThenBy(p => p.Employee.FirstName).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.AttendanceLogState).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }



        public void DetachLog(AttendanceLog log)
        {
            try
            {
                _context.Entry(log).State = EntityState.Detached;
            }
            catch(Exception )
            {
                throw ;
            }
        }

        public async Task<AttendanceLog> GetAttendanceLog(int id)
        {
            try
            {
                return await _context.AttendanceLogs.Where(p => p.Id == id).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogState).Include(p => p.AttendanceLogStatus).Include(p => p.Employee).FirstOrDefaultAsync();
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<AttendanceLog> CreateAttendanceLog(AttendanceLog attendanceLog)
        {
            try
            {
                _context.AttendanceLogs.Add(attendanceLog);
                await _context.SaveChangesAsync();
                return attendanceLog;
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task CreateAttendanceLogVoid(AttendanceLog attendanceLog)
        {
            try
            {
                _context.AttendanceLogs.Add(attendanceLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AttendanceLog> UpdateAttendanceLog(AttendanceLog attendanceLog)
        {
            try
            {
                _context.AttendanceLogs.Update(attendanceLog);
                await _context.SaveChangesAsync();
                return attendanceLog;
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<bool> DeleteAttendanceLog(AttendanceLog attendanceLog)
        {
            try
            {
                _context.AttendanceLogs.Remove(attendanceLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<bool> DeleteAttendanceLogs(List<AttendanceLog> logs)
        {
            try
            {
                _context.AttendanceLogs.RemoveRange(logs);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
