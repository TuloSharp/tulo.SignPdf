using tulo.SigningPdfA3.Services;

namespace tulo.SigningPdfA3Tests.Services;

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
        _testRunDirectory = Path.Combine(Path.GetTempPath(), "PdfSignatureServiceTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRunDirectory);

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        _inputPdfPath = Path.Combine(baseDir, "Examples", "ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3.pdf");
        _certificatePath = Path.Combine(baseDir, "Certificates", "dummyPdfA3Signing.pfx");
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
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            if (Directory.Exists(_testRunDirectory))
                Directory.Delete(_testRunDirectory, recursive: true);
        }
        catch
        {
            // ignore
        }
    }
}
