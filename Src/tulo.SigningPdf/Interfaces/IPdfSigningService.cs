using Tulo.SigningPdf.ResultPattern;

namespace Tulo.SigningPdf.Interfaces;

public interface IPdfSignatureService
{
    OperationResult SignPdf(string inputPdfPath, string outputPdfPath, string certificatePath, string certificatePassword, string? reason, string? location, string? contactInfo);
}