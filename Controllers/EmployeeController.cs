using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller    
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        public EmployeeController(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Employee>))]
        public IActionResult GetEmployees()
        {
            var employees = _mapper.Map<List<EmployeeDto>>(_employeeRepository.GetEmployees());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(employees);
        }

        [HttpGet("{isAdmin:bool}/employees")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        [ProducesResponseType(400)]
        public IActionResult GetEmployeesByType(bool isAdmin)
        {

            var employees = _mapper.Map<List<EmployeeDto>>(_employeeRepository.GetEmployeesByType(isAdmin));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(employees);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        [ProducesResponseType(400)]
        public IActionResult GetEmployee(int id)
        {
            if (!_employeeRepository.EmployeeExists(id))
            {
                return NotFound();
            }

            var person = _mapper.Map<EmployeeDto>(_employeeRepository.GetEmployee(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(person);
        }

        [HttpGet("{firstname}-{middlename}-{lastname}")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        [ProducesResponseType(400)]
        public IActionResult GetEmployee(string firstname, string middlename, string lastname)
        {
            if (!_employeeRepository.EmployeeExists(firstname, middlename, lastname))
            {
                return NotFound();
            }

            var person = _mapper.Map<EmployeeDto>(_employeeRepository.GetEmployee(firstname, middlename, lastname));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(person);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateEmployee([FromBody] EmployeeDto employeeCreate)
        {
            if (employeeCreate == null)
                return BadRequest(ModelState);

            if (_employeeRepository.GetEmployee(employeeCreate.FirstName, employeeCreate.MiddleName, employeeCreate.LastName) != null)
            {
                ModelState.AddModelError("", "Employee already exists");
                return StatusCode(422, ModelState);
            }

            if (_employeeRepository.GetEmployee(employeeCreate.EmployeeId) != null)
            {
                ModelState.AddModelError("", "Employee Id already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var personMap = _mapper.Map<Employee>(employeeCreate);

            if (!_employeeRepository.CreateEmployee(personMap))
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
        public IActionResult UpdateEmployee(int id, [FromBody] EmployeeDto updatedEmployee)
        {
            if (updatedEmployee == null)
                return BadRequest(ModelState);

            if (id != updatedEmployee.Id)
                return BadRequest(ModelState);

            if (!_employeeRepository.EmployeeExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var employeeMap = _mapper.Map<Employee>(updatedEmployee);

            if (!_employeeRepository.UpdateEmployee(employeeMap))
            {
                ModelState.AddModelError("", "Something went wrong updating employee");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteEmployee(int id)
        {
            if (!_employeeRepository.EmployeeExists(id))
            {
                return NotFound();
            }

            var employee = _employeeRepository.GetEmployee(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_employeeRepository.DeleteEmployee(employee))
            {
                ModelState.AddModelError("", "Something went wrong deleting employee");
            }

            return Ok("Successfully deleted");
        }
    }
}
