namespace StudentEventsAPI.Services.GraphSync;

public interface IGraphSyncService
{
    Task SyncStudentsAsync(CancellationToken cancellationToken = default);
    Task SyncEventsAsync(CancellationToken cancellationToken = default);
}
