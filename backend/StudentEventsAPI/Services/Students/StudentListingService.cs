using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services.Mappings;

namespace StudentEventsAPI.Services.Students;

public class StudentListingService : IStudentListingService
{
    private readonly ApplicationDbContext _db;
    public StudentListingService(ApplicationDbContext db) { _db = db; }

    public async Task<PaginatedResult<StudentDto>> GetStudentsAsync(int page, int pageSize, string? search, string? department, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        var query = _db.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(s => s.DisplayName.ToLower().Contains(lower) || s.Email.ToLower().Contains(lower));
        }
        if (!string.IsNullOrWhiteSpace(department))
        {
            var depLower = department.ToLower();
            query = query.Where(s => s.Department != null && s.Department.ToLower().Contains(depLower));
        }

        var total = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(s => s.DisplayName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => s.ToDto())
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PaginatedResult<StudentDto>(data, page, pageSize, total, totalPages);
    }
}
