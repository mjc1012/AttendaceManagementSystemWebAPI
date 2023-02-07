using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAttendanceLogRepository
    {
        ICollection<AttendanceLog> GetAttendaceLogs();

        ICollection<AttendanceLog> GetAttendaceLogsByEmployee(int id);

        AttendanceLog GetAttendanceLog(int id);

        AttendanceLog GetAttendanceLog(DateTime timeLog, string attendanceLogType, int employeeId);

        bool AttendanceLogExists(int id);

        bool CreateAttendanceLog(AttendanceLog attendanceLog);

        bool UpdateAttendanceLog(AttendanceLog attendanceLog);

        bool DeleteAttendanceLog(AttendanceLog attendanceLog);

        bool Save();
    }
}
