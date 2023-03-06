using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAttendanceLogTypeRepository
    {
        public Task<AttendanceLogType> GetAttendanceLogType(int id);
        public Task<AttendanceLogType> GetAttendanceLogType(string name);
        public int GetAttendanceLogType(DateTime date, Employee employee);
    }
}
