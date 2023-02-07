namespace AttendaceManagementSystemWebAPI.Models
{
    public class AttendanceLog
    {
        public int Id { get; set; }

        public DateTime TimeLog { get; set; }

        public string AttendanceLogType { get; set; }

        public Employee Employee { get; set; }
    }
}
