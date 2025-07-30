using WeatherApp_API.Server.Data;
using WeatherApp_API.Server.Models;
using WeatherApp_API.Shared.Models;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WeatherApp_API.Server.Services;

public class AuthService
{
    private readonly MongoDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(MongoDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<bool> RegisterAsync(UserDto dto)
    {
        var existing = await _context.Users.Find(u => u.Username == dto.Username).FirstOrDefaultAsync();
        if (existing != null) return false;

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = dto.Username ?? string.Empty,
            PasswordHash = hash
        };
        await _context.Users.InsertOneAsync(user);
        return true;
    }

    public async Task<string?> LoginAsync(UserDto dto)
    {
        var user = await _context.Users.Find(u => u.Username == dto.Username).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return GenerateJwtToken(user.Username);
    }

    private string GenerateJwtToken(string username)
    {
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
    public async Task<bool> SaveFavouriteAsync(string username, UserDto dto)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var update = Builders<User>.Update
                .Set(u => u.FavoriteCity, dto.FavoriteCity)
                .Set(u => u.FavoriteWeather, dto.FavoriteWeather);

            var result = await _context.Users.UpdateOneAsync(filter, update);
            Console.WriteLine($"Update result: Matched {result.MatchedCount}, Modified {result.ModifiedCount}");
            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during SaveFavouriteAsync: " + ex.Message);
            return false;
        }
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

}
