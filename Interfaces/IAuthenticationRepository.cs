using AttendaceManagementSystemWebAPI.Models;
using System.Security.Claims;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IAuthenticationRepository
    {
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken);
        public Task<bool> EmployeeRefreshTokenExists(string refreshToken);

        public Task<string> CreateRefreshToken();

        public Task<Employee> saveTokens(Employee employee, string accessToken, string refreshToken);

        public Task<Employee> saveTokens(Employee employee, string accessToken, string refreshToken, DateTime refreshTokenExpiryTime);

        public string CheckPasswordStrength(string password);

        public string CreateJwt(Employee employee);
    }
}
