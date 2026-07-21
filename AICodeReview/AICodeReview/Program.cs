using AICodeReview.Interfaces;
using AICodeReview.Services.AI;
using AICodeReview.Services.AIConnection;
using AICodeReview.Services.CodeAnalyser;
using AICodeReview.Services.Git;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

builder.Services.AddOpenTelemetry()
    .WithTracing(tracer =>
    {
        tracer
            .AddSource("AiReview")
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
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddConsoleExporter();
    });

object value = builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICodeAnalyzerService, CodeAnalyzerService>();
builder.Services.AddScoped<IAiReviewService, AiReviewService>();
builder.Services.AddScoped<IGitService, GitService>();

var aiBaseUrl = builder.Configuration["AiReview:BaseUrl"] ?? "http://localhost:11434";

builder.Services.AddHttpClient<IAiResponseService, AiResponseService>(client =>
{
    client.BaseAddress = new Uri(aiBaseUrl);
});

builder.Configuration.AddJsonFile("appsettings.json");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AngularPolicy");

app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();

app.MapControllers();

app.Run();