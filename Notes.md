# Prep

- Remove containers from containers window to ensure we don't have anything running.

# Slides

# Code Walk through

- Walk through architecture
- Demo of what we get back from the API

```powershell
(iwr https://localhost:8080/WeatherForecast -Headers @{ Authorization = "Basic Ymxha2U6cEA1NXcwcmQ=" }).Content | ConvertFrom-Json
```

- Start talking about OTEL, let's instrument the AuthenticationService.
- Show packages.

- Basic startup code:

```csharp
using OpenTelemetry.Trace;

// ...

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddConsoleExporter();
    });
```

- Talk through Activity structure, then export to otel collector (within `WithTracing(tracing => { ... })`)

```csharp
tracing.AddOtlpExporter(options =>
{
    options.Endpoint = new Uri("http://collector:4317");
});
```

- Talk through open telemetry collector config

- Add resource builder to show info about service (just below `AddOpenTelemetry()`):

```csharp
.ConfigureResource(resource =>
{
    resource.AddService(builder.Environment.ApplicationName,
        serviceNamespace: "otelTalk",                                                // Group related services
        serviceVersion: Assembly.GetEntryAssembly()!.GetName().Version!.ToString()); // REALLY useful for deployment tracking

    var attributes = new Dictionary<string, object>()
    {
        { "environment", "production" },            // Check the docs for your APM tool for correct naming. (DataDog is deployment.environment)
        { "mycompany.service.owningteam", "SRE" }   // Useful for filtering and alerting
    };

    resource.AddAttributes(attributes);
})
```

- Set a breakpoint in the weather api and auth service to show trace context flow.
- Show demo code for custom context propagation

- Add sql client instrumention:

```csharp
// EntityFramework:
tracing.AddEntityFrameworkCoreInstrumentation();

// Raw SQL:
// tracing.AddSqlClientInstrumentation();
```

```csharp
tracing.AddEntityFrameworkCoreInstrumentation(x=>
{
    //x.SetDbStatementForText = true;
    x.EnrichWithIDbCommand = (activity, command) =>
    {
        activity.AddTag("db.statement", command.CommandText);
        activity.DisplayName = command.CommandText;
    };
});
```

- Custom spans - hash password

```csharp
// in AuthenticationService.cs
private readonly ActivitySource _activitySource = new(TelemetryConstants.ActivitySource);

// in HashPassword function:
using var activity = _activitySource.StartActivity(nameof(HashPassword));

// in program.cs
tracing.AddSource(TelemetryConstants.ActivitySource);
```

- What about errors? `iwr https://localhost:8080/weatherforecast -Headers @{Authorization = "Basic thisisntbase64"}`

```csharp
tracing.AddAspNetCoreInstrumentation(x =>
{
    x.RecordException = true;
});
```

- NOT TESTED: Tags and baggage

```csharp
// WeatherApi Controller
Activity.Current.AddTag("country", user.Country);
Activity.Current.AddBaggage("country", user.Country);

// ForecastService Program.cs
app.Use(async (context, next) =>
{
    foreach (var (key, value) in Activity.Current.Baggage)
    {
        Activity.Current.SetTag(key, value);
    }

    await next();
});
```

Custom events in Jaeger:

```csharp
Activity.Current.AddEvent(new ActivityEvent("authEvent", tags: new ActivityTagsCollection()
{
    { "customTag", "myValue" }
}));
```