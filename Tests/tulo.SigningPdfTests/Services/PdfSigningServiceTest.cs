using System.Diagnostics;
using tulo.SigningPdf.Services;

namespace tulo.SigningPdfTests.Services;

[TestClass]
public class PdfSignatureServiceTests
{
    private string _testRunDirectory = null!;
    private string _inputPdfPath = null!;
    private string _outputPdfPath = null!;
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
}
