using Microsoft.Extensions.Hosting;
using Serilog;
using tulo.SigningPdfA3.Serilog;

namespace tulo.SigningPdfA3.HostBuilders;

/// <summary>
/// Provides an extension method to configure Serilog for an <see cref="IHostBuilder"/>.
/// </summary>
public static class AddSeriLogHostBuilderExtension
{
    /// <summary>
    /// Configures Serilog as the logging provider for the host.
    /// </summary>
    /// <param name="host">The <see cref="IHostBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder AddSerilog(this IHostBuilder host)
    {
        var bootstrapLogger = Log.Logger.ForContext<AddSerilog>();

        host.UseSerilog((host, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(host.Configuration)
                               .Enrich.With<UsernameEnrichment>()
                               .Enrich.WithThreadId()
                               .Enrich.WithProcessId()
                               .Enrich.FromLogContext();

            // MSSqlServer sink exists
            if (SerilogConfigUtility.HasMSSqlServerSink(host.Configuration))
            {
                loggerConfiguration.Enrich.WithProperty("IsAcknowledged", false);
                loggerConfiguration.Enrich.WithProperty("Direction", "none");
            }
        });

        bootstrapLogger.Information("Serilog has been initialized successfully.");

        return host;
    }
}

internal class AddSerilog;
