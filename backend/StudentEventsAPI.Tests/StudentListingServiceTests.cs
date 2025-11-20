using System;
using System.Threading.Tasks;
using StudentEventsAPI.Services.Students;
using Xunit;

namespace StudentEventsAPI.Tests;

public class StudentListingServiceTests
{
    [Fact]
    public async Task GetStudentsAsync_PaginatesAndFiltersCorrectly()
    {
        var ctx = TestHelpers.CreateContext(nameof(GetStudentsAsync_PaginatesAndFiltersCorrectly));
        TestHelpers.SeedStudents(ctx, 30);
        var svc = new StudentListingService(ctx);

        var result = await svc.GetStudentsAsync(page:2, pageSize:10, search:"Student 2", department:null);

        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.True(result.TotalCount >= 1);
        Assert.All(result.Data, s => Assert.Contains("Student", s.DisplayName));

        var techResult = await svc.GetStudentsAsync(page:1, pageSize:50, search:null, department:"Tech");
        Assert.Equal(15, techResult.TotalCount); 
        Assert.All(techResult.Data, s => Assert.Equal("Tech", s.Department));
    }
}
