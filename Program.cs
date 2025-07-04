using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

using Learning.Constants;
using Learning.Models;
using Learning.ServiceRegister;


var builder = WebApplication.CreateBuilder(args);

//Getting local secrets config and configuring DbContext
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Services.AddDbContext<ApplicationDbContext>(contextOptions => 
        contextOptions.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
    );
}

builder
    .AddSerilog()
    .AddControllers()
    .AddCors()
    .AddVersioningAndSwagger()
    .AddCaching();

//Building App
var app = builder.Build();

if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/error");

app.MapGet("/error/test",
    [EnableCors("AnyOriginGetOnly")]
    [ResponseCache(NoStore = true)]
    ([FromQuery]int errorCode) =>
    {
        throw errorCode switch
        {
            501 => new NotImplementedException(),
            504 => new TimeoutException(),
            _ => new Exception("Generic exception")
        };
    });

app.MapGet("/error",
    [ResponseCache(NoStore = true)]
    (HttpContext context) =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
        var details = new ProblemDetails();
        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] =
            System.Diagnostics.Activity.Current?.Id ??
            context.TraceIdentifier;
        details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        details.Status = exceptionHandler?.Error switch
        {
            NotImplementedException => StatusCodes.Status501NotImplemented,
            TimeoutException => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError
        }; 
        
        app.Logger.LogError(CustomLogEvents.Error, exceptionHandler?.Error, "Some unhandled error happened");
        
        return Results.Problem(details);
    })
    .RequireCors("AnyOrigin");

app.MapGet("/cache/test1", (HttpContext context) => Results.Ok());

app.Use((context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
    {
        NoCache = true,
        NoStore = true,
    };
    return next.Invoke();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();