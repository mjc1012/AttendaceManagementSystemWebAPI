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

            ResponseDto<List<AttendanceLogDto>> response;
            try
            {
                List<AttendanceLogDto> logs = _mapper.Map<List<AttendanceLogDto>>(await _uow.attendanceLogRepository.GetAttendanceLogs());

                if (logs.Count > 0)
                {
                    response = new ResponseDto<List<AttendanceLogDto>>() { Status = true, Message = "Got All Attendance Logs", Value = logs };
                }
                else
                {
                    response = new ResponseDto<List<AttendanceLogDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<AttendanceLogDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [Authorize]
        [HttpGet("{pairId}")]
        public async Task<IActionResult> GetAttendanceLogs(string pairId)
        {

            ResponseDto<List<AttendanceLogDto>> response;
            try
            {
                List<AttendanceLogDto> logs = _mapper.Map<List<AttendanceLogDto>>(await _uow.attendanceLogRepository.GetAttendanceLogs(pairId));

                if (logs.Count > 0)
                {
                    response = new ResponseDto<List<AttendanceLogDto>>() { Status = true, Message = "Got Attendance Logs", Value = logs };
                }
                else
                {
                    response = new ResponseDto<List<AttendanceLogDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<AttendanceLogDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateAttendanceLog([FromBody] AttendanceLogDto request)
        {
            ResponseDto<AttendanceLogDto> response;
            try
            {
                if (request.Base64String == "" || request.Base64String == null)
                    request.ImageName = "default_image.jpg";
                else
                    request.ImageName = _uow.imageService.SaveImage(request.Base64String);
                AttendanceLog log = _mapper.Map<AttendanceLog>(request);
                if (request.PairId == null || request.PairId == "")
                {
                    log.Employee = await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(request.EmployeeIdNumber);
                }
                else
                {
                    log.Employee = await _uow.employeeRepository.GetEmployeeByPairId(request.PairId);
                }
                DateTime requestTimeLog = DateTime.ParseExact(request.TimeLog, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                if (request.AttendanceLogTypeName == null || request.AttendanceLogTypeName == "")
                {
                    int logTypeId = _uow.attendanceLogTypeRepository.GetAttendanceLogType(requestTimeLog, log.Employee);
                    if (logTypeId == -1)
                    {
                        response = new ResponseDto<AttendanceLogDto>() { Status = false, Message = "You already logged two times today" };
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

                if (log.AttendanceLogStatus.Id == 1)
                {
                    if ((TimeSpan.Compare(requestTimeLog.TimeOfDay, new TimeSpan(9, 0, 0)) == 1 && log.AttendanceLogType.Id == 1) || (TimeSpan.Compare(requestTimeLog.TimeOfDay, new TimeSpan(18, 0, 0)) == 1 && log.AttendanceLogType.Id == 2))
                    {
                        log.AttendanceLogState = await _uow.attendanceLogStateRepository.GetAttendanceLogState(2);
                    }
                    else
                    {
                        log.AttendanceLogState = await _uow.attendanceLogStateRepository.GetAttendanceLogState(1);
                    }
                }
                else
                {
                    log.AttendanceLogState = await _uow.attendanceLogStateRepository.GetAttendanceLogState(3);
                }


                AttendanceLog logCreated = await _uow.attendanceLogRepository.CreateAttendanceLog(log);

                if (logCreated.Id != 0)
                {
                    response = new ResponseDto<AttendanceLogDto>() { Status = true, Message = "Attendance Log Created", Value = _mapper.Map<AttendanceLogDto>(logCreated) };
                }
                else
                {
                    response = new ResponseDto<AttendanceLogDto>() { Status = false, Message = "Attendance Log Not Created" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<AttendanceLogDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
           
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateAttendanceLog([FromBody] AttendanceLogDto request)
        {

            ResponseDto<AttendanceLogDto> response;
            try
            {
                AttendanceLog oldlog = await _uow.attendanceLogRepository.GetAttendanceLog(request.Id);
                request.ImageName= oldlog.ImageName;
                request.Id = oldlog.Id;
                _uow.attendanceLogRepository.DetachLog(oldlog);
                AttendanceLog log = _mapper.Map<AttendanceLog>(request);
                log.Employee = await _uow.employeeRepository.GetEmployeeByEmployeeIdNumber(request.EmployeeIdNumber);
                log.AttendanceLogType = await _uow.attendanceLogTypeRepository.GetAttendanceLogType(request.AttendanceLogTypeName);
                log.AttendanceLogStatus = await _uow.attendanceLogStatusRepository.GetAttendanceLogStatus(request.AttendanceLogStatusName);

                if (log.AttendanceLogStatus.Id == 1)
                {
                    if ((TimeSpan.Compare(log.TimeLog.TimeOfDay, new TimeSpan(9, 0, 0)) == 1 && log.AttendanceLogType.Id == 1) || (TimeSpan.Compare(log.TimeLog.TimeOfDay, new TimeSpan(18, 0, 0)) == 1 && log.AttendanceLogType.Id == 2))
                    {
                        log.AttendanceLogState = await _uow.attendanceLogStateRepository.GetAttendanceLogState(2);
                    }
                    else
                    {
                        log.AttendanceLogState = await _uow.attendanceLogStateRepository.GetAttendanceLogState(1);
                    }
                }
                else
                {
                    log.AttendanceLogState = await _uow.attendanceLogStateRepository.GetAttendanceLogState(3);
                }

                AttendanceLog logEdited = await _uow.attendanceLogRepository.UpdateAttendanceLog(log);

                response = new ResponseDto<AttendanceLogDto>() { Status = true, Message = "Attendance Log Updated", Value = _mapper.Map<AttendanceLogDto>(logEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<AttendanceLogDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendanceLog(int id)
        {

            ResponseDto<bool> response;
            try
            {

                AttendanceLog log = await _uow.attendanceLogRepository.GetAttendanceLog(id);
                if (log.ImageName != "default_image.jpg")
                    _uow.imageService.DeleteImage(log);
                bool deleted = await _uow.attendanceLogRepository.DeleteAttendanceLog(log);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Attendance Log Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Attendance Log Not Deleted" };
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
        [HttpPut("delete-logs")]
        public async Task<IActionResult> DeleteEmployees([FromBody] DeleteRangeDto deleteRange)
        {
            ResponseDto<bool> response;
            try
            {

                List<AttendanceLog> logs = _mapper.Map<List<AttendanceLog>>(await _uow.attendanceLogRepository.GetAttendanceLogs(deleteRange.Ids));
                bool deleted = await _uow.attendanceLogRepository.DeleteAttendanceLogs(logs);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Attendance Logs Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Attendance Logs Not Deleted" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
