using JTradeERP.Api.Endpoints;
using JTradeERP.Api.Infrastructure;
using JTradeERP.Application;
using JTradeERP.Infrastructure;
using JTradeERP.Infrastructure.Persistence;
using JTradeERP.Application.Features.MasterData;
using JTradeERP.Application.Features.Security;
using JTradeERP.Application.Features.Inventory;
using JTradeERP.Application.Features.Purchasing;
using JTradeERP.Api.Security;
using JTradeERP.Application.Features.Billing;
using JTradeERP.Application.Features.Reports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJTradeErpAuthentication(builder.Configuration);
builder.Services.AddHealthChecks().AddDbContextCheck<JTradeERPDbContext>("database", tags: ["ready"]);

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Ok(new { service = "JTradeERP.Api", status = "running" })).AllowAnonymous();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()).AllowAnonymous();
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
}).AllowAnonymous();
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
}).AllowAnonymous();
app.MapFoundationEndpoints(app.Environment.IsEnvironment("Testing"));
app.MapMasterDataEndpoints();
app.MapSecurityEndpoints();
app.MapInventoryEndpoints();
app.MapPurchasingEndpoints();
app.MapBillingEndpoints();
app.MapReportEndpoints();

app.Run();

public partial class Program;
