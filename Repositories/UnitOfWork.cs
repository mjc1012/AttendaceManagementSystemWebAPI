using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;
        public UnitOfWork(DataContext context, IWebHostEnvironment environment, IConfiguration config)
        {
            _config = config;
            _environment = environment;
            _context = context;
        }

        public IAttendanceLogRepository attendanceLogRepository => new AttendanceLogRepository(_context);
        public IAttendanceLogTypeRepository attendanceLogTypeRepository => new AttendanceLogTypeRepository(_context);
        public IAuthenticationRepository authenticationRepository => new AuthenticationRepository(_context);
        public IEmployeeRepository employeeRepository => new EmployeeRepository(_context);
        public IEmployeeRoleRepository employeeRoleRepository => new EmployeeRoleRepository(_context);
        public IImageService imageService => new ImageService(_environment);

        public IAttendanceLogStatusRepository attendanceLogStatusRepository => new AttendanceLogStatusRepository(_context);

        public IEmailService emailService => new EmailService(_config);
    }
}
