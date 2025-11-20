using System;
using System.Linq;
using System.Threading.Tasks;
using StudentEventsAPI.Services.Students;
using StudentEventsAPI.Models;
using Xunit;

namespace StudentEventsAPI.Tests;

public class StudentEventsServiceTests
{
    [Fact]
    public async Task GetStudentEventsAsync_ReturnsSortedEvents()
    {
        var ctx = TestHelpers.CreateContext(nameof(GetStudentEventsAsync_ReturnsSortedEvents));
        var student = new Student { Id = "s1", DisplayName = "Stud", Email = "s1@example.com" };
        ctx.Students.Add(student);
        ctx.SaveChanges();

        ctx.Events.Add(new Event { Id = Guid.NewGuid().ToString(), StudentId = student.Id, Subject = "Later", StartDateTime = DateTime.UtcNow.AddDays(5), EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(1) });
        ctx.Events.Add(new Event { Id = Guid.NewGuid().ToString(), StudentId = student.Id, Subject = "Soon", StartDateTime = DateTime.UtcNow.AddDays(1), EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(1) });
        ctx.Events.Add(new Event { Id = Guid.NewGuid().ToString(), StudentId = student.Id, Subject = "Middle", StartDateTime = DateTime.UtcNow.AddDays(3), EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(1) });
        ctx.SaveChanges();

        var svc = new StudentEventsService(ctx);
        var result = await svc.GetStudentEventsAsync(student.Id);

        Assert.NotNull(result);
        var events = result!.Value.Events.ToList();
        Assert.Equal(3, events.Count);
        Assert.True(events[0].StartDateTime < events[1].StartDateTime);
        Assert.True(events[1].StartDateTime < events[2].StartDateTime);
    }
}
