using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentEventsAPI.Data;
using StudentEventsAPI.Models;

namespace StudentEventsAPI.Tests;

public static class TestHelpers
{
    public static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    public static IConfiguration CreateJwtConfig(int newKeyLength = 40)
    {
        if (newKeyLength < 40) newKeyLength = 40; // ensure >256 bits (32 bytes)
        var key = new string('X', newKeyLength);
        var dict = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = key,
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience",
            ["Jwt:ExpirationHours"] = "1"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();
    }

    public static void SeedStudents(ApplicationDbContext ctx, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            ctx.Students.Add(new Student
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = $"Student {i}",
                Email = $"student{i}@example.com",
                Department = i % 2 == 0 ? "Tech" : "Biz",
                LastSyncDate = DateTime.UtcNow
            });
        }
        ctx.SaveChanges();
    }

    public static void SeedEvents(ApplicationDbContext ctx, Student student, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ctx.Events.Add(new Event
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = student.Id,
                Subject = $"Event {i}",
                StartDateTime = DateTime.UtcNow.AddDays(i),
                EndDateTime = DateTime.UtcNow.AddDays(i).AddHours(1),
                Location = i % 2 == 0 ? "Room A" : "Room B",
                IsOnlineMeeting = i % 2 == 0
            });
        }
        ctx.SaveChanges();
    }
}
