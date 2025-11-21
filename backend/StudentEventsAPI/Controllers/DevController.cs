using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEventsAPI.Data;
using StudentEventsAPI.Models;

namespace StudentEventsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DevController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public DevController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost("seed-sample")]
    public async Task<IActionResult> SeedSample()
    {
        // Add a few sample students and events for frontend testing
        if (_db.Students.Any()) return Ok(new { Message = "Students already exist" });

        var s1 = new Student
        {
            Id = Guid.NewGuid().ToString(),
            DisplayName = "Alice Silva",
            Email = "alice.silva@example.com",
            Department = "Engineering",
            LastSyncDate = DateTime.UtcNow
        };
        var s2 = new Student
        {
            Id = Guid.NewGuid().ToString(),
            DisplayName = "Bruno Costa",
            Email = "bruno.costa@example.com",
            Department = "Business",
            LastSyncDate = DateTime.UtcNow
        };

        _db.Students.AddRange(s1, s2);

        var e1 = new Event
        {
            Id = Guid.NewGuid().ToString(),
            Subject = "Introdução ao C#",
            StartDateTime = DateTime.UtcNow.AddDays(1),
            EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            Location = "Sala 101",
            IsOnlineMeeting = false,
            StudentId = s1.Id
        };
        var e2 = new Event
        {
            Id = Guid.NewGuid().ToString(),
            Subject = "Reunião de Projeto",
            StartDateTime = DateTime.UtcNow.AddDays(2),
            EndDateTime = DateTime.UtcNow.AddDays(2).AddHours(1),
            Location = "Teams",
            IsOnlineMeeting = true,
            StudentId = s2.Id
        };

        _db.Events.AddRange(e1, e2);

        await _db.SaveChangesAsync();
        return Ok(new { Message = "Sample data seeded" });
    }
}
