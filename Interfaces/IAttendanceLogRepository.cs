using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAttendanceLogRepository
    {
        public Task<List<AttendanceLog>> GetAttendanceLogs();
        public Task<List<AttendanceLog>> GetAttendanceLogs(string employeeIdNumber);

        public Task<AttendanceLog> GetAttendanceLog(int id);

        public void DetachLog(AttendanceLog log);

        public Task<AttendanceLog> CreateAttendanceLog(AttendanceLog attendanceLog);

        public Task<AttendanceLog> UpdateAttendanceLog(AttendanceLog attendanceLog);

        public Task<bool> DeleteAttendanceLog(AttendanceLog attendanceLog);

        //bool Save();
    }
}
