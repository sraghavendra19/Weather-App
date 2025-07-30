using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherApp_API.Server.Models;

public class User
{
    [BsonId]
    public string Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FavoriteCity { get; set; }
    public string FavoriteWeather { get; set; }
}
