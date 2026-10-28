using System.Diagnostics; using JTradeERP.Application.Common.Persistence; using JTradeERP.Domain.Security;
namespace JTradeERP.Application.Common.Security;
public static class SecurityAuditWriter { public static void Add(IApplicationDbContext db,Guid? actor,string action,string entity,string entityId,string? oldValue,string? newValue)=>db.Add(new SecurityAuditEntry(actor,action,entity,entityId,oldValue,newValue,Activity.Current?.Id??Guid.NewGuid().ToString())); }
