namespace AttendaceManagementSystemWebAPI.Dto
{
    public class ImageFileDto
    {
        public string EmployeeIdNumber { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
