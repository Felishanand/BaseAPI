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
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _clientFactory = httpClientFactory;
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

            _logger.LogInformation($"Get Data From Micro Service 1 {requestEndPoint} - Started");

            try
            {
                // Content from BBC One: Dr. Who website (©BBC)
                var request = new HttpRequestMessage(HttpMethod.Get,
                   requestEndPoint);

                var client = _clientFactory.CreateClient();

                Request.Headers.TryGetValue("CorrelationId", out var correlationId).ToString();

                var id = correlationId;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {                    
                    var data = await response.Content.ReadAsStringAsync();

                    return Ok(data);
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
