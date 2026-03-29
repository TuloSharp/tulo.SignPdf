namespace tulo.SigningPdf.Runners;

public interface ISignedPdfCliRunner
{
    Task<int> RunAsync(CancellationToken ct = default);
}
