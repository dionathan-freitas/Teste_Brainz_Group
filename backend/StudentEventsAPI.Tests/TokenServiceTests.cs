using System;
using System.IdentityModel.Tokens.Jwt;
using StudentEventsAPI.Models;
using StudentEventsAPI.Services;
using Xunit;

namespace StudentEventsAPI.Tests;

public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_ReturnsValidJwt()
    {
        var config = TestHelpers.CreateJwtConfig(newKeyLength:64);
        var service = new TokenService(config);
        var user = new User { Id = 1, Username = "admin", Role = "Admin", PasswordHash = "hash" };

        var (token, expires) = service.GenerateToken(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.True(expires > DateTime.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Equal("TestIssuer", jwt.Issuer);
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == "admin");
        Assert.Contains(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");
    }
}
