using System.Reflection;
using Learning.Attributes;
using Learning.DTO;
using Learning.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Learning.Swagger;

public class SortColumnFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        var attribute = context.ParameterInfo?.GetCustomAttribute<SortColumnValidationAttribute>();

        if (attribute is null)
            return;
        
        parameter.Schema.Extensions.Add("pattern", 
            new OpenApiString(string.Join('|', 
                attribute.EntityProperties.Select(p => $"^{p.Name}$"))
            )
        );
    }
}