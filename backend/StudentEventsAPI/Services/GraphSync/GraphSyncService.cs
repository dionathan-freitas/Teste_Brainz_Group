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
        // Basic retrieval of users (filtering to those with mail)
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

    public Task SyncEventsAsync(CancellationToken cancellationToken = default)
    {
        // Placeholder for events sync (calendar)
        return Task.CompletedTask;
    }
}
