using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;

namespace StudentEventsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public StudentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _db.Students
            .OrderBy(s => s.DisplayName)
            .Select(s => new { s.Id, s.DisplayName, s.Email, s.Department, s.LastSyncDate })
            .ToListAsync();
        return Ok(students);
    }

    [HttpGet("{id}/events")]
    public async Task<IActionResult> GetStudentEvents(string id)
    {
        var student = await _db.Students
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return NotFound();

        var eventsList = student.Events
            .OrderBy(e => e.StartDateTime)
            .Select(e => new {
                e.Id, e.Subject, e.StartDateTime, e.EndDateTime, e.Location, e.IsOnlineMeeting
            });

        return Ok(new { student.Id, student.DisplayName, student.Email, Events = eventsList });
    }
}
