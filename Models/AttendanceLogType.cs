﻿using System.ComponentModel.DataAnnotations;

namespace AttendaceManagementSystemWebAPI.Models
{
    public class AttendanceLogType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; }
    }
}
