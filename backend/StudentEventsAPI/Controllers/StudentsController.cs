using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services.Mappings;

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
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
    {
        var students = await _db.Students
            .OrderBy(s => s.DisplayName)
            .Select(s => s.ToDto())
            .ToListAsync();
        return Ok(students);
    }

    [HttpGet("{id}/events")]
    public async Task<ActionResult<object>> GetStudentEvents(string id)
    {
        var student = await _db.Students
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return NotFound();

        var eventsList = student.Events
            .OrderBy(e => e.StartDateTime)
            .Select(e => e.ToDto())
            .ToList();

        return Ok(new {
            Student = student.ToDto(),
            Events = eventsList
        });
    }
}
