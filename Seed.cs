using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }

        public void SeedDataContext()
        {
            if (!dataContext.Employees.Any())
            {
                var employees = new List<Employee>()
                {
                    new Employee()
                    {
                        FirstName = "John",
                        MiddleName = "Bryant",
                        LastName = "Doe",
                        Email = "johndoe@gmail.com",
                        EmployeeId = "19-2424-665",
                        IsAdmin = true,
                        Password = "password",
                        AttendanceLogs = new List<AttendanceLog>()
                        {
                            new AttendanceLog()
                            {
                                TimeLog = new DateTime(2000, 1, 1),
                                AttendanceLogType = "TimeIn"
                            },
                            new AttendanceLog()
                            {
                                TimeLog = new DateTime(2000, 1, 1),
                                AttendanceLogType = "TimeOut"
                            },
                        }
                    },
                    new Employee()
                    {
                        FirstName = "Jane",
                        MiddleName = "Judge",
                        LastName = "Dawn",
                        Email = "janedawn@gmail.com",
                        EmployeeId = "22-5656-333",
                        IsAdmin = false,
                        Password = "secret",
                        AttendanceLogs = new List<AttendanceLog>()
                        {
                            new AttendanceLog()
                            {
                                TimeLog = new DateTime(2020, 1, 1),
                                AttendanceLogType = "TimeIn"
                            },
                            new AttendanceLog()
                            {
                                TimeLog = new DateTime(2020, 1, 1),
                                AttendanceLogType = "TimeOut"
                            },
                        }
                    }
                };

                dataContext.Employees.AddRange(employees);
                dataContext.SaveChanges();
            }
        }
    }
}
