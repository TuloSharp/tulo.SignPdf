using Serilog;

namespace Tulo.SigningPdf.Serilog;

/// <summary>
/// Provides functionality for creating and initializing a Serilog bootstrap logger.
/// This logger is configured to output structured logs to the console with enriched context,
/// and is intended for use during early application startup before the full logger is configured.
/// </summary>
public static class SerilogBootstrapLogger
{
    /// <summary>
    /// Creates and initializes the Serilog bootstrap logger with a predefined configuration.
    /// The logger enriches log events with thread ID, process ID, and username, and writes to the console.
    /// A logger instance for the <see cref="BootstrapLogger"/> context is returned for early logging purposes.
    /// </summary>
    /// <returns>
    /// An <see cref="ILogger"/> instance scoped to the <see cref="BootstrapLogger"/> context.
    /// </returns>
    public static ILogger Create()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.With<UsernameEnrichment>()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy.MM.dd-HH:mm:ss.fff} [{Username}] [Thread:{ThreadId}] [ProcessID:{ProcessId}] [{Level:u2}] ({SourceContext}) {Message:lj}{NewLine}{Exception}")
            .CreateBootstrapLogger();

        var bootstrapLogger = Log.Logger.ForContext<BootstrapLogger>();
        bootstrapLogger.Information("Bootstrap logger initialized");
        return bootstrapLogger;
    }
}

internal class BootstrapLogger { }
