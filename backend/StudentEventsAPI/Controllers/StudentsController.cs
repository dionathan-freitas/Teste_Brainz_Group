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
    public async Task<ActionResult<object>> GetStudents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? department = null)
    {
        var query = _db.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => s.DisplayName.ToLower().Contains(searchLower) ||
                                     s.Email.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(s => s.Department != null && s.Department.ToLower() == department.ToLower());
        }

        var total = await query.CountAsync();
        var students = await query
            .OrderBy(s => s.DisplayName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => s.ToDto())
            .ToListAsync();

        return Ok(new
        {
            Data = students,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
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
