using AICodeReview.Interfaces;
using AICodeReview.Services;
using AICodeReview.Telemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

#region Controllers

builder.Services.AddControllers();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region OpenTelemetry

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService(ActivitySources.SourceName);
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(ActivitySources.SourceName)

            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })

            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
            })

            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter(AiReviewTelemetry.Meter.Name)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddConsoleExporter()
            .AddPrometheusExporter();
    });

#endregion

#region Swagger

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Dependency Injection

builder.Services.AddScoped<IAnalyzeService, AnalyzeService>();
builder.Services.AddScoped<IAiReviewService, AiReviewService>();
builder.Services.AddScoped<IGitService, GitService>();

#endregion

#region HttpClient

builder.Configuration.AddJsonFile("appsettings.json");

var aiBaseUrl =
    builder.Configuration["AiReview:BaseUrl"]
    ?? "http://localhost:11434";

builder.Services.AddHttpClient<IAiCommunicationService, AiCommunicationService>(client =>
{
    client.BaseAddress = new Uri(aiBaseUrl);
});

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AngularPolicy");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();

#endregion

#region Endpoints

app.MapControllers();

app.MapPrometheusScrapingEndpoint();

#endregion

app.Run();
