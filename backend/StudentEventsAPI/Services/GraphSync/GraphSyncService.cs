using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;

namespace StudentEventsAPI.Services.GraphSync;

public class GraphSyncService : IGraphSyncService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private GraphServiceClient? _client;

    public GraphSyncService(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    private GraphServiceClient Client => _client ??= CreateClient();

    private GraphServiceClient CreateClient()
    {
        var tenantId = _config["MicrosoftGraph:TenantId"]!;
        var clientId = _config["MicrosoftGraph:ClientId"]!;
        var clientSecret = _config["MicrosoftGraph:ClientSecret"]!;
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        return new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task SyncStudentsAsync(CancellationToken cancellationToken = default)
    {
        var users = await Client.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = new[] { "id", "displayName", "mail", "department" };
            requestConfiguration.QueryParameters.Top = 50;
        }, cancellationToken);

        if (users?.Value == null) return;

        foreach (var u in users.Value.Where(u => !string.IsNullOrEmpty(u.Mail)))
        {
            var existing = await _db.Students.FirstOrDefaultAsync(s => s.GraphUserId == u.Id, cancellationToken);
            if (existing == null)
            {
                _db.Students.Add(new StudentEventsAPI.Models.Student
                {
                    Id = Guid.NewGuid().ToString(),
                    GraphUserId = u.Id,
                    DisplayName = u.DisplayName ?? string.Empty,
                    Email = u.Mail ?? string.Empty,
                    Department = u.Department,
                    LastSyncDate = DateTime.UtcNow
                });
            }
            else
            {
                existing.DisplayName = u.DisplayName ?? existing.DisplayName;
                existing.Email = u.Mail ?? existing.Email;
                existing.Department = u.Department ?? existing.Department;
                existing.LastSyncDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SyncEventsAsync(CancellationToken cancellationToken = default)
    {
        var students = await _db.Students.ToListAsync(cancellationToken);

        foreach (var student in students)
        {
            if (string.IsNullOrEmpty(student.GraphUserId)) continue;

            try
            {
                var startDate = DateTime.UtcNow.AddMonths(-1);
                var endDate = DateTime.UtcNow.AddMonths(3);

                var events = await Client.Users[student.GraphUserId].Calendar.CalendarView
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.StartDateTime = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        requestConfiguration.QueryParameters.EndDateTime = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "start", "end", "location", "body", "isOnlineMeeting" };
                        requestConfiguration.QueryParameters.Top = 100;
                    }, cancellationToken);

                if (events?.Value == null) continue;

                foreach (var evt in events.Value)
                {
                    if (evt.Start?.DateTime == null || evt.End?.DateTime == null) continue;

                    var existing = await _db.Events.FirstOrDefaultAsync(
                        e => e.GraphEventId == evt.Id && e.StudentId == student.Id, 
                        cancellationToken);

                    if (existing == null)
                    {
                        _db.Events.Add(new StudentEventsAPI.Models.Event
                        {
                            Id = Guid.NewGuid().ToString(),
                            GraphEventId = evt.Id,
                            StudentId = student.Id,
                            Subject = evt.Subject ?? string.Empty,
                            StartDateTime = DateTime.Parse(evt.Start.DateTime),
                            EndDateTime = DateTime.Parse(evt.End.DateTime),
                            Location = evt.Location?.DisplayName,
                            Body = evt.Body?.Content,
                            IsOnlineMeeting = evt.IsOnlineMeeting ?? false
                        });
                    }
                    else
                    {
                        existing.Subject = evt.Subject ?? existing.Subject;
                        existing.StartDateTime = DateTime.Parse(evt.Start.DateTime);
                        existing.EndDateTime = DateTime.Parse(evt.End.DateTime);
                        existing.Location = evt.Location?.DisplayName ?? existing.Location;
                        existing.Body = evt.Body?.Content ?? existing.Body;
                        existing.IsOnlineMeeting = evt.IsOnlineMeeting ?? existing.IsOnlineMeeting;
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                continue;
            }
        }
    }
}
