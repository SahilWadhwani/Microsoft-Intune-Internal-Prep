using EndpointGuardian.Api.Middleware;
using EndpointGuardian.Api.Options;
using EndpointGuardian.Api.Models;
using EndpointGuardian.AApi.Services;
using Microsoft.AspNetCore.Builder;
using EndpointGuardian.Api.Data;

using Microsoft.EntityFrameworkCore;

using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

using EndpointGuardian.Api.Security;

using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddScoped<IRemediationService, RemediationService>();

builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddTransient<IComplianceEvaluator, BasicComplianceEvaluator>();

builder.Services.AddScoped<IDeviceRepository, EfDeviceRepository>();
builder.Services.AddScoped<IPolicyRepository, EfPolicyRepository>();
builder.Services.AddScoped<IPolicyAssignmentRepository, EfPolicyAssignmentRepository>();
builder.Services.AddScoped<IRemediationActionRepository, EfRemediationActionRepository>();

builder.Services.AddScoped<IAuditEventRepository, EfAuditEventRepository>();

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSigningKey!)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadDevices", policy =>
        policy.RequireClaim("permission", "devices.read"));

    options.AddPolicy("CanWriteDevices", policy =>
        policy.RequireClaim("permission", "devices.write"));

    options.AddPolicy("CanReadPolicies", policy =>
        policy.RequireClaim("permission", "policies.read"));

    options.AddPolicy("CanWritePolicies", policy =>
        policy.RequireClaim("permission", "policies.write"));

    options.AddPolicy("CanAssignPolicies", policy =>
        policy.RequireClaim("permission", "assignments.write"));

    options.AddPolicy("CanRunEvaluations", policy =>
        policy.RequireClaim("permission", "evaluations.run"));

    options.AddPolicy("CanRequestAccessDecision", policy =>
        policy.RequireClaim("permission", "access.decide"));

    options.AddPolicy("CanExecuteRemediation", policy =>
        policy.RequireClaim("permission", "remediation.execute"));

    options.AddPolicy("CanReadAudit", policy =>
        policy.RequireClaim("permission", "audit.read"));
});


var app = builder.Build();



app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<RequestTimingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.MapPost("/dev/token", (CreateDevTokenRequest request, IConfiguration config) =>
    {
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];
        var signingKey = config["Jwt:SigningKey"];

        var claims = new List<Claim>
        {
            new Claim("sub", request.Subject)
        };

        claims.AddRange(
            request.Permissions.Select(permission =>
                new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(signingKey!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new DevTokenResponse(tokenString));
    });
}

app.Map("/health", healthApp =>
{
    healthApp.Run(async context =>
    {
        await context.Response.WriteAsync("Healthy");
    });
});





app.MapControllers();
app.Run();

// Make the Program class public for integration testing
// This allows the WebApplicationFactory in the integration tests to reference the Program class as the entry point for hosting the API in-memory
public partial class Program { }
