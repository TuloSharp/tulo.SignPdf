using System.Diagnostics;
using tulo.SigningPdfA3.Runners;

namespace tulo.SigningPdfA3Tests.Runners;

[TestClass]
public class SignedPdfCliRunnerTests
{
    private string _testRunDirectory = null!;
    private string _inputPdfPath = null!;
    private string _outputPdfPath = null!;
    private string _certificatePath = null!;

    [TestInitialize]
    public void Setup()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));

        _testRunDirectory = Path.Combine(solutionRoot, "TestResults", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRunDirectory);

        _inputPdfPath = Path.Combine(baseDir, "Examples", "ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3.pdf");
        _certificatePath = Path.Combine(baseDir, "Certificates", "dummyPdfA3Signing.pfx");
        _outputPdfPath = Path.Combine(_testRunDirectory, "ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed.pdf");
    }

    [TestMethod]
    public async Task RunAsync_Should_Sign_Pdf_Successfully()
    {
        // Arrange
        var runner = SignedPdfCliRunnerTestFactory.CreateRunner(
            inputPdfPath: _inputPdfPath,
            outputPdfPath: _outputPdfPath,
            certificatePath: _certificatePath,
            certificatePassword: "12345@",
            reason: "Unit Test Signatur",
            location: "Deutschland",
            contactInfo: "test@example.com"
        );

        Assert.IsTrue(File.Exists(_inputPdfPath), $"Input PDF not found: {_inputPdfPath}");
        Assert.IsTrue(File.Exists(_certificatePath), $"Certificate not found: {_certificatePath}");

        // Act
        var exitCode = await runner.RunAsync();

        // Assert
        Assert.AreEqual(SignedPdfCliRunner.ExitCodes.Okay, exitCode,
            $"Expected exit code {SignedPdfCliRunner.ExitCodes.Okay}, but got {exitCode}.");

        Assert.IsTrue(File.Exists(_outputPdfPath),
            $"Signed PDF was not created: {_outputPdfPath}");

        var fileInfo = new FileInfo(_outputPdfPath);
        Assert.IsTrue(fileInfo.Length > 0, "Signed PDF is empty.");

        // Open the signed PDF with the default application
        if (File.Exists(_outputPdfPath))
        {
            Process.Start(new ProcessStartInfo(_outputPdfPath) { UseShellExecute = true });
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Intentionally not deleting _testRunDirectory,
        // so the signed PDF remains accessible under TestResults/
    }
}
