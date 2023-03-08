using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAttendanceLogStatusRepository
    {
        public Task<AttendanceLogStatus> GetAttendanceLogStatus(int id);
        public Task<AttendanceLogStatus> GetAttendanceLogStatus(string name);
    }
}
