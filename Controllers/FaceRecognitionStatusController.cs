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
    public class FaceRecognitionStatusController : Controller
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public FaceRecognitionStatusController()
        {
            _baseUrl = "https://localhost:7032/api/FaceRecognitionStatus";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFaceRecognitionStatuses()
        {
            ResponseDto<List<FaceRecognitionStatusDto>> response;
            try
            {
                HttpResponseMessage getData = await _client.GetAsync(_baseUrl);

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<List<FaceRecognitionStatusDto>>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<List<FaceRecognitionStatusDto>>>(getData.Content.ReadAsStringAsync().Result);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<FaceRecognitionStatusDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateFaceRecognitionStatus([FromBody] FaceRecognitionStatusDto request)
        {
            ResponseDto<FaceRecognitionStatusDto> response;
            try
            {
                HttpResponseMessage getData = await _client.PostAsJsonAsync(_baseUrl, request);

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<FaceRecognitionStatusDto>() { Status = false, Message = "Face Recognition Status Not Created" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = new ResponseDto<FaceRecognitionStatusDto>() { Status = true, Message = "Face Recognition Status Created" };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceRecognitionStatusDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
