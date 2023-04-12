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
    public class FaceRecognitionServiceController : Controller
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public FaceRecognitionServiceController()
        {
            _baseUrl = "https://localhost:7032/api/FaceRecognitionService";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        [HttpGet("recognize-face/{id}")]
        public async Task<IActionResult> RecognizeFace(int id)
        {
            ResponseDto<PersonDto> response;
            try
            {
                HttpResponseMessage getData = await _client.GetAsync($"{_baseUrl}/recognize-face/{id}");

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

        [HttpGet("train-model")]
        public async Task<IActionResult> TrainModel()
        {
            ResponseDto<bool> response;
            try
            {
                HttpResponseMessage getData = await _client.GetAsync($"{_baseUrl}/train-model");

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Model Not Trained" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<bool>>(getData.Content.ReadAsStringAsync().Result);

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
