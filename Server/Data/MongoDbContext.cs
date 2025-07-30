using MongoDB.Driver;
using WeatherApp_API.Server.Models;
using WeatherApp_API.Shared.Models;

namespace WeatherApp_API.Server.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("Blazors");
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("WUsers");
    public IMongoCollection<WeatherInfo> WeatherData => _database.GetCollection<WeatherInfo>("WeatherData");
}
