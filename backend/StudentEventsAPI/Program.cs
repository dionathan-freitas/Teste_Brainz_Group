using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using Hangfire.SqlServer;
using System.Text;
using StudentEventsAPI.Data;
using StudentEventsAPI.Models;
using StudentEventsAPI.Services;
using StudentEventsAPI.Services.GraphSync;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hangfire (Background Jobs)
var hangfireConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(hangfireConnection));
builder.Services.AddHangfireServer();

// Authentication - JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key") ?? "";
var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// App Services
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IGraphSyncService, GraphSyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/jobs");

// Run migrations and seed basic data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Apply migrations when available; otherwise ensure database
    if (db.Database.GetPendingMigrations().Any() || db.Database.GetMigrations().Any())
        db.Database.Migrate();
    else
        db.Database.EnsureCreated();

    // Seed a default admin user (simple hashing for bootstrap)
    if (!db.Users.Any())
    {
        var username = "admin";
        var password = "admin123";
        var passwordHash = Convert.ToHexString(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        db.Users.Add(new User { Username = username, PasswordHash = passwordHash, Role = "Admin" });
        db.SaveChanges();
    }
}

// Recurring job (hourly students sync)
RecurringJob.AddOrUpdate<IGraphSyncService>("sync-students", svc => svc.SyncStudentsAsync(CancellationToken.None), Cron.Hourly);

app.Run();
