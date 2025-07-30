// WeatherApp.Shared/Models/UserDto.cs
namespace WeatherApp_API.Shared.Models;

public class UserDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FavoriteCity { get; set; }
    public string? FavoriteWeather { get; set; }
}

