namespace StudentEventsAPI.Models;

public class Student
{
    public string Id { get; set; } = string.Empty;
    public string? GraphUserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime LastSyncDate { get; set; }
    
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
