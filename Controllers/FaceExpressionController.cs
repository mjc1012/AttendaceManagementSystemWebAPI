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
    public class FaceExpressionController : Controller
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public FaceExpressionController()
        {
            _baseUrl = "https://localhost:7032/api/FaceExpression";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFaceExpressions()
        {
            ResponseDto<List<FaceExpressionDto>> response;
            try
            {
                HttpResponseMessage getData = await _client.GetAsync(_baseUrl);

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<List<FaceExpressionDto>>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<List<FaceExpressionDto>>>(getData.Content.ReadAsStringAsync().Result);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<FaceExpressionDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
