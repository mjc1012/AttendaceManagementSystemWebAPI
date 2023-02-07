namespace AttendaceManagementSystemWebAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string EmployeeId { get; set; }

        public bool IsAdmin { get; set; }

        public string Password { get; set; }
        public ICollection<AttendanceLog> AttendanceLogs { get; set; }
    }
}
