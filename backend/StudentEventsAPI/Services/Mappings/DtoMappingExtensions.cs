using StudentEventsAPI.DTOs;
using StudentEventsAPI.Models;

namespace StudentEventsAPI.Services.Mappings;

public static class DtoMappingExtensions
{
    public static StudentDto ToDto(this Student s) => new(
        s.Id,
        s.DisplayName,
        s.Email,
        s.Department,
        s.LastSyncDate
    );

    public static EventDto ToDto(this Event e) => new(
        e.Id,
        e.Subject,
        e.StartDateTime,
        e.EndDateTime,
        e.Location,
        e.IsOnlineMeeting
    );
}
