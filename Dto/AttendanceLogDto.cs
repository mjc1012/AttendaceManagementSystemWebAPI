﻿using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Dto
{
    public class AttendanceLogDto
    {
        public int Id { get; set; }

        public string TimeLog { get; set; }
        public string ImageName { get; set; }

        public string Base64String { get; set; }

        public string AttendanceLogTypeName { get; set; }

        public string EmployeeIdNumber { get; set; }

        public string EmployeeName { get; set; }
    }
}