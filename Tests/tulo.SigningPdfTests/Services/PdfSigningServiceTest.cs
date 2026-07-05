using PdfSharp.Drawing;
using PdfSharp.Pdf.Signatures;
using System.Diagnostics;
using Tulo.SigningPdf.Services;

namespace Tulo.SigningPdfTests.Services;

[TestClass]
public class PdfSignatureServiceTests
{
    private string _testRunDirectory = null!;
    private string _inputPdfPath = null!;
    private string _outputPdfPath = null!;
    private string _outputVisiblePdfPath = null!;
    private string _certificatePath = null!;

    [TestInitialize]
    public void Setup()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));

        _testRunDirectory = Path.Combine(solutionRoot, "TestResults", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRunDirectory);

        _inputPdfPath = Path.Combine(solutionRoot, "Shared", "Examples", "ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3.pdf");
        _certificatePath = Path.Combine(solutionRoot, "Shared", "Certificates", "dummyPdfA3Signing.pfx");
        _outputPdfPath = Path.Combine(_testRunDirectory, "ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed.pdf");
        _outputVisiblePdfPath = Path.Combine(_testRunDirectory, "ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed_visible.pdf");
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            //if (Directory.Exists(_testRunDirectory))
            //    Directory.Delete(_testRunDirectory, recursive: true);
        }
        catch
        {
            // ignore
        }
    }

    [TestMethod]
    public void SignPdf_Should_Create_Signed_Pdf_Successfully()
    {
        // Arrange
        var service = new PdfSignatureService();

        const string certificatePassword = "12345@";
        const string reason = "Unit Test Signatur";
        const string location = "Deutschland";
        const string contactInfo = "test@example.com";

        Assert.IsTrue(File.Exists(_inputPdfPath), $"Input PDF not found: {_inputPdfPath}");
        Assert.IsTrue(File.Exists(_certificatePath), $"Certificate not found: {_certificatePath}");

        // Act
        var result = service.SignPdf(_inputPdfPath, _outputPdfPath, _certificatePath, certificatePassword, reason, location, contactInfo);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success, $"Expected success, but got error: {result.Message}");
        Assert.IsTrue(File.Exists(_outputPdfPath), $"Signed PDF was not created: {_outputPdfPath}");

        var fileInfo = new FileInfo(_outputPdfPath);
        Assert.IsTrue(fileInfo.Length > 0, "Signed PDF is empty.");

        // Open the signed PDF with the default application
        if (result.Success && File.Exists(_outputPdfPath))
        {
            Process.Start(new ProcessStartInfo(_outputPdfPath) { UseShellExecute = true });
        }
    }

    [TestMethod]
    public void SignPdf_Should_Create_Visible_Signed_Pdf_With_Custom_Options()
    {
        // Arrange
        var signingService = new PdfSignatureService();

        const string certificatePassword = "12345@";
        const string reason = "Visible signature test";
        const string location = "Germany";
        const string contactInfo = "visible@test.example";

        var signatureRect = new XRect(400, 180, 140, 60);
        const int signaturePageIndex = 0;
        const PdfMessageDigestType digestType = PdfMessageDigestType.SHA256;

        Assert.IsTrue(File.Exists(_inputPdfPath), $"Input PDF not found: {_inputPdfPath}");
        Assert.IsTrue(File.Exists(_certificatePath), $"Certificate not found: {_certificatePath}");

        // Act
        var signResult = signingService.SignPdf(_inputPdfPath, _outputVisiblePdfPath, _certificatePath,
                                                certificatePassword, reason, location, contactInfo, digestType,
                                                visibleSignature: true, signatureRect: signatureRect,
                                                signaturePageIndex: signaturePageIndex);

        // Assert signing result
        Assert.IsNotNull(signResult);
        Assert.IsTrue(signResult.Success, $"Expected success, but got error: {signResult.Message}");
        Assert.IsTrue(File.Exists(_outputVisiblePdfPath), $"Signed PDF was not created: {_outputVisiblePdfPath}");

        var fileInfo = new FileInfo(_outputVisiblePdfPath);
        Assert.IsTrue(fileInfo.Length > 0, "Visible signed PDF is empty.");

        // Assert raw PDF markers for signature presence
        var pdfBytes = File.ReadAllBytes(_outputVisiblePdfPath);
        var pdfText = System.Text.Encoding.Latin1.GetString(pdfBytes);

        StringAssert.Contains(pdfText, "/ByteRange");
        StringAssert.Contains(pdfText, "/Contents");
        StringAssert.Contains(pdfText, "/Type/Sig");

        // Optional: open the signed PDF with the default application
        if (signResult.Success && File.Exists(_outputVisiblePdfPath))
        {
            Process.Start(new ProcessStartInfo(_outputVisiblePdfPath) { UseShellExecute = true });
        }
    }

    [TestMethod]
    public void SignPdf_Should_Fail_When_SignaturePageIndex_Is_Out_Of_Range()
    {
        // Arrange
        var service = new PdfSignatureService();

        const string certificatePassword = "12345@";

        Assert.IsTrue(File.Exists(_inputPdfPath), $"Input PDF not found: {_inputPdfPath}");
        Assert.IsTrue(File.Exists(_certificatePath), $"Certificate not found: {_certificatePath}");

        // Act
        var result = service.SignPdf(_inputPdfPath, _outputVisiblePdfPath, _certificatePath, certificatePassword,
                                     reason: "Invalid page index test", location: "Germany",
                                     contactInfo: "test@example.com", digestType: PdfMessageDigestType.SHA256,
                                     visibleSignature: true, signatureRect: new XRect(50, 700, 250, 60),
                                     signaturePageIndex: 999);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success, "Expected failure for invalid signaturePageIndex.");
        StringAssert.Contains(result.Message, "signaturePageIndex");

        Assert.IsFalse(File.Exists(_outputVisiblePdfPath),
            "Output PDF should not be created when signaturePageIndex is invalid.");
    }
}
