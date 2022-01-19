using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = 69,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("{cityId}")]
        public async Task<WeatherForecast> GetWeather(string cityId)
        {
            const string apiKey = "a45895b5d65216f5c164378ace4b1b9e";
            const string url = "https://api.openweathermap.org/data/2.5/weather";

            HttpResponseMessage response = await client.GetAsync($"{url}?id={cityId}&units=metric&appid={apiKey}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject data = (JObject)JsonConvert.DeserializeObject(responseBody);

            string city = (string)data.SelectToken("name");
            double grades = (double)data.SelectToken("main.temp");
            double minTemp = (double)data.SelectToken("main.temp_min");
            double maxTemp = (double)data.SelectToken("main.temp_max");
            int pressure = (int)data.SelectToken("main.pressure");
            int humidity = (int)data.SelectToken("main.humidity");

            string summary = $"City: {city} --- " +
                $"Minimum Temperature: {minTemp} --- " +
                $"Maximum Temperature: {maxTemp} --- " +
                $"Pressure: {pressure} --- " +
                $"Humidity: {humidity}";
            return new WeatherForecast(DateTime.Now, Convert.ToInt32(grades), summary);
        }
    }
}