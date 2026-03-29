using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tulo.SigningPdf.Interfaces;
using tulo.SigningPdf.Runners;
using tulo.SigningPdf.Services;

namespace tulo.SigningPdf.HostBuilders;

public static class AddServicesHostBuilderExtension
{
    public static IHostBuilder AddServices(this IHostBuilder host)
    {
        host.ConfigureServices((context, services) =>
        {
            IConfiguration configuration = context.Configuration;

            //#region Options
            //services.AddOptions<AppOptions>().Bind(configuration).ValidateDataAnnotations().ValidateOnStart();
            //services.AddSingleton<AppOptions>(sp => sp.GetRequiredService<IOptions<AppOptions>>().Value);
            //services.AddSingleton<IAppOptions>(sp => sp.GetRequiredService<AppOptions>());
            //#endregion

            // CLI runner
            services.AddTransient<IPdfSignatureService, PdfSignatureService>();
            services.AddTransient<ISignedPdfCliRunner, SignedPdfCliRunner>();
        });

        return host;
    }
}