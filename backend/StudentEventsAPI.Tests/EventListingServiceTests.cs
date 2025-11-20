using System;
using System.Linq;
using System.Threading.Tasks;
using StudentEventsAPI.Services.Events;
using StudentEventsAPI.Models;
using Xunit;

namespace StudentEventsAPI.Tests;

public class EventListingServiceTests
{
    [Fact]
    public async Task GetEventsAsync_FiltersAndOrders()
    {
        var ctx = TestHelpers.CreateContext(nameof(GetEventsAsync_FiltersAndOrders));
        var student = new Student { Id = "s1", DisplayName = "Stud", Email = "s1@example.com" };
        ctx.Students.Add(student);
        ctx.SaveChanges();
        TestHelpers.SeedEvents(ctx, student, 20);
        var svc = new EventListingService(ctx);

        var start = DateTime.UtcNow.AddDays(5);
        var end = DateTime.UtcNow.AddDays(10);
        var result = await svc.GetEventsAsync(page:1, pageSize:50, studentId:student.Id, startDate:start, endDate:end, search:"Room A");

        Assert.True(result.TotalCount >= 1);
        Assert.All(result.Data, e => Assert.True(e.StartDateTime >= start && e.EndDateTime <= end));
        Assert.All(result.Data, e => Assert.Contains("Room", e.Location ?? ""));

        var ordered = result.Data.Select(e => e.StartDateTime).ToList();
        var sorted = ordered.OrderBy(d => d).ToList();
        Assert.Equal(sorted, ordered);
    }
}
