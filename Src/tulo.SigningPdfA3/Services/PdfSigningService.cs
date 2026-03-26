using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Signatures;
using System.Security.Cryptography.X509Certificates;
using tulo.SigningPdfA3.Interfaces;
using tulo.SigningPdfA3.ResultPattern;

namespace tulo.SigningPdfA3.Services;

public sealed class PdfSignatureService : IPdfSignatureService
{
    public OperationResult SignPdf(string inputPdfPath, string outputPdfPath, string certificatePath, string certificatePassword, string? reason, string? location, string? contactInfo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
                return OperationResult.Fail("Input PDF path is empty.");

            if (string.IsNullOrWhiteSpace(outputPdfPath))
                return OperationResult.Fail("Output PDF path is empty.");

            if (string.IsNullOrWhiteSpace(certificatePath))
                return OperationResult.Fail("Certificate path is empty.");

            if (!File.Exists(inputPdfPath))
                return OperationResult.Fail($"Input PDF file not found: {inputPdfPath}");

            if (!File.Exists(certificatePath))
                return OperationResult.Fail($"Certificate file not found: {certificatePath}");

            var outputDirectory = Path.GetDirectoryName(outputPdfPath);
            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var certificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            if (!certificate.HasPrivateKey)
                return OperationResult.Fail("The certificate does not contain a private key. Please use a .pfx or .p12 file.");

            using var document = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify);

            var signatureOptions = new DigitalSignatureOptions
            {
                Reason = reason ?? string.Empty,
                Location = location ?? string.Empty,
                ContactInfo = contactInfo ?? string.Empty,
                // Optional: visible signature
                //Rectangle = new XRect(50, 50, 200, 50),
            };

            var signer = new PdfSharpDefaultSigner(certificate, PdfMessageDigestType.SHA256);

            DigitalSignatureHandler.ForDocument(document, signer, signatureOptions);

            document.Save(outputPdfPath);

            return OperationResult.Ok($"Signed PDF created successfully: {outputPdfPath}");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail($"Signing failed: {ex.Message}");
        }
    }
}