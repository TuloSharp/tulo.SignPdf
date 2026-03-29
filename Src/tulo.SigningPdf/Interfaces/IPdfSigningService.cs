using tulo.SigningPdf.ResultPattern;

namespace tulo.SigningPdf.Interfaces;

public interface IPdfSignatureService
{
    OperationResult SignPdf(string inputPdfPath, string outputPdfPath, string certificatePath, string certificatePassword, string? reason, string? location, string? contactInfo);
}