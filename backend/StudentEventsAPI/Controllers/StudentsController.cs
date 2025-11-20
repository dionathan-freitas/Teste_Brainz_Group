using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services.Students;

namespace StudentEventsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentListingService _listing;
    private readonly IStudentEventsService _events;
    public StudentsController(IStudentListingService listing, IStudentEventsService events)
    { _listing = listing; _events = events; }

    [HttpGet]
    public async Task<ActionResult<object>> GetStudents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? department = null)
    {
        var result = await _listing.GetStudentsAsync(page, pageSize, search, department);
        return Ok(result);
    }

    [HttpGet("{id}/events")]
    public async Task<ActionResult<object>> GetStudentEvents(string id)
    {
        var result = await _events.GetStudentEventsAsync(id);
        if (result == null) return NotFound();
        return Ok(new { result.Value.Student, Events = result.Value.Events });
    }
}
