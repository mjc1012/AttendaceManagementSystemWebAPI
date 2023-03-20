using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Dto
{
    public class AttendanceLogDto
    {
        public int Id { get; set; }

        public string TimeLog { get; set; }
        public string ImageName { get; set; }

        public string Base64String { get; set; }

        public string AttendanceLogStateName { get; set; }

        public string AttendanceLogTypeName { get; set; }

        public string AttendanceLogStatusName { get; set; }

        public string EmployeeIdNumber { get; set; }

        public string PairId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public bool ToDelete { get; set; }
    }
}
