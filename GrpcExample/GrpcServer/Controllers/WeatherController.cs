using Microsoft.AspNetCore.Mvc;
using WeatherServer;

namespace GrpcServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private static readonly Random _random = new Random();

        [HttpGet]
        [Route("[action]")]
        public Task<IActionResult> Current(string city)
        {
            return Task.FromResult(
                (IActionResult)Ok(new WeatherResponse
            {
                City = city,
                Temperature = _random.Next(10, 30),
                Description = "Солнечно",
                Timestamp = DateTime.UtcNow.ToString("O")
            }));
        }

        [HttpGet]
        [Route("[action]")]
        public Task<IActionResult> Multiple(string city)
        {
            var response = new List<WeatherResponse>(10000);
            for (var i = 0; i < 10000; i++)
            {
                response.Add(new WeatherResponse
                {
                    City = city,
                    Temperature = _random.Next(10, 30),
                    Description = "Солнечно",
                    Timestamp = DateTime.UtcNow.ToString("O")
                });
            }
            return Task.FromResult((IActionResult)Ok(response));
        }
    }
}
