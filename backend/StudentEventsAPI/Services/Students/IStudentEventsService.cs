using StudentEventsAPI.DTOs;

namespace StudentEventsAPI.Services.Students;

public interface IStudentEventsService
{
    Task<(StudentDto Student, IReadOnlyCollection<EventDto> Events)?> GetStudentEventsAsync(string studentId, CancellationToken cancellationToken = default);
}
