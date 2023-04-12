using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public PersonController()
        {
            _baseUrl = "https://localhost:7032/api/Person";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Authorize]
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetPerson(int employeeId)
        {
            ResponseDto<PersonDto> response;
            try
            {
                HttpResponseMessage getData = await _client.GetAsync($"{_baseUrl}/{employeeId}");

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<PersonDto>>(getData.Content.ReadAsStringAsync().Result);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
