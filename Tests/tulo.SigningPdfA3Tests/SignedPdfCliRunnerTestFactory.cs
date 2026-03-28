using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tulo.SigningPdfA3;
using tulo.SigningPdfA3.Interfaces;
using tulo.SigningPdfA3.Runners;
using tulo.SigningPdfA3.Services;
using ExtConfig = Microsoft.Extensions.Configuration;
using ExtLogging = Microsoft.Extensions.Logging;

namespace tulo.SigningPdfA3Tests;

public static class SignedPdfCliRunnerTestFactory
{
    public static SignedPdfCliRunner CreateRunner(string inputPdfPath, string outputPdfPath, string certificatePath, string certificatePassword, string? reason = null, string? location = null, string? contactInfo = null)
    {
        var configValues = new Dictionary<string, string?>
        {
            [ConsoleApp.KeyInputPathPdf] = inputPdfPath,
            [ConsoleApp.KeyOutputPathSignedPdf] = outputPdfPath,
            [ConsoleApp.KeySignaturePath] = certificatePath,
            [ConsoleApp.KeyPublicKey] = certificatePassword,
            [ConsoleApp.KeyReason] = reason,
            [ConsoleApp.KeyLocation] = location,
            [ConsoleApp.KeyContactInfo] = contactInfo,
        };

        var configuration = new ExtConfig.ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<ExtConfig.IConfiguration>(configuration);

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
            builder.SetMinimumLevel(ExtLogging.LogLevel.Debug);
        });

        services.AddSingleton<IPdfSignatureService, PdfSignatureService>();

        var provider = services.BuildServiceProvider();

        return new SignedPdfCliRunner(
            provider.GetRequiredService<ExtConfig.IConfiguration>(),
            provider.GetRequiredService<IPdfSignatureService>(),
            provider.GetRequiredService<ExtLogging.ILoggerFactory>());
    }
}
