using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceToRecognizeController : Controller
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public FaceToRecognizeController()
        {
            _baseUrl = "https://localhost:7032/api/FaceToRecognize";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateFaceToRecognize([FromBody] FaceToRecognizeDto request)
        {
            ResponseDto<FaceToRecognizeDto> response;
            try
            {
                HttpResponseMessage getData = await _client.PostAsJsonAsync(_baseUrl, request);

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<FaceToRecognizeDto>() { Status = false, Message = "Face To Recognize Not Created" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<FaceToRecognizeDto>>(getData.Content.ReadAsStringAsync().Result);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceToRecognizeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
