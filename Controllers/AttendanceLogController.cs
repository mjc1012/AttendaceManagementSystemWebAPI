using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Helper;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using AttendaceManagementSystemWebAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceLogController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public AttendanceLogController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAttendanceLogs()
        {

            ResponseApi<List<AttendanceLogDto>> response;
            try
            {
                List<AttendanceLogDto> logs = _mapper.Map<List<AttendanceLogDto>>(await _uow.attendanceLogRepository.GetAttendanceLogs());

                if (logs.Count > 0)
                {
                    response = new ResponseApi<List<AttendanceLogDto>>() { Status = true, Message = "Got All Attendance Logs", Value = logs };
                }
                else
                {
                    response = new ResponseApi<List<AttendanceLogDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<AttendanceLogDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [Authorize]
        [HttpGet("{employeeIdNumber}")]
        public async Task<IActionResult> GetAttendanceLogs(string employeeIdNumber)
        {

            ResponseApi<List<AttendanceLogDto>> response;
            try
            {
                List<AttendanceLogDto> logs = _mapper.Map<List<AttendanceLogDto>>(await _uow.attendanceLogRepository.GetAttendanceLogs(employeeIdNumber));

                if (logs.Count > 0)
                {
                    response = new ResponseApi<List<AttendanceLogDto>>() { Status = true, Message = "Got All Attendance Logs", Value = logs };
                }
                else
                {
                    response = new ResponseApi<List<AttendanceLogDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<AttendanceLogDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateAttendanceLog([FromBody] AttendanceLogDto request)
        {
            ResponseApi<AttendanceLogDto> response;
            try
            {
                if (request.Base64String == "" || request.Base64String == null)
                    request.ImageName = "default_image.jpg";
                else
                    request.ImageName = _uow.imageService.SaveImage(request.Base64String);
                AttendanceLog log = _mapper.Map<AttendanceLog>(request);
                log.Employee = await _uow.employeeRepository.GetEmployee(request.EmployeeIdNumber);
                var requestTimeLog = DateTime.ParseExact(request.TimeLog, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                if(request.AttendanceLogTypeName == null || request.AttendanceLogTypeName == "")
                {
                    int logTypeId = _uow.attendanceLogTypeRepository.GetAttendanceLogType(requestTimeLog, log.Employee);
                    if (logTypeId == -1)
                    {
                        response = new ResponseApi<AttendanceLogDto>() { Status = false, Message = "You already logged two times today" };
                        return StatusCode(StatusCodes.Status200OK, response);
                    }
                    log.AttendanceLogType = await _uow.attendanceLogTypeRepository.GetAttendanceLogType(logTypeId);
                }
                else
                {
                    log.AttendanceLogType = await _uow.attendanceLogTypeRepository.GetAttendanceLogType(request.AttendanceLogTypeName);
                }

                if (request.AttendanceLogStatusName == null || request.AttendanceLogStatusName == "")
                {
                    log.AttendanceLogStatus = await _uow.attendanceLogStatusRepository.GetAttendanceLogStatus(1);
                }
                else
                {
                    log.AttendanceLogStatus = await _uow.attendanceLogStatusRepository.GetAttendanceLogStatus(request.AttendanceLogStatusName);
                }

                AttendanceLog logCreated = await _uow.attendanceLogRepository.CreateAttendanceLog(log);

                if (logCreated.Id != 0)
                {
                    response = new ResponseApi<AttendanceLogDto>() { Status = true, Message = "Attendance Log Created", Value = _mapper.Map<AttendanceLogDto>(logCreated) };
                }
                else
                {
                    response = new ResponseApi<AttendanceLogDto>() { Status = false, Message = "Could not create log" };
                }
                
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<AttendanceLogDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateAttendanceLog([FromBody] AttendanceLogDto request)
        {

            ResponseApi<AttendanceLogDto> response;
            try
            {
                AttendanceLog oldlog = await _uow.attendanceLogRepository.GetAttendanceLog(request.Id);
                request.ImageName= oldlog.ImageName;
                _uow.attendanceLogRepository.DetachLog(oldlog);
                //request.ImageName = _imageService.SaveImage(request.Base64String);
                AttendanceLog log = _mapper.Map<AttendanceLog>(request);
                //_imageService.DeleteImage(await _attendanceLogRepository.GetAttendanceLog(request.Id));
                log.Employee = await _uow.employeeRepository.GetEmployee(request.EmployeeIdNumber);
                log.AttendanceLogType = await _uow.attendanceLogTypeRepository.GetAttendanceLogType(request.AttendanceLogTypeName);
                log.AttendanceLogStatus = await _uow.attendanceLogStatusRepository.GetAttendanceLogStatus(request.AttendanceLogStatusName);

                AttendanceLog logEdited = await _uow.attendanceLogRepository.UpdateAttendanceLog(log);

                response = new ResponseApi<AttendanceLogDto>() { Status = true, Message = "Attendance Log Updated", Value = _mapper.Map<AttendanceLogDto>(logEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<AttendanceLogDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendanceLog(int id)
        {

            ResponseApi<bool> response;
            try
            {

                AttendanceLog log = await _uow.attendanceLogRepository.GetAttendanceLog(id);
                if (log.ImageName != "default_image.jpg")
                    _uow.imageService.DeleteImage(log);
                bool deleted = await _uow.attendanceLogRepository.DeleteAttendanceLog(log);

                if (deleted)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Attendance Log Deleted" };
                }
                else
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Could not delete" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
