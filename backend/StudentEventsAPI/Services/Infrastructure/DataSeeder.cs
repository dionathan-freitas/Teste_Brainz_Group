using Microsoft.EntityFrameworkCore;
using StudentEventsAPI.Data;
using StudentEventsAPI.Models;

namespace StudentEventsAPI.Services.Infrastructure;

public class DataSeeder : IDataSeeder
{
    private readonly ApplicationDbContext _db;
    public DataSeeder(ApplicationDbContext db) { _db = db; }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (_db.Database.GetPendingMigrations().Any() || _db.Database.GetMigrations().Any())
            await _db.Database.MigrateAsync(cancellationToken);
        else
            await _db.Database.EnsureCreatedAsync(cancellationToken);

        if (!_db.Users.Any())
        {
            var username = "admin";
            var password = "admin123";
            var passwordHash = Convert.ToHexString(System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            _db.Users.Add(new User { Username = username, PasswordHash = passwordHash, Role = "Admin" });
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
