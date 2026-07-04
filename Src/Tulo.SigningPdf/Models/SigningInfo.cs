namespace Tulo.SigningPdf.Models;

public sealed class SigningInfo
{
    public string OutputPath { get; set; } = string.Empty;
    public string? SignerName { get; set; }
    public DateTime SignedAt { get; set; }
    public string DigestAlgorithm { get; set; } = string.Empty;
    public bool IsCertificateExpired { get; set; }
    public DateTime? CertValidFrom { get; set; }
    public DateTime? CertValidTo { get; set; }
    public string? CertificateSubject { get; set; }
    public string? CertificateIssuer { get; set; }
    public bool AlreadySigned { get; set; }
}
