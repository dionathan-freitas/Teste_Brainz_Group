using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services;

namespace StudentEventsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _tokenService = new TokenService(config);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null) return Unauthorized();

        var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password)));
        if (!string.Equals(hash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }

        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return Ok(new LoginResponse(token, expiresAt, user.Username, user.Role));
    }
}
