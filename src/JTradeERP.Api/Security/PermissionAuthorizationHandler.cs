using System.Security.Claims; using JTradeERP.Application.Common.Security; using Microsoft.AspNetCore.Authorization;
namespace JTradeERP.Api.Security;
public sealed class PermissionAuthorizationHandler(IPermissionResolver resolver) : AuthorizationHandler<PermissionRequirement>
{ protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,PermissionRequirement requirement){if(!Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,out var id))return;if(await resolver.HasCapabilityAsync(id,requirement.Code,requirement.Capability,CancellationToken.None))context.Succeed(requirement);} }
