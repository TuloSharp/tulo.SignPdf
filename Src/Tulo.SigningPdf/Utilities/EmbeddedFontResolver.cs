

using PdfSharp.Fonts;
using System.Diagnostics;
using System.Reflection;

namespace Tulo.SigningPdf.Utilities;
public class EmbeddedFontResolver : IFontResolver
{
    private static class FaceKeys
    {
        public const string VerdanaRegular = "Verdana#Regular";
        public const string VerdanaBold = "Verdana#Bold";
        public const string VerdanaItalic = "Verdana#Italic";
        public const string VerdanaBoldItalic = "Verdana#BoldItalic";

        public const string ArialRegular = "Arial#Regular";
        public const string ArialBold = "Arial#Bold";
        public const string ArialItalic = "Arial#Italic";
        public const string ArialBoldItalic = "Arial#BoldItalic";

        public const string CascadiaCodeRegular = "CascadiaCode#Regular";
        public const string CascadiaCodeBold = "CascadiaCode#Bold";
        public const string CascadiaCodeItalic = "CascadiaCode#Italic";
        public const string CascadiaCodeBoldItalic = "CascadiaCode#BoldItalic";

        public const string RobotoRegular = "Roboto#Regular";
        public const string RobotoBold = "Roboto#Bold";
        public const string RobotoItalic = "Roboto#Italic";
        public const string RobotoBoldItalic = "Roboto#BoldItalic";
    }

    private static readonly Dictionary<string, (string regular, string bold, string italic, string boldItalic)> _familyMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "verdana",
                (
                    FaceKeys.VerdanaRegular,
                    FaceKeys.VerdanaRegular,
                    FaceKeys.VerdanaRegular,
                    FaceKeys.VerdanaRegular
                )
            },
            {
                "arial",
                (
                    FaceKeys.ArialRegular,
                    FaceKeys.ArialRegular,
                    FaceKeys.ArialRegular,
                    FaceKeys.ArialRegular
                )
            },
            {
                "cascadiacode",
                (
                    FaceKeys.CascadiaCodeRegular,
                    FaceKeys.CascadiaCodeRegular,
                    FaceKeys.CascadiaCodeRegular,
                    FaceKeys.CascadiaCodeRegular
                )
            },
            {
                "roboto",
                (
                    FaceKeys.RobotoRegular,
                    FaceKeys.RobotoRegular,
                    FaceKeys.RobotoRegular,
                    FaceKeys.RobotoRegular
                )
            }
        };

    private static readonly Dictionary<string, string> _resourceMap;

    static EmbeddedFontResolver()
    {
        _resourceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var asm = Assembly.GetExecutingAssembly();
        var resources = asm.GetManifestResourceNames();

        Register(resources, FaceKeys.VerdanaRegular, "VERDANA.TTF");

        Register(resources, FaceKeys.ArialRegular, "ARIAL.TTF");

        Register(resources, FaceKeys.CascadiaCodeRegular, "CASCADIACODE.TTF", "CASCADIACODEPL.TTF");

        Register(resources, FaceKeys.RobotoRegular, "ROBOTO.TTF", "ROBOTO-REGULAR.TTF");
    }

    private static void Register(string[] resources, string faceKey, params string[] fileNames)
    {
        var match = resources.FirstOrDefault(r =>
            fileNames.Any(f => r.EndsWith("Fonts." + f, StringComparison.OrdinalIgnoreCase)));

        if (match == null)
        {
            Debug.WriteLine($"Font Resource für '{faceKey}' nicht gefunden. Kandidaten: {string.Join(", ", fileNames)}");
            return;
        }

        _resourceMap[faceKey] = match;
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (string.IsNullOrWhiteSpace(familyName))
            return null;

        if (_familyMap.TryGetValue(familyName.Trim(), out var faces))
        {
            if (_resourceMap.ContainsKey(faces.regular))
                return new FontResolverInfo(faces.regular);

            return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
        }

        return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
    }

    public byte[]? GetFont(string faceName)
    {
        if (string.IsNullOrWhiteSpace(faceName))
            return null;

        if (!_resourceMap.TryGetValue(faceName, out var resourceName))
            return null;

        var asm = Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var names = asm.GetManifestResourceNames();
            Debug.WriteLine($"Font Resource '{resourceName}' nicht gefunden. Verfügbare Resources:\n{string.Join("\n", names)}");
            return null;
        }

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}

