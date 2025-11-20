namespace StudentEventsAPI.DTOs;

public record EventDto(
    string Id,
    string Subject,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string? Location,
    bool IsOnlineMeeting
);
