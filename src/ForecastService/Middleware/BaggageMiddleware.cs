using System.Diagnostics;

namespace ForecastService.Middleware;

public class BaggageMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if(Activity.Current != null && Activity.Current.Baggage.Any())
        {
            foreach (var item in Activity.Current.Baggage)
            {
                Activity.Current.AddTag(item.Key, item.Value);
            }
        }

        return next(context);
    }
}
