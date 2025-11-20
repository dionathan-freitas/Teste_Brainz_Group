using StudentEventsAPI.Models;

namespace StudentEventsAPI.Services;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) GenerateToken(User user);
}
