namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IUnitOfWork
    {
        IAttendanceLogRepository attendanceLogRepository { get; }
        IAttendanceLogTypeRepository attendanceLogTypeRepository { get; }
        IAuthenticationRepository authenticationRepository { get; }
        IEmployeeRepository employeeRepository { get; }
        IEmployeeRoleRepository employeeRoleRepository { get; }
        IImageService imageService { get; }
    }
}
