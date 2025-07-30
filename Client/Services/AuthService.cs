// WeatherApp.Client/Services/AuthService.cs
using System.Net.Http.Json;
using System.Text.Json;
using WeatherApp_API.Shared.Models;
using Blazored.LocalStorage;

namespace WeatherApp_API.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<bool> Register(UserDto dto)
    {
        var result = await _http.PostAsJsonAsync("api/auth/register", dto);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> Login(UserDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode) return false;

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

        if (json != null && json.TryGetValue("token", out var token))
        {
            await _localStorage.SetItemAsync("authToken", token);
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return true;
        }
        return false;
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto>("api/auth/me");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> SaveFavouriteAsync(string? city, string? weather)
    {
        var dto = new UserDto
        {
            FavoriteCity = city,
            FavoriteWeather = weather
        };

        var response = await _http.PostAsJsonAsync("api/auth/favourites", dto);
        return response.IsSuccessStatusCode;
    }

}
