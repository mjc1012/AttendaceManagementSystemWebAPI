namespace AttendaceManagementSystemWebAPI.Dto
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }

        public string EmailToken { get; set; }
        public string NewPassword { get; set; }

    }
}
