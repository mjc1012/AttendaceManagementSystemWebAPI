namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IUnitOfWork
    {
        IAttendanceLogRepository attendanceLogRepository { get; }
        IAttendanceLogTypeRepository attendanceLogTypeRepository { get; }
        IAttendanceLogStatusRepository attendanceLogStatusRepository { get; }
        IAuthenticationRepository authenticationRepository { get; }
        IEmployeeRepository employeeRepository { get; }
        IEmployeeRoleRepository employeeRoleRepository { get; }
        IImageService imageService { get; }

        IEmailService emailService { get; } 
    }
}
