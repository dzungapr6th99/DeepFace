using CommonLib;
using DeepFace;

using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using Microsoft.OpenApi.Models;
using NLog.Web;
string ConfigFolder = "./ConfigApp"; //can be relative or fullpath
string ConfigLogFile = "ConfigLog/nlog.config";
var logger = NLogBuilder.ConfigureNLog(ConfigLogFile).GetCurrentClassLogger();

IConfiguration configuration = null; //so it can be used on other configuration functions bellow
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot")), //point where if serving static files from default location. Works for my case. May need some adjustments in other cases.
});

builder.WebHost.ConfigureAppConfiguration(
(hostingContext, config) =>
{
    var path = Path.Combine(Path.GetFullPath(ConfigFolder), "appsettings.json");
    config.AddJsonFile(path, optional: false, reloadOnChange: true);
    config.AddEnvironmentVariables();
    configuration = config.Build();
});
// Add services to the container.
builder.Host.UseNLog();
// Init configs
ConfigData.InitConfigData(builder.Configuration);
Starting.InitProject(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    }); 
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "TOKEN",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer string_token\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// Add services to the container.

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAllCors");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
