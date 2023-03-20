using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAttendanceLogRepository
    {
        public Task<List<AttendanceLog>> GetAttendanceLogs();
        public Task<List<AttendanceLog>> GetAttendanceLogs(string pairId);



        public Task<List<AttendanceLog>> GetAttendanceLogs(List<string> ids);


        public Task<AttendanceLog> GetAttendanceLog(int id);

        public void DetachLog(AttendanceLog log);

        public Task<AttendanceLog> CreateAttendanceLog(AttendanceLog attendanceLog);

        public Task CreateAttendanceLogVoid(AttendanceLog attendanceLog);

        public Task<AttendanceLog> UpdateAttendanceLog(AttendanceLog attendanceLog);

        public Task<bool> DeleteAttendanceLog(AttendanceLog attendanceLog);

        public Task<bool> DeleteAttendanceLogs(List<AttendanceLog> logs);


    }
}
