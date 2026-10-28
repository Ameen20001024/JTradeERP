using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JTradeERP.Infrastructure.Persistence;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Infrastructure.Security;
using JTradeERP.Application.Common.Billing;
using JTradeERP.Application.Common.Reports;
using JTradeERP.Infrastructure.Reports;

namespace JTradeERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");
        }

        services.AddDbContext<JTradeERPDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<INumberSeriesAllocator, SqlNumberSeriesAllocator>();
        services.AddScoped<IReportReadService, DapperReportReadService>();
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<JTradeERPDbContext>());
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IRefreshTokenCookie, RefreshTokenCookie>();
        services.AddScoped<IPermissionResolver, PermissionResolver>();
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        return services;
    }
}
