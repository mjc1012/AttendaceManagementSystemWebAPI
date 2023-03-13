using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Helper;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using AttendaceManagementSystemWebAPI.Repositories;
using AttendaceManagementSystemWebAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Globalization;
using System.Security.Cryptography;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public EmployeeController(IUnitOfWork uow, IMapper mapper, IConfiguration config)
        {
            _uow = uow;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] EmployeeDto request)
        {
            ResponseDto<TokenDto> response;
            try
            {
                Employee employee = await _uow.employeeRepository.GetEmployee(request.EmployeeIdNumber);

                if (employee == null || !PasswordHasher.VerifyPassword(request.Password, employee.Password))
                {
                    response = new ResponseDto<TokenDto>() { Status = false, Message = "User Not Found" };
                }
                else
                {
                    string accessToken = _uow.authenticationRepository.CreateJwt(employee);
                    string refreshToken = await _uow.authenticationRepository.CreateRefreshToken();
                    DateTime refreshTokenExpiryTime = DateTime.Now.AddDays(5);
                    Employee employeeWithToken = await _uow.authenticationRepository.saveTokens(employee, accessToken, refreshToken, refreshTokenExpiryTime);
                    TokenDto token = new() {
                        AccessToken = employeeWithToken.AccessToken,
                        RefreshToken = employeeWithToken.RefreshToken
                    };
                    response = new ResponseDto<TokenDto>() { Status = true, Message = "User Found", Value = token };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<TokenDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("authenticate-for-attendance")]
        public async Task<IActionResult> AuthenticateForAttendance([FromBody] EmployeeDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                Employee employee = await _uow.employeeRepository.GetEmployee(request.EmployeeIdNumber);

                if (employee == null || !PasswordHasher.VerifyPassword(request.Password, employee.Password))
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "User Not Found" };
                }
                else
                {
                    response = new ResponseDto<EmployeeDto>() { Status = true, Message = "User Found", Value = _mapper.Map<EmployeeDto>(employee) };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            ResponseDto<TokenDto> response;
            try
            {
                var principal = _uow.authenticationRepository.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
                var idNumber = principal.Identity.Name;
                Employee employee = await _uow.employeeRepository.GetEmployee(idNumber);
                if (employee == null || employee.RefreshToken != tokenDto.RefreshToken || employee.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    response = new ResponseDto<TokenDto>() { Status = false, Message = "Invalid Request" };
                }
                else
                {
                    string newAccessToken = _uow.authenticationRepository.CreateJwt(employee);
                    string newRefreshToken = await _uow.authenticationRepository.CreateRefreshToken();
                    Employee employeeWithToken = await _uow.authenticationRepository.saveTokens(employee, newAccessToken, newRefreshToken);
                    TokenDto token = new()
                    {
                        AccessToken = employeeWithToken.AccessToken,
                        RefreshToken = employeeWithToken.RefreshToken
                    };
                    response = new ResponseDto<TokenDto>() { Status = true, Message = "User Found", Value = token };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<TokenDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            ResponseDto<List<EmployeeDto>> response;
            try
            {
                List<EmployeeDto> employees = _mapper.Map<List<EmployeeDto>>(await _uow.employeeRepository.GetEmployees());

                if (employees.Count > 0)
                {
                    response = new ResponseDto<List<EmployeeDto>>() { Status = true, Message = "Got All Employees", Value = employees };
                }
                else
                {
                    response = new ResponseDto<List<EmployeeDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<EmployeeDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [Authorize]
        [HttpGet("date/{date}")]
        public async Task<IActionResult> RecordAbsencesOnDate(string date)
        {
            ResponseDto<bool> response;
            try
            {
                List<Employee> employeesTimeInAbsent = _uow.employeeRepository.GetEmployeesAbsentOnDate(DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture), 1);
                List<Employee> employeesTimeOutAbsent = _uow.employeeRepository.GetEmployeesAbsentOnDate(DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture), 2);

                if(employeesTimeInAbsent.Count > 0 || employeesTimeOutAbsent.Count > 0)
                {
                    foreach (Employee employee in employeesTimeInAbsent)
                    {
                        await _uow.attendanceLogRepository.CreateAttendanceLogVoid(new AttendanceLog
                        {
                            TimeLog = DateTime.Now,
                            ImageName = "default_image.jpg",
                            AttendanceLogStatusId = 2,
                            EmployeeId = employee.Id,
                            AttendanceLogTypeId = 1
                        });
                    }

                    foreach (Employee employee in employeesTimeOutAbsent)
                    {
                        await _uow.attendanceLogRepository.CreateAttendanceLogVoid(new AttendanceLog
                        {
                            TimeLog = DateTime.Now,
                            ImageName = "default_image.jpg",
                            AttendanceLogStatusId = 2,
                            EmployeeId = employee.Id,
                            AttendanceLogTypeId = 2
                        });
                    }

                    response = new ResponseDto<bool>() { Status = true, Message = "Successfully Added Absences" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Everyone is Persent" };
                }
                

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [Authorize]
        [HttpGet("{employeeIdNumber}")]
        public async Task<IActionResult> GetEmployee(string employeeIdNumber)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                EmployeeDto employees = _mapper.Map<EmployeeDto>(await _uow.employeeRepository.GetEmployee(employeeIdNumber));

                if (employees != null)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Got Employee Data", Value = employees };
                }
                else
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                if (await _uow.employeeRepository.EmployeeIdNumberExists(request.EmployeeIdNumber))
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Id Number Already Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                if (await _uow.employeeRepository.EmailAddressExists(request.EmailAddress))
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Email Address Already Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                request.ProfilePictureImageName = "default_image.jpg";
                request.Password = PasswordHasher.HashPassword(request.FirstName.Replace(" ", String.Empty) + "Alliance" + request.LastName + "@123");
                Employee employee = _mapper.Map<Employee>(request);
                employee.EmployeeRole = await _uow.employeeRoleRepository.GetEmployeeRole(request.EmployeeRoleName);

                Employee employeeCreated = await _uow.employeeRepository.CreateEmployee(employee);

                if (employeeCreated.Id != 0)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Employee Created", Value = _mapper.Map<EmployeeDto>(employeeCreated) };
                }
                else
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Could not create Person" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                Employee oldEmployee = await _uow.employeeRepository.GetEmployee(request.Id);
                oldEmployee.FirstName = request.FirstName;
                oldEmployee.MiddleName = request.MiddleName;
                oldEmployee.LastName = request.LastName;
                oldEmployee.EmailAddress = request.EmailAddress;
                if (request.EmployeeRoleName != null || request.EmployeeRoleName != "") oldEmployee.EmployeeRole = await _uow.employeeRoleRepository.GetEmployeeRole(request.EmployeeRoleName);

                _uow.employeeRepository.DetachEmployee(oldEmployee);
                Employee employeeEdited = await _uow.employeeRepository.UpdateEmployee(oldEmployee);

                response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Employee Updated", Value = _mapper.Map<EmployeeDto>(employeeEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] EmployeeDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                string passwordError = _uow.authenticationRepository.CheckPasswordStrength(request.Password);
                if (passwordError != "")
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = passwordError };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                Employee oldEmployee = await _uow.employeeRepository.GetEmployee(request.EmployeeIdNumber);
                oldEmployee.Password = PasswordHasher.HashPassword(request.Password);
                _uow.employeeRepository.DetachEmployee(oldEmployee);
                Employee employeeEdited = await _uow.employeeRepository.UpdateEmployee(oldEmployee);

                response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Password Updated", Value = _mapper.Map<EmployeeDto>(employeeEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [Authorize]
        [HttpPut("profile-pic")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] ImageFileDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                Employee oldEmployee = await _uow.employeeRepository.GetEmployee(request.EmployeeIdNumber);
                if (oldEmployee.ProfilePictureImageName != "default_image.jpg") _uow.imageService.DeleteImage(oldEmployee);
                oldEmployee.ProfilePictureImageName = _uow.imageService.SaveImage(request.ImageFile);

                _uow.employeeRepository.DetachEmployee(oldEmployee);
                Employee employeeEdited = await _uow.employeeRepository.UpdateEmployee(oldEmployee);

                response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Profile Picture Updated", Value = _mapper.Map<EmployeeDto>(employeeEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            ResponseDto<bool> response;
            try
            {

                Employee employee = await _uow.employeeRepository.GetEmployee(id);
                if (employee.ProfilePictureImageName != "default_image.jpg") _uow.imageService.DeleteImage(employee);
                bool deleted = await _uow.employeeRepository.DeleteEmployee(employee);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Employee Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Could not delete" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut("deleteEmployees")]
        public async Task<IActionResult> DeleteEmployees([FromBody] DeleteRangeDto deleteRange)
        {
            ResponseDto<bool> response;
            try
            {

                List<Employee> employees = _mapper.Map<List<Employee>>(await _uow.employeeRepository.GetEmployees(deleteRange.IdNumbers));
                bool deleted = await _uow.employeeRepository.DeleteEmployees(employees);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Employee Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Could not delete" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            ResponseDto<bool> response;
            try
            {

                Employee employee = await _uow.employeeRepository.GetEmployeeByEmail(email);

                if (employee != null)
                {
                    var tokenBytes = RandomNumberGenerator.GetBytes(64);
                    var emailToken = Convert.ToBase64String(tokenBytes);
                    employee.ResetPasswordToken = emailToken;
                    employee.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
                    string from = _config["EmailSettings:From"];
                    var emailObj = new Email(email, "Reset Password", EmailBody.EmailStringBody(email, emailToken));
                    _uow.emailService.SendEmail(emailObj);
                    _uow.employeeRepository.DetachEmployee(employee);
                    Employee employeeUpdated = await _uow.employeeRepository.UpdateEmployee(employee);
                    response = new ResponseDto<bool>() { Status = true, Message = "Email Sent" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Email Does not Exist" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPasssword([FromBody] ResetPasswordDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                var newToken = request.EmailToken.Replace(" ", "+");
                string passwordError = _uow.authenticationRepository.CheckPasswordStrength(request.NewPassword);
                if (passwordError != "")
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = passwordError };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                Employee oldEmployee = await _uow.employeeRepository.GetEmployeeByEmail(request.Email);
                var tokenCode = oldEmployee.ResetPasswordToken;
                var emailTokenExpiry = oldEmployee.ResetPasswordExpiry;

                if (oldEmployee == null || tokenCode != newToken || emailTokenExpiry < DateTime.Now)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Invalid Request" };
                }
                else
                {
                    oldEmployee.Password = PasswordHasher.HashPassword(request.NewPassword);
                    _uow.employeeRepository.DetachEmployee(oldEmployee);
                    Employee employeeEdited = await _uow.employeeRepository.UpdateEmployee(oldEmployee);

                    response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Password Reset is Successful", Value = _mapper.Map<EmployeeDto>(employeeEdited) };
                }

                

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<EmployeeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}
