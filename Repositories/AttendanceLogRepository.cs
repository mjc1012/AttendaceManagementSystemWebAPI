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
                return await _context.AttendanceLogs.OrderBy(p => p.Id).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<List<AttendanceLog>> GetAttendanceLogs(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.AttendanceLogs.Where(p => p.TimeLog >= startDate && p.TimeLog <= endDate).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.Employee).ToListAsync();
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
                return await _context.AttendanceLogs.Where(p => p.TimeLog >= startDate && p.TimeLog <= endDate && p.AttendanceLogTypeId == attendaceLogTypeId).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.Employee).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<Int32> GetAttendanceLogsCount(DateTime startDate, DateTime endDate, int attendaceLogTypeId)
        //{
        //    try
        //    {
        //        return await _context.AttendanceLogs.Where(p => p.TimeLog >= startDate && p.TimeLog <= endDate && p.AttendanceLogTypeId == attendaceLogTypeId).Include(p => p.AttendanceLogType).Include(p => p.Employee).CountAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public double NumberOfWeekdays(DateTime startDate, DateTime endDate)
        //{
        //    var startD = startDate.Date;
        //    var endD = endDate.Date;


        //    double calcBusinessDays =
        //1 + ((endD - startD).TotalDays * 5 -
        //(startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

        //    if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
        //    if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

        //    return calcBusinessDays;
        //}

        public async Task<List<AttendanceLog>> GetAttendanceLogs(string employeeIdNumber)
        {
            try
            {
                return await _context.AttendanceLogs.OrderBy(p => p.Id).Include(p => p.AttendanceLogType).Include(p => p.AttendanceLogStatus).Include(p => p.Employee).Where(p => p.Employee.EmployeeIdNumber == employeeIdNumber).ToListAsync();
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
                return await _context.AttendanceLogs.Where(p => p.Id == id).Include(p => p.AttendanceLogType).Include(p => p.Employee).FirstOrDefaultAsync();
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
    }
}
