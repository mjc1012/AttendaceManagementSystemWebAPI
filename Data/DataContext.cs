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

            modelBuilder.Entity<EmployeeRole>().HasData(
                new AttendanceLogType
                {
                    Id = 1,
                    Name = "Admin",
                },
                new AttendanceLogType
                {
                    Id = 2,
                    Name = "User",
                });

        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<AttendanceLog> AttendanceLogs { get; set; }

        public DbSet<AttendanceLogType> AttendanceLogTypes { get; set; }
    }
}
