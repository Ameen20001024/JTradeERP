using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using JTradeERP.Application.Common.Behaviors;
using JTradeERP.Application.Common.Inventory;

namespace JTradeERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(BillingAuditBehavior<,>));
        services.AddScoped<IStockPostingService, StockPostingService>();

        return services;
    }
}
