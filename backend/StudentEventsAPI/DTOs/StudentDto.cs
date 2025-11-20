namespace StudentEventsAPI.DTOs;

public record StudentDto(
    string Id,
    string DisplayName,
    string Email,
    string? Department,
    DateTime LastSyncDate
);
