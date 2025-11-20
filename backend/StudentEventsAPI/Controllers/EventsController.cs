using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEventsAPI.DTOs;
using StudentEventsAPI.Services.Events;

namespace StudentEventsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventListingService _events;
    public EventsController(IEventListingService events) { _events = events; }

    [HttpGet]
    public async Task<ActionResult<object>> GetEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? studentId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? search = null)
    {
        var result = await _events.GetEventsAsync(page, pageSize, studentId, startDate, endDate, search);
        return Ok(result);
    }
}
