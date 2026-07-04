using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf.Annotations;

namespace Tulo.SigningPdf.Handlers;

internal sealed class VisibleSignatureAppearanceHandler : IAnnotationAppearanceHandler
{
    private readonly string? _signerName;
    private readonly string? _reason;
    private readonly string? _location;

    public VisibleSignatureAppearanceHandler(string? signerName, string? reason, string? location)
    {
        _signerName = signerName;
        _reason = reason;
        _location = location;
    }

    public void DrawAppearance(XGraphics gfx, XRect rect)
    {
        var borderPen = new XPen(XColors.DarkBlue, 0.8);
        var titleFont = new XFont("Arial", 7, XFontStyleEx.Bold);
        var textFont = new XFont("Arial", 6.5, XFontStyleEx.Regular);
        var formatter = new XTextFormatter(gfx);

        gfx.DrawRectangle(borderPen, rect);

        const double padding = 4d;

        var headerRect = new XRect(
            rect.X + padding,
            rect.Y + padding,
            rect.Width - (padding * 2),
            12);

        formatter.DrawString(
            "Digitally signed",
            titleFont,
            XBrushes.DarkBlue,
            headerRect,
            XStringFormats.TopLeft);

        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(_signerName))
            lines.Add($"Signer: {_signerName}");

        lines.Add($"Date: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");

        if (!string.IsNullOrWhiteSpace(_reason))
            lines.Add($"Reason: {_reason}");

        if (!string.IsNullOrWhiteSpace(_location))
            lines.Add($"Location: {_location}");

        var bodyRect = new XRect(
            rect.X + padding,
            rect.Y + 18,
            rect.Width - (padding * 2),
            rect.Height - 22);

        formatter.DrawString(
            string.Join(Environment.NewLine, lines),
            textFont,
            XBrushes.Black,
            bodyRect,
            XStringFormats.TopLeft);
    }
}

