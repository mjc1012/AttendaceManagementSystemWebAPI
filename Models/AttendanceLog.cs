using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendaceManagementSystemWebAPI.Models
{
    public class AttendanceLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime TimeLog { get; set; }
        public string ImageName { get; set; }

        public int AttendanceLogTypeId { get; set; }

        public virtual AttendanceLogType AttendanceLogType { get; set; }

        public int AttendanceLogStatusId { get; set; }

        public virtual AttendanceLogStatus AttendanceLogStatus { get; set; }

        public int AttendanceLogStateId { get; set; }

        public virtual AttendanceLogState AttendanceLogState { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

    }
}
