using Serilog;

namespace Learning.ServiceRegister;

public static partial class AppBuilderExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, config) => {
                config.ReadFrom.Configuration(context.Configuration);
                config.WriteTo.Console();
                config.Enrich.WithMachineName();
                config.Enrich.WithThreadId();
            }
        );
        return builder;
    }
}