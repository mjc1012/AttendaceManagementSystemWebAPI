using AttendaceManagementSystemWebAPI.Models;

namespace AttendaceManagementSystemWebAPI.Interfaces
{
    public interface IImageService
    {
        public string SaveImage(string base64String);
        public string SaveImage(IFormFile imageFile);
        public void DeleteImage(AttendanceLog attendanceLog);

        public void DeleteImage(Employee employee);
    }
}
