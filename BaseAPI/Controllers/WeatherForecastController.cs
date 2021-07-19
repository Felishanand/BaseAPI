using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private HttpClient _client;

        private string CorreleationId;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient();                       
        }

        private void AddCustomHeader()
        {
            Request.Headers.TryGetValue("CorrelationId", out var objCorreleationId).ToString();

            CorreleationId = objCorreleationId;

            _client.DefaultRequestHeaders.Add("CorrelationId", CorreleationId);
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Get Method Called...");

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("Test")]
        public async Task<IActionResult> GetData() 
        {
            var requestEndPoint = "https://localhost:44387/weatherforecast/Test";

            _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Started By {CorreleationId}");

            try
            {
                // Content from BBC One: Dr. Who website (©BBC)
                var request = new HttpRequestMessage(HttpMethod.Get,
                   requestEndPoint);

                //AddCustomHeader();

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();                    

                    _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Completed By {CorreleationId}");

                    return Ok($"{data} and Request by {CorreleationId}");
                }

                return BadRequest(response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from GetData - {requestEndPoint}: {ex.Message}");
                throw;
            }

        }       

        [HttpGet]
        [Route("Test1")]
        public async Task<IActionResult> GetData1()
        {
            var requestEndPoint = "https://localhost:44387/weatherforecast/Test";           

            _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Started By {CorreleationId}");

            try
            {
                // Content from BBC One: Dr. Who website (©BBC)
                var request = new HttpRequestMessage(HttpMethod.Get,
                   requestEndPoint);

                AddCustomHeader();

                  var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Completed By {CorreleationId}");

                    return Ok($"{data} and Request by {CorreleationId}");
                }

                return BadRequest(response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from GetData - {requestEndPoint}: {ex.Message}");
                throw;
            }

        }

        [HttpGet]
        [Route("Test2")]
        public async Task<IActionResult> GetData2()
        {
            var requestEndPoint = "https://localhost:44387/weatherforecast/Test";         

            _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Started By {CorreleationId}");

            try
            {
                // Content from BBC One: Dr. Who website (©BBC)
                var request = new HttpRequestMessage(HttpMethod.Get,
                   requestEndPoint);

                AddCustomHeader();

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Completed By {CorreleationId}");

                    return Ok($"{data} and Request by {CorreleationId}");
                }

                return BadRequest(response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from GetData - {requestEndPoint}: {ex.Message}");
                throw;
            }

        }
    }
}
