using System.ComponentModel.DataAnnotations;

namespace AttendaceManagementSystemWebAPI.Models
{
    public class EmployeeRole
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
