using AuthenticationService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AuthenticationService.AuthenticationService>();

builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer("Server=mssqlsvr;Database=userdb;User=sa;Password=p@55w0rd;TrustServerCertificate=true"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// TODO: Add OTEL

var app = builder.Build();

// create db schema
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<UserDbContext>();
context.Database.EnsureDeleted();
context.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/status");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
