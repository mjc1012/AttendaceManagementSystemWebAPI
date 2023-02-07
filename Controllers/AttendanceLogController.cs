using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using AttendaceManagementSystemWebAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceLogController : Controller
    {
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        public AttendanceLogController(IAttendanceLogRepository attendanceLogRepository, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _attendanceLogRepository= attendanceLogRepository;
            _employeeRepository= employeeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AttendanceLog>))]
        public IActionResult GetEmployees()
        {
            var logs = _mapper.Map<List<AttendanceLogDto>>(_attendanceLogRepository.GetAttendaceLogs());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(logs);
        }

        [HttpGet("employee/{id}/attendance-logs")]
        [ProducesResponseType(200, Type = typeof(AttendanceLog))]
        [ProducesResponseType(400)]
        public IActionResult GetAttendaceLogsByEmployee(int id)
        {

            var logs = _mapper.Map<List<AttendanceLogDto>>(_attendanceLogRepository.GetAttendaceLogsByEmployee(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(logs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(AttendanceLog))]
        [ProducesResponseType(400)]
        public IActionResult GetAttendanceLog(int id)
        {
            if (!_attendanceLogRepository.AttendanceLogExists(id))
            {
                return NotFound();
            }

            var log = _mapper.Map<AttendanceLogDto>(_attendanceLogRepository.GetAttendanceLog(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(log);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAttendanceLog([FromQuery] int employeeId, [FromBody] AttendanceLogDto attendanceLogCreate)
        {
            if (attendanceLogCreate == null)
                return BadRequest(ModelState);

            var employee = _employeeRepository.GetEmployee(employeeId);

            if (employee == null)
            {
                ModelState.AddModelError("", "Employee does not exists");
                return StatusCode(422, ModelState);
            }

            if (_attendanceLogRepository.GetAttendanceLog(attendanceLogCreate.TimeLog, attendanceLogCreate.AttendanceLogType, employeeId) != null)
            {
                ModelState.AddModelError("", "Log already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var logMap = _mapper.Map<AttendanceLog>(attendanceLogCreate);
            logMap.Employee = employee;

            if (!_attendanceLogRepository.CreateAttendanceLog(logMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateAttendanceLog(int id, [FromBody] AttendanceLogDto updatedAttendanceLog)
        {
            if (updatedAttendanceLog == null)
                return BadRequest(ModelState);

            if (id != updatedAttendanceLog.Id)
                return BadRequest(ModelState);

            if (!_attendanceLogRepository.AttendanceLogExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var logMap = _mapper.Map<AttendanceLog>(updatedAttendanceLog);

            if (!_attendanceLogRepository.UpdateAttendanceLog(logMap))
            {
                ModelState.AddModelError("", "Something went wrong updating log");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteAttendanceLog(int id)
        {
            if (!_attendanceLogRepository.AttendanceLogExists(id))
            {
                return NotFound();
            }

            var log = _attendanceLogRepository.GetAttendanceLog(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_attendanceLogRepository.DeleteAttendanceLog(log))
            {
                ModelState.AddModelError("", "Something went wrong deleting log");
            }

            return Ok("Successfully deleted");
        }
    }
}
