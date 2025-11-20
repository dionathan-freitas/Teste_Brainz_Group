using StudentEventsAPI.DTOs;

namespace StudentEventsAPI.Services.Students;

public interface IStudentListingService
{
    Task<PaginatedResult<StudentDto>> GetStudentsAsync(int page, int pageSize, string? search, string? department, CancellationToken cancellationToken = default);
}
