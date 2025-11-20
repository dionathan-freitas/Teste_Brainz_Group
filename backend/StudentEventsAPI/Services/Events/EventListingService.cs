using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services.Mappings;

namespace StudentEventsAPI.Services.Events;

public class EventListingService : IEventListingService
{
    private readonly ApplicationDbContext _db;
    public EventListingService(ApplicationDbContext db) { _db = db; }

    public async Task<PaginatedResult<EventDto>> GetEventsAsync(int page, int pageSize, string? studentId, DateTime? startDate, DateTime? endDate, string? search, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        var query = _db.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(studentId))
            query = query.Where(e => e.StudentId == studentId);
        if (startDate.HasValue)
            query = query.Where(e => e.StartDateTime >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(e => e.EndDateTime <= endDate.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(e => e.Subject.ToLower().Contains(s) || (e.Location != null && e.Location.ToLower().Contains(s)));
        }

        var total = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(e => e.StartDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => e.ToDto())
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PaginatedResult<EventDto>(data, page, pageSize, total, totalPages);
    }
}
