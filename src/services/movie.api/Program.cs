using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using movie.api.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory()
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// default setup
builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "Movie Service",
        Version = "v1",
        Description = "The Movie Service HTTP API"
    });
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddDbContext<MovieContext>(options =>
{
    options.UseNpgsql(configuration["ConnectionString"]);
});

var seqServerUrl = configuration["Serilog:SeqServerUrl"];
var logstashUrl = configuration["Serilog:LogstashgUrl"];

builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Verbose()
    .Enrich.WithProperty("ApplicationContext", environment.ApplicationName)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
    .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
    .ReadFrom.Configuration(configuration));

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var pathBase = configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Movie Service v1"));
}

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });
});

app.Run();

public partial class Program
{
    public static string AppName = AppDomain.CurrentDomain.FriendlyName;
}