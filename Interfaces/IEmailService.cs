using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Email email);
    }
}
