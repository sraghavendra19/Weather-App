using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WeatherApp_API.Shared.Models;
using WeatherApp_API.Server.Services;
using MongoDB.Driver;

namespace WeatherApp_API.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        return result ? Ok() : BadRequest("User already exists.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto dto)
    {
        var token = await _auth.LoginAsync(dto);
        return token != null ? Ok(new { token }) : Unauthorized();
    }

    [HttpPost("favourites")]
    [Authorize]
    public async Task<IActionResult> SaveFavourite([FromBody] UserDto dto)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized();

        var result = await _auth.SaveFavouriteAsync(username, dto);
        return result ? Ok() : BadRequest("Failed to save favourite.");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var user = await _auth.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        return new UserDto
        {
            Username = user.Username,
            FavoriteCity = user.FavoriteCity,
            FavoriteWeather = user.FavoriteWeather
        };
    }


}
