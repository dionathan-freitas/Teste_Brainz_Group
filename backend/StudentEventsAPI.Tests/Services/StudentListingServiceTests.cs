using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.Services.Students;
using StudentEventsAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEventsAPI.Tests.Services;

public class StudentListingServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        
        // Seed test data
        context.Students.AddRange(
            new Student 
            { 
                Id = "1", 
                GraphUserId = "graph-1", 
                Email = "alice@example.com", 
                DisplayName = "Alice Silva", 
                Department = "Engineering" 
            },
            new Student 
            { 
                Id = "2", 
                GraphUserId = "graph-2", 
                Email = "bob@example.com", 
                DisplayName = "Bob Costa", 
                Department = "Business" 
            },
            new Student 
            { 
                Id = "3", 
                GraphUserId = "graph-3", 
                Email = "charlie@example.com", 
                DisplayName = "Charlie Lima", 
                Department = "Engineering" 
            }
        );
        context.SaveChanges();
        
        return context;
    }

    [Fact]
    public async Task GetStudentsAsync_ReturnsAllStudents_WhenNoFilters()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentListingService(context);

        // Act
        var result = await service.GetStudentsAsync(page: 1, pageSize: 10, search: null, department: null);

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetStudentsAsync_FiltersBy_SearchTerm()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentListingService(context);

        // Act
        var result = await service.GetStudentsAsync(page: 1, pageSize: 10, search: "alice", department: null);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("Alice Silva", result.Data.First().DisplayName);
    }

    [Fact]
    public async Task GetStudentsAsyncFiltersByDepartment()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentListingService(context);

        // Act
        var result = await service.GetStudentsAsync(page: 1, pageSize: 10, search: null, department: "eng");

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, s => Assert.Contains("Engineering", s.Department));
    }

    [Fact]
    public async Task GetStudentsAsync_HandlesPagination_Correctly()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentListingService(context);

        // Act
        var page1 = await service.GetStudentsAsync(page: 1, pageSize: 2, search: null, department: null);
        var page2 = await service.GetStudentsAsync(page: 2, pageSize: 2, search: null, department: null);

        // Assert
        Assert.Equal(3, page1.TotalCount);
        Assert.Equal(2, page1.Data.Count);
        Assert.Equal(2, page1.TotalPages);
        
        Assert.Equal(3, page2.TotalCount);
        Assert.Single(page2.Data);
        Assert.Equal(2, page2.TotalPages);
    }

    [Fact]
    public async Task GetStudentsAsync_ReturnsEmpty_WhenNoMatchingResults()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new StudentListingService(context);

        // Act
        var result = await service.GetStudentsAsync(page: 1, pageSize: 10, search: "nonexistent", department: null);

        // Assert
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Data);
    }
}
