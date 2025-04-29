using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Learning;

/// <summary>
/// Configures swagger UI options to use default file name convention for versioned OpenApi specifications
/// </summary>
public class ConfigureSwaggerUI : IConfigureOptions<SwaggerUIOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerUI(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }
    
    public void Configure(SwaggerUIOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName);
    }
}