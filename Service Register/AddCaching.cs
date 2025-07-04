using Community.Microsoft.Extensions.Caching.PostgreSql;

namespace Learning.ServiceRegister;

public static partial class AppBuilderExtensions
{
    public static WebApplicationBuilder AddCaching(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCaching();

        builder.Services.AddMemoryCache();
        
        builder.Services.AddDistributedPostgreSqlCache(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Postgres");
            options.CreateInfrastructure = true;
            options.SchemaName = "public";
            options.TableName = "AppCache";
        });
        
        builder.Services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = builder.Configuration.GetConnectionString("Redis");
        });
        
        builder.Services.AddSingleton<DistributedCacheProvider>();
        
        return builder;
    }
}