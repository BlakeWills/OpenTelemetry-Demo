using AuthenticationService;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<AuthenticationService.AuthenticationService>();

builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer("Server=userdb;Database=userdb;User=sa;Password=p@55w0rd;"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(builder =>
{
    var resourceBuilder = ResourceBuilder.CreateEmpty()
        .AddService(Assembly.GetEntryAssembly()!.GetName().Name,                        // Name of the service
            serviceNamespace: "otelTalk",                                               // Group related services
            serviceVersion: Assembly.GetEntryAssembly()!.GetName().Version!.ToString(), // REALLY useful for deployment tracking
            autoGenerateServiceInstanceId: true);                                       // You can provide an instance id or provide your own, E.G. host name.

    var attributes = new Dictionary<string, object>()
    {
        { "environment", "production" },            // Check the docs for your APM tool for correct naming. (DataDog is deployment.environment)
        { "mycompany.service.owningteam", "SRE" }   // Useful for filtering and alerting
    };

    resourceBuilder.AddAttributes(attributes);
    builder.SetResourceBuilder(resourceBuilder);    // Ensure resource attributes get added to our exported spans

    // Instrumentation
    // Automatic, search NuGet for OpenTelemetry.Instrumentation to view all.
    builder.AddAspNetCoreInstrumentation();
    builder.AddSqlClientInstrumentation();


    // Custom
    // builder.AddSource("");

    // Exporters: Where do we want traces to go?
    // Console is really useful for debugging trace and architecture issues. (E.G. Is this trace even being listened too?)
    // builder.AddConsoleExporter(options => options.Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Console);

    // Send traces to the OpenTelemetryCollector
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); // required for HTTP Grpc
    builder.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://collector:4317"); // @ Codat we fetch this from KeyVault, I promise
    });
});

var app = builder.Build();

// create schema
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
