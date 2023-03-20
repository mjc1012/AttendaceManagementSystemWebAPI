using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAttendanceLogStateRepository
    {

        public Task<AttendanceLogState> GetAttendanceLogState(int id);
        public Task<AttendanceLogState> GetAttendanceLogState(string name);
    }
}
