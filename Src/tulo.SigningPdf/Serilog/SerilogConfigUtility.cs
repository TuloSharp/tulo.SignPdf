using Microsoft.Extensions.Configuration;

namespace tulo.SigningPdf.Serilog;

/// <summary>
/// Utility class for working with Serilog configuration settings.
/// Provides helper methods to retrieve log paths and connection strings from the application's configuration.
/// </summary>
public static class SerilogConfigUtility
{
    private const string MsSqlServer = "MSSqlServer";

    /// <summary>
    /// Checks whether the configuration contains a Microsoft SQL Server sink.
    /// </summary>
    /// <param name="configuration">The application's configuration object.</param>
    /// <returns>
    /// True if an MSSQL Server sink is configured; otherwise, false.
    /// </returns>
    public static bool HasMSSqlServerSink(IConfiguration configuration)
    {
        var writeToSection = configuration.GetSection("Serilog:WriteTo");
        foreach (var sink in writeToSection.GetChildren())
        {
            var name = sink.GetValue<string>("Name");

            if (name == MsSqlServer)
            {
                return true;
            }

            // Check if it's an Async wrapper with configure array
            if (name == "Async")
            {
                var configureSection = sink.GetSection("Args:configure");
                foreach (var innerSink in configureSection.GetChildren())
                {
                    var innerName = innerSink.GetValue<string>("Name");
                    if (innerName == MsSqlServer)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieves the connection string for the Microsoft SQL Server sink from the configuration.
    /// </summary>
    /// <param name="configuration">The application's configuration object.</param>
    /// <returns>
    /// The MSSQL Server connection string if found; otherwise, null.
    /// </returns>
    public static string? GetMSSqlServerConnectionString(IConfiguration configuration)
    {
        var writeToSection = configuration.GetSection("Serilog:WriteTo");

        foreach (var sink in writeToSection.GetChildren())
        {
            var name = sink.GetValue<string>("Name");

            if (name == MsSqlServer)
            {
                return sink.GetSection("Args").GetValue<string>("connectionString");
            }

            // Check if it's an Async wrapper with configure array
            if (name == "Async")
            {
                var configureSection = sink.GetSection("Args:configure");
                foreach (var innerSink in configureSection.GetChildren())
                {
                    var innerName = innerSink.GetValue<string>("Name");
                    if (innerName == MsSqlServer)
                    {
                        return innerSink.GetSection("Args").GetValue<string>("connectionString");
                    }
                }
            }
        }
        return null; // MSSqlServer Sink not found
    }

    /// <summary>
    /// Retrieves the general log file path from the configuration, regardless of log level.
    /// </summary>
    /// <param name="configuration">The application's configuration object.</param>
    /// <returns>The path to the general log file if configured; otherwise, null.</returns>
    public static string? GetGeneralLogPath(IConfiguration configuration)
    {
        return GetLogPath(configuration, restrictedToMinimumLevel: null);
    }

    /// <summary>
    /// Retrieves the error log file path from the configuration, filtered by "Error" log level.
    /// </summary>
    /// <param name="configuration">The application's configuration object.</param>
    /// <returns>The path to the error log file if configured; otherwise, null.</returns>
    public static string? GetErrorLogPath(IConfiguration configuration)
    {
        return GetLogPath(configuration, restrictedToMinimumLevel: "Error");
    }

    /// <summary>
    /// Retrieves the log file path from the configuration, optionally filtered by minimum log level.
    /// </summary>
    /// <param name="configuration">The application's configuration object.</param>
    /// <param name="restrictedToMinimumLevel">
    /// The minimum log level to filter log paths by (e.g., "Error"). If null, retrieves an unrestricted log path.
    /// </param>
    /// <returns>The log file path if found; otherwise, null.</returns>
    private static string? GetLogPath(IConfiguration configuration, string? restrictedToMinimumLevel)
    {
        var serilogSection = configuration.GetSection("Serilog:WriteTo");
        if (!serilogSection.Exists()) return null;

        foreach (var writer in serilogSection.GetChildren())
        {
            var name = writer["Name"];
            if (!string.Equals(name, "Async", StringComparison.OrdinalIgnoreCase)) continue;

            var configureArray = writer.GetSection("Args:configure").GetChildren();
            foreach (var innerWriter in configureArray)
            {
                var innerName = innerWriter["Name"];
                if (!string.Equals(innerName, "File", StringComparison.OrdinalIgnoreCase)) continue;

                var args = innerWriter.GetSection("Args");
                var level = args["restrictedToMinimumLevel"];
                var path = args["path"];

                if (restrictedToMinimumLevel == null && string.IsNullOrEmpty(level) ||
                    restrictedToMinimumLevel != null && string.Equals(level, restrictedToMinimumLevel, StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
            }
        }
        return null;
    }
}
