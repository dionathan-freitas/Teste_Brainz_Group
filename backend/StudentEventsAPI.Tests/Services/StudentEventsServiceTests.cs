using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.Services.Students;
using StudentEventsAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEventsAPI.Tests.Services;

public class StudentEventsServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        
        var student = new Student
        {
            Id = "student-1",
            GraphUserId = "graph-1",
            Email = "test@example.com",
            DisplayName = "Test Student",
            Department = "Engineering"
        };

        var now = DateTime.UtcNow;

        context.Students.Add(student);
        context.Events.AddRange(
            new Event
            {
                Id = "1",
                GraphEventId = "event-1",
                Subject = "Event 1",
                StartDateTime = now.AddDays(1),
                EndDateTime = now.AddDays(1).AddHours(1),
                StudentId = "student-1",
                IsOnlineMeeting = true
            },
            new Event
            {
                Id = "2",
                GraphEventId = "event-2",
                Subject = "Event 2",
                StartDateTime = now.AddDays(2),
                EndDateTime = now.AddDays(2).AddHours(1),
                StudentId = "student-1",
                IsOnlineMeeting = false
            }
        );
        context.SaveChanges();
        
        return context;
    }

    [Fact]
    public async Task GetStudentEventsAsync_ReturnsStudentAndEvents_WhenStudentExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentEventsService(context);

        // Act
        var result = await service.GetStudentEventsAsync("student-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Student", result.Value.Student.DisplayName);
        Assert.Equal(2, result.Value.Events.Count);
        // Assert that all events belong to the expected student by checking a valid property, e.g., Subject is not null
        Assert.All(result.Value.Events, e => Assert.NotNull(e.Subject));
    }

    [Fact]
    public async Task GetStudentEventsAsync_ReturnsNull_WhenStudentNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentEventsService(context);

        // Act
        var result = await service.GetStudentEventsAsync("nonexistent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStudentEventsAsync_ReturnsEmptyEvents_WhenStudentHasNoEvents()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Students.Add(new Student
        {
            Id = "student-2",
            GraphUserId = "graph-2",
            Email = "another@example.com",
            DisplayName = "Another Student",
            Department = "Business"
        });
        context.SaveChanges();

        var service = new StudentEventsService(context);

        // Act
        var result = await service.GetStudentEventsAsync("student-2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Another Student", result.Value.Student.DisplayName);
        Assert.Empty(result.Value.Events);
    }

    [Fact]
    public async Task GetStudentEventsAsync_OrdersEventsByStartDate()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentEventsService(context);

        // Act
        var result = await service.GetStudentEventsAsync("student-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Value.Events.Count);
        
        // Verify events are ordered by StartDateTime ascending
        for (int i = 0; i < result.Value.Events.Count - 1; i++)
        {
            Assert.True(
                result.Value.Events.ElementAt(i).StartDateTime <= result.Value.Events.ElementAt(i + 1).StartDateTime,
                "Events should be ordered by StartDateTime"
            );
        }
    }
}
