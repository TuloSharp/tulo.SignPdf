using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tulo.SigningPdfA3.Runners;

namespace tulo.SigningPdfA3.HostBuilders;

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
            services.AddSingleton<ISignedPdfCliRunner, SignedPdfCliRunner>();
        });

        return host;
    }
}