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
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public EmployeeController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _baseUrl = "https://localhost:7032/api/Person";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] EmployeeDto request)
        {
            ResponseDto<TokenDto> response;
            try
            {
                Employee employee = await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(request.EmployeeIdNumber);

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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            ResponseDto<TokenDto> response;
            try
            {
                var principal = _uow.authenticationRepository.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
                Employee employee = await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(principal.Identity.Name);
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
                            AttendanceLogTypeId = 1,
                            AttendanceLogStateId = 3
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
                            AttendanceLogTypeId = 2,
                            AttendanceLogStateId = 3
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
                EmployeeDto employees = _mapper.Map<EmployeeDto>(await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(employeeIdNumber));

                if (employees != null)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Got Employees", Value = employees };
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
        public async Task<IActionResult> CreateEmployee([FromForm] EmployeeDto request)
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

                try
                {
                    var emailObj = new Email(request.EmailAddress, "Checking Email", EmailBody.CheckEmailBody());
                    _uow.emailService.SendEmail(emailObj);
                }
                catch (Exception)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Cannot contact Email" };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                if(request.ImageFile == null)
                {
                    request.ProfilePictureImageName = "default_image.jpg";
                }
                else
                {
                    request.ProfilePictureImageName = _uow.imageService.SaveImage(request.ImageFile);
                }
                string tempPassword = request.FirstName.Replace(" ", string.Empty) + "Alliance" + request.LastName + "@123";
                request.Password = PasswordHasher.HashPassword(tempPassword);
                Employee employee = _mapper.Map<Employee>(request);
                employee.EmployeeRole = await _uow.employeeRoleRepository.GetEmployeeRole(request.EmployeeRoleName);

                Employee employeeCreated = await _uow.employeeRepository.CreateEmployee(employee);

                if (employeeCreated != null)
                {
                    PersonDto person = new()
                    {
                        FirstName = employeeCreated.FirstName,
                        LastName = employeeCreated.LastName,
                        MiddleName = employeeCreated.MiddleName,
                        PairId = employeeCreated.Id
                    };

                    HttpResponseMessage getData = await _client.PostAsJsonAsync(_baseUrl, person);

                    if (getData.IsSuccessStatusCode)
                    {
                        var emailObj = new Email(request.EmailAddress, "Account Credentials", EmailBody.CredentialsEmailBody(employeeCreated.EmployeeIdNumber, tempPassword));
                        _uow.emailService.SendEmail(emailObj);
                        response = new ResponseDto<EmployeeDto>() { Status = true, Message = "User Created", Value = _mapper.Map<EmployeeDto>(employeeCreated) };
                    }
                    else
                    {
                        if (employee.ProfilePictureImageName != "default_image.jpg") _uow.imageService.DeleteImage(employeeCreated);
                        bool deleted = await _uow.employeeRepository.DeleteEmployee(employeeCreated);
                        response = new ResponseDto<EmployeeDto>() { Status = false, Message = "User Not Created" };
                    }
                }
                else
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "User Not Created" };
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
        public async Task<IActionResult> UpdateEmployee([FromForm] EmployeeDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                
                try
                {
                    var emailObj = new Email(request.EmailAddress, "Checking Email", EmailBody.CheckEmailBody());
                    _uow.emailService.SendEmail(emailObj);
                }
                catch (Exception)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Cannot contact Email" };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                Employee oldEmployee = await _uow.employeeRepository.GetEmployee(request.Id);
                Employee tempEmployee = new()
                {
                    Id = oldEmployee.Id,
                    FirstName = oldEmployee.FirstName,
                    MiddleName = oldEmployee.MiddleName,
                    LastName = oldEmployee.LastName,
                    EmailAddress = oldEmployee.EmailAddress,
                    EmployeeIdNumber = oldEmployee.EmployeeIdNumber,
                    EmployeeRole = oldEmployee.EmployeeRole
                };

                if (await _uow.employeeRepository.EmployeeIdNumberExists(request.EmployeeIdNumber) && oldEmployee.EmployeeIdNumber != request.EmployeeIdNumber)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Id Number Already Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                if (await _uow.employeeRepository.EmailAddressExists(request.EmailAddress) && oldEmployee.EmailAddress != request.EmailAddress)
                {
                    response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Email Address Already Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                oldEmployee.FirstName = request.FirstName;
                oldEmployee.MiddleName = request.MiddleName;
                oldEmployee.LastName = request.LastName;
                oldEmployee.EmailAddress = request.EmailAddress;
                oldEmployee.EmployeeIdNumber = request.EmployeeIdNumber;
                oldEmployee.EmployeeRole = await _uow.employeeRoleRepository.GetEmployeeRole(request.EmployeeRoleName); 
                if (oldEmployee.ProfilePictureImageName != "default_image.jpg" && request.ImageFile != null) _uow.imageService.DeleteImage(oldEmployee);
                if (request.ImageFile != null) oldEmployee.ProfilePictureImageName = _uow.imageService.SaveImage(request.ImageFile);
                

                _uow.employeeRepository.DetachEmployee(oldEmployee);
                Employee employeeEdited = await _uow.employeeRepository.UpdateEmployee(oldEmployee);

                if (employeeEdited.FirstName != tempEmployee.FirstName || employeeEdited.MiddleName != tempEmployee.MiddleName || employeeEdited.LastName != tempEmployee.LastName)
                {

                    PersonDto person = new()
                    {
                        FirstName = employeeEdited.FirstName,
                        LastName = employeeEdited.LastName,
                        MiddleName = employeeEdited.MiddleName,
                        PairId = employeeEdited.Id
                    };

                    HttpResponseMessage getData = await _client.PutAsJsonAsync(_baseUrl, person);

                    if (!getData.IsSuccessStatusCode)
                    {

                        _uow.employeeRepository.DetachEmployee(tempEmployee);
                        Employee temp = await _uow.employeeRepository.UpdateEmployee(tempEmployee);
                        response = new ResponseDto<EmployeeDto>() { Status = false, Message = "Something went wrong" };
                        return StatusCode(StatusCodes.Status200OK, response);
                    }
                }

                response = new ResponseDto<EmployeeDto>() { Status = true, Message = "User Updated", Value = _mapper.Map<EmployeeDto>(employeeEdited) };

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

                Employee oldEmployee = await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(request.EmployeeIdNumber);
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
        [HttpPut("profile-picture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] ImageFileDto request)
        {
            ResponseDto<EmployeeDto> response;
            try
            {
                Employee oldEmployee = await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(request.EmployeeIdNumber);
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

                HttpResponseMessage getData = await _client.DeleteAsync($"{_baseUrl}/{id}");

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                Employee employee = await _uow.employeeRepository.GetEmployee(id);
                if (employee.ProfilePictureImageName != "default_image.jpg") _uow.imageService.DeleteImage(employee);
                bool deleted = await _uow.employeeRepository.DeleteEmployee(employee);
                
                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "User Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "User Not Deleted" };
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
        [HttpPut("delete-employees")]
        public async Task<IActionResult> DeleteEmployees([FromBody] DeleteRangeDto deleteRange)
        {
            ResponseDto<bool> response;
            try
            {
                HttpResponseMessage getData = await _client.PutAsJsonAsync($"{_baseUrl}/delete-people", deleteRange);

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                List<Employee> employees =await _uow.employeeRepository.GetEmployees(deleteRange.Ids);
                bool deleted = await _uow.employeeRepository.DeleteEmployees(employees);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Users Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Users Not Deleted" };
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
                    var emailObj = new Email(email, "Reset Password", EmailBody.ResetEmailBody(email, emailToken));
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

                    response = new ResponseDto<EmployeeDto>() { Status = true, Message = "Password Reset Successful", Value = _mapper.Map<EmployeeDto>(employeeEdited) };
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
