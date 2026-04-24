using EndpointGuardian.Api.Middleware;
using EndpointGuardian.Api.Options;
using EndpointGiardian.Api.Models;
using EndPointGuardian.APi.Services;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.Configure<CompliancePolicyOptions>(

    builder.Configuration.GetSection("CompliancePolicy"));

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddTransient<IComplianceEvaluator, BasicComplianceEvaluator>();
builder.Services.AddSingleton<IDeviceRepository, InMemoryDeviceRepository>();



var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<RequestTimingMiddleware>();

app.Map("/health", healthApp =>
{
    healthApp.Run(async context =>
    {
        await context.Response.WriteAsync("Healthy");
    });
});

app.MapControllers();
app.Run();

