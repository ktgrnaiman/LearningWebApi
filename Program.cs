using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Learning;

var builder = WebApplication.CreateBuilder(args);

//Add Http api endpoint controllers
builder.Services.AddControllers();

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
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<ConfigureSwaggerGenerator>();
builder.Services.ConfigureOptions<ConfigureSwaggerUI>();

//Building App
var app = builder.Build();

// Configure the HTTP request pipeline.
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
    () => { throw new Exception("test"); });

app.MapGet("/error",
    [ResponseCache(NoStore = true)]
    () => Results.Problem()).RequireCors("AnyOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
