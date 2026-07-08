using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf.Annotations;

namespace Tulo.SigningPdf.Handlers;

public sealed class VisibleSignatureAppearanceHandler : IAnnotationAppearanceHandler
{
    private readonly string? _signerName;
    private readonly string? _organization;
    private readonly string? _email;
    private readonly string? _reason;
    private readonly string? _location;

    public VisibleSignatureAppearanceHandler(string? signerName, string? organization, string? email, string? reason, string? location)
    {
        _signerName = signerName;
        _organization = organization;
        _email = email;
        _reason = reason;
        _location = location;
    }

    public void DrawAppearance(XGraphics gfx, XRect rect)
    {
        var localRect = new XRect(0, 0, rect.Width, rect.Height);

        gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(220, 235, 255)), localRect); //rectagle background color
        gfx.DrawRectangle(new XPen(XColors.Gray, 1), localRect);

        var titleFont = new XFont("Arial", 8, XFontStyleEx.Bold);
        var textFont = new XFont("Arial", 7, XFontStyleEx.Regular);
        var formatter = new XTextFormatter(gfx);

        var titleRect = new XRect(6, 4, rect.Width - 12, 12);
        var bodyRect = new XRect(6, 18, rect.Width - 12, rect.Height - 24);

        formatter.DrawString("Digitally signed", titleFont, XBrushes.DarkSlateGray, titleRect, XStringFormats.TopLeft);

        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(_signerName))
            lines.Add($"Signer: {_signerName}");

        if (!string.IsNullOrWhiteSpace(_organization)) 
            lines.Add($"Organization: {_organization}");

        if (!string.IsNullOrWhiteSpace(_email))
            lines.Add($"Email:        {_email}");

        if (!string.IsNullOrWhiteSpace(_reason))
            lines.Add($"Reason: {_reason}");

        if (!string.IsNullOrWhiteSpace(_location))
            lines.Add($"Location: {_location}");

        lines.Add($"Date: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");

        formatter.DrawString(string.Join(Environment.NewLine, lines), textFont, XBrushes.Black, bodyRect, XStringFormats.TopLeft);
    }
}
