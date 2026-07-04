using Serilog.Core;
using Serilog.Events;

namespace Tulo.SigningPdf.Serilog;

/// <inheritdoc />
public class UsernameEnrichment : ILogEventEnricher
{
    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        string username = "Unknown";

        if (OperatingSystem.IsWindows())
            username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        var customProperty = new LogEventProperty("Username", new ScalarValue($"{username}"));
        logEvent.AddPropertyIfAbsent(customProperty);
    }
}
