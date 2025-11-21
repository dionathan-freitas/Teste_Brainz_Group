using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.Services.Events;
using StudentEventsAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEventsAPI.Tests.Services;

public class EventListingServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        
        var now = DateTime.UtcNow;
        
        // Seed test data
        context.Events.AddRange(
            new Event
            {
                Id = "1",
                GraphEventId = "event-1",
                Subject = "Team Meeting",
                StartDateTime = now.AddDays(1),
                EndDateTime = now.AddDays(1).AddHours(1),
                StudentId = "student-1",
                IsOnlineMeeting = true
            },
            new Event
            {
                Id = "2",
                GraphEventId = "event-2",
                Subject = "Project Review",
                StartDateTime = now.AddDays(2),
                EndDateTime = now.AddDays(2).AddHours(2),
                StudentId = "student-1",
                IsOnlineMeeting = false,
                Location = "Room 101"
            },
            new Event
            {
                Id = "3",
                GraphEventId = "event-3",
                Subject = "Workshop",
                StartDateTime = now.AddDays(3),
                EndDateTime = now.AddDays(3).AddHours(3),
                StudentId = "student-2",
                IsOnlineMeeting = true
            }
        );
        context.SaveChanges();
        
        return context;
    }

    [Fact]
    public async Task GetEventsAsync_ReturnsAllEvents_WhenNoFilters()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new EventListingService(context);

        // Act
        var result = await service.GetEventsAsync(
            page: 1, 
            pageSize: 10, 
            studentId: null, 
            startDate: null, 
            endDate: null, 
            search: null
        );

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Data.Count);
    }

    [Fact]
    public async Task GetEventsAsync_FiltersBy_StudentId()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new EventListingService(context);

        // Act
        var result = await service.GetEventsAsync(
            page: 1, 
            pageSize: 10, 
            studentId: "student-1", 
            startDate: null, 
            endDate: null, 
            search: null
        );

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, e => Assert.True(
            e.Subject.Contains("Meeting") || e.Subject.Contains("Review")
        ));
    }

    [Fact]
    public async Task GetEventsAsync_FiltersBy_SearchTerm()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new EventListingService(context);

        // Act
        var result = await service.GetEventsAsync(
            page: 1, 
            pageSize: 10, 
            studentId: null, 
            startDate: null, 
            endDate: null, 
            search: "meeting"
        );

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Contains("Meeting", result.Data.First().Subject);
    }

    [Fact]
    public async Task GetEventsAsync_FiltersBy_DateRange()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new EventListingService(context);
        var now = DateTime.UtcNow;
        var startDate = now;
        var endDate = now.AddDays(2);

        // Act
        var result = await service.GetEventsAsync(
            page: 1, 
            pageSize: 10, 
            studentId: null, 
            startDate: startDate, 
            endDate: endDate, 
            search: null
        );

        // Assert
        // Only the event on day 1 ends before or at endDate; the event starting day 2 ends after endDate and is excluded by EndDateTime <= endDate filter.
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetEventsAsync_HandlesPagination_Correctly()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new EventListingService(context);

        // Act
        var page1 = await service.GetEventsAsync(
            page: 1, 
            pageSize: 2, 
            studentId: null, 
            startDate: null, 
            endDate: null, 
            search: null
        );
        
        var page2 = await service.GetEventsAsync(
            page: 2, 
            pageSize: 2, 
            studentId: null, 
            startDate: null, 
            endDate: null, 
            search: null
        );

        // Assert
        Assert.Equal(3, page1.TotalCount);
        Assert.Equal(2, page1.Data.Count);
        Assert.Equal(2, page1.TotalPages);
        
        Assert.Equal(3, page2.TotalCount);
        Assert.Single(page2.Data);
    }
}
