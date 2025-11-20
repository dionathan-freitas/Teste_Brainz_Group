namespace StudentEventsAPI.DTOs;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string Username, string Role);
