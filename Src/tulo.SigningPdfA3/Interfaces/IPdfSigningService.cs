using tulo.SigningPdfA3.ResultPattern;

namespace tulo.SigningPdfA3.Interfaces;

public interface IPdfSignatureService
{
    OperationResult SignPdf(string inputPdfPath, string outputPdfPath, string certificatePath, string certificatePassword, string? reason, string? location, string? contactInfo);
}