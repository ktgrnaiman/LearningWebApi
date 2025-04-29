using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Learning;

/// <summary>
/// Configures swagger generator to add all existing api versions
/// </summary>
public class ConfigureSwaggerGenerator : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly ApiVersion _defaultApiVersion;
    
    /// <param name="provider">Used to get versions info</param>
    /// <param name="verOptions">Option from AddApiVersioning service builder extension. Used to get specified default version</param>
    public ConfigureSwaggerGenerator(IApiVersionDescriptionProvider provider, IOptions<ApiVersioningOptions> verOptions)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _defaultApiVersion = verOptions?.Value.DefaultApiVersion ?? throw new ArgumentNullException(nameof(verOptions));
    }
    
    /// <summary>
    /// Populates swagger generator options with descriptions for all api versions
    /// in order for them to represented in individual OpenApi scheme json document
    /// </summary>
    /// <param name="options">SwaggerGenOptions to configure</param>
    public void Configure(SwaggerGenOptions options)
    {
        var relativeXmlPath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, relativeXmlPath);

        if (!File.Exists(xmlPath))
            throw new FileNotFoundException(xmlPath);
        options.IncludeXmlComments(xmlPath, true);
        
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo()
            {
                Title = $"Api {description.GroupName}",
                Version = description.ApiVersion.ToString(),
            };

            if (description.ApiVersion.Equals(_defaultApiVersion))
                info.Description += "Default API version";

            if (description.IsDeprecated)
                info.Description += "This API is deprecated";
                
            options.SwaggerDoc(description.GroupName, info);
        }
    }
}