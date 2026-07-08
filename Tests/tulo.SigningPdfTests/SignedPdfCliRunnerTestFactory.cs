using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharp.Drawing;
using Tulo.SigningPdf;
using Tulo.SigningPdf.Interfaces;
using Tulo.SigningPdf.Runners;
using Tulo.SigningPdf.Services;
using ExtConfig = Microsoft.Extensions.Configuration;
using ExtLogging = Microsoft.Extensions.Logging;

namespace Tulo.SigningPdfTests;

public static class SignedPdfCliRunnerTestFactory
{
    public static SignedPdfCliRunner CreateRunner(string inputPdfPath,
                                                  string outputPdfPath,
                                                  string certificatePath,
                                                  string certificatePassword,
                                                  string? reason = null,
                                                  string? location = null,
                                                  string? contactInfo = null,
                                                  XRect? signatureRect = null)
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
            [ConsoleApp.KeySignatureRect] = signatureRect.HasValue ? $"{signatureRect.Value.X},{signatureRect.Value.Y},{signatureRect.Value.Width},{signatureRect.Value.Height}" : null
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
