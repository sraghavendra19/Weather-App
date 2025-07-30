// WeatherApp.Shared/Models/WeatherInfo.cs
namespace WeatherApp_API.Shared.Models;

public class WeatherInfo {
    public string City { get; set; }
    public string Description { get; set; }
    public float Temperature { get; set; }
    public DateTime Date { get; set; }
}
