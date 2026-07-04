using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Frozen;
using Tulo.SigningPdf.HostBuilders;

namespace Tulo.SigningPdf;

public sealed class ConsoleApp
{
    private readonly string[] _normalizedArgs;

    // CLI switches
    public const string ArgInputPathPdf = "--inputPathPdf";
    public const string ArgSignaturePath = "--signaturePath";
    public const string ArgPublicKey = "--publicKey";
    public const string ArgReason = "--reason";
    public const string ArgLocation = "--location";
    public const string ArgContactInfo = "--contactInfo";
    public const string ArgOutputPathSignedPdf = "--outputPathSignedPdf";

    // Config keys
    public const string KeyInputPathPdf = "SIGNPDF:InputPathPdf";
    public const string KeySignaturePath = "SIGNPDF:SignaturePath";
    public const string KeyPublicKey = "SIGNPDF:PublicKey";
    public const string KeyReason = "SIGNPDF:Reason";
    public const string KeyLocation = "SIGNPDF:Location";
    public const string KeyContactInfo = "SIGNPDF:ContactInfo";
    public const string KeyOutputPathSignedPdf = "SIGNPDF:OutputPathSignedPdf";

    // CLI → IConfiguration mapping
    private static readonly FrozenDictionary<string, string> _commandLineMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [ArgInputPathPdf] = KeyInputPathPdf,
            [ArgSignaturePath] = KeySignaturePath,
            [ArgPublicKey] = KeyPublicKey,
            [ArgReason] = KeyReason,
            [ArgLocation] = KeyLocation,
            [ArgContactInfo] = KeyContactInfo,
            [ArgOutputPathSignedPdf] = KeyOutputPathSignedPdf,
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> _multiTokenArgs =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ArgInputPathPdf,
            ArgSignaturePath,
            ArgPublicKey,
            ArgReason,
            ArgLocation,
            ArgContactInfo,
            ArgOutputPathSignedPdf,
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    public ConsoleApp(string[]? args)
    {
        var safeArgs = args is { Length: > 0 } ? args : [];
        _normalizedArgs = NormalizeArgsForMultiTokenValues(safeArgs, _multiTokenArgs);
    }

    public IHostBuilder InitiateHostBuilder() =>
        Host.CreateDefaultBuilder(_normalizedArgs)
            .ConfigureAppConfiguration((_, config) =>
                config.AddCommandLine(_normalizedArgs, _commandLineMap))
            .AddAppSettings()
            .AddSerilog()
            .AddServices();

    public IHost BuildHost(IHostBuilder hostBuilder) => hostBuilder.Build();

    private static string[] NormalizeArgsForMultiTokenValues(
        string[] args,
        IReadOnlySet<string> multiTokenSwitches)
    {
        if (args.Length == 0) return args;

        var normalized = new List<string>(args.Length);

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];

            if (!multiTokenSwitches.Contains(current))
            {
                normalized.Add(current);
                continue;
            }

            // Collect all following tokens that are not switches
            var nextSwitchIndex = i + 1;
            while (nextSwitchIndex < args.Length && !IsSwitch(args[nextSwitchIndex]))
                nextSwitchIndex++;

            normalized.Add(current);
            normalized.Add(nextSwitchIndex > i + 1
                ? string.Join(' ', args, i + 1, nextSwitchIndex - i - 1)
                : string.Empty);

            i = nextSwitchIndex - 1;
        }

        return [.. normalized];
    }

    private static bool IsSwitch(string value) =>
        value.StartsWith("--", StringComparison.Ordinal);
}


