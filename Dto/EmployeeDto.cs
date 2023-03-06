namespace AttendaceManagementSystemWebAPI.Dto
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string EmployeeIdNumber { get; set; }

        public string EmployeeRoleName   { get; set; }
        public string ProfilePictureImageName { get; set; }
        public string Password { get; set; }

        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public IFormFile ImageFile { get; set; }

    }
}
