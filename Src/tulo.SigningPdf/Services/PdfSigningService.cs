using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Signatures;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Tulo.SigningPdf.Handlers;
using Tulo.SigningPdf.Interfaces;
using Tulo.SigningPdf.Models;
using Tulo.SigningPdf.ResultPattern;
using Tulo.SigningPdf.Utilities;

namespace Tulo.SigningPdf.Services;

public sealed class PdfSignatureService : IPdfSignatureService
{
    private static readonly HashSet<PdfMessageDigestType> _allowedAlgorithms =
    [
        PdfMessageDigestType.SHA256,
        PdfMessageDigestType.SHA384,
        PdfMessageDigestType.SHA512
    ];

    public PdfSignatureService()
    {
        GlobalFontSettings.FontResolver ??= new EmbeddedFontResolver();
    }

    public OperationResult SignPdf(string inputPdfPath,
                                   string outputPdfPath,
                                   string certificatePath,
                                   string certificatePassword,
                                   string? reason = null,
                                   string? location = null,
                                   string? contactInfo = null,
                                   PdfMessageDigestType digestType = PdfMessageDigestType.SHA256,
                                   bool visibleSignature = false,
                                   XRect? signatureRect = null,
                                   int signaturePageIndex = 0)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
                return OperationResult<SigningInfo>.Fail("Input PDF path is empty.");

            if (string.IsNullOrWhiteSpace(outputPdfPath))
                return OperationResult<SigningInfo>.Fail("Output PDF path is empty.");

            if (string.IsNullOrWhiteSpace(certificatePath))
                return OperationResult<SigningInfo>.Fail("Certificate path is empty.");

            if (!File.Exists(inputPdfPath))
                return OperationResult<SigningInfo>.Fail($"Input PDF file not found: {inputPdfPath}");

            if (!File.Exists(certificatePath))
                return OperationResult<SigningInfo>.Fail($"Certificate file not found: {certificatePath}");

            if (!_allowedAlgorithms.Contains(digestType))
                return OperationResult<SigningInfo>.Fail(
                    $"Digest algorithm '{digestType}' is not allowed. Use SHA256, SHA384 or SHA512.");

            var outputDirectory = Path.GetDirectoryName(outputPdfPath);
            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            X509Certificate2 certificate;
            try
            {
                certificate = new X509Certificate2(
                    certificatePath,
                    certificatePassword,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception ex)
            {
                return OperationResult<SigningInfo>.Fail(
                    $"Failed to load certificate: {ex.Message}");
            }

            if (!certificate.HasPrivateKey)
                return OperationResult<SigningInfo>.Fail(
                    "The certificate does not contain a private key. Please use a .pfx or .p12 file.");

            var isCertificateExpired = DateTime.UtcNow > certificate.NotAfter;

            var inputPdfBytes = File.ReadAllBytes(inputPdfPath);
            var inputPdfText = Encoding.Latin1.GetString(inputPdfBytes);

            // CHANGED
            var alreadySigned = Regex.IsMatch(
                inputPdfText,
                @"/ByteRange\s*\[\s*\d+\s+\d+\s+\d+\s+\d+\s*\]");

            using var document = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify);

            // CHANGED
            if (visibleSignature &&
                (signaturePageIndex < 0 || signaturePageIndex >= document.Pages.Count))
            {
                return OperationResult<SigningInfo>.Fail(
                    $"signaturePageIndex {signaturePageIndex} is out of range. Document has {document.Pages.Count} page(s).");
            }

            // CHANGED
            DigitalSignatureOptions signatureOptions;

            if (visibleSignature)
            {
                var worldRect = signatureRect ?? new XRect(50, 700, 220, 60);
                var targetPage = document.Pages[signaturePageIndex];
                var pdfRect = ConvertTopLeftToPdfRect(worldRect, targetPage.Height.Point);

                signatureOptions = new DigitalSignatureOptions
                {
                    Reason = reason ?? string.Empty,
                    Location = location ?? string.Empty,
                    ContactInfo = contactInfo ?? string.Empty,
                    PageIndex = signaturePageIndex,
                    Rectangle = pdfRect,
                    AppearanceHandler = new VisibleSignatureAppearanceHandler(certificate.GetNameInfo(X509NameType.SimpleName, false), reason, location)
                };
            }
            else
            {
                signatureOptions = new DigitalSignatureOptions
                {
                    Reason = reason ?? string.Empty,
                    Location = location ?? string.Empty,
                    ContactInfo = contactInfo ?? string.Empty,
                    PageIndex = 0,
                    Rectangle = new XRect(0, 0, 0, 0),
                    AppearanceHandler = null
                };
            }

            var signer = new PdfSharpDefaultSigner(certificate, digestType);

            DigitalSignatureHandler.ForDocument(document, signer, signatureOptions);

            document.Save(outputPdfPath);

            var warnings = new List<string>();

            if (alreadySigned)
                warnings.Add("Input PDF was already signed. An additional signature was added.");

            if (isCertificateExpired)
                warnings.Add($"Certificate expired on {certificate.NotAfter:dd.MM.yyyy}.");

            var message = warnings.Count == 0
                ? $"Signed PDF created successfully: {outputPdfPath}"
                : $"Signed PDF created successfully with warnings: {string.Join(" | ", warnings)}";

            return OperationResult<SigningInfo>.Ok(
                new SigningInfo
                {
                    OutputPath = outputPdfPath,
                    SignerName = certificate.GetNameInfo(X509NameType.SimpleName, false),
                    SignedAt = DateTime.UtcNow,
                    DigestAlgorithm = digestType.ToString(),
                    IsCertificateExpired = isCertificateExpired,
                    CertValidFrom = certificate.NotBefore,
                    CertValidTo = certificate.NotAfter,
                    CertificateSubject = certificate.Subject,
                    CertificateIssuer = certificate.Issuer,
                    AlreadySigned = alreadySigned
                },
                message);
        }
        catch (Exception ex)
        {
            return OperationResult<SigningInfo>.Fail($"Signing failed: {ex.Message}");
        }
    }

    private static XRect ConvertTopLeftToPdfRect(XRect worldRect, double pageHeight)
    {
        return new XRect(
            worldRect.X,
            pageHeight - worldRect.Y - worldRect.Height,
            worldRect.Width,
            worldRect.Height);
    }
}