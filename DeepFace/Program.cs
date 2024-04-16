using CommonLib;
using DeepFace;
using Microsoft.Extensions.Configuration;
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
builder.Services.AddRazorPages();
// Init configs
ConfigData.InitConfigData(builder.Configuration);
Starting.InitProject(builder.Services);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
