using AttendaceManagementSystemWebAPI.Helper;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;

namespace AttendaceManagementSystemWebAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceLogType>().HasData(
                new AttendanceLogType
                {
                    Id = 1,
                    Name = "TimeIn",
                },
                new AttendanceLogType
                {
                    Id = 2,
                    Name = "TimeOut",
                });

            modelBuilder.Entity<AttendanceLogStatus>().HasData(
               new AttendanceLogStatus
               {
                   Id = 1,
                   Name = "Present",
               },
               new AttendanceLogStatus
               {
                   Id = 2,
                   Name = "Absent",
               });

            modelBuilder.Entity<EmployeeRole>().HasData(
                new EmployeeRole
                {
                    Id = 1,
                    Name = "Admin",
                },
                new EmployeeRole
                {
                    Id = 2,
                    Name = "User",
                });

            modelBuilder.Entity<Employee>().HasData(
               new Employee
               {
                   Id = 1,
                   FirstName = "Admin",
                   MiddleName = "",
                   LastName = "Admin",
                   EmailAddress = "Admin",
                   EmployeeIdNumber = "Admin",
                   ProfilePictureImageName = "default_image.jpg",
                   Password = PasswordHasher.HashPassword("Admin@123"),
                   EmployeeRoleId = 1
               });



        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<AttendanceLog> AttendanceLogs { get; set; }

        public DbSet<AttendanceLogType> AttendanceLogTypes { get; set; }
        public DbSet<AttendanceLogStatus> AttendanceLogStatuses { get; set; }
    }
}
