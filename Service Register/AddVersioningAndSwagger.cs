using Asp.Versioning;
using Learning.Swagger;

namespace Learning.ServiceRegister;

public static partial class AppBuilderExtensions
{
    public static WebApplicationBuilder AddVersioningAndSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options => {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        }).AddApiExplorer(options => {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
                options.ParameterFilter<SortColumnFilter>();
                options.ParameterFilter<SortDirectionFilter>();
            }
        );

        builder.Services.ConfigureOptions<ConfigureSwaggerGenerator>();
        builder.Services.ConfigureOptions<ConfigureSwaggerUI>();
        return builder;
    }
}