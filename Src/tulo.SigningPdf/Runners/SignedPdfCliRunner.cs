using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using tulo.SigningPdf.Interfaces;
using tulo.SigningPdf.ResultPattern;

namespace tulo.SigningPdf.Runners;

public sealed class SignedPdfCliRunner(IConfiguration configuration, IPdfSignatureService pdfSignatureService, ILoggerFactory loggerFactory) : ISignedPdfCliRunner
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IPdfSignatureService _pdfSignatureService = pdfSignatureService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<SignedPdfCliRunner>();

    public async Task<int> RunAsync(CancellationToken ct = default)
    {
        try
        {
            var inputPdfPath = _configuration[ConsoleApp.KeyInputPathPdf];
            var outputPathSignedPdf = _configuration[ConsoleApp.KeyOutputPathSignedPdf];
            var signaturePath = _configuration[ConsoleApp.KeySignaturePath];
            var publicKey = _configuration[ConsoleApp.KeyPublicKey];
            var reason = _configuration[ConsoleApp.KeyReason];
            var location = _configuration[ConsoleApp.KeyLocation];
            var contactInfo = _configuration[ConsoleApp.KeyContactInfo];

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeyInputPathPdf,
                string.IsNullOrWhiteSpace(inputPdfPath) ? "<empty>" : inputPdfPath);

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeyOutputPathSignedPdf,
                string.IsNullOrWhiteSpace(outputPathSignedPdf) ? "<empty>" : outputPathSignedPdf);

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeySignaturePath,
                string.IsNullOrWhiteSpace(signaturePath) ? "<empty>" : signaturePath);

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeyPublicKey,
                string.IsNullOrWhiteSpace(publicKey) ? "<empty>" : "<provided>");

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeyReason,
                string.IsNullOrWhiteSpace(reason) ? "<empty>" : reason);

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeyLocation,
                string.IsNullOrWhiteSpace(location) ? "<empty>" : location);

            _logger.LogInformation("CLI: {Key} = {Value}",
                ConsoleApp.KeyContactInfo,
                string.IsNullOrWhiteSpace(contactInfo) ? "<empty>" : contactInfo);

            if (string.IsNullOrWhiteSpace(inputPdfPath))
            {
                _logger.LogError("Missing required argument: {Arg}", ConsoleApp.ArgInputPathPdf);
                return ExitCodes.MissingInputPathPdf;
            }

            if (string.IsNullOrWhiteSpace(outputPathSignedPdf))
            {
                _logger.LogError("Missing required argument: {Arg}", ConsoleApp.ArgOutputPathSignedPdf);
                return ExitCodes.MissingOutputPathSignedPdf;
            }

            if (string.IsNullOrWhiteSpace(signaturePath))
            {
                _logger.LogError("Missing required argument: {Arg}", ConsoleApp.ArgSignaturePath);
                return ExitCodes.MissingSignaturePath;
            }

            if (string.IsNullOrWhiteSpace(publicKey))
            {
                _logger.LogError("Missing required argument: {Arg}", ConsoleApp.ArgPublicKey);
                return ExitCodes.MissingCertificatePassword;
            }

            if (!File.Exists(inputPdfPath))
            {
                _logger.LogError("Input PDF file not found: {Path}", inputPdfPath);
                return ExitCodes.InputPdfNotFound;
            }

            if (!File.Exists(signaturePath))
            {
                _logger.LogError("Certificate file not found: {Path}", signaturePath);
                return ExitCodes.CertificateFileNotFound;
            }

            var outputDirectory = Path.GetDirectoryName(outputPathSignedPdf);
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                outputDirectory = Directory.GetCurrentDirectory();
            }

            Directory.CreateDirectory(outputDirectory);

            _logger.LogInformation("Starting PDF signing.");
            _logger.LogInformation("Input PDF: {Path}", inputPdfPath);
            _logger.LogInformation("Output signed PDF: {Path}", outputPathSignedPdf);

            OperationResult result = _pdfSignatureService.SignPdf(
                inputPdfPath: inputPdfPath,
                outputPdfPath: outputPathSignedPdf,
                certificatePath: signaturePath,
                certificatePassword: publicKey,
                reason: reason,
                location: location,
                contactInfo: contactInfo);

            if (!result.Success)
            {
                _logger.LogError("SignPdf failed: {Message}", result.Message);
                return ExitCodes.SigningFailed;
            }

            if (!File.Exists(outputPathSignedPdf))
            {
                _logger.LogError("Signed PDF was not created: {Path}", outputPathSignedPdf);
                return ExitCodes.OutputSignedPdfMissing;
            }

            _logger.LogInformation("SUCCESS: Signed PDF created at {Path}", outputPathSignedPdf);
            await Task.CompletedTask;
            return ExitCodes.Okay;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CLI processing failed.");
            return ExitCodes.ProcessingFailed;
        }
    }

    public static class ExitCodes
    {
        public const int Okay = 0;
        public const int MissingInputPathPdf = 1;
        public const int MissingOutputPathSignedPdf = 2;
        public const int MissingSignaturePath = 3;
        public const int MissingCertificatePassword = 4;
        public const int InputPdfNotFound = 5;
        public const int CertificateFileNotFound = 6;
        public const int SigningFailed = 7;
        public const int OutputSignedPdfMissing = 8;
        public const int ProcessingFailed = 10;
    }
}
