using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class AttendanceLogRepository : IAttendanceLogRepository
    {
        private readonly DataContext _context;
        public AttendanceLogRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<AttendanceLog> GetAttendaceLogs()
        {
            return _context.AttendanceLogs.OrderBy(p => p.Id).ToList();
        }

        public ICollection<AttendanceLog> GetAttendaceLogsByEmployee(int id)
        {
            return _context.AttendanceLogs.Where(p => p.Employee.Id == id).ToList();
        }

        public AttendanceLog GetAttendanceLog(int id)
        {
            return _context.AttendanceLogs.Where(p => p.Id == id).FirstOrDefault();
        }

        public AttendanceLog GetAttendanceLog(DateTime timeLog, string attendanceLogType, int employeeId)
        {
            return _context.AttendanceLogs.Where(p => p.TimeLog == timeLog && p.AttendanceLogType.Trim().ToUpper() == attendanceLogType.Trim().ToUpper() && p.Employee.Id == employeeId).FirstOrDefault();
        }
        public bool AttendanceLogExists(int id)
        {
            return _context.AttendanceLogs.Any(p => p.Id == id);
        }

        public bool CreateAttendanceLog(AttendanceLog attendanceLog)
        {
            _context.AttendanceLogs.Add(attendanceLog);

            return Save();
        }

        public bool UpdateAttendanceLog(AttendanceLog attendanceLog)
        {
            _context.Update(attendanceLog);

            return Save();
        }

        public bool DeleteAttendanceLog(AttendanceLog attendanceLog)
        {
            _context.Remove(attendanceLog);

            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
