using JTradeERP.Application.Common.Security;
namespace JTradeERP.Application.Features.Security.GetMe;
public sealed record GetMeResponse(Guid Id,string UserName,string Email,IReadOnlyCollection<string> Roles,IReadOnlyCollection<EffectivePermission> Permissions);
