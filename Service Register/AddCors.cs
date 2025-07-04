namespace Learning.ServiceRegister;

public static partial class AppBuilderExtensions
{
    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
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
        return builder;
    }
}