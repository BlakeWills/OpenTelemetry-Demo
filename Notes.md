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

- Basic startup code.

```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Data;
using System.Reflection;

// ...

// Configure OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder.AddAspNetCoreInstrumentation();

    builder.AddConsoleExporter();
});
```

- Talk through Activity structure, then export to otel collector

```csharp
builder.AddOtlpExporter(options =>
{
    options.Endpoint = new Uri("http://collector:4317"); 
});
```

- Talk through open telemetry collector config

- Add resource builder to show info about service:

```csharp
var assemblyName = typeof(Program).Assembly.GetName();

var resourceBuilder = ResourceBuilder.CreateEmpty()
    .AddService(assemblyName.Name, serviceNamespace: "OtelTalk", assemblyName.Version!.ToString(), serviceInstanceId: Environment.MachineName);

resourceBuilder.AddAttributes(new Dictionary<string, object>
{
    { "deployment.environment", "production" },
    { "mycompany.service.owningteam", "SRE" }
});

builder.SetResourceBuilder(resourceBuilder);
```

- Set a breakpoint in the weather api and auth service to show trace context flow.
- Show demo code for custom context propagation

- Add sql client instrumention:

```csharp
builder.AddSqlClientInstrumentation();
```

```csharp
builder.AddSqlClientInstrumentation(x =>
{
    x.Enrich = (activity, name, cmdObj) =>
    {
        var command = (IDbCommand)cmdObj;
        activity.DisplayName = command.CommandText;
        activity.SetTag("peer.service", command.Connection.Database);
    };
});
```

- Custom spans - hash password

```csharp
\\ in program.cs
var activitySourceProvider = new ActivitySourceProvider();
builder.Services.AddSingleton<ActivitySourceProvider>(activitySourceProvider);

...
builder.AddSource(activitySourceProvider.Current.Name);

\\ in AuthenticationService.cs
\\ inject ActivitySourceProvider
using var activity = _activitySourceProvider.Current.StartActivity(nameof(HashPassword), System.Diagnostics.ActivityKind.Internal);
```

- What about errors? `iwr https://localhost:8080/weatherforecast -Headers @{Authorization = "Basic thisisntbase64"}`

```csharp
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Activity.Current.RecordException(ex);
        throw;
    }
});
```

- Tags and baggage

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