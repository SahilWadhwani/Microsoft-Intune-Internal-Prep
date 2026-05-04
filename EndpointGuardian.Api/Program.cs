using EndpointGuardian.Api.Middleware;
using EndpointGuardian.Api.Options;
using EndpointGuardian.Api.Models;
using EndpointGuardian.AApi.Services;
using Microsoft.AspNetCore.Builder;
using EndpointGuardian.Api.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.Configure<CompliancePolicyOptions>(

    builder.Configuration.GetSection("CompliancePolicy"));

builder.Services.AddDbContext<EndpointGuardianDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EndpointGuardianDb")));

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IEffectivePolicyResolver, EffectivePolicyResolver>();
builder.Services.AddScoped<IPolicyAssignmentService, PolicyAssignmentService>();
builder.Services.AddScoped<IAccessDecisionService, AccessDecisionService>();

builder.Services.AddTransient<IComplianceEvaluator, BasicComplianceEvaluator>();

builder.Services.AddScoped<IDeviceRepository, EfDeviceRepository>();
builder.Services.AddScoped<IPolicyRepository, EfPolicyRepository>();
builder.Services.AddScoped<IPolicyAssignmentRepository, EfPolicyAssignmentRepository>();



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

