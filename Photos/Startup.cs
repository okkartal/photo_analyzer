using Photos;
using Photos.AnalyzerService;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Photos;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<IAnalyzerService, ComputerVisionAnalyzerService>();
    }
}