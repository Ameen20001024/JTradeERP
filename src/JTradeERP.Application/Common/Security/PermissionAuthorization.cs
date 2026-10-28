using JTradeERP.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

namespace JTradeERP.Application.Common.Security;

public interface IPermissionResolver 
{ 
    Task<bool> HasCapabilityAsync(Guid userId,
        string permissionCode, 
        Capability required, 
        CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<EffectivePermission>> GetEffectivePermissionsAsync(Guid userId, CancellationToken cancellationToken);

}

public sealed record EffectivePermission(string Code, Capability Capability);

public sealed record PermissionRequirement(string Code, Capability Capability) : IAuthorizationRequirement;

public static class PermissionEndpointExtensions
{
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder endpoint, 
        string code, 
        Capability capability) => endpoint.RequireAuthorization(
            new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(code, capability))
            .Build());
}

public sealed class MasterDataPermissionFilter(ICurrentUserService currentUser, IPermissionResolver resolver) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,EndpointFilterDelegate next)
    {
        var path = context.HttpContext.Request.Path.Value ?? string.Empty;
        
        var method = context.HttpContext.Request.Method;
        
        var module = path.Contains("/customers", StringComparison.OrdinalIgnoreCase)
            ? "Customers.Manage" : path.Contains("/suppliers",StringComparison.OrdinalIgnoreCase)
            ?"Suppliers.Manage":null;

        var deleting = HttpMethods.IsDelete(method);
        
        var query=HttpMethods.IsGet(method);
        
        var code=module??(query?"MasterData.View":"MasterData.Manage");
        
        var capability = query ? Capability.View : deleting ? Capability.Full : module is null ? Capability.Full : Capability.CreateEdit;

        if(!currentUser.UserId.HasValue
            || !await resolver.HasCapabilityAsync(currentUser.UserId.Value,code,capability,context.HttpContext.RequestAborted))
            
            return Results.Forbid();
        
        
        return await next(context);
    }
}
