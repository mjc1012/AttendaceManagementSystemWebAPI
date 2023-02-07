using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Models;
using AutoMapper;

namespace AttendaceManagementSystemWebAPI.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<AttendanceLog, AttendanceLogDto>();
            CreateMap<AttendanceLogDto, AttendanceLog>();
        }
    }
}
