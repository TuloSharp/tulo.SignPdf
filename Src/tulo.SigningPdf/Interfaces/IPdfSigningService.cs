using PdfSharp.Drawing;
using PdfSharp.Pdf.Signatures;
using Tulo.SigningPdf.ResultPattern;

namespace Tulo.SigningPdf.Interfaces;

public interface IPdfSignatureService
{
    OperationResult SignPdf(string inputPdfPath,
                            string outputPdfPath,
                            string certificatePath,
                            string certificatePassword,
                            string? reason = null,
                            string? location = null,
                            string? contactInfo = null,
                            PdfMessageDigestType digestType = PdfMessageDigestType.SHA256,
                            bool visibleSignature = false,
                            XRect? signatureRect = null,
                            int signaturePageIndex = 0);
}