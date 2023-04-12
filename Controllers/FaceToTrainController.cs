using AttendaceManagementSystemWebAPI.Dto;
using AttendaceManagementSystemWebAPI.Helper;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace AttendaceManagementSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceToTrainController : Controller
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public FaceToTrainController()
        {
            _baseUrl = "https://localhost:7032/api/FaceToTrain";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Authorize]
        [HttpGet("{employeeId}/missing-expression")]
        public async Task<IActionResult> GetMissingFaceExpression(int employeeId)
        {
            ResponseDto<FaceExpressionDto> response;
            try
            {
                HttpResponseMessage getData = await _client.GetAsync($"{_baseUrl}/{employeeId}/missing-expression");

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<FaceExpressionDto>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<FaceExpressionDto>>(getData.Content.ReadAsStringAsync().Result);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceExpressionDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpGet("{employeeId}/person-faces")]
        public async Task<IActionResult> GetFacesToTrain(int employeeId)
        {
            ResponseDto<List<FaceToTrainDto>> response;
            try
            {

                HttpResponseMessage getData = await _client.GetAsync($"{_baseUrl}/{employeeId}/person-faces");

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<List<FaceToTrainDto>>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                response = JsonConvert.DeserializeObject<ResponseDto<List<FaceToTrainDto>>>(getData.Content.ReadAsStringAsync().Result);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<FaceToTrainDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateFaceToTrain([FromBody] FaceToTrainDto request)
        {

            ResponseDto<FaceToTrainDto> response;
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.PostAsJsonAsync(_baseUrl, request);

                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<FaceToTrainDto>() { Status = false, Message = "Face To Train Not Created" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
            
                response = new ResponseDto<FaceToTrainDto>() { Status = true, Message = "Face To Train Created"};
               
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceToTrainDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaceToTrain(int id)
        {

            ResponseDto<bool> response;
            try
            {

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.DeleteAsync($"{_baseUrl}/{id}");
                if (!getData.IsSuccessStatusCode)
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Face To Train Not Deleted" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
            
                response = new ResponseDto<bool>() { Status = true, Message = "Face To Train Deleted" };
                
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
