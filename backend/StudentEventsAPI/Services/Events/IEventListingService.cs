using StudentEventsAPI.DTOs;

namespace StudentEventsAPI.Services.Events;

public interface IEventListingService
{
    Task<PaginatedResult<EventDto>> GetEventsAsync(int page, int pageSize, string? studentId, DateTime? startDate, DateTime? endDate, string? search, CancellationToken cancellationToken = default);
}
