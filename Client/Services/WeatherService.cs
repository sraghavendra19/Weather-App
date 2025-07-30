// WeatherApp.Client/Services/WeatherService.cs
using System.Net.Http.Json;
using WeatherApp_API.Shared.Models;

namespace WeatherApp_API.Client.Services;

public class WeatherService
{
    private readonly HttpClient _http;

    public WeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<WeatherResponse> GetLiveWeather(string city)
    {

        return await _http.GetFromJsonAsync<WeatherResponse>($"api/weather/{city}");
    }

    public async Task<WeatherInfo> GetLiveWeatherObject(string city)
    {
        return await _http.GetFromJsonAsync<WeatherInfo>($"api/weather/{city}");
    }

    public async Task<List<WeatherInfo>> GetStaticWeather(string city)
    {
        return await _http.GetFromJsonAsync<List<WeatherInfo>>($"api/weather/static/{city}") ?? new List<WeatherInfo>();
    }
}
