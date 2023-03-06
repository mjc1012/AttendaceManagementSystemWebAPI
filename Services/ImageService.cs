using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using System.Drawing;

namespace AttendaceManagementSystemWebAPI.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveImage(string base64String)
        {
            try
            {
                var path = Path.Combine(_environment.ContentRootPath, "Attendance Pictures");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);

                string uniqueString = Guid.NewGuid().ToString();
                // create a unique filename here
                var newFileName = uniqueString + ".jpg";
                var fileWithPath = Path.Combine(path, newFileName);
                image.Save(fileWithPath);
                ms.Close();
                return newFileName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string SaveImage(IFormFile imageFile)
        {
            try
            {
                var path = Path.Combine(_environment.ContentRootPath, "Profile Pictures");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check the allowed extenstions
                var ext = Path.GetExtension(imageFile.FileName);
                
                string uniqueString = Guid.NewGuid().ToString();
                // we are trying to create a unique filename here
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();
                return newFileName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteImage(AttendanceLog attendanceLog)
        {
            try
            {

                var path = Path.Combine(_environment.ContentRootPath, "Attendance Pictures" + "\\" + attendanceLog.ImageName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteImage(Employee employee)
        {
            try
            {

                var path = Path.Combine(_environment.ContentRootPath, "Profile Pictures" + "\\" + employee.ProfilePictureImageName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
