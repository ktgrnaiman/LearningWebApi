using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Learning;
using Learning.Constants;
using Learning.Models;
using Learning.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;

var builder = WebApplication.CreateBuilder(args);

//Getting local secrets config and configuring DbContext
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Services.AddDbContext<ApplicationDbContext>(contextOptions => 
        contextOptions.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
}

//Registering serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, config) => {
        config.ReadFrom.Configuration(context.Configuration);
        config.WriteTo.Console();
        config.Enrich.WithMachineName();
        config.Enrich.WithThreadId();
    }
);

//Add Http api endpoint controllers
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

//Add cross-origin resource sharing policies
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(plc => { 
        plc.WithOrigins(builder.Configuration["AllowedOrigins"] ?? throw new Exception());
        plc.AllowAnyHeader();
        plc.AllowAnyMethod(); 
    });
    options.AddPolicy(name: "AnyOriginGetOnly", plc => {
        plc.AllowAnyOrigin();
        plc.AllowAnyHeader();
        plc.AllowAnyMethod();
    }); 
});

//Adding api versioning and configuring it
builder.Services.AddApiVersioning(options => {
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
}).AddApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


//Documentation
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

builder.Services.AddResponseCaching();

builder.Services.AddMemoryCache();

//Building App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Using the minimal api
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

//Configuring middleware pipeline
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();