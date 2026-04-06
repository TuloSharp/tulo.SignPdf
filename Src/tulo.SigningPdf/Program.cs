using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using System.Collections.Concurrent;
using tulo.SigningPdf;
using tulo.SigningPdf.Exceptions;
using tulo.SigningPdf.Runners;
using tulo.SigningPdf.Serilog;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        #region Set Working Directory
        // ✅ Also works with single-file publishing
       var exeDir = AppContext.BaseDirectory;
        if (!string.IsNullOrEmpty(exeDir))
        {
            Directory.SetCurrentDirectory(exeDir);
        }
        #endregion

        #region Create SerilogBootstrapLogger
        var bootstrapLogger = SerilogBootstrapLogger.Create();
        #endregion

        #region Enable selfLogErrors
#if DEBUG
        var selfLogErrors = new ConcurrentBag<string>();
        SelfLog.Enable(selfLogErrors.Add);
#endif
        #endregion

        #region Create Host
        var app = new ConsoleApp(args);
        var hostBuilder = app.InitiateHostBuilder();
        using var host = app.BuildHost(hostBuilder);
        #endregion

        #region Get Services
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Program).FullName!);
        #endregion

        #region Check SelfLog errors before starting app
#if DEBUG
        // Check for SelfLog errors before starting app
        if (!selfLogErrors.IsEmpty)
        {
            logger.LogError("Serilog internal errors detected, application will not start.");
            foreach (var msg in selfLogErrors) { logger.LogError("{Message}", msg); }

            // Throw exception to prevent app startup
            throw new StartupException("Serilog internal errors detected. Startup aborted.");
        }
#endif
        #endregion

        #region RunApp - One shot
        try
        {
            var runner = scope.ServiceProvider.GetRequiredService<ISignedPdfCliRunner>();
            var exitCode = await runner.RunAsync();
            return exitCode;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "signed PdfA3 extended createion is failed");
            return 99;
        }
        finally
        {
            Log.CloseAndFlush();
        }
        #endregion
    }
}