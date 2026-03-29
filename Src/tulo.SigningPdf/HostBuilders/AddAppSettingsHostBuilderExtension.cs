using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace tulo.SigningPdf.HostBuilders;

/// <summary>
/// Provides extension methods for configuring application settings in an <see cref="IHostBuilder"/>.
/// </summary>
public static class AddAppSettingsHostBuilderExtension
{
    /// <summary>
    /// Adds additional appsettings.json files to the host configuration, including:
    /// - a machine-specific settings file (appsettings.{machine}.json)
    /// - a centralized application-level appsettings.json file from a sibling directory.
    /// </summary>
    /// <param name="host">The <see cref="IHostBuilder"/> to extend.</param>
    /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder AddAppSettings(this IHostBuilder host)
    {
        var bootstrapLogger = Log.Logger.ForContext<AddAppSettings>();

        host.ConfigureAppConfiguration((context, configBuilder) =>
        {
            // load appsettings.json from different locations
            // the starting user can have a appsettings.json-file with the machine name in it
            var machineNameAppSettingsPath = $"appsettings.{Environment.MachineName.ToLowerInvariant()}.json";
            if (File.Exists(machineNameAppSettingsPath))
            {
                configBuilder.AddJsonFile(machineNameAppSettingsPath, true);
                bootstrapLogger.Information($"settings file '{machineNameAppSettingsPath}' for machine '{Environment.MachineName.ToLowerInvariant()}' loaded...");
            }

            // Notice: these appsettings.json must always be added last
            // the application can have an appsettings.json file in folder located within parent directory of the application
            // folder name = "name of application directory" + -"appsettings"
            var parentAppDirInfo = new DirectoryInfo(context.HostingEnvironment.ContentRootPath).Parent;
            if (parentAppDirInfo != null)
            {
                var appDirInfo = new DirectoryInfo(context.HostingEnvironment.ContentRootPath);
                var applicationAppSettingsFolder = Path.Combine(parentAppDirInfo.FullName, $"{appDirInfo.Name}-appsettings");
                var applicationAppSettingsPath = Path.Combine(applicationAppSettingsFolder, "appsettings.json");
                if (File.Exists(applicationAppSettingsPath))
                {
                    configBuilder.AddJsonFile(applicationAppSettingsPath, true);
                    bootstrapLogger.Information($"settings file 'appsettings.json' from app data folder '{applicationAppSettingsFolder}' loaded...");
                }
                else
                {
                    bootstrapLogger.Information($"settings file 'appsettings.json' from app data folder '{applicationAppSettingsFolder}' not loaded...");
                }
            }
        });

        bootstrapLogger.Information($"{nameof(AddAppSettings)} has been initialized successfully.");

        return host;
    }
}

internal class AddAppSettings;
