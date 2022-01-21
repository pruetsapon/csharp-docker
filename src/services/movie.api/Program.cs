using Serilog;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// default setup
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "Movie Service",
        Version = "v1",
        Description = "The Movie Service HTTP API"
    });
});

var seqServerUrl = configuration["Serilog:SeqServerUrl"];
var logstashUrl = configuration["Serilog:LogstashgUrl"];

builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Verbose()
    .Enrich.WithProperty("ApplicationContext", "Movie.API")
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
    .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
    .ReadFrom.Configuration(configuration));

var app = builder.Build();

//IConfiguration configuration = app.Configuration;
//IWebHostEnvironment environment = app.Environment;

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
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
});

app.UseSerilogRequestLogging();

app.Run();