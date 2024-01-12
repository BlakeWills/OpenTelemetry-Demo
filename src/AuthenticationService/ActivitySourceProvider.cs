using System.Diagnostics;

namespace AuthenticationService;

public class ActivitySourceProvider
{
    public ActivitySourceProvider()
    {
        Current = new ActivitySource("mycompany.instrumentation");
    }

    public ActivitySource Current { get; }
}
