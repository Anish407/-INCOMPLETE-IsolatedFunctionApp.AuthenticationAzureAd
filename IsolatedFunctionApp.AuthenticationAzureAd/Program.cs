using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

var host = new HostBuilder()
    .ConfigureAppConfiguration(config =>
    {
        var path = config.SetBasePath(Directory.GetCurrentDirectory());
        config
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    })
    .ConfigureServices((hostBuilder, services) =>
    {
        var configuration = hostBuilder.Configuration;

        services.AddAuthentication((sharedOptions) =>
        {
            sharedOptions.DefaultScheme = Microsoft.Identity.Web.Constants.Bearer;
            sharedOptions.DefaultChallengeScheme = Microsoft.Identity.Web.Constants.Bearer;

        })
        .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();