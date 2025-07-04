using Microsoft.AspNetCore.Mvc;

namespace Learning.ServiceRegister;

public static partial class AppBuilderExtensions
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.CacheProfiles.Add("NoCache", new CacheProfile() {
                NoStore = true,
                Location = ResponseCacheLocation.None
            });
    
            options.CacheProfiles.Add("Any60", new CacheProfile() {
                Duration = 60,
                Location = ResponseCacheLocation.Any,
            });
        });
        return builder;
    }
}