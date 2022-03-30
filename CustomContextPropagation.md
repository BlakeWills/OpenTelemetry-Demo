To create an activity with a known trace id:

```
/// Azure Isolated Function Middleware
public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
{
    var activityContext = GetActivityContext(context.BindingContext);

    using var activity = myActivitySource.StartActivity(context.FunctionDefinition.Name, ActivityKind.Server, activityContext);

    // ...
}

private static ActivityContext GetActivityContext(BindingContext context)
{
    if(!TryGetTraceIdFromServiceBusHeaders(context, out var traceId))
    {
        return default;
    }

    _ = ActivityContext.TryParse(traceId, null, out var activityContext);
    return activityContext;
}
```