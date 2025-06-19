using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using CoreAPI;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2147483647;
});

startup.ConfigureServices(builder.Services);

var app = builder.Build();

ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
startup.Configure(app, loggerFactory, app.Environment);

await app.RunAsync();
