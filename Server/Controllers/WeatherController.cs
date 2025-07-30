using Microsoft.AspNetCore.Mvc;
using WeatherApp_API.Server.Data;
using WeatherApp_API.Shared.Models;
using MongoDB.Driver;

namespace WeatherApp_API.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly HttpClient _http;
    private readonly MongoDbContext _context;
    private readonly IConfiguration _config;

    public WeatherController(IHttpClientFactory httpFactory, MongoDbContext context, IConfiguration config)
    {
        _http = httpFactory.CreateClient();
        _context = context;
        _config = config;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var apiKey = _config["OpenWeatherMap:ApiKey"];
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
        var response = await _http.GetStringAsync(url);
        return Ok(response);
    }

    [HttpGet("static/{city}")]
    public async Task<IActionResult> GetStaticWeather(string city)
    {
        var filter = Builders<WeatherInfo>.Filter.Eq(w => w.City, city);

        var data = await _context.WeatherData
            .Find(filter)
            .SortByDescending(w => w.Date)
            .Limit(5)
            .ToListAsync();

        return Ok(data);
    }
}
