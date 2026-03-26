namespace tulo.SigningPdfA3.Runners;

public interface ISignedPdfCliRunner
{
    Task<int> RunAsync(CancellationToken ct = default);
}
