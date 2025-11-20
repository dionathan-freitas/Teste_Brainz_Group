namespace StudentEventsAPI.Services.Infrastructure;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
