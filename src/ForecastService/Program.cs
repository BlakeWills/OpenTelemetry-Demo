using ForecastService.Controllers;
using ForecastService.Middleware;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ForecastProvider>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService(builder.Environment.ApplicationName,
            serviceNamespace: "otelTalk",                                               // Group related services
            serviceVersion: Assembly.GetEntryAssembly()!.GetName().Version!.ToString(), // REALLY useful for deployment tracking
            autoGenerateServiceInstanceId: true);

        var attributes = new Dictionary<string, object>()
        {
            { "environment", "production" },            // Check the docs for your APM tool for correct naming. (DataDog is deployment.environment)
            { "mycompany.service.owningteam", "SRE" }   // Useful for filtering and alerting
        };

        resource.AddAttributes(attributes);
    })
    .WithTracing(tracingConfig =>
    {
        tracingConfig.AddAspNetCoreInstrumentation(cfg =>
        {
            cfg.RecordException = true;
        });

        tracingConfig.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://collector:4317");
        });
    });

builder.Services.AddLogging(logging => logging.AddOpenTelemetry(otel =>
{
    otel.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://collector:4317");
    });
}));

builder.Services.AddSingleton<BaggageMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<BaggageMiddleware>();

app.Run();
