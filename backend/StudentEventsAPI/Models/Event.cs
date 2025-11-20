namespace StudentEventsAPI.Models;

public class Event
{
    public string Id { get; set; } = string.Empty;
    public string? GraphEventId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Location { get; set; }
    public string? Body { get; set; }
    public bool IsOnlineMeeting { get; set; }
    
    public string StudentId { get; set; } = string.Empty;
    public Student? Student { get; set; }
}
