using System.Reflection;
using Learning.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Learning.Swagger;

public class SortDirectionFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        var attribute = context.ParameterInfo?.GetCustomAttribute<SortDirectionValidationAttribute>();

        if (attribute is null)
            return;
        
        parameter.Schema.Extensions.Add("pattern", 
            new OpenApiString(string.Join('|', 
                attribute.ValidStrings.Select(s => $"^{s}$"))
            )
        );
    }
}