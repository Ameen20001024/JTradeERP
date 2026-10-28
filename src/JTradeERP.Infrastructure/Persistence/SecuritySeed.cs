using JTradeERP.Domain.Security;

namespace JTradeERP.Infrastructure.Persistence;

internal static class SecuritySeed
{
    private static readonly string[] RoleNames = ["System Administrator", "Finance Manager", "Sales Manager", "Sales User", "Warehouse Manager", "Warehouse User", "Purchase User", "Auditor", "Read-Only User"];
    private static readonly string[] PermissionCodes = ["MasterData.View", "MasterData.Manage", "Customers.Manage", "Suppliers.Manage", "Billing.Quotation", "Billing.SalesOrder", "Billing.Invoice", "Billing.Post", "Billing.Cancel", "Billing.CreditNote", "Billing.Payment", "Purchasing.PurchaseOrder", "Purchasing.GRN", "Stock.View", "Stock.OpeningBalance", "Stock.Adjust", "Stock.Transfer", "Stock.Post", "Reports.View", "AuditLog.View", "Admin.Users.Manage", "Admin.Roles.Manage", "Admin.NumberSeries.Manage", "Admin.Configuration.Manage"];
    internal static readonly object[] Roles = RoleNames.Select((name, i) => new { Id = Id($"role-{i + 1}"), Name = name }).Cast<object>().ToArray();
    internal static readonly object[] Permissions = PermissionCodes.Select((code, i) => new { Id = Id($"permission-{i + 1}"), Code = code }).Cast<object>().ToArray();
    internal static object[] RolePermissions => Matrix.SelectMany((row, p) => row.Select((capability, r) => new { capability, r }).Where(x => x.capability != Capability.None).Select(x => new { RoleId = Id($"role-{x.r + 1}"), PermissionId = Id($"permission-{p + 1}"), Capability = x.capability })).Cast<object>().ToArray();

    // Columns: System Administrator, Finance Manager, Sales Manager, Sales User, Warehouse Manager, Warehouse User, Purchase User, Auditor.
    private static readonly Capability[][] Matrix =
    [
        [Capability.Full,Capability.Full,Capability.Full,Capability.View,Capability.Full,Capability.View,Capability.Full,Capability.View],
        [Capability.Full,Capability.Full,Capability.Full,Capability.None,Capability.Full,Capability.None,Capability.Full,Capability.None],
        [Capability.Full,Capability.Full,Capability.Full,Capability.CreateEdit,Capability.View,Capability.None,Capability.View,Capability.View],
        [Capability.Full,Capability.Full,Capability.None,Capability.None,Capability.View,Capability.None,Capability.CreateEdit,Capability.View],
        [Capability.Full,Capability.Full,Capability.Full,Capability.CreateEdit,Capability.View,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.Full,Capability.CreateEdit,Capability.View,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.Post,Capability.CreateEdit,Capability.View,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.Post,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None],
        [Capability.Full,Capability.Full,Capability.Post,Capability.None,Capability.None,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.Post,Capability.CreateEdit,Capability.View,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.View,Capability.View,Capability.None,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.View,Capability.None,Capability.View,Capability.None,Capability.Full,Capability.View],
        [Capability.Full,Capability.Full,Capability.View,Capability.None,Capability.Full,Capability.CreateEdit,Capability.View,Capability.View],
        [Capability.Full,Capability.Full,Capability.View,Capability.View,Capability.Full,Capability.Full,Capability.View,Capability.View],
        [Capability.Full,Capability.View,Capability.None,Capability.None,Capability.Full,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.View,Capability.None,Capability.None,Capability.Full,Capability.CreateEdit,Capability.None,Capability.View],
        [Capability.Full,Capability.View,Capability.None,Capability.None,Capability.Full,Capability.CreateEdit,Capability.None,Capability.View],
        [Capability.Full,Capability.View,Capability.None,Capability.None,Capability.Full,Capability.Post,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.Full,Capability.View,Capability.Full,Capability.View,Capability.View,Capability.Full],
        [Capability.Full,Capability.View,Capability.View,Capability.None,Capability.View,Capability.None,Capability.None,Capability.Full],
        [Capability.Full,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None],
        [Capability.Full,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None],
        [Capability.Full,Capability.Full,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None,Capability.View],
        [Capability.Full,Capability.Full,Capability.None,Capability.None,Capability.None,Capability.None,Capability.None,Capability.View]
    ];
    private static Guid Id(string key) => Guid.ParseExact(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("JTradeERP:" + key)))[..32], "N");
}
