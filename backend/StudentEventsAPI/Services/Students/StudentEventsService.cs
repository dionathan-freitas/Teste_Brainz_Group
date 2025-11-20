using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services.Mappings;

namespace StudentEventsAPI.Services.Students;

public class StudentEventsService : IStudentEventsService
{
    private readonly ApplicationDbContext _db;
    public StudentEventsService(ApplicationDbContext db) { _db = db; }

    public async Task<(StudentDto Student, IReadOnlyCollection<EventDto> Events)?> GetStudentEventsAsync(string studentId, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students.Include(s => s.Events).FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        if (student == null) return null;
        var eventsList = student.Events.OrderBy(e => e.StartDateTime).Select(e => e.ToDto()).ToList();
        return (student.ToDto(), eventsList);
    }
}
